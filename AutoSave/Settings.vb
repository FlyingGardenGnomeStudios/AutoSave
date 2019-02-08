
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.Win32

Public Class Settings
    Public Sub New()
        InitializeComponent()
        'Dim Reg As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\AutoSave", True)

        cmbProjects.SelectedIndex = My.Settings.Projects
        chkReadOnlySave.Checked = My.Settings.ReadOnlySave
        numInt.Value = My.Settings.SaveInterval
        cmbTime.SelectedIndex = My.Settings.IntervalType
        txtSaveLoc.Text = My.Settings.SaveLocation
        chkDocLoc.Checked = My.Settings.UseDocumentLocation
        numVer.Value = My.Settings.SaveVersions
        cmbOlderThan.SelectedIndex = My.Settings.OldFiles
        numOld.Value = My.Settings.SaveOld
        ChkAutoSave.Checked = My.Settings.Autosave
        rdoVersions.Checked = My.Settings.KeepVersions
        rdoOlderThan.Checked = My.Settings.KeepOlderThan
        rdoKeepEverything.Checked = My.Settings.KeepEverything
        chkClean.Checked = My.Settings.Cleanup
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
    Private Sub rdoVersions_CheckedChanged(sender As Object, e As EventArgs) Handles rdoVersions.CheckedChanged
        If rdoVersions.Checked = True Then
            cmbOlderThan.Enabled = False
            numOld.Enabled = False
            numVer.Enabled = True
        End If
    End Sub
    Private Sub rdoOlderThan_CheckedChanged(sender As Object, e As EventArgs) Handles rdoOlderThan.CheckedChanged
        If rdoOlderThan.Checked = True Then
            cmbOlderThan.Enabled = True
            numOld.Enabled = True
            numVer.Enabled = False
        End If
    End Sub
    Private Sub rdoKeepEverything_CheckedChanged(sender As Object, e As EventArgs) Handles rdoKeepEverything.CheckedChanged
        If rdoKeepEverything.Checked = True Then
            cmbOlderThan.Enabled = False
            numOld.Enabled = False
            numVer.Enabled = False
        End If
    End Sub
    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim Folder As FolderBrowserDialog = New FolderBrowserDialog
        Folder.Description = "Choose the location you wish to save your files"
        Folder.RootFolder = System.Environment.SpecialFolder.Desktop
        Try
            Folder.SelectedPath = txtSaveLoc.Text
            If Folder.ShowDialog() = DialogResult.OK Then
                txtSaveLoc.Text = Folder.SelectedPath & "\"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles chkDocLoc.CheckedChanged
        If chkDocLoc.Checked = True Then
            btnBrowse.Enabled = False
            txtSaveLoc.Enabled = False
        Else
            btnBrowse.Enabled = True
            txtSaveLoc.Enabled = True
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If chkDocLoc.Checked = False And txtSaveLoc.Text = "" Then
            MsgBox("Invalid save location" & vbNewLine & "Enter a valid location or select 'Use Document Location'")
            Exit Sub
        ElseIf chkDocLoc.Checked = False Then
            Try
                My.Computer.FileSystem.CreateDirectory(txtSaveLoc.Text)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
        My.Settings.ReadOnlySave = chkReadOnlySave.Checked
        Dim Interval As Integer
        Select Case cmbTime.SelectedIndex
            Case 0
                Interval = numInt.Value * 60
            Case 1
                Interval = numInt.Value * 3600
            Case 2
                Interval = numInt.Value * 86400
        End Select
        My.Settings.Interval = Interval
        My.Settings.Projects = cmbProjects.SelectedIndex
        My.Settings.SaveInterval = numInt.Value
        My.Settings.IntervalType = cmbTime.SelectedIndex
        My.Settings.SaveLocation = txtSaveLoc.Text
        My.Settings.UseDocumentLocation = chkDocLoc.Checked
        My.Settings.SaveVersions = numVer.Value
        My.Settings.OldFiles = cmbOlderThan.SelectedIndex
        My.Settings.SaveOld = numOld.Value

        Select Case cmbOlderThan.SelectedIndex
            Case 0
                Interval = numOld.Value * 60
            Case 1
                Interval = numOld.Value * 3600
            Case 2
                Interval = numOld.Value * 86400
        End Select
        My.Settings.OldInterval = Interval
        My.Settings.Autosave = ChkAutoSave.Checked
        My.Settings.KeepVersions = rdoVersions.Checked
        My.Settings.KeepOlderThan = rdoOlderThan.Checked
        My.Settings.KeepEverything = rdoKeepEverything.Checked
        My.Settings.Cleanup = chkClean.Checked
        My.Settings.Save()
        Me.Close()
    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        LoadDefaults()
    End Sub

    Private Sub ChkAutoSave_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAutoSave.CheckedChanged
        If ChkAutoSave.Checked = True Then
            cmbProjects.Enabled = True
            gbSaveInterval.Enabled = True
            gbSaveLoc.Enabled = True
            chkDocLoc.Enabled = True
            chkClean.Enabled = True
        Else
            cmbProjects.Enabled = False
            gbSaveInterval.Enabled = False
            gbSaveLoc.Enabled = False
            chkDocLoc.Enabled = False
            chkClean.Enabled = False
        End If
        If numInt.Value = 0 Then
            LoadDefaults()
        End If
    End Sub
    Private Sub LoadDefaults()
        cmbProjects.SelectedIndex = My.Settings.cmbProj
        cmbTime.SelectedIndex = My.Settings.cmbInt
        cmbOlderThan.SelectedIndex = My.Settings.cmbOld
        chkReadOnlySave.Checked = My.Settings.PropertyValues("ReadOnlySave").Property.DefaultValue
        numInt.Value = My.Settings.PropertyValues("SaveInterval").Property.DefaultValue
        numVer.Value = My.Settings.PropertyValues("SaveVersions").Property.DefaultValue
        numOld.Value = My.Settings.PropertyValues("SaveOld").Property.DefaultValue
        chkDocLoc.Checked = My.Settings.PropertyValues("DocLoc").Property.DefaultValue
        rdoVersions.Checked = My.Settings.PropertyValues("KeepVersions").Property.DefaultValue
        rdoOlderThan.Checked = My.Settings.PropertyValues("KeepOlderThan").Property.DefaultValue
        chkClean.Checked = My.Settings.PropertyValues("Cleanup").Property.DefaultValue
        txtSaveLoc.Text = My.Settings.PropertyValues("SaveLocation").Property.DefaultValue
        chkDocLoc.Checked = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        txtLog.Clear()
        For Each Line As String In File.ReadLines(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"))
            txtLog.AppendText(Line & Environment.NewLine)
        Next
        If Button1.Text = "V" Then
            Me.Height = 510
            Button1.Text = "Λ"
        Else
            Me.Height = 360
            Button1.Text = "V"
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtLog.Clear()
        Dim fileExists As Boolean = IO.File.Exists(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"))
        If fileExists = True Then
            Kill(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"))
        End If
        Using sw As New StreamWriter(IO.File.Open(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"), FileMode.Append))
            sw.WriteLine("Log reset:" & DateTime.Now)
        End Using
        txtLog.Text = "Log reset:" & DateTime.Now
    End Sub

    Private Sub Settings_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Height = 360
        Button1.Text = "V"
    End Sub
End Class
