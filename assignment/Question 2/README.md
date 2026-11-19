# Question 2: Community Insurers

The supplied Visual Basic Windows Forms starter now searches the supplied Access database and lists the clients associated with a policy. The question paper is still missing, so the exact required output columns and marking instructions remain unconfirmed.

## Open and run on Windows

1. Open `Policies/Policies/Policies.sln` in Visual Studio 2010 (or Visual Basic 2010 Express), with Visual Basic, Windows Forms and .NET Framework 4.0 development tools installed.
2. Build and run the solution. The project builds as **x86**, even though the solution configuration retains its original `Any CPU` label.
3. Enter a policy number such as **20001** and click **List Associated Clients**, or press Enter.
4. Close the application using the window's close button.

A compiled application is also supplied at `Policies/Policies/bin/Release/Policies.exe`. Keep `Policies.exe.config` and the adjacent `Database` folder with the executable. Extract the ZIP fully before running it.

The source database is `Database/PolicyValidation.mdb`. Building automatically copies it to `Database/PolicyValidation.mdb` beneath the output directory. The application resolves that location relative to its own executable, so it works independently of the current working directory. The supplied database contents have not been changed.

The existing examination-number placeholder in the window title is preserved. Replace it with your examination number in the form designer when preparing a submission.

## Behaviour

- Accepts positive whole policy numbers, with optional surrounding whitespace or leading zeroes. Rejects blanks, negative numbers, zero, decimals, grouping separators, text and integer overflow.
- Looks up the policy first, then retrieves only that policy's associated clients.
- Shows client number, title, first name, surname, policy number, policy name and premium in rand.
- Sorts clients by surname, first name and client number. The grid supports header sorting and column rearrangement.
- Shows a count when clients are found. Distinguishes a valid policy with no clients from a policy that does not exist.
- Clears previous results when the search input changes or a new search fails, so old rows cannot be mistaken for a new result.
- Uses typed positional SQL parameters, `Using` blocks for connection/command disposal, a read-only connection, and a read-only grid.
- Reports missing database files, unavailable Access providers, unreadable/locked databases and access problems without leaving stale results in the grid.

No facilities for changing clients, policies or premiums have been added. The source interface provides a lookup button only.

## Supplied database

The actual database is Jet 4 MDB format, with these two tables:

| Table | Column | Access type |
| --- | --- | --- |
| policies | policyno | Integer, not null |
| policies | policyname | Memo |
| policies | premium | Currency |
| clients | policyno | Long Integer |
| clients | title | Text |
| clients | fname | Text |
| clients | surname | Text |
| clients | clientno | Long Integer |

The search joins `policies.policyno` to `clients.policyno`. A client can appear under more than one policy, so the policy filter is applied to the join. The application does not infer a premium payment frequency because none is specified in the supplied files.

These are the expected results for the unchanged database:

| Input | Policy | Premium (R) | Expected outcome |
| --- | --- | ---: | --- |
| 20001 | HOME CONTENT UNDER 300 000 | 250.00 | 4 clients: AFRIKA, PETERSEN, SILENCE, TSHONA |
| 20002 | CARS OLDER THAN 12 YEARS | 300.00 | 3 clients: PETERSEN, SILENCE, TSHONA |
| 20203 | LIFE COVER 2 MILLION | 400.00 | 2 clients: PETERSEN, SILENCE |
| 22567 | EDUCATION 3 - CHILDREN | 600.00 | Policy exists, but has no associated clients |
| 99999 | Not present | — | Policy was not found |

## Access provider

The application requires Windows, .NET Framework 4.0 and an Access OLE DB provider matching the application's 32-bit architecture. It first tries `Microsoft.Jet.OLEDB.4.0`, then `Microsoft.ACE.OLEDB.12.0` and `Microsoft.ACE.OLEDB.16.0` if a provider is unregistered. All connections request `Mode=Read`.

