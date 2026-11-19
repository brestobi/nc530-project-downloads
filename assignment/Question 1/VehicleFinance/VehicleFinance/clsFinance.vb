Option Strict On
Option Explicit On

Imports System

'Provisional rules until the question paper is supplied:
'20% of vehicle price as a balloon; nominal annual rate / 12;
'equal end-of-month payments on a reducing balance, with no extra fees.
Public Class clsFinance
    Public Const BalloonPercentage As Decimal = 20D

    Private ReadOnly _vehiclePrice As Decimal
    Private ReadOnly _deposit As Decimal
    Private ReadOnly _applyBalloon As Boolean
    Private ReadOnly _paymentMonths As Integer
    Private ReadOnly _annualInterestRate As Decimal

    Public Sub New(vehiclePrice As Decimal, deposit As Decimal, applyBalloon As Boolean,
                   paymentMonths As Integer, annualInterestRate As Decimal)
        If vehiclePrice <= 0D Then Throw New ArgumentOutOfRangeException("vehiclePrice", "The vehicle price must be greater than zero.")
        If deposit < 0D OrElse deposit > vehiclePrice Then
            Throw New ArgumentOutOfRangeException("deposit", "The deposit must be between zero and the vehicle price.")
        End If
        If vehiclePrice <> RoundMoney(vehiclePrice) OrElse deposit <> RoundMoney(deposit) Then
            Throw New ArgumentException("Enter the vehicle price and deposit with no more than two decimal places.")
        End If
        If Not IsSupportedPeriod(paymentMonths) Then
            Throw New ArgumentOutOfRangeException("paymentMonths", "Choose a payment period: 24, 36, 48, 60 or 72 months.")
        End If
        If annualInterestRate < 0D OrElse annualInterestRate > 100D Then
            Throw New ArgumentOutOfRangeException("annualInterestRate", "The annual interest rate must be between 0 and 100 percent.")
        End If

        _vehiclePrice = vehiclePrice
        _deposit = deposit
        _applyBalloon = applyBalloon
        _paymentMonths = paymentMonths
        _annualInterestRate = annualInterestRate

        If BalloonAmount > AmountFinanced Then
            Throw New ArgumentException("The deposit leaves less to finance than the balloon payment. Reduce the deposit or choose NO for the balloon option.")
        End If
    End Sub

    Public ReadOnly Property VehiclePrice As Decimal
        Get
            Return _vehiclePrice
        End Get
    End Property

    Public ReadOnly Property Deposit As Decimal
        Get
            Return _deposit
        End Get
    End Property

    Public ReadOnly Property ApplyBalloon As Boolean
        Get
            Return _applyBalloon
        End Get
    End Property

    Public ReadOnly Property PaymentMonths As Integer
        Get
            Return _paymentMonths
        End Get
    End Property

    Public ReadOnly Property AnnualInterestRate As Decimal
        Get
            Return _annualInterestRate
        End Get
    End Property

    Public ReadOnly Property AmountFinanced As Decimal
        Get
            Return VehiclePrice - Deposit
        End Get
    End Property

    Public ReadOnly Property BalloonAmount As Decimal
        Get
            If Not ApplyBalloon Then Return 0D
            Return RoundMoney(VehiclePrice * (BalloonPercentage / 100D))
        End Get
    End Property

    Public Function CalculateMonthlyInstallment() As Decimal
        Dim installment As Decimal = RoundMoney(UnroundedInstallment())
        'For very small amounts, rounding up could repay the loan too early.
        If FinalUnroundedInstallment(installment) < 0D AndAlso installment > 0D Then
            installment -= 0.01D
        End If
        Return installment
    End Function

    'Recalculate the remaining balance after the rounded regular payments.
    'This includes the interest effect of cent rounding in earlier months.
    'The balloon is an additional payment, not part of this installment.
    Public ReadOnly Property FinalInstallment As Decimal
        Get
            Return Math.Max(0D, RoundMoney(FinalUnroundedInstallment(CalculateMonthlyInstallment())))
        End Get
    End Property

    Public ReadOnly Property TotalLoanRepayment As Decimal
        Get
            Return CalculateMonthlyInstallment() * (PaymentMonths - 1) + FinalInstallment + BalloonAmount
        End Get
    End Property

    Public ReadOnly Property TotalInterest As Decimal
        Get
            Return TotalLoanRepayment - AmountFinanced
        End Get
    End Property

    Public ReadOnly Property TotalPaid As Decimal
        Get
            Return Deposit + TotalLoanRepayment
        End Get
    End Property

    Private Function FinalUnroundedInstallment(regularInstallment As Decimal) As Decimal
        Dim balance As Decimal = AmountFinanced
        Dim monthlyRate As Decimal = AnnualInterestRate / 1200D
        For month As Integer = 1 To PaymentMonths - 1
            balance = balance * (1D + monthlyRate) - regularInstallment
        Next
        Return balance * (1D + monthlyRate) - BalloonAmount
    End Function

    Private Function UnroundedInstallment() As Decimal
        If AmountFinanced = 0D Then Return 0D

        Dim monthlyRate As Decimal = AnnualInterestRate / 1200D
        If monthlyRate = 0D Then Return (AmountFinanced - BalloonAmount) / PaymentMonths

        'P = payment * (v + v^2 + ... + v^n) + balloon * v^n,
        'where v = 1 / (1 + monthlyRate). Decimal arithmetic also handles tiny rates.
        'The balloon remains financed and therefore accrues interest until the end.
        Dim discount As Decimal = 1D / (1D + monthlyRate)
        Dim discountPower As Decimal = 1D
        Dim annuityFactor As Decimal = 0D
        For month As Integer = 1 To PaymentMonths
            discountPower *= discount
            annuityFactor += discountPower
        Next
        Return (AmountFinanced - BalloonAmount * discountPower) / annuityFactor
    End Function

    Public Shared Function IsSupportedPeriod(months As Integer) As Boolean
        Select Case months
            Case 24, 36, 48, 60, 72
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Shared Function RoundMoney(value As Decimal) As Decimal
        Return Decimal.Round(value, 2, MidpointRounding.AwayFromZero)
    End Function
End Class
