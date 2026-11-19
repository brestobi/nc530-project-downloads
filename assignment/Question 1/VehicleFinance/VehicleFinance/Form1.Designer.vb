<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtIntRate = New System.Windows.Forms.TextBox()
        Me.cmbPayPeriod = New System.Windows.Forms.ComboBox()
        Me.cmbBalloon = New System.Windows.Forms.ComboBox()
        Me.txtDeposit = New System.Windows.Forms.TextBox()
        Me.txtCarPrice = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lstDisplay = New System.Windows.Forms.ListBox()
        Me.btnCalculate = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtIntRate)
        Me.GroupBox1.Controls.Add(Me.cmbPayPeriod)
        Me.GroupBox1.Controls.Add(Me.cmbBalloon)
        Me.GroupBox1.Controls.Add(Me.txtDeposit)
        Me.GroupBox1.Controls.Add(Me.txtCarPrice)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 75)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(366, 239)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Enter Vehicle Details:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(14, 164)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(114, 16)
        Me.Label10.TabIndex = 12
        Me.Label10.Text = "Interest Rate (p/a)"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(14, 132)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(158, 16)
        Me.Label9.TabIndex = 11
        Me.Label9.Text = "Payment Period (months)"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(12, 64)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(76, 16)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "Deposit (R)"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(14, 96)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(141, 16)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Apply Balloon Option?"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(12, 34)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(108, 16)
        Me.Label6.TabIndex = 8
        Me.Label6.Text = "Vehicle Price (R)"
        '
        'txtIntRate
        '
        Me.txtIntRate.Location = New System.Drawing.Point(177, 160)
        Me.txtIntRate.Name = "txtIntRate"
        Me.txtIntRate.Size = New System.Drawing.Size(144, 20)
        Me.txtIntRate.TabIndex = 7
        '
        'cmbPayPeriod
        '
        Me.cmbPayPeriod.FormattingEnabled = True
        Me.cmbPayPeriod.Items.AddRange(New Object() {"24", "36", "48", "60", "72"})
        Me.cmbPayPeriod.Location = New System.Drawing.Point(177, 127)
        Me.cmbPayPeriod.Name = "cmbPayPeriod"
        Me.cmbPayPeriod.Size = New System.Drawing.Size(144, 21)
        Me.cmbPayPeriod.TabIndex = 6
        '
        'cmbBalloon
        '
        Me.cmbBalloon.FormattingEnabled = True
        Me.cmbBalloon.Items.AddRange(New Object() {"YES", "NO"})
        Me.cmbBalloon.Location = New System.Drawing.Point(177, 96)
        Me.cmbBalloon.Name = "cmbBalloon"
        Me.cmbBalloon.Size = New System.Drawing.Size(144, 21)
        Me.cmbBalloon.TabIndex = 5
        '
        'txtDeposit
        '
        Me.txtDeposit.Location = New System.Drawing.Point(180, 60)
        Me.txtDeposit.Name = "txtDeposit"
        Me.txtDeposit.Size = New System.Drawing.Size(141, 20)
        Me.txtDeposit.TabIndex = 4
        '
        'txtCarPrice
        '
        Me.txtCarPrice.Location = New System.Drawing.Point(180, 30)
        Me.txtCarPrice.Name = "txtCarPrice"
        Me.txtCarPrice.Size = New System.Drawing.Size(141, 20)
        Me.txtCarPrice.TabIndex = 3
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lstDisplay)
        Me.GroupBox2.Location = New System.Drawing.Point(406, 75)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(221, 239)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Finance Summary"
        '
        'lstDisplay
        '
        Me.lstDisplay.FormattingEnabled = True
        Me.lstDisplay.Location = New System.Drawing.Point(6, 18)
        Me.lstDisplay.Name = "lstDisplay"
        Me.lstDisplay.Size = New System.Drawing.Size(209, 212)
        Me.lstDisplay.TabIndex = 0
        '
        'btnCalculate
        '
        Me.btnCalculate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCalculate.Location = New System.Drawing.Point(12, 333)
        Me.btnCalculate.Name = "btnCalculate"
        Me.btnCalculate.Size = New System.Drawing.Size(240, 49)
        Me.btnCalculate.TabIndex = 2
        Me.btnCalculate.Text = "Calculate Monthly Installment"
        Me.btnCalculate.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(258, 333)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(120, 49)
        Me.btnClose.TabIndex = 3
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(14, 23)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(257, 29)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Grand Vehicle Finance"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(666, 416)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnCalculate)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.Text = "Enter Your EXAMINATION NUMBER "
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents lstDisplay As System.Windows.Forms.ListBox
    Friend WithEvents btnCalculate As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtIntRate As System.Windows.Forms.TextBox
    Friend WithEvents cmbPayPeriod As System.Windows.Forms.ComboBox
    Friend WithEvents cmbBalloon As System.Windows.Forms.ComboBox
    Friend WithEvents txtDeposit As System.Windows.Forms.TextBox
    Friend WithEvents txtCarPrice As System.Windows.Forms.TextBox

End Class
