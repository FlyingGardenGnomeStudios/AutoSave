<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Open_Version
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Open_Version))
        Me.rtbInfo = New System.Windows.Forms.RichTextBox()
        Me.rbOpenOld = New System.Windows.Forms.RadioButton()
        Me.rbRestore = New System.Windows.Forms.RadioButton()
        Me.rbOpenCurrent = New System.Windows.Forms.RadioButton()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblOriginal = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'rtbInfo
        '
        Me.rtbInfo.BackColor = System.Drawing.SystemColors.Control
        Me.rtbInfo.Location = New System.Drawing.Point(12, 12)
        Me.rtbInfo.Name = "rtbInfo"
        Me.rtbInfo.ReadOnly = True
        Me.rtbInfo.Size = New System.Drawing.Size(364, 185)
        Me.rtbInfo.TabIndex = 0
        Me.rtbInfo.Text = ""
        '
        'rbOpenOld
        '
        Me.rbOpenOld.AutoSize = True
        Me.rbOpenOld.Checked = True
        Me.rbOpenOld.Location = New System.Drawing.Point(13, 222)
        Me.rbOpenOld.Name = "rbOpenOld"
        Me.rbOpenOld.Size = New System.Drawing.Size(226, 17)
        Me.rbOpenOld.TabIndex = 1
        Me.rbOpenOld.TabStop = True
        Me.rbOpenOld.Text = "Open autosave version (Save not allowed)"
        Me.rbOpenOld.UseVisualStyleBackColor = True
        '
        'rbRestore
        '
        Me.rbRestore.AutoSize = True
        Me.rbRestore.Location = New System.Drawing.Point(12, 243)
        Me.rbRestore.Name = "rbRestore"
        Me.rbRestore.Size = New System.Drawing.Size(231, 17)
        Me.rbRestore.TabIndex = 2
        Me.rbRestore.TabStop = True
        Me.rbRestore.Text = "Restore autosave version to current version"
        Me.rbRestore.UseVisualStyleBackColor = True
        '
        'rbOpenCurrent
        '
        Me.rbOpenCurrent.AutoSize = True
        Me.rbOpenCurrent.Location = New System.Drawing.Point(12, 264)
        Me.rbOpenCurrent.Name = "rbOpenCurrent"
        Me.rbOpenCurrent.Size = New System.Drawing.Size(124, 17)
        Me.rbOpenCurrent.TabIndex = 3
        Me.rbOpenCurrent.TabStop = True
        Me.rbOpenCurrent.Text = "Open current version"
        Me.rbOpenCurrent.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(12, 287)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(93, 287)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblOriginal
        '
        Me.lblOriginal.AutoSize = True
        Me.lblOriginal.Location = New System.Drawing.Point(13, 203)
        Me.lblOriginal.Name = "lblOriginal"
        Me.lblOriginal.Size = New System.Drawing.Size(42, 13)
        Me.lblOriginal.TabIndex = 6
        Me.lblOriginal.Text = "Original"
        Me.lblOriginal.Visible = False
        '
        'Open_Version
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(388, 331)
        Me.Controls.Add(Me.lblOriginal)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.rbOpenCurrent)
        Me.Controls.Add(Me.rbRestore)
        Me.Controls.Add(Me.rbOpenOld)
        Me.Controls.Add(Me.rtbInfo)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Open_Version"
        Me.Text = "Open_Version"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents rtbInfo As system.Windows.Forms.RichTextBox
    Friend WithEvents rbOpenOld As System.Windows.Forms.RadioButton
    Friend WithEvents rbRestore As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpenCurrent As System.Windows.Forms.RadioButton
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblOriginal As System.Windows.Forms.Label
End Class
