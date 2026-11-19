Imports System
Imports System.Data

Module Program
    Private assertions As Integer

    Sub Main()
        Dim number As Integer
        AssertTrue(PolicyQueries.TryParsePolicyNumber("20001", number) AndAlso number = 20001, "Valid policy number")
        AssertTrue(PolicyQueries.TryParsePolicyNumber(" 22567 ", number) AndAlso number = 22567, "Surrounding whitespace")
        AssertTrue(PolicyQueries.TryParsePolicyNumber("00020001", number) AndAlso number = 20001, "Leading zeroes")
        AssertTrue(PolicyQueries.TryParsePolicyNumber("2147483647", number) AndAlso number = Integer.MaxValue, "32-bit upper bound")
        For Each invalid As String In New String() {Nothing, "", " ", "0", "-1", "+20001", "20001.0", "20,001", "2e4", "NaN", "20001 OR 1=1", "'", "2147483648", "99999999999999999999999999", "20 001"}
            number = 123
            AssertTrue(Not PolicyQueries.TryParsePolicyNumber(invalid, number), "Reject invalid input: " & invalid)
            AssertTrue(number = 0, "Failed parsing clears the previous value")
        Next

        Using table As DataTable = PolicyQueries.CreateResultsTable()
            AssertTrue(table.Rows.Count = 0, "Empty results contain no placeholder row")
            AssertTrue(table.Columns.Count = 7, "Seven result columns")
            AssertTrue(table.Columns("clientno").DataType Is GetType(Integer), "Numeric client number")
            AssertTrue(table.Columns("policyno").DataType Is GetType(Integer), "Numeric policy number")
            AssertTrue(table.Columns("premium").DataType Is GetType(Decimal), "Decimal premium")
            table.Rows.Add(112, "MS", "PRECIOUS", "AFRIKA", 20001, "HOME CONTENT UNDER 300 000", 250D)
            Dim result As New PolicySearchResult(True, table)
            AssertTrue(result.PolicyExists AndAlso result.Clients.Rows.Count = 1, "Results retain found policy and clients")
            table.Rows.Add(999, DBNull.Value, DBNull.Value, DBNull.Value, 20001, DBNull.Value, DBNull.Value)
            AssertTrue(table.Rows(1).IsNull("fname"), "Missing optional text is supported")
            AssertTrue(table.Rows(1).IsNull("premium"), "Missing premium remains missing rather than zero")
        End Using
        Using table As DataTable = PolicyQueries.CreateResultsTable()
            Dim missing As New PolicySearchResult(False, table)
            AssertTrue(Not missing.PolicyExists AndAlso missing.Clients.Rows.Count = 0, "Unknown policy representation")
            Dim empty As New PolicySearchResult(True, table)
            AssertTrue(empty.PolicyExists AndAlso empty.Clients.Rows.Count = 0, "Existing policy with no clients is distinct")
        End Using
        Try
            Dim invalidResult As New PolicySearchResult(True, Nothing)
            Throw New Exception("FAIL: null results table accepted")
        Catch ex As ArgumentNullException
            assertions += 1
        End Try
        Console.WriteLine("PASS: " & assertions.ToString() & " input-validation and result-table assertions.")
    End Sub

    Private Sub AssertTrue(condition As Boolean, label As String)
        If Not condition Then Throw New Exception("FAIL: " & label)
        assertions += 1
    End Sub
End Module
