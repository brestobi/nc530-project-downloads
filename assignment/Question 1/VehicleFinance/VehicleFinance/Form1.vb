Option Strict On
Option Explicit On

Imports System.Globalization

Public Class Form1
    Private Shared ReadOnly RandCulture As CultureInfo = CultureInfo.GetCultureInfo("en-ZA")

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Keep the supplied controls, with enough room to read the complete summary.
        Me.ClientSize = New Size(820, 450)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        GroupBox1.Height = 270
        GroupBox2.Size = New Size(390, 310)
        lstDisplay.Size = New Size(378, 277)
        lstDisplay.HorizontalScrollbar = True
        btnCalculate.Top = 400
        btnClose.Top = 400
        btnCalculate.Height = 36
        btnClose.Height = 36

        cmbBalloon.DropDownStyle = ComboBoxStyle.DropDownList
        cmbPayPeriod.DropDownStyle = ComboBoxStyle.DropDownList
        cmbBalloon.SelectedItem = "NO"
        cmbPayPeriod.SelectedItem = "60"
        txtDeposit.Text = "0"
        Label10.Text = "Interest Rate (% p/a)"

        txtCarPrice.TabIndex = 0
        txtDeposit.TabIndex = 1
        cmbBalloon.TabIndex = 2
        cmbPayPeriod.TabIndex = 3
        txtIntRate.TabIndex = 4
        GroupBox2.TabStop = False
        lstDisplay.TabStop = False
        Me.AcceptButton = btnCalculate
        Me.CancelButton = btnClose

        Dim guidance As New Label()
        guidance.AutoSize = False
        guidance.Location = New Point(14, 195)
        guidance.Size = New Size(340, 60)
        guidance.Text = "Use numbers such as 250000.00 (no R or separators)." & Environment.NewLine &
                        "Balloon: " & clsFinance.BalloonPercentage.ToString("0.##", CultureInfo.InvariantCulture) &
                        "% of vehicle price, due at the end."
        GroupBox1.Controls.Add(guidance)
        Me.ActiveControl = txtCarPrice
    End Sub

    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        'Never leave an old quote on screen after a failed calculation.
        lstDisplay.Items.Clear()
        Dim price As Decimal
        Dim deposit As Decimal
        Dim annualRate As Decimal
        Dim months As Integer

        If Not TryReadNumber(txtCarPrice, "Enter a positive vehicle price, for example 250000.00.", price) Then Return
        If price <= 0D Then
            ShowInputError(txtCarPrice, "The vehicle price must be greater than zero.")
            Return
        End If

        If price <> Decimal.Round(price, 2) Then
            ShowInputError(txtCarPrice, "Enter the vehicle price with no more than two decimal places.")
            Return
        End If

        If Not TryReadNumber(txtDeposit, "Enter a deposit, or 0 if there is no deposit.", deposit) Then Return
        If deposit < 0D OrElse deposit > price Then
            ShowInputError(txtDeposit, "The deposit must be between zero and the vehicle price.")
            Return
        End If

        If deposit <> Decimal.Round(deposit, 2) Then
            ShowInputError(txtDeposit, "Enter the deposit with no more than two decimal places.")
            Return
        End If

        If cmbBalloon.SelectedIndex < 0 Then
            ShowInputError(cmbBalloon, "Choose YES or NO for the balloon option.")
            Return
        End If
        If Not Integer.TryParse(cmbPayPeriod.Text, months) OrElse Not clsFinance.IsSupportedPeriod(months) Then
            ShowInputError(cmbPayPeriod, "Choose a payment period: 24, 36, 48, 60 or 72 months.")
            Return
        End If

        If Not TryReadNumber(txtIntRate, "Enter the annual interest percentage, for example 12 or 12.5.", annualRate) Then Return
        If annualRate < 0D OrElse annualRate > 100D Then
            ShowInputError(txtIntRate, "The annual interest rate must be between 0 and 100 percent.")
            Return
        End If

        Try
            Dim finance As New clsFinance(price, deposit, cmbBalloon.Text = "YES", months, annualRate)
            DisplaySummary(finance)
        Catch ex As ArgumentException
            ShowInputError(txtDeposit, ex.Message)
        Catch ex As OverflowException
            ShowInputError(txtCarPrice, "These amounts are too large. Enter smaller amounts and try again.")
        End Try
    End Sub

    Private Function TryReadNumber(input As TextBox, message As String, ByRef value As Decimal) As Boolean
        Dim styles As NumberStyles = NumberStyles.AllowLeadingSign Or NumberStyles.AllowDecimalPoint
        If Decimal.TryParse(input.Text.Trim(), styles, CultureInfo.InvariantCulture, value) Then Return True
        ShowInputError(input, message & Environment.NewLine & "Use a decimal point; do not include R or grouping separators.")
        Return False
    End Function

    Private Sub ShowInputError(input As Control, message As String)
        MessageBox.Show(Me, message, "Check vehicle details", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        input.Focus()
        Dim textInput As TextBox = TryCast(input, TextBox)
        If textInput IsNot Nothing Then textInput.SelectAll()
    End Sub

    Private Sub DisplaySummary(finance As clsFinance)
        Dim monthly As Decimal = finance.CalculateMonthlyInstallment()
        Dim finalPayment As Decimal = finance.FinalInstallment
        Dim balloon As Decimal = finance.BalloonAmount
        Dim interest As Decimal = finance.TotalInterest
        Dim loanTotal As Decimal = finance.TotalLoanRepayment
        Dim overallTotal As Decimal = finance.TotalPaid

        lstDisplay.BeginUpdate()
        Try
            lstDisplay.Items.Clear()
            lstDisplay.Items.Add("Vehicle price: " & Money(finance.VehiclePrice))
            lstDisplay.Items.Add("Deposit: " & Money(finance.Deposit))
            lstDisplay.Items.Add("Amount financed: " & Money(finance.AmountFinanced))
            lstDisplay.Items.Add("Payment period: " & finance.PaymentMonths.ToString() & " months")
            lstDisplay.Items.Add("Annual interest: " & finance.AnnualInterestRate.ToString("0.####", RandCulture) & "%")
            lstDisplay.Items.Add("Balloon option: " & If(finance.ApplyBalloon, "YES", "NO"))
            lstDisplay.Items.Add("")
            lstDisplay.Items.Add("Monthly installment: " & Money(monthly))
            lstDisplay.Items.Add("Final installment: " & Money(finalPayment))
            lstDisplay.Items.Add("Extra balloon due at end: " & Money(balloon))
            lstDisplay.Items.Add("Final month total: " & Money(finalPayment + balloon))
            lstDisplay.Items.Add("")
            lstDisplay.Items.Add("Total interest: " & Money(interest))
            lstDisplay.Items.Add("Total loan repayment: " & Money(loanTotal))
            lstDisplay.Items.Add("Total including deposit: " & Money(overallTotal))
        Finally
            lstDisplay.EndUpdate()
        End Try
    End Sub

    Private Shared Function Money(value As Decimal) As String
        Return "R " & value.ToString("N2", RandCulture)
    End Function

    Private Sub VehicleDetailsChanged(sender As Object, e As EventArgs) _
        Handles txtCarPrice.TextChanged, txtDeposit.TextChanged, txtIntRate.TextChanged,
                cmbBalloon.SelectedIndexChanged, cmbPayPeriod.SelectedIndexChanged
        lstDisplay.Items.Clear()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class
