Imports Inventor
Imports Microsoft.Win32
Imports System.Linq
Imports System.Windows.Forms
Imports System.IO
Imports System.Collections.Generic
Imports Autodesk.WebServices
Imports System.Text
Imports RestSharp
Imports System.Net
Imports RestSharp.Deserializers
Imports Scripting
Imports System.Runtime.InteropServices

Namespace AutoSave
    <ProgIdAttribute("AutoSave.StandardAddInServer"),
    GuidAttribute("ac9c27a4-5461-4487-96e2-5c2fd1073cdc")>
    Public Class StandardAddInServer
        Implements Inventor.ApplicationAddInServer
        Dim WithEvents m_UIEvents2 As UserInputEvents
        Private WithEvents m_uiEvents As UserInterfaceEvents
        Private WithEvents m_AutoSaveSAButton As ButtonDefinition
        Dim WithEvents m_AppEvents As ApplicationEvents
        Dim bkgAutoSave As System.Threading.Thread
        Dim _invApp As Inventor.Application
        Dim Changes As ApplicationEvents
        Dim SkipSave As Boolean
        Dim ChangeReport As New List(Of String)
        Dim IsIdle As Boolean = True
        Dim Settings As New Settings
        Public ReportLog As String
        Dim Fail As Boolean = True
        Dim LicenseError As String = ""

