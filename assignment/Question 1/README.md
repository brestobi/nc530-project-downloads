# Question 1: Grand Vehicle Finance

The supplied Visual Basic Windows Forms starter is now implemented. This is a working provisional solution: the question paper has not yet been supplied, so the exact marking requirements remain unconfirmed.

## Open and run

1. On Windows, open `VehicleFinance/VehicleFinance/VehicleFinance.sln` in Visual Studio 2010 (or Visual Basic 2010 Express), with Visual Basic, Windows Forms and .NET Framework 4.0 development tools installed.
2. Build the solution, then press F5 to run it.
3. Enter the vehicle price, deposit, balloon option, repayment period and annual interest percentage. Click **Calculate Monthly Installment**, or press Enter.
4. Click **Close**, or press Escape, to exit.

A compiled Windows application is also included at `VehicleFinance/VehicleFinance/bin/Release/SalesSystems.exe`. Keep its adjacent `.config` file with it. The executable retains the starter project's original assembly name.

Use plain numbers such as `200000` or `200000.00`. Do not enter `R`, grouping separators, or a `%` symbol. Enter `12` for 12% annually. Price and deposit may have at most two decimal places. A zero deposit and zero interest rate are supported.

The window title still contains the supplied examination-number placeholder. Replace it with your examination number in the form designer when preparing your submission.

## Provisional calculation rules

These rules are assumptions, not requirements recovered from the missing paper:

- The amount financed is vehicle price minus deposit.
- Repayment periods are 24, 36, 48, 60 or 72 months, matching the supplied dropdown.
- Selecting YES sets the balloon to 20% of the original vehicle price, rounded to cents. Change `BalloonPercentage` in `clsFinance.vb` if the paper specifies a different percentage.
- The balloon cannot exceed the amount financed. If a large deposit causes this, reduce the deposit or select NO.
- Interest uses a reducing balance, with a fixed nominal annual percentage divided by 1200 to obtain the monthly rate. Payments fall at the end of each month. The balloon remains outstanding and accrues interest until the end.
- The supported annual interest range is 0% to 100%.
- No initiation fees, insurance, service fees, or other charges are added.
- A deposit equal to the full price is accepted when the balloon option is NO; all loan amounts are then zero.

Let `P` be the amount financed, `B` the balloon, `n` the number of months, and `r` the monthly rate. For positive interest, let `v = 1 / (1 + r)`. The unrounded installment is:

```
M = (P - B * v^n) / (v + v^2 + ... + v^n)
```

At zero interest, `M = (P - B) / n`.

Monthly installments are rounded to cents, away from zero at a midpoint. The last installment is recalculated from the remaining balance after the first `n - 1` rounded payments. This accounts for the interest effect of earlier cent rounding. For very small loans, the regular installment is reduced by one cent when rounding up would cause overpayment before the end.

The balloon is additional to the last installment. The summary explicitly shows the final month total, total interest, total loan repayment, and overall amount including the deposit. These totals reconcile with the actual displayed payment schedule.

If the paper requires simple interest, a different balloon basis or percentage, particular class members, or particular summary lines, update this provisional solution to match it before submission.

## Worked example

Inputs: vehicle price **R 200,000**, deposit **R 20,000**, period **60 months**, annual interest **12%**.

| Output | Balloon NO | Balloon YES |
| --- | ---: | ---: |
| Amount financed | R 180,000.00 | R 180,000.00 |
| Monthly installment (first 59 months) | R 4,004.00 | R 3,514.22 |
| Last regular installment | R 4,004.05 | R 3,514.44 |
| Extra balloon payment at end | R 0.00 | R 40,000.00 |
| Final month total | R 4,004.05 | R 43,514.44 |
| Total interest | R 60,240.05 | R 70,853.42 |
| Total loan repayment | R 240,240.05 | R 250,853.42 |
| Total including deposit | R 260,240.05 | R 270,853.42 |

A balloon lowers the regular installment but leaves more principal outstanding, so the total interest is higher under these assumptions.

## Code guide

- `VehicleFinance/VehicleFinance/Form1.vb`: form initialization, input validation, Calculate and Close handlers, and summary display. Editing inputs clears the previous quote.
- `VehicleFinance/VehicleFinance/clsFinance.vb`: validated finance object and all calculation rules. The class has no dependency on the Windows form and can be tested independently.
- `VehicleFinance/VehicleFinance/Form1.Designer.vb`: supplied form controls, unchanged. Small layout adjustments happen when the form loads so the whole summary is readable.
- `Tests/Program.vb`: standalone Visual Basic calculation tests, with no external testing packages.
- `Question 1 - original.zip`: backup of the supplied Question 1 project before changes.

## Verification

The converted .NET Framework 4.0 Windows Forms project compiled successfully in Release configuration with Microsoft's framework reference assemblies and Visual Basic 2010 syntax restrictions. The actual Windows interface has not been launched in this Linux workspace.

The finance tests passed **12,265 assertions**. They cover:

- Comparison with Visual Basic's independent `Financial.Pmt` function across all five allowed terms, with and without balloons, at rates from 0% to 100%.
- Monthly balance simulation showing that the final installment and balloon clear the loan within half a cent, and that the totals reconcile.
- Zero and tiny nonzero interest rates, full deposits, interest-only balloon cases, and loans as small as one cent.
- Invalid prices, deposits, balloon/deposit combinations, repayment terms, rates, and fractional-cent amounts.

The optional test project is also a Visual Studio 2010 / .NET Framework 4.0 console project. In a Visual Studio 2010 Command Prompt, from this Question 1 folder, run:

```bat
msbuild Tests\FinanceTests.vbproj /p:Configuration=Release
Tests\bin\Release\FinanceTests.exe
```

After conversion, the application and test project compiled against .NET Framework 4.0 reference assemblies with the compiler restricted to Visual Basic 2010 syntax. The converted test executable passed the same 12,265 assertions on the available .NET 10 runtime. Visual Studio 2010 itself and the Windows UI are not available in this Linux workspace, so a native IDE/runtime check remains to be done on Windows.

For a manual Windows interface check: try the worked example with each balloon choice; blank or nonnumeric inputs; a negative or excessive deposit; a rate of 0; a deposit above 80% with balloon YES; editing inputs after calculating; Enter to calculate; and Escape to close.

Reference for the independent payment-function check: [Microsoft Visual Basic Financial.Pmt documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualbasic.financial.pmt?view=net-10.0).
