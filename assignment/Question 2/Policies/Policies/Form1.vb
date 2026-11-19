Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Data
Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO

Public Class Form1
    Private bs As BindingSource
    Private repository As PolicyRepository
    Private ReadOnly lblStatus As New Label()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If components Is Nothing Then components = New Container()
        bs = New BindingSource(components)
        bs.DataSource = PolicyQueries.CreateResultsTable()
        repository = New PolicyRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "PolicyValidation.mdb"))

        DataGridView1.AutoGenerateColumns = False
        DataGridView1.Columns.Clear()
        DataGridView1.ReadOnly = True
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.AllowUserToOrderColumns = True
        DataGridView1.RowHeadersVisible = False
        DataGridView1.MultiSelect = False
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.DefaultCellStyle.NullValue = ""
        AddColumn("clientno", "Client No.", 70)
        AddColumn("title", "Title", 45)
        AddColumn("fname", "First Name", 95)
        AddColumn("surname", "Surname", 105)
        AddColumn("policyno", "Policy No.", 75)
        AddColumn("policyname", "Policy Name", 200)
        AddColumn("premium", "Premium (R)", 90)
        DataGridView1.Columns("premium").DefaultCellStyle.Format = "N2"
        DataGridView1.Columns("premium").DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("en-ZA")
        DataGridView1.Columns("premium").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DataGridView1.DataSource = bs
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right

        lblStatus.Location = New Point(23, 353)
        lblStatus.Size = New Size(793, 30)
        lblStatus.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        lblStatus.Text = "Enter a policy number to list its clients."
        Me.Controls.Add(lblStatus)
        Me.MinimumSize = Me.Size
        txtPolicyNo.TabIndex = 0
        btnListClients.TabIndex = 1
        DataGridView1.TabIndex = 2
        txtPolicyNo.MaxLength = 32
        Me.AcceptButton = btnListClients
        Me.ActiveControl = txtPolicyNo
    End Sub

    Private Sub AddColumn(field As String, header As String, weight As Single)
        Dim column As New DataGridViewTextBoxColumn()
        column.Name = field
        column.DataPropertyName = field
        column.HeaderText = header
        column.FillWeight = weight
        column.MinimumWidth = 45
        DataGridView1.Columns.Add(column)
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnListClients.Click
        ClearResults()
        Dim policyNumber As Integer
        If Not PolicyQueries.TryParsePolicyNumber(txtPolicyNo.Text, policyNumber) Then
            lblStatus.Text = "Enter a positive whole policy number."
            MessageBox.Show(Me, "Enter a positive whole policy number, for example 20001.", "Check policy number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPolicyNo.Focus()
            txtPolicyNo.SelectAll()
            Return
        End If

        btnListClients.Enabled = False
        Me.UseWaitCursor = True
        lblStatus.Text = "Searching..."
        Try
            Dim result As PolicySearchResult = repository.FindClients(policyNumber)
            bs.DataSource = result.Clients
            If Not result.PolicyExists Then
                lblStatus.Text = "Policy " & policyNumber.ToString() & " was not found."
            ElseIf result.Clients.Rows.Count = 0 Then
                lblStatus.Text = "Policy " & policyNumber.ToString() & " exists, but has no associated clients."
            Else
                Dim count As Integer = result.Clients.Rows.Count
                lblStatus.Text = count.ToString() & If(count = 1, " client", " clients") & " found for policy " & policyNumber.ToString() & "."
                DataGridView1.ClearSelection()
            End If
        Catch ex As FileNotFoundException
            ShowDatabaseError("The supplied database is missing. Restore Database\PolicyValidation.mdb beside the application, or rebuild the solution to copy it again.")
        Catch ex As InvalidOperationException
            ShowDatabaseError(ex.Message)
        Catch ex As OleDbException
            ShowDatabaseError("The database could not be read. Check that PolicyValidation.mdb is a valid Access database and is not exclusively locked by another program.")
        Catch ex As UnauthorizedAccessException
            ShowDatabaseError("The database cannot be accessed. Check that your Windows account has permission to read PolicyValidation.mdb.")
        Catch ex As IOException
            ShowDatabaseError("The database file could not be read. Check that it is available and try again.")
        Finally
            Me.UseWaitCursor = False
            btnListClients.Enabled = True
        End Try
    End Sub

    Private Sub ClearResults()
        If bs IsNot Nothing Then bs.DataSource = PolicyQueries.CreateResultsTable()
    End Sub

    Private Sub ShowDatabaseError(message As String)
        ClearResults()
        lblStatus.Text = "The search could not be completed."
        MessageBox.Show(Me, message, "Policy lookup", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub txtPolicyNo_TextChanged(sender As Object, e As EventArgs) Handles txtPolicyNo.TextChanged
        ClearResults()
        lblStatus.Text = "Enter a policy number to list its clients."
    End Sub
End Class