#Region "ApplicationAddInServer Members"

        ' This method is called by Inventor when it loads the AddIn. The AddInSiteObject provides access  
        ' to the Inventor Application object. The FirstTime flag indicates if the AddIn is loaded for
        ' the first time. However, with the introduction of the ribbon this argument is always true.
        Public Sub Activate(ByVal addInSiteObject As Inventor.ApplicationAddInSite, ByVal firstTime As Boolean) Implements Inventor.ApplicationAddInServer.Activate
            ' Initialize AddIn members.

            If IsFile(IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "Autodesk\ApplicationPlugins"), "AutoSave.dll") = True Then
                'If IO.File.Exists(IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "Autodesk\ApplicationPlugins\FlyingGarden_AutoSave.bundle\Contents\AutoSave.dll")) Then
                MsgBox("It appears as though the subscription version of Autosave is installed" & vbNewLine &
                       "In order to stop save conflicts, please uninstal one of the AutoSave versions." & vbNewLine &
                       "The standalone version will not be loaded.")
                Exit Sub
            End If
            g_inventorApplication = addInSiteObject.Application
            ' Connect to the user-interface events to handle a ribbon reset.
            m_uiEvents = g_inventorApplication.UserInterfaceManager.UserInterfaceEvents
            m_UIEvents2 = g_inventorApplication.CommandManager.UserInputEvents
            Dim largeIcon As stdole.IPictureDisp = PictureDispConverter.ToIPictureDisp(My.Resources.AutoSave_Icon_Perm_32)
            Dim smallIcon As stdole.IPictureDisp = PictureDispConverter.ToIPictureDisp(My.Resources.AutoSave_Icon_Perm_16)
            Dim controlDefs As Inventor.ControlDefinitions = g_inventorApplication.CommandManager.ControlDefinitions
            ' ActivationCheck()

            m_AutoSaveSAButton = controlDefs.AddButtonDefinition("Settings", "UIAutoSaveSA", CommandTypesEnum.kShapeEditCmdType, AddInClientID,, "Change options for AutoSave", smallIcon, largeIcon, ButtonDisplayEnum.kDisplayTextInLearningMode)
            m_AppEvents = g_inventorApplication.ApplicationEvents
            ' Add to the user interface, if it's the first time.
            If firstTime Then
                AddToUserInterface()
            End If
        End Sub

        Function IsFile(ByVal DName As String, ByVal FName As String) As Boolean
            For Each foundFile As String In My.Computer.FileSystem.GetFiles(DName, FileIO.SearchOption.SearchAllSubDirectories, FName)
                Return True
                Exit Function
            Next
        End Function
        ' This method is called by Inventor when the AddIn is unloaded. The AddIn will be
        ' unloaded either manually by the user or when the Inventor session is terminated.
        Public Sub Deactivate() Implements Inventor.ApplicationAddInServer.Deactivate
            ' TODO:  Add ApplicationAddInServer.Deactivate implementation
            ' Release objects.
            Try
                Kill(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"))
            Catch ex As Exception

            End Try
            m_uiEvents = Nothing
            m_UIEvents2 = Nothing
            g_inventorApplication = Nothing
            m_AppEvents = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Sub

        ' This property is provided to allow the AddIn to expose an API of its own to other 
        ' programs. Typically, this  would be done by implementing the AddIn's API
        ' interface in a class and returning that class object through this property.
        Public ReadOnly Property Automation() As Object Implements Inventor.ApplicationAddInServer.Automation
            Get
                Return Nothing
            End Get
        End Property

        ' Note:this method is now obsolete, you should use the 
        ' ControlDefinition functionality for implementing commands.
        Public Sub ExecuteCommand(ByVal commandID As Integer) Implements Inventor.ApplicationAddInServer.ExecuteCommand
        End Sub
#End Region
#Region "AppStore Authentication"
        Private Sub ActivationCheck()
            Dim mgr As CWebServicesManager = New CWebServicesManager
            Dim isInitialize As Boolean = mgr.Initialize
            If (isInitialize = True) Then
                Dim userId As String = ""
                mgr.GetUserId(userId)
                Dim username As String = ""
                mgr.GetLoginUserName(username)
                'replace your App id here...
                'contact appsubmissions@autodesk.com for the App Id
                Dim appId As String = "2011674918500320289"
                Dim isValid As Boolean = Entitlement(appId, userId)
                Dim Reg As Object
                Try
                    Reg = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\AutoSaveSA", True).GetValue("Arb1")
                Catch ex As Exception
                    My.Computer.Registry.CurrentUser.CreateSubKey("Software\Autodesk\Inventor\Current Version\AutoSaveSA")
                    Reg = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\AutoSaveSA", True)
                    Reg.SetValue("Arb1", (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds, RegistryValueKind.DWord) ' date
                    Reg.SetValue("Arb2", userId, RegistryValueKind.String) 'UserID
                    Reg.SetValue("Arb3", appId, RegistryValueKind.String) ' Appid
                End Try
                ' Get the auth token. 

                Dim Settings As New Settings
                If isValid = True Then
                    Try
                        Dim uTime As Integer
                        uTime = (DateTime.UtcNow.AddDays(15) - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
                        My.Computer.Registry.CurrentUser.SetValue("Software\Autodesk\Inventor\Current Version\AutoSaveSA", uTime, RegistryValueKind.DWord)
                        Fail = False
                    Catch ex As Exception
                    End Try
                ElseIf isValid = False Then
                    Dim FDay As DateTime = ConvertFromUnixTimestamp(My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\AutoSaveSA", True).GetValue("Arb1"))
                    If userId = "" Then
                        LicenseError = "No user logged in" & vbNewLine &
                                        "Please sign in to Autodesk 360 to use AutoSave."
                    ElseIf DateTime.Today < FDay AndAlso FDay > DateTime.Today.AddDays(16) AndAlso
                    My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\AutoSaveSA", True).GetValue("Arb2") = userId AndAlso
                    My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\AutoSaveSA", True).GetValue("Arb3") = appId Then
                        Dim days As Integer = FDay.Subtract(Today).Days
                        LicenseError = "License-check failed" & vbNewLine &
                                        "Confirm you have access to the internet and retry." & vbNewLine &
                                        "The app is currently functioning in a grace period." & vbNewLine &
                                        "There are currently " & days & " days remaining."
                        Fail = False
                    Else
                        LicenseError = "AutoSave license-check fail" & vbNewLine &
                                        "Please purchase the add-in from the appstore."
                        Fail = True
                    End If
                End If
            End If
        End Sub
        Private Shared Function ConvertFromUnixTimestamp(ByVal timestamp As Long) As DateTime
            Dim origin As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            Return origin.AddSeconds(timestamp)
        End Function
        Private Function Entitlement(ByVal appId As String, ByVal userId As String) As Boolean
            'REST API call for the entitlement API.
            'We are using RestSharp for simplicity.
            'You may choose to use another library.
            '(1) Build request
            Dim client = New RestClient
            client.BaseUrl = New System.Uri("https://apps.exchange.autodesk.com")
            'Set resource/end point
            Dim request = New RestRequest
            request.Resource = "webservices/checkentitlement"
            request.Method = Method.GET
            'Add parameters
            request.AddParameter("userid", userId)
            request.AddParameter("appid", appId)
            ' (2) Execute request and get response
            Dim response As IRestResponse = client.Execute(request)
            ' (3) Parse the response and get the value of IsValid. 
            Dim isValid As Boolean = False
            If (response.StatusCode = HttpStatusCode.OK) Then
                Dim deserial As JsonDeserializer = New JsonDeserializer
                Dim entitlementResponse As EntitlementResponse = deserial.Deserialize(Of EntitlementResponse)(response)
                isValid = entitlementResponse.IsValid
            End If
            Return isValid
        End Function
#End Region
        Private fso As New FileSystemObject

#Region "User interface definition"
        ' Sub where the user-interface creation is done.  This is called when
        ' the add-in loaded and also if the user interface is reset.
        Private Sub AddToUserInterface()
            '' Get the part ribbon.

            For Each Ribbon As Inventor.Ribbon In g_inventorApplication.UserInterfaceManager.Ribbons
                Dim GetStartedRibbon As Ribbon = g_inventorApplication.UserInterfaceManager.Ribbons.Item(Ribbon.InternalName)
                Dim GetStarted As RibbonTab = GetStartedRibbon.RibbonTabs.Item("id_GetStarted")
                '' Create a new panel.
                Dim AutoSave As RibbonPanel = GetStarted.RibbonPanels.Add("AutoSave", "AutoSaveSA", AddInClientID)
                '' Add a button.
                AutoSave.CommandControls.AddButton(m_AutoSaveSAButton, True)
            Next
            bkgAutoSave = New System.Threading.Thread(AddressOf runAutoSave)
            bkgAutoSave.Start()
        End Sub

        Private Sub m_uiEvents_OnResetRibbonInterface(Context As NameValueMap) Handles m_uiEvents.OnResetRibbonInterface
            ' The ribbon was reset, so add back the add-ins user-interface.
            AddToUserInterface()
        End Sub
        ' Sample handler for the button.
        Private Sub m_AutoSaveSAButton_OnExecute(Context As NameValueMap) Handles m_AutoSaveSAButton.OnExecute
            ActivationCheck()

            If Fail = False Then
                Settings.ShowDialog()
            Else
                MessageBox.Show(LicenseError, "AutoSave Add-in")
            End If
        End Sub
        Private Sub runAutoSave() 'Reg As RegistryKey)
            Dim InvProcess() As Process = Process.GetProcessesByName("Inventor")
            Log.Log("Inventor Accessed")
            Do While Process.GetProcessesByName("Inventor").Count = 1
                If My.Settings.Autosave = True Then
                    Dim interval As Integer = My.Settings.Interval
                    If interval <> Nothing AndAlso g_inventorApplication.Documents.VisibleDocuments.Count <> 0 Then
                        InvProcess = Process.GetProcessesByName("Inventor")
                        If InvProcess.Count > 1 Then
                            MessageBox.Show("AutoSave has detected multiple instances of Inventor" & vbNewLine &
                                                "This function can only work while a single instance is running")
                            Exit Sub
                        End If
                        Dim Time As TimeSpan = Now() - Process.GetCurrentProcess.StartTime
                        If CInt(Math.Round(Time.TotalSeconds / 60, 0) * 60) Mod interval = 0 Then
                            Try
                                While IsIdle = False
                                    Threading.Thread.Sleep(1000)
                                End While
                                Dim processes As Process() = Process.GetProcessesByName("Inventor")
                                For Each process As Process In processes
                                    Dim WindowCount As New WindowCount
                                    Dim windows As IDictionary(Of IntPtr, String) = WindowCount.GetOpenWindowsFromPID(process.Id)
                                    Do Until windows.Count = 1
                                        If windows.Count > 1 Then
                                            Threading.Thread.Sleep(1000)
                                        End If
                                        windows.Clear()
                                        windows = WindowCount.GetOpenWindowsFromPID(process.Id)
                                    Loop
                                Next
                                SaveFiles()
                            Catch ex As Exception
                                Log.Log("AutoSave encurred an error while saving " & g_inventorApplication.ActiveDocument.DisplayName & vbNewLine &
                                        ex.Message)
                            End Try
                        End If
                    End If
                End If
                Threading.Thread.Sleep(60000)
            Loop
        End Sub

        Private Sub SaveFiles()
            Dim Proj As Integer = My.Settings.Projects
            If My.Settings.KeepOlderThan = True And My.Settings.Cleanup = True Then
                Cleanup(g_inventorApplication.ActiveDocument)
            Else
                Select Case Proj
                    Case 0
                        DirtyWork(g_inventorApplication.ActiveDocument)
                    Case 1
                        For Each Document As Document In g_inventorApplication.Documents.VisibleDocuments
                            DirtyWork(Document)
                        Next
                    Case 2
                        For Each Document As Document In g_inventorApplication.Documents
                            DirtyWork(Document)
                        Next
                End Select
            End If
        End Sub

        Private Sub DirtyWork(oDoc As Inventor.Document)
            If Not ChangeReport.Contains(oDoc.FullFileName) Then
                Log.Log(oDoc.DisplayName & " Skipped save - No changes since last save")
                Exit Sub
            End If
            If oDoc.DocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                Dim oAssDoc As AssemblyDocument = g_inventorApplication.ActiveDocument
                If Not oAssDoc.ComponentDefinition.ActiveOccurrence Is Nothing Then
                    Log.Log("Skipped saving " & oAssDoc.DisplayName & vbNewLine & "Currently being edited by user.")
                    Exit Sub
                End If
            ElseIf oDoc.DocumentType = DocumentTypeEnum.kPartDocumentObject Then
                Dim oPartDoc = g_inventorApplication.ActiveDocument
                If Not oPartDoc.ActivatedObject Is Nothing Then
                    Log.Log("Skipped saving " & oPartDoc.DisplayName & vbNewLine & "Currently being edited by user.")
                    Exit Sub
                End If
            End If
            If oDoc.FullFileName = "" Then
                Dim ans As MsgBoxResult
                ans = MsgBox("The document " & oDoc.DisplayName & " has not yet been saved." _
                    & vbNewLine & "Do you wish to save it now?", vbYesNo Or MsgBoxStyle.SystemModal, "AutoSave document " & oDoc.DisplayName)
                If ans = MsgBoxResult.Yes Then
                    Try
                        oDoc.Save()
                        Log.Log("Initial save: " & oDoc.DisplayName)
                    Catch
                        Log.Log("Skipped initial save of " & oDoc.DisplayName)
                        Exit Sub
                    End Try
                End If
                Exit Sub
            End If
            Dim ReadCheck As New IO.FileInfo(oDoc.FullFileName)
            If ReadCheck.IsReadOnly = False Or My.Settings.ReadOnlySave = True Then
                Dim SaveName, Tag, Location, Directory As String
                Try
                    g_inventorApplication.SilentOperation = True
                    SkipSave = True
                    If My.Settings.UseDocumentLocation = True Then
                        If Not My.Computer.FileSystem.DirectoryExists(IO.Path.GetDirectoryName(oDoc.FullFileName) & "\AutoSave") Then
                            My.Computer.FileSystem.CreateDirectory(IO.Path.GetDirectoryName(oDoc.FullFileName) & "\AutoSave")
                        End If
                        Location = oDoc.FullFileName
                        SaveName = Location.Insert(InStrRev(oDoc.FullFileName, "\"), "Autosave\")
                        Tag = GetTag(oDoc, IO.Path.GetDirectoryName(SaveName))
                        SaveName = SaveName.Insert(InStrRev(SaveName, "."), Tag & ".")
                        Try
                            oDoc.SaveAs(SaveName, True)
                            Write_Save_Data(oDoc.DisplayName, SaveName, oDoc.FullDocumentName)
                        Catch ex As Exception
                            Log.Log("Error encountered while saving " & oDoc.DisplayName & vbNewLine & ex.Message)
                        End Try
                    Else
                        Location = My.Settings.SaveLocation
                        If Not My.Computer.FileSystem.DirectoryExists(Location) Then
                            My.Computer.FileSystem.CreateDirectory(Location)
                        End If
                        Tag = GetTag(oDoc, Location)
                        SaveName = Location & IO.Path.GetFileName(oDoc.FullFileName).Insert(InStrRev(IO.Path.GetFileName(oDoc.FullFileName), "."), Tag & ".")
                        Try
                            oDoc.SaveAs(SaveName, True)
                            Write_Save_Data(oDoc.DisplayName, SaveName, oDoc.FullDocumentName)
                        Catch ex As Exception
                            Log.Log("Error encountered while saving " & oDoc.DisplayName & vbNewLine & ex.Message)
                        End Try
                    End If
                    Log.Log("Saved document: " & SaveName)
                    Directory = IO.Path.GetDirectoryName(SaveName)
                    If My.Settings.KeepVersions = True Then
                        Dim dir As New DirectoryInfo(Directory)
                        Dim FileList As List(Of FileInfo) = dir.GetFiles(IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) & ".????" & IO.Path.GetExtension(oDoc.FullFileName)).ToList
                        FileList.Sort(AddressOf SortByDate)
                        For X = 0 To FileList.Count - CInt(My.Settings.SaveVersions) - 1
                            System.IO.File.SetAttributes(Directory & "\" & FileList.Item(X).ToString, FileAttributes.ReadOnly = False)
                            Kill(Directory & "\" & FileList.Item(X).ToString)
                            Log.Log("Deleted old version: " & Directory & "\" & FileList.Item(X).ToString)
                        Next
                    ElseIf My.Settings.KeepOlderThan = True Then
                        Dim SearchDir As New DirectoryInfo(Directory)
                        For Each file As FileInfo In SearchDir.GetFiles("*")
                            Dim span As TimeSpan = DateTime.Now.Subtract(file.CreationTime)
                            If span.TotalSeconds > My.Settings.OldInterval Then
                                file.IsReadOnly = False
                                Kill(file.FullName)
                                Log.Log("Deleted old version: " & file.FullName)
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Log.Log("Error encountered while saving: " & ex.Message)
                Finally
                    g_inventorApplication.SilentOperation = False
                    SkipSave = False
                End Try
                ChangeReport.Remove(oDoc.FullFileName)
            ElseIf ReadCheck.IsReadOnly = True Then
                Log.Log(oDoc.DisplayName & " Not saved - Read only - User parameter")
            End If
        End Sub

        Private Sub Write_Save_Data(ByRef DisplayName As String, ByRef SaveName As String, OldName As String)
            Dim sDoc As Document = g_inventorApplication.Documents.Open(SaveName, False)
            Dim Prop As Inventor.Property = Nothing
            Dim customPropSet As Inventor.PropertySet
            Dim Read As IO.FileInfo
            sDoc.PropertySets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("5").Value = DisplayName
            customPropSet = sDoc.PropertySets.Item("Inventor User Defined Properties")
            Try
                Prop = customPropSet.Add(OldName, "Original")
            Catch
                Try
                    Prop = customPropSet.Item("Original")
                    Prop.Value = sDoc.FullDocumentName
                Catch ex As Exception
                    Log.Log("Error encountered while writing data to " & sDoc.DisplayName & vbNewLine & ex.Message)
                End Try
            Finally
                Read = My.Computer.FileSystem.GetFileInfo(SaveName)
                sDoc.Save()
                Read.IsReadOnly = True
                sDoc.Close()
            End Try
        End Sub
        Private Sub Cleanup(oDoc As Document)
            Dim Location, Savename As String
            If My.Settings.UseDocumentLocation = True Then
                If Not My.Computer.FileSystem.DirectoryExists((IO.Path.GetDirectoryName(oDoc.FullFileName) & "\AutoSave")) Then
                    My.Computer.FileSystem.CreateDirectory((IO.Path.GetDirectoryName(oDoc.FullFileName) & "\AutoSave"))
                End If
                Location = oDoc.FullFileName
                Savename = Location.Insert(InStrRev(oDoc.FullFileName, "\"), "Autosave\")
                Location = IO.Path.GetDirectoryName(Savename)
                Try
                    Dim SearchDir As New DirectoryInfo(Location)
                    If My.Computer.FileSystem.DirectoryExists(Location) Then
                        For Each file As FileInfo In SearchDir.GetFiles(IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) & ".????" & IO.Path.GetExtension(oDoc.FullFileName).ToString)
                            file.IsReadOnly = False
                            Kill(file.FullName)
                            Log.Log("Deleted old version: " & file.FullName)
                        Next
                        If Directory.GetFiles(Location, "*.*", SearchOption.AllDirectories).Length = 0 Then Directory.Delete(Location)
                    End If
                Catch ex As Exception
                    MessageBox.Show("Unable to clean " & Location & vbNewLine & ". Some items will need to be deleted manually")
                End Try
            Else
                Location = My.Settings.SaveLocation
                Dim SearchDir As New DirectoryInfo(Location)
                Try
                    If My.Computer.FileSystem.DirectoryExists(Location) Then
                        For Each file As FileInfo In SearchDir.GetFiles(IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) & ".????" & IO.Path.GetExtension(oDoc.FullFileName).ToString)
                            file.IsReadOnly = False
                            Kill(file.FullName)
                            Log.Log("Deleted old version: " & file.FullName)
                        Next
                        If Directory.GetFiles(Location, "*.*", SearchOption.AllDirectories).Length = 0 Then Directory.Delete(Location)
                    End If
                Catch ex As Exception
                    MessageBox.Show("Unable to clean " & Location & vbNewLine & "Some items will need to be deleted manually" & vbNewLine & vbNewLine & ex.ToString)
                End Try
            End If
        End Sub
        Function GetTag(ByRef oDoc As Inventor.Document, ByRef Location As String) As String
            Dim dir As New DirectoryInfo(Location)
            Dim FileList As List(Of FileInfo) = dir.GetFiles(IO.Path.GetFileNameWithoutExtension(oDoc.FullFileName) & ".????" & IO.Path.GetExtension(oDoc.FullFileName)).ToList
            Dim List As String = Nothing
            FileList.Sort(AddressOf SortByDate)
            Dim Tag As Integer = 0
            If FileList.Count <> 0 Then
                Tag = (FileList.Item(FileList.Count - 1).ToString).Substring(Len(FileList.Item(FileList.Count - 1).ToString) - 8, 4)
            End If
            CStr(Tag + 1).PadLeft(4, "0")
            Return CStr(Tag + 1).PadLeft(4, "0")
        End Function
        Private Function SortByDate(X As FileInfo, Y As FileInfo) As Integer
            Return X.CreationTime.CompareTo(Y.CreationTime)
        End Function

        Private Sub m_AppEvents_OnSaveDocument(DocumentObject As _Document, BeforeOrAfter As EventTimingEnum, Context As NameValueMap, ByRef HandlingCode As HandlingCodeEnum) Handles m_AppEvents.OnSaveDocument
            If BeforeOrAfter <> EventTimingEnum.kAfter Then
                Exit Sub
            End If
            If My.Settings.Cleanup = True And SkipSave = False Then
                Cleanup(DocumentObject)
            End If
        End Sub
        Private Sub m_AppEvents_OnOpenDocument(DocumentObject As _Document, FullDocumentName As String, BeforeOrAfter As EventTimingEnum, Context As NameValueMap, ByRef HandlingCode As HandlingCodeEnum) Handles m_AppEvents.OnOpenDocument
            Dim OpenVersion As New Open_Version
            If BeforeOrAfter = EventTimingEnum.kAfter Then
                Dim oDoc As Document = g_inventorApplication.ActiveDocument
                Dim Read As IO.FileInfo = My.Computer.FileSystem.GetFileInfo(DocumentObject.FullFileName)
                If Read.IsReadOnly = True Then
                    Dim Copy, Original As String
                    Try
                        Dim customPropSet As Inventor.PropertySet = DocumentObject.PropertySets.Item("Inventor User Defined Properties")
                        Dim Prop As Inventor.Property
                        Prop = customPropSet.Item("Original")
                        If customPropSet.Item("Original").Value = "" Then Exit Sub
                        g_inventorApplication.SilentOperation = True
                        Original = Prop.Value
                        OpenVersion.lblOriginal.Text = Original
                        Prop.Delete()
                        Copy = IO.Path.GetFileName(DocumentObject.FullFileName)
                        If Not My.Computer.FileSystem.FileExists(Original) Then OpenVersion.rbOpenCurrent.Enabled = False
                        If My.Computer.FileSystem.GetFileInfo(Original).IsReadOnly = True Then OpenVersion.rbRestore.Enabled = False
                        For Each document In g_inventorApplication.Documents
                            If IO.Path.GetFileName(document.FullFileName) = IO.Path.GetFileName(Original) Then OpenVersion.rbRestore.Enabled = False
                        Next
                        If Strings.Len(Original) > 45 Then Original = Strings.Left(Original, 45 - Strings.Len(IO.Path.GetFileName(Original))) & "...\" & IO.Path.GetFileName(Original)
                        If Strings.Len(Copy) > 45 Then Copy = Strings.Left(Copy, 45 - Strings.Len(IO.Path.GetFileName(Copy))) & "...\" & IO.Path.GetFileName(Copy)
                        OpenVersion.rtbInfo.Text = "You have opened an autosaved version (" & Copy & ") of the file (" & Original & "). If you proceed with opening the old version, you will not be allowed to save. You can restore the old version to be the current version if no other versions are open and the current version is not checked out."
                        OpenVersion.ShowDialog()
                    Catch
                    Finally
                        g_inventorApplication.SilentOperation = False
                    End Try
                End If
            End If
        End Sub
        Private Sub m_UIEvents2_OnActivateCommand(CommandName As String, Context As NameValueMap) Handles m_UIEvents2.OnActivateCommand
            IsIdle = False
        End Sub

        Private Sub m_UIEvents2_OnTerminateCommand(CommandName As String, Context As NameValueMap) Handles m_UIEvents2.OnTerminateCommand
            IsIdle = True
        End Sub
        Private Sub m_AppEvents_OnDocumentChange(DocumentObject As _Document, BeforeOrAfter As EventTimingEnum, ReasonsForChange As CommandTypesEnum, Context As NameValueMap, ByRef HandlingCode As HandlingCodeEnum) Handles m_AppEvents.OnDocumentChange
            If BeforeOrAfter = EventTimingEnum.kAfter Then
                If Not ChangeReport.Contains(DocumentObject.FullFileName) Then
                    ChangeReport.Add(DocumentObject.FullFileName)
                End If
            End If
        End Sub
    End Class
#End Region
    Public Module Log
        Public Sub Log(Text As String)
            Dim fileExists As Boolean = IO.File.Exists(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"))
            Using sw As New StreamWriter(IO.File.Open(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "log.txt"), FileMode.Append))
                sw.WriteLine(
         IIf(fileExists,
            DateTime.Now & " " & Text,
             "Logging started:" & DateTime.Now & vbNewLine & DateTime.Now & " " & Text))
            End Using
        End Sub
    End Module

    <Serializable()>
    Public Class EntitlementResponse
        Dim _IsValid As Boolean
        Public Property IsValid As Boolean
            Get
                Return _IsValid
            End Get
            Set(value As Boolean)
                _IsValid = value
            End Set
        End Property
    End Class
End Namespace
Public Module Globals
    ' Inventor application object.
    Public g_inventorApplication As Inventor.Application
    Public IsIdle As Boolean

#Region "Function to get the add-in client ID."
    ' This function uses reflection to get the GuidAttribute associated with the add-in.
    Public Function AddInClientID() As String
        Dim guid As String = ""
        Try
            Dim t As Type = GetType(AutoSave.StandardAddInServer)
            Dim customAttributes() As Object = t.GetCustomAttributes(GetType(GuidAttribute), False)
            Dim guidAttribute As GuidAttribute = CType(customAttributes(0), GuidAttribute)
            guid = "{" + guidAttribute.Value.ToString() + "}"
        Catch
        End Try

        Return guid
    End Function
#End Region

#Region "hWnd Wrapper Class"
    ' This class is used to wrap a Win32 hWnd as a .Net IWind32Window class.
    ' This is primarily used for parenting a dialog to the Inventor window.
    '
    ' For example:
    ' myForm.Show(New WindowWrapper(g_inventorApplication.MainFrameHWND))
    '
    Public Class WindowWrapper
        Implements System.Windows.Forms.IWin32Window
        Public Sub New(ByVal handle As IntPtr)
            _hwnd = handle
        End Sub

        Public ReadOnly Property Handle() As IntPtr _
          Implements System.Windows.Forms.IWin32Window.Handle
            Get
                Return _hwnd
            End Get
        End Property

        Private _hwnd As IntPtr
    End Class
#End Region

#Region "Image Converter"
    ' Class used to convert bitmaps and icons from their .Net native types into
    ' an IPictureDisp object which is what the Inventor API requires. A typical
    ' usage is shown below where MyIcon is a bitmap or icon that's available
    ' as a resource of the project.
    '
    'Dim smallIcon As stdole.IPictureDisp = PictureDispConverter.ToIPictureDisp(My.Resources.MyIcon)

    Public NotInheritable Class PictureDispConverter
        <DllImport("OleAut32.dll", EntryPoint:="OleCreatePictureIndirect", ExactSpelling:=True, PreserveSig:=False)>
        Private Shared Function OleCreatePictureIndirect(
            <MarshalAs(UnmanagedType.AsAny)> ByVal picdesc As Object,
            ByRef iid As Guid,
            <MarshalAs(UnmanagedType.Bool)> ByVal fOwn As Boolean) As stdole.IPictureDisp
        End Function

        Shared iPictureDispGuid As Guid = GetType(stdole.IPictureDisp).GUID

        Private NotInheritable Class PICTDESC
            Private Sub New()
            End Sub

            'Picture Types
            Public Const PICTYPE_BITMAP As Short = 1
            Public Const PICTYPE_ICON As Short = 3

            <StructLayout(LayoutKind.Sequential)>
            Public Class Icon
                Friend cbSizeOfStruct As Integer = Marshal.SizeOf(GetType(PICTDESC.Icon))
                Friend picType As Integer = PICTDESC.PICTYPE_ICON
                Friend hicon As IntPtr = IntPtr.Zero
                Friend unused1 As Integer
                Friend unused2 As Integer

                Friend Sub New(ByVal icon As System.Drawing.Icon)
                    Me.hicon = icon.ToBitmap().GetHicon()
                End Sub
            End Class

            <StructLayout(LayoutKind.Sequential)>
            Public Class Bitmap
                Friend cbSizeOfStruct As Integer = Marshal.SizeOf(GetType(PICTDESC.Bitmap))
                Friend picType As Integer = PICTDESC.PICTYPE_BITMAP
                Friend hbitmap As IntPtr = IntPtr.Zero
                Friend hpal As IntPtr = IntPtr.Zero
                Friend unused As Integer

                Friend Sub New(ByVal bitmap As System.Drawing.Bitmap)
                    Me.hbitmap = bitmap.GetHbitmap()
                End Sub
            End Class
        End Class

        Public Shared Function ToIPictureDisp(ByVal icon As System.Drawing.Icon) As stdole.IPictureDisp
            Dim pictIcon As New PICTDESC.Icon(icon)
            Return OleCreatePictureIndirect(pictIcon, iPictureDispGuid, True)
        End Function

        Public Shared Function ToIPictureDisp(ByVal bmp As System.Drawing.Bitmap) As stdole.IPictureDisp
            Dim pictBmp As New PICTDESC.Bitmap(bmp)
            Return OleCreatePictureIndirect(pictBmp, iPictureDispGuid, True)
        End Function
    End Class

#End Region
    Public Class WindowCount

        <DllImport("USER32.DLL")>
        Private Shared Function GetShellWindow() As IntPtr
        End Function

        <DllImport("USER32.DLL")>
        Private Shared Function GetWindowText(ByVal hWnd As IntPtr, ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
        End Function

        <DllImport("USER32.DLL")>
        Private Shared Function GetWindowTextLength(ByVal hWnd As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Private Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, <Out()> ByRef lpdwProcessId As UInt32) As UInt32
        End Function

        <DllImport("USER32.DLL")>
        Private Shared Function IsWindowVisible(ByVal hWnd As IntPtr) As Boolean
        End Function

        Private Delegate Function EnumWindowsProc(ByVal hWnd As IntPtr, ByVal lParam As Integer) As Boolean

        <DllImport("USER32.DLL")>
        Private Shared Function EnumWindows(ByVal enumFunc As EnumWindowsProc, ByVal lParam As Integer) As Boolean
        End Function

        Private hShellWindow As IntPtr = GetShellWindow()
        Private dictWindows As New Dictionary(Of IntPtr, String)
        Private currentProcessID As Integer

        Public Function GetOpenWindowsFromPID(ByVal processID As Integer) As IDictionary(Of IntPtr, String)
            dictWindows.Clear()
            currentProcessID = processID
            EnumWindows(AddressOf enumWindowsInternal, 0)
            Return dictWindows
        End Function

        Private Function enumWindowsInternal(ByVal hWnd As IntPtr, ByVal lParam As Integer) As Boolean
            If (hWnd <> hShellWindow) Then
                Dim windowPid As UInt32
                If Not IsWindowVisible(hWnd) Then
                    Return True
                End If
                Dim length As Integer = GetWindowTextLength(hWnd)
                If (length = 0) Then
                    Return True
                End If
                GetWindowThreadProcessId(hWnd, windowPid)
                If (windowPid <> currentProcessID) Then
                    Return True
                End If
                Dim stringBuilder As New StringBuilder(length)
                GetWindowText(hWnd, stringBuilder, (length + 1))
                dictWindows.Add(hWnd, stringBuilder.ToString)
            End If
            Return True
        End Function
    End Class
End Module
