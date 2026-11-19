Option Strict On
Option Explicit On

Imports System
Imports System.Data
Imports System.Globalization

Public NotInheritable Class PolicyQueries
    Private Sub New()
    End Sub

    'OLE DB uses positional ? parameters; never concatenate user input into SQL.
    Public Const FindPolicySql As String = "SELECT COUNT(*) FROM [policies] WHERE [policyno] = ?"
    Public Const ListClientsSql As String = "SELECT c.[clientno], c.[title], c.[fname], c.[surname], p.[policyno], p.[policyname], p.[premium] FROM [policies] AS p INNER JOIN [clients] AS c ON p.[policyno] = c.[policyno] WHERE p.[policyno] = ? ORDER BY c.[surname], c.[fname], c.[clientno]"

    Public Shared Function TryParsePolicyNumber(input As String, ByRef policyNumber As Integer) As Boolean
        policyNumber = 0
        If input Is Nothing Then Return False
        Dim parsed As Integer
        If Not Integer.TryParse(input.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, parsed) OrElse parsed <= 0 Then Return False
        policyNumber = parsed
        Return True
    End Function

    Public Shared Function CreateResultsTable() As DataTable
        Dim table As New DataTable("PolicyClients")
        table.Locale = CultureInfo.InvariantCulture
        table.Columns.Add("clientno", GetType(Integer))
        table.Columns.Add("title", GetType(String))
        table.Columns.Add("fname", GetType(String))
        table.Columns.Add("surname", GetType(String))
        table.Columns.Add("policyno", GetType(Integer))
        table.Columns.Add("policyname", GetType(String))
        table.Columns.Add("premium", GetType(Decimal))
        Return table
    End Function
End Class

Public Class PolicySearchResult
    Private ReadOnly _policyExists As Boolean
    Private ReadOnly _clients As DataTable

    Public Sub New(policyExists As Boolean, clients As DataTable)
        If clients Is Nothing Then Throw New ArgumentNullException("clients")
        _policyExists = policyExists
        _clients = clients
    End Sub

    Public ReadOnly Property PolicyExists As Boolean
        Get
            Return _policyExists
        End Get
    End Property

    Public ReadOnly Property Clients As DataTable
        Get
            Return _clients
        End Get
    End Property
End Class
