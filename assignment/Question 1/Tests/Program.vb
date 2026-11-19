Imports System
Imports Microsoft.VisualBasic

Module Program
    Private assertions As Integer

    Sub Main()
        Dim standard As New clsFinance(200000D, 20000D, False, 60, 12D)
        AssertEqual(4004D, standard.CalculateMonthlyInstallment(), "Known repayment without balloon")
        Dim balloon As New clsFinance(200000D, 20000D, True, 60, 12D)
        AssertEqual(40000D, balloon.BalloonAmount, "Balloon is 20% of vehicle price")
        AssertTrue(balloon.CalculateMonthlyInstallment() < standard.CalculateMonthlyInstallment(), "Balloon lowers monthly payment")
        AssertTrue(balloon.TotalInterest > standard.TotalInterest, "Deferred principal accrues interest")

        Dim zeroRate As New clsFinance(120000D, 0D, False, 24, 0D)
        AssertEqual(5000D, zeroRate.CalculateMonthlyInstallment(), "Zero interest payment")
        AssertEqual(0D, zeroRate.TotalInterest, "Zero interest total")
        Dim zeroBalloon As New clsFinance(120000D, 0D, True, 24, 0D)
        AssertEqual(4000D, zeroBalloon.CalculateMonthlyInstallment(), "Zero interest balloon payment")
        AssertEqual(0D, zeroBalloon.TotalInterest, "Zero interest balloon total")
        Dim fullDeposit As New clsFinance(100000D, 100000D, False, 36, 12D)
        AssertEqual(0D, fullDeposit.TotalLoanRepayment, "Fully paid vehicle has no loan")
        AssertEqual(100000D, fullDeposit.TotalPaid, "Deposit remains in overall cost")
        Dim interestOnly As New clsFinance(200000D, 160000D, True, 60, 12D)
        AssertEqual(400D, interestOnly.CalculateMonthlyInstallment(), "Interest-only payments when principal equals balloon")
        AssertEqual(40000D, interestOnly.BalloonAmount, "Interest-only principal paid at end")
        Dim tinyRate As New clsFinance(120000D, 0D, False, 24, 0.000000000001D)
        AssertEqual(5000D, tinyRate.CalculateMonthlyInstallment(), "Tiny nonzero rate remains stable")

        ExpectInvalid(Sub() NewFinance(0D, 0D, False, 24, 12D), "Zero price")
        ExpectInvalid(Sub() NewFinance(-1D, 0D, False, 24, 12D), "Negative price")
        ExpectInvalid(Sub() NewFinance(100D, -1D, False, 24, 12D), "Negative deposit")
        ExpectInvalid(Sub() NewFinance(100D, 101D, False, 24, 12D), "Deposit exceeds price")
        ExpectInvalid(Sub() NewFinance(100D, 81D, True, 24, 12D), "Balloon exceeds financed balance")
        ExpectInvalid(Sub() NewFinance(100D, 100D, True, 24, 12D), "Fully paid vehicle cannot have balloon")
        ExpectInvalid(Sub() NewFinance(100D, 0D, False, 0, 12D), "Zero term")
        ExpectInvalid(Sub() NewFinance(100D, 0D, False, 25, 12D), "Unsupported term")
        ExpectInvalid(Sub() NewFinance(100D, 0D, False, 24, -1D), "Negative rate")
        ExpectInvalid(Sub() NewFinance(100D, 0D, False, 24, 101D), "Rate exceeds supported range")
        ExpectInvalid(Sub() NewFinance(100.001D, 0D, False, 24, 12D), "Fractional cent price")
        ExpectInvalid(Sub() NewFinance(100D, 0.001D, False, 24, 12D), "Fractional cent deposit")

        For Each months As Integer In New Integer() {24, 36, 48, 60, 72}
            For Each rate As Decimal In New Decimal() {0D, 0.1D, 7.5D, 12D, 100D}
                For Each useBalloon As Boolean In New Boolean() {False, True}
                    Dim quote As New clsFinance(250000D, 25000D, useBalloon, months, rate)
                    'Independent implementation in the Visual Basic financial library.
                    Dim expected As Decimal = Decimal.Round(CDec(-Financial.Pmt(CDbl(rate / 1200D), months,
                        CDbl(quote.AmountFinanced), -CDbl(quote.BalloonAmount))), 2, MidpointRounding.AwayFromZero)
                    AssertEqual(expected, quote.CalculateMonthlyInstallment(), "Matches Financial.Pmt")
                    VerifySchedule(quote)
                Next
            Next
            For Each price As Decimal In New Decimal() {0.01D, 0.12D, 0.24D, 1D, 99.99D, 200000D}
                For Each rate As Decimal In New Decimal() {0D, 12D, 100D}
                    VerifySchedule(New clsFinance(price, 0D, False, months, rate))
                    VerifySchedule(New clsFinance(price, 0D, True, months, rate))
                Next
            Next
        Next
        Console.WriteLine("PASS: " & assertions.ToString() & " assertions, including independent PMT comparisons, payment schedules and invalid inputs.")
    End Sub

    Private Sub NewFinance(price As Decimal, deposit As Decimal, balloon As Boolean, months As Integer, rate As Decimal)
        Dim quote As New clsFinance(price, deposit, balloon, months, rate)
    End Sub

    Private Sub VerifySchedule(quote As clsFinance)
        Dim balance As Decimal = quote.AmountFinanced
        Dim monthly As Decimal = quote.CalculateMonthlyInstallment()
        For month As Integer = 1 To quote.PaymentMonths - 1
            balance = balance * (1D + quote.AnnualInterestRate / 1200D) - monthly
            AssertTrue(balance >= -0.005D, "Loan does not overpay before final month")
        Next
        balance = balance * (1D + quote.AnnualInterestRate / 1200D) - quote.FinalInstallment - quote.BalloonAmount
        AssertTrue(Math.Abs(balance) <= 0.005D, "Final installment plus balloon clears balance within half a cent")
        AssertTrue(monthly >= 0D AndAlso quote.FinalInstallment >= 0D, "No negative installments")
        AssertTrue(quote.TotalInterest >= 0D, "No negative interest")
        AssertEqual(monthly * (quote.PaymentMonths - 1) + quote.FinalInstallment + quote.BalloonAmount,
                    quote.TotalLoanRepayment, "Displayed payments reconcile with loan total")
        AssertEqual(quote.TotalLoanRepayment - quote.AmountFinanced, quote.TotalInterest, "Interest reconciles")
        AssertEqual(quote.Deposit + quote.TotalLoanRepayment, quote.TotalPaid, "Overall cost includes deposit")
    End Sub

    Private Sub ExpectInvalid(action As Action, label As String)
        Try
            action()
        Catch ex As ArgumentException
            assertions += 1
            Return
        End Try
        Throw New Exception("FAIL: " & label & " was accepted.")
    End Sub

    Private Sub AssertEqual(expected As Decimal, actual As Decimal, label As String)
        AssertTrue(expected = actual, label & ": expected " & expected.ToString() & ", got " & actual.ToString())
    End Sub

    Private Sub AssertTrue(condition As Boolean, label As String)
        If Not condition Then Throw New Exception("FAIL: " & label)
        assertions += 1
    End Sub
End Module
