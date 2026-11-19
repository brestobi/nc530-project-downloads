Imports System
Imports System.Data
Imports System.IO
Imports System.Security.Cryptography

Module Program
    Private assertions As Integer

    Sub Main()
        Dim database As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "PolicyValidation.mdb")
        Dim before As String = Hash(database)
        Dim repository As New PolicyRepository(database)
        Verify(repository, 20001, True, New Integer() {112, 114, 113, 111}, "HOME CONTENT UNDER 300 000", 250D)
        Verify(repository, 20002, True, New Integer() {114, 113, 111}, "CARS OLDER THAN 12 YEARS", 300D)
        Verify(repository, 20203, True, New Integer() {114, 113}, "LIFE COVER 2 MILLION", 400D)
        Verify(repository, 22567, True, New Integer() {}, "EDUCATION 3 - CHILDREN", 600D)
        Verify(repository, 99999, False, New Integer() {}, Nothing, 0D)
        Verify(repository, 20001, True, New Integer() {112, 114, 113, 111}, "HOME CONTENT UNDER 300 000", 250D)
        Try
            repository.FindClients(0)
            Throw New Exception("FAIL: invalid policy number accepted")
        Catch ex As ArgumentOutOfRangeException
            assertions += 1
        End Try
        Dim missing As New PolicyRepository(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() & ".mdb"))
        Try
            missing.FindClients(20001)
            Throw New Exception("FAIL: missing database accepted")
        Catch ex As FileNotFoundException
            assertions += 1
        End Try
        AssertTrue(Hash(database) = before, "Database contents remain unchanged")
        Console.WriteLine("PASS: " & assertions.ToString() & " Windows OLE DB integration assertions.")
    End Sub

    Private Sub Verify(repository As PolicyRepository, number As Integer, exists As Boolean, clientIds As Integer(), description As String, premium As Decimal)
        Dim result As PolicySearchResult = repository.FindClients(number)
        Using table As DataTable = result.Clients
            AssertTrue(result.PolicyExists = exists, "Correct policy existence")
            AssertTrue(table.Rows.Count = clientIds.Length, "Correct client count")
            For index As Integer = 0 To clientIds.Length - 1
                Dim row As DataRow = table.Rows(index)
                AssertTrue(CInt(row("clientno")) = clientIds(index), "Correct clients and order")
                AssertTrue(CInt(row("policyno")) = number, "Correct policy")
                AssertTrue(CStr(row("policyname")) = description, "Correct policy description")
                AssertTrue(CDec(row("premium")) = premium, "Correct premium")
            Next
        End Using
    End Sub

    Private Function Hash(path As String) As String
        Using sha As SHA256 = SHA256.Create()
            Return Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path)))
        End Using
    End Function

    Private Sub AssertTrue(condition As Boolean, label As String)
        If Not condition Then Throw New Exception("FAIL: " & label)
        assertions += 1
    End Sub
End Module