If no provider can be opened, use the supplied x86 build with a registered 32-bit Jet or ACE provider. A 64-bit-only ACE provider cannot be loaded by this x86 application. If your classroom requires a particular provider or architecture, adjust the build and provider selection accordingly.

## Code guide

- `Policies/Policies/Form1.vb`: supplied UI initialization, read-only grid, input handling, search button, status messages and error handling.
- `Policies/Policies/PolicyQueries.vb`: shared parameterized SQL, policy-number validation, typed result-table schema and the search-result object.
- `Policies/Policies/PolicyRepository.vb`: Access database connection and policy/client lookup. No user input is concatenated into SQL.
- `Policies/Policies/Form1.Designer.vb`: supplied designer controls, unchanged.
- `Tests/Program.vb`: portable Visual Basic checks for validation and result-table behaviour.
- `Tests/verify_database.py`: checks the exact production SELECT statements using records exported from the actual supplied MDB.
- `Tests/WindowsIntegration.vb`: checks the actual repository and native OLE DB connection when run on Windows.
- `Question 2 - original.zip`: local backup of the starter project and database before implementation. It is excluded from the completed-project ZIP.

## Verification completed here

- The converted .NET Framework 4.0 Windows Forms application compiled successfully in Release configuration as x86, with Visual Basic 2010 syntax restrictions.
- **45** portable Visual Basic input-validation and result-table assertions passed.
- **51** SQL/data assertions passed using the supplied MDB's four policies and nine client associations. The tests run the production query strings in an in-memory SQLite database populated by MDB Tools exports. They check exact clients and sort order, joined descriptions and premiums, valid policies with no clients, unknown policies, parameter handling, and the unchanged original file.
- The output database's SHA-256 matches the source database: `feb3915068006167fa48fd5296994e4126d0a4f698a19eb622ac214e7e35f1ee`.
- The Windows OLE DB integration runner also compiled successfully.

The Windows interface and native Access provider have **not** been executed in this Linux workspace. The SQLite query checks do not establish native Jet/ACE runtime compatibility; use the integration runner and manual checks below on Windows.

## Repeat the checks

Validation checks, using a Visual Studio 2010 Command Prompt from this Question 2 folder:

```bat
msbuild Tests\PolicyTests.vbproj /p:Configuration=Release
Tests\bin\Release\PolicyTests.exe
```

The optional validation and Windows integration projects are also Visual Studio 2010 / .NET Framework 4.0 console projects. Both compiled against .NET Framework 4.0 references with Visual Basic 2010 syntax restrictions. The converted validation executable passed its 45 assertions on the available .NET 10 runtime; native Windows OLE DB execution still requires Windows.

Database query checks, using Python 3 and MDB Tools (`mdb-schema` and `mdb-export` on PATH):

```sh
python3 Tests/verify_database.py
```

Windows OLE DB integration check, using a Visual Studio 2010 Command Prompt from this Question 2 folder:

```bat
msbuild Tests\WindowsIntegration.vbproj /p:Configuration=Release
Tests\bin\Windows\Release\PolicyIntegrationTests.exe
```

Manual Windows checks:

1. Search for each policy and the unknown number listed in the results table above.
2. Search with blank input, letters, `-1`, `20001.5`, and `2147483648`; each should produce a validation message with an empty grid.
3. Search successfully, then edit the input. The previous rows should disappear immediately. Press Enter to search again.
4. Confirm that grid cells cannot be edited and the premium is displayed with two decimal places.
5. Close and reopen the application, temporarily move the output `Database` folder away, and search. A missing-database message should appear. Restore the folder afterwards.
6. Run the executable from a different working directory to confirm the relative-to-executable database path works.

Before submission, compare the displayed columns, wording and any additional required behaviour with the actual question paper.

References: [Microsoft's parameter guidance](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types) and [OleDbConnection documentation](https://learn.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbconnection?view=netframework-4.8.1).
