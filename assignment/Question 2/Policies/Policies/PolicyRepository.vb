Option Strict On
Option Explicit On

Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.IO

Public Class PolicyRepository
    Private ReadOnly _databasePath As String

    Public Sub New(databasePath As String)
        If String.IsNullOrWhiteSpace(databasePath) Then Throw New ArgumentException("A database path is required.", "databasePath")
        _databasePath = Path.GetFullPath(databasePath)
    End Sub

    Public Function FindClients(policyNumber As Integer) As PolicySearchResult
        If policyNumber <= 0 Then Throw New ArgumentOutOfRangeException("policyNumber", "Enter a positive whole policy number.")
        If Not File.Exists(_databasePath) Then
            Throw New FileNotFoundException("The supplied PolicyValidation.mdb database could not be found.", _databasePath)
        End If

        Dim clients As DataTable = PolicyQueries.CreateResultsTable()
        Using connection As OleDbConnection = OpenConnection()
            Dim policyExists As Boolean
            Using command As New OleDbCommand(PolicyQueries.FindPolicySql, connection)
                command.Parameters.Add("policyNumber", OleDbType.Integer).Value = policyNumber
                policyExists = Convert.ToInt32(command.ExecuteScalar()) > 0
            End Using

            If policyExists Then
                Using command As New OleDbCommand(PolicyQueries.ListClientsSql, connection)
                    command.Parameters.Add("policyNumber", OleDbType.Integer).Value = policyNumber
                    Using adapter As New OleDbDataAdapter(command)
                        adapter.Fill(clients)
                    End Using
                End Using
            End If
            Return New PolicySearchResult(policyExists, clients)
        End Using
    End Function

    Private Function OpenConnection() As OleDbConnection
        'The supplied database is Jet 4 format. Build x86 for the 32-bit Jet provider.
        'Try ACE too when Jet is not registered on the Windows computer.
        Dim providers As String() = {"Microsoft.Jet.OLEDB.4.0", "Microsoft.ACE.OLEDB.12.0", "Microsoft.ACE.OLEDB.16.0"}
        Dim providerError As InvalidOperationException = Nothing
        For Each provider As String In providers
            Dim builder As New OleDbConnectionStringBuilder()
            builder.Provider = provider
            builder.DataSource = _databasePath
            builder.PersistSecurityInfo = False
            builder("Mode") = "Read"

            Dim connection As New OleDbConnection(builder.ConnectionString)
            Try
                connection.Open()
                Return connection
            Catch ex As InvalidOperationException
                'An unregistered provider raises InvalidOperationException.
                connection.Dispose()
                providerError = ex
            Catch
                connection.Dispose()
                Throw
            End Try
        Next
        Throw New InvalidOperationException("No compatible Access database provider is available. Run the 32-bit build with a registered Jet or 32-bit ACE provider.", providerError)
    End Function
End Class
