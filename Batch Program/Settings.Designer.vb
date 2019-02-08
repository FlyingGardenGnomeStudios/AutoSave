<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Settings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Settings))
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnDefaults = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbProjects = New System.Windows.Forms.ComboBox()
        Me.cmbTime = New System.Windows.Forms.ComboBox()
        Me.numInt = New System.Windows.Forms.NumericUpDown()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.gbSaveLoc = New System.Windows.Forms.GroupBox()
        Me.rdoKeepEverything = New System.Windows.Forms.RadioButton()
        Me.chkDocLoc = New System.Windows.Forms.CheckBox()
        Me.cmbOlderThan = New System.Windows.Forms.ComboBox()
        Me.numOld = New System.Windows.Forms.NumericUpDown()
        Me.numVer = New System.Windows.Forms.NumericUpDown()
        Me.rdoOlderThan = New System.Windows.Forms.RadioButton()
        Me.rdoVersions = New System.Windows.Forms.RadioButton()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.txtSaveLoc = New System.Windows.Forms.TextBox()
        Me.gbSaveInterval = New System.Windows.Forms.GroupBox()
        Me.chkClean = New System.Windows.Forms.CheckBox()
        Me.ChkAutoSave = New System.Windows.Forms.CheckBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.chkReadOnlySave = New System.Windows.Forms.CheckBox()
        Me.btnClear = New System.Windows.Forms.Button()
        CType(Me.numInt, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbSaveLoc.SuspendLayout()
        CType(Me.numOld, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numVer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbSaveInterval.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(212, 288)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 16
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(131, 288)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 15
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(43, 288)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(82, 23)
        Me.btnDefaults.TabIndex = 14
        Me.btnDefaults.Text = "Load Defaults"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 36)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(102, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Documents to save:"
        '
        'cmbProjects
        '
        Me.cmbProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProjects.Enabled = False
        Me.cmbProjects.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmbProjects.FormattingEnabled = True
        Me.cmbProjects.Items.AddRange(New Object() {"Current Document", "All Open Documents", "Everything"})
        Me.cmbProjects.Location = New System.Drawing.Point(126, 33)
        Me.cmbProjects.Name = "cmbProjects"
        Me.cmbProjects.Size = New System.Drawing.Size(142, 21)
        Me.cmbProjects.TabIndex = 2
        '
        'cmbTime
        '
        Me.cmbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTime.FormattingEnabled = True
        Me.cmbTime.Items.AddRange(New Object() {"Minutes", "Hours", "Days"})
        Me.cmbTime.Location = New System.Drawing.Point(93, 19)
        Me.cmbTime.Name = "cmbTime"
        Me.cmbTime.Size = New System.Drawing.Size(157, 21)
        Me.cmbTime.TabIndex = 4
        '
        'numInt
        '
        Me.numInt.Location = New System.Drawing.Point(46, 19)
        Me.numInt.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        Me.numInt.Name = "numInt"
        Me.numInt.Size = New System.Drawing.Size(41, 20)
        Me.numInt.TabIndex = 3
        Me.numInt.Value = New Decimal(New Integer() {15, 0, 0, 0})
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 21)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(34, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Every"
        '
        'gbSaveLoc
        '
        Me.gbSaveLoc.Controls.Add(Me.rdoKeepEverything)
        Me.gbSaveLoc.Controls.Add(Me.chkDocLoc)
        Me.gbSaveLoc.Controls.Add(Me.cmbOlderThan)
        Me.gbSaveLoc.Controls.Add(Me.numOld)
        Me.gbSaveLoc.Controls.Add(Me.numVer)
        Me.gbSaveLoc.Controls.Add(Me.rdoOlderThan)
        Me.gbSaveLoc.Controls.Add(Me.rdoVersions)
        Me.gbSaveLoc.Controls.Add(Me.btnBrowse)
        Me.gbSaveLoc.Controls.Add(Me.txtSaveLoc)
        Me.gbSaveLoc.Enabled = False
        Me.gbSaveLoc.Location = New System.Drawing.Point(21, 129)
        Me.gbSaveLoc.Name = "gbSaveLoc"
        Me.gbSaveLoc.Size = New System.Drawing.Size(260, 130)
        Me.gbSaveLoc.TabIndex = 10
        Me.gbSaveLoc.TabStop = False
        Me.gbSaveLoc.Text = "Save Location"
        '
        'rdoKeepEverything
        '
        Me.rdoKeepEverything.AutoSize = True
        Me.rdoKeepEverything.Location = New System.Drawing.Point(6, 107)
        Me.rdoKeepEverything.Name = "rdoKeepEverything"
        Me.rdoKeepEverything.Size = New System.Drawing.Size(105, 17)
        Me.rdoKeepEverything.TabIndex = 13
        Me.rdoKeepEverything.Text = "Keep all versions"
        Me.rdoKeepEverything.UseVisualStyleBackColor = True
        '
        'chkDocLoc
        '
        Me.chkDocLoc.AutoSize = True
        Me.chkDocLoc.Location = New System.Drawing.Point(12, 42)
        Me.chkDocLoc.Name = "chkDocLoc"
        Me.chkDocLoc.Size = New System.Drawing.Size(135, 17)
        Me.chkDocLoc.TabIndex = 6
        Me.chkDocLoc.Text = "Use document location"
        Me.chkDocLoc.UseVisualStyleBackColor = True
        '
        'cmbOlderThan
        '
        Me.cmbOlderThan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbOlderThan.Enabled = False
        Me.cmbOlderThan.FormattingEnabled = True
        Me.cmbOlderThan.Items.AddRange(New Object() {"Minutes", "Hours", "Days"})
        Me.cmbOlderThan.Location = New System.Drawing.Point(191, 85)
        Me.cmbOlderThan.Name = "cmbOlderThan"
        Me.cmbOlderThan.Size = New System.Drawing.Size(59, 21)
        Me.cmbOlderThan.TabIndex = 12
        '
        'numOld
        '
        Me.numOld.Enabled = False
        Me.numOld.Location = New System.Drawing.Point(151, 86)
        Me.numOld.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        Me.numOld.Name = "numOld"
        Me.numOld.Size = New System.Drawing.Size(40, 20)
        Me.numOld.TabIndex = 11
        Me.numOld.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'numVer
        '
        Me.numVer.Location = New System.Drawing.Point(110, 65)
        Me.numVer.Name = "numVer"
        Me.numVer.Size = New System.Drawing.Size(41, 20)
        Me.numVer.TabIndex = 9
        Me.numVer.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'rdoOlderThan
        '
        Me.rdoOlderThan.AutoSize = True
        Me.rdoOlderThan.Location = New System.Drawing.Point(6, 86)
        Me.rdoOlderThan.Name = "rdoOlderThan"
        Me.rdoOlderThan.Size = New System.Drawing.Size(139, 17)
        Me.rdoOlderThan.TabIndex = 10
        Me.rdoOlderThan.Text = "Remove files older than:"
        Me.rdoOlderThan.UseVisualStyleBackColor = True
        '
        'rdoVersions
        '
        Me.rdoVersions.AutoSize = True
        Me.rdoVersions.Checked = True
        Me.rdoVersions.Location = New System.Drawing.Point(6, 65)
        Me.rdoVersions.Name = "rdoVersions"
        Me.rdoVersions.Size = New System.Drawing.Size(107, 17)
        Me.rdoVersions.TabIndex = 8
        Me.rdoVersions.TabStop = True
        Me.rdoVersions.Text = "Versions to keep:"
        Me.rdoVersions.UseVisualStyleBackColor = True
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(175, 43)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 7
        Me.btnBrowse.Text = "Browse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'txtSaveLoc
        '
        Me.txtSaveLoc.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.txtSaveLoc.Location = New System.Drawing.Point(12, 20)
        Me.txtSaveLoc.Name = "txtSaveLoc"
        Me.txtSaveLoc.ReadOnly = True
        Me.txtSaveLoc.Size = New System.Drawing.Size(238, 20)
        Me.txtSaveLoc.TabIndex = 5
        '
        'gbSaveInterval
        '
        Me.gbSaveInterval.Controls.Add(Me.Label3)
        Me.gbSaveInterval.Controls.Add(Me.cmbTime)
        Me.gbSaveInterval.Controls.Add(Me.numInt)
        Me.gbSaveInterval.Enabled = False
        Me.gbSaveInterval.Location = New System.Drawing.Point(21, 75)
        Me.gbSaveInterval.Name = "gbSaveInterval"
        Me.gbSaveInterval.Size = New System.Drawing.Size(260, 48)
        Me.gbSaveInterval.TabIndex = 11
        Me.gbSaveInterval.TabStop = False
        Me.gbSaveInterval.Text = "Save Interval"
        '
        'chkClean
        '
        Me.chkClean.AutoSize = True
        Me.chkClean.Checked = True
        Me.chkClean.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkClean.Enabled = False
        Me.chkClean.Location = New System.Drawing.Point(21, 265)
        Me.chkClean.Name = "chkClean"
        Me.chkClean.Size = New System.Drawing.Size(206, 17)
        Me.chkClean.TabIndex = 13
        Me.chkClean.Text = "Remove saved files after manual save"
        Me.chkClean.UseVisualStyleBackColor = True
        '
        'ChkAutoSave
        '
        Me.ChkAutoSave.AutoSize = True
        Me.ChkAutoSave.Location = New System.Drawing.Point(21, 13)
        Me.ChkAutoSave.Name = "ChkAutoSave"
        Me.ChkAutoSave.Size = New System.Drawing.Size(109, 17)
        Me.ChkAutoSave.TabIndex = 1
        Me.ChkAutoSave.Text = "Enable AutoSave"
        Me.ChkAutoSave.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(16, 288)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(21, 23)
        Me.Button1.TabIndex = 17
        Me.Button1.Text = "V"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(16, 329)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLog.Size = New System.Drawing.Size(271, 110)
        Me.txtLog.TabIndex = 18
        '
        'chkReadOnlySave
        '
        Me.chkReadOnlySave.AutoSize = True
        Me.chkReadOnlySave.Location = New System.Drawing.Point(30, 55)
        Me.chkReadOnlySave.Name = "chkReadOnlySave"
        Me.chkReadOnlySave.Size = New System.Drawing.Size(162, 17)
        Me.chkReadOnlySave.TabIndex = 19
        Me.chkReadOnlySave.Text = "Include read only documents"
        Me.chkReadOnlySave.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(212, 445)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(75, 23)
        Me.btnClear.TabIndex = 20
        Me.btnClear.Text = "Clear"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'Settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(296, 322)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.chkReadOnlySave)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ChkAutoSave)
        Me.Controls.Add(Me.chkClean)
        Me.Controls.Add(Me.gbSaveInterval)
        Me.Controls.Add(Me.gbSaveLoc)
        Me.Controls.Add(Me.cmbProjects)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(312, 510)
        Me.MinimumSize = New System.Drawing.Size(312, 360)
        Me.Name = "Settings"
        Me.Text = "Settings"
        CType(Me.numInt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbSaveLoc.ResumeLayout(False)
        Me.gbSaveLoc.PerformLayout()
        CType(Me.numOld, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numVer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbSaveInterval.ResumeLayout(False)
        Me.gbSaveInterval.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnDefaults As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbProjects As System.Windows.Forms.ComboBox
    Friend WithEvents cmbTime As System.Windows.Forms.ComboBox
    Friend WithEvents numInt As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents gbSaveLoc As System.Windows.Forms.GroupBox
    Friend WithEvents cmbOlderThan As System.Windows.Forms.ComboBox
    Friend WithEvents numOld As System.Windows.Forms.NumericUpDown
    Friend WithEvents numVer As System.Windows.Forms.NumericUpDown
    Friend WithEvents rdoOlderThan As System.Windows.Forms.RadioButton
    Friend WithEvents rdoVersions As System.Windows.Forms.RadioButton
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents txtSaveLoc As System.Windows.Forms.TextBox
    Friend WithEvents gbSaveInterval As System.Windows.Forms.GroupBox
    Friend WithEvents chkClean As System.Windows.Forms.CheckBox
    Friend WithEvents chkDocLoc As System.Windows.Forms.CheckBox
    Friend WithEvents ChkAutoSave As System.Windows.Forms.CheckBox
    Friend WithEvents rdoKeepEverything As System.Windows.Forms.RadioButton
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents chkReadOnlySave As System.Windows.Forms.CheckBox
    Friend WithEvents btnClear As System.Windows.Forms.Button
End Class
