"""Check the production SELECT statements against records exported from the supplied MDB.

Requires Python 3 and mdbtools. SQLite checks query logic; WindowsIntegration.vb
checks the actual OLE DB repository on Windows.
"""
import argparse
import csv
import hashlib
import io
from pathlib import Path
import re
import sqlite3
import subprocess

BASE = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--mdb-tools-dir", type=Path, help="Directory containing mdb-schema and mdb-export")
args = parser.parse_args()
database = BASE / "Database/PolicyValidation.mdb"
before = hashlib.sha256(database.read_bytes()).hexdigest()
assertions = 0


def check(condition, label):
    global assertions
    if not condition:
        raise AssertionError(label)
    assertions += 1


def mdb(tool, *arguments):
    executable = str(args.mdb_tools_dir / tool) if args.mdb_tools_dir else tool
    return subprocess.run([executable, str(database), *arguments], check=True, capture_output=True, text=True).stdout


source = (BASE / "Policies/Policies/PolicyQueries.vb").read_text(encoding="utf-8-sig")
queries = dict(re.findall(r'Public Const (\w+) As String = "((?:""|[^"])*)"', source))
find_sql = queries["FindPolicySql"].replace('""', '"')
list_sql = queries["ListClientsSql"].replace('""', '"')
check(find_sql.count("?") == 1 and list_sql.count("?") == 1, "One positional parameter per SELECT")

with sqlite3.connect(":memory:") as connection:
    connection.row_factory = sqlite3.Row
    connection.executescript(mdb("mdb-schema", "sqlite"))
    for table, expected_count in (("policies", 4), ("clients", 9)):
        rows = list(csv.reader(io.StringIO(mdb("mdb-export", table))))
        header, data = rows[0], rows[1:]
        check(len(data) == expected_count, f"Original {table} count")
        columns = ", ".join(f"[{column}]" for column in header)
        placeholders = ", ".join("?" for _ in header)
        connection.executemany(f"INSERT INTO [{table}] ({columns}) VALUES ({placeholders})", data)

    expected = {
        20001: (True, [112, 114, 113, 111], "HOME CONTENT UNDER 300 000", 250),
        20002: (True, [114, 113, 111], "CARS OLDER THAN 12 YEARS", 300),
        20203: (True, [114, 113], "LIFE COVER 2 MILLION", 400),
        22567: (True, [], "EDUCATION 3 - CHILDREN", 600),
        99999: (False, [], None, None),
    }
    for number, (exists, client_ids, policy_name, premium) in expected.items():
        actual_exists = connection.execute(find_sql, (number,)).fetchone()[0] > 0
        results = connection.execute(list_sql, (number,)).fetchall()
        check(actual_exists == exists, f"Policy {number}: existence")
        check([row["clientno"] for row in results] == client_ids, f"Policy {number}: exact clients and sort order")
        for row in results:
            check(row["policyno"] == number, "No clients from another policy")
            check(row["policyname"] == policy_name, "Joined policy description")
            check(row["premium"] == premium, "Joined premium")
    for payload in ("20001 OR 1=1", "20001' OR '1'='1", "'", "20001; DROP TABLE clients"):
        check(connection.execute(find_sql, (payload,)).fetchone()[0] == 0, "Parameter input is not executable SQL")
        check(connection.execute(list_sql, (payload,)).fetchall() == [], "Parameterized client SELECT rejects injection text")
    check(connection.execute("SELECT COUNT(*) FROM clients").fetchone()[0] == 9, "Records remain present")

check(hashlib.sha256(database.read_bytes()).hexdigest() == before, "Original MDB unchanged")
copy = BASE / "Policies/Policies/bin/Release/Database/PolicyValidation.mdb"
if copy.exists():
    check(hashlib.sha256(copy.read_bytes()).hexdigest() == before, "Build copies the exact supplied database")
print(f"PASS: {assertions} query/data assertions using the supplied MDB records. OLE DB runtime still requires Windows.")
