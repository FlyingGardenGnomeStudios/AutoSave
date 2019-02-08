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
Imports System.Runtime.InteropServices
Namespace BatchProgram
    <ProgIdAttribute("BatchProgram.StandardAddInServer"),
    GuidAttribute("28c77d41-4c93-4cdd-aabd-3d200d491d8f")>
    Public Class StandardAddInServer
        Implements Inventor.ApplicationAddInServer
        Dim WithEvents m_UIEvents2 As UserInputEvents
        Private WithEvents m_uiEvents As UserInterfaceEvents
        Private WithEvents m_BatchProgramButton As ButtonDefinition
        Dim WithEvents m_AppEvents As ApplicationEvents
        Dim _invApp As Inventor.Application
        Dim Changes As ApplicationEvents
        Dim SkipSave As Boolean
        Dim ChangeReport As New List(Of String)
        Dim IsIdle As Boolean = True
        Public ReportLog As String
        Dim Fail As Boolean = True
        Dim LicenseError As String = ""


#Region "ApplicationAddInServer Members"
        ' This method is called by Inventor when it loads the AddIn. The AddInSiteObject provides access  
        ' to the Inventor Application object. The FirstTime flag indicates if the AddIn is loaded for
        ' the first time. However, with the introduction of the ribbon this argument is always true.
        Public Sub Activate(ByVal addInSiteObject As Inventor.ApplicationAddInSite, ByVal firstTime As Boolean) Implements Inventor.ApplicationAddInServer.Activate
            ' Initialize AddIn members.
            g_inventorApplication = addInSiteObject.Application
            ' Connect to the user-interface events to handle a ribbon reset.
            m_uiEvents = g_inventorApplication.UserInterfaceManager.UserInterfaceEvents
            m_UIEvents2 = g_inventorApplication.CommandManager.UserInputEvents
            Dim largeIcon As stdole.IPictureDisp = PictureDispConverter.ToIPictureDisp(My.Resources.BatchProgram)
            Dim controlDefs As Inventor.ControlDefinitions = g_inventorApplication.CommandManager.ControlDefinitions
            ' ActivationCheck()
            m_BatchProgramButton = controlDefs.AddButtonDefinition("Batch Program", "UIBatchProgram", CommandTypesEnum.kShapeEditCmdType, AddInClientID,, "Run the Batch Program", , largeIcon, ButtonDisplayEnum.kDisplayTextInLearningMode)
            m_AppEvents = g_inventorApplication.ApplicationEvents

            ' Add to the user interface, if it's the first time.
            If firstTime Then
                AddToUserInterface()
            End If
        End Sub
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
                    Reg = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\BatchProgram", True).GetValue("Arb1")
                Catch ex As Exception
                    My.Computer.Registry.CurrentUser.CreateSubKey("Software\Autodesk\Inventor\Current Version\BatchProgram")
                    Reg = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\BatchProgram", True)
                    Reg.SetValue("Arb1", (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds, RegistryValueKind.DWord) ' date
                    Reg.SetValue("Arb2", userId, RegistryValueKind.String) 'UserID
                    Reg.SetValue("Arb3", appId, RegistryValueKind.String) ' Appid
                End Try
                ' Get the auth token. 

                If isValid = True Then
                    Try
                        Dim uTime As Integer
                        uTime = (DateTime.UtcNow.AddDays(15) - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
                        My.Computer.Registry.CurrentUser.SetValue("Software\Autodesk\Inventor\Current Version\BatchProgram", uTime, RegistryValueKind.DWord)
                        Fail = False
                    Catch ex As Exception
                    End Try
                ElseIf isValid = False Then
                    Dim FDay As DateTime = ConvertFromUnixTimestamp(My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\BatchProgram", True).GetValue("Arb1"))
                    If userId = "" Then
                        LicenseError = "No user logged in" & vbNewLine &
                                        "Please sign in to Autodesk 360 to use the Batch Program."
                    ElseIf DateTime.Today < FDay AndAlso FDay > DateTime.Today.AddDays(16) AndAlso
                    My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\BatchProgram", True).GetValue("Arb2") = userId AndAlso
                    My.Computer.Registry.CurrentUser.OpenSubKey("Software\Autodesk\Inventor\Current Version\BatchProgram", True).GetValue("Arb3") = appId Then
                        Dim days As Integer = FDay.Subtract(Today).Days
                        LicenseError = "License-check failed" & vbNewLine &
                                        "Confirm you have access to the internet and retry." & vbNewLine &
                                        "The app is currently functioning in a grace period." & vbNewLine &
                                        "There are currently " & days & " days remaining."
                        Fail = False
                    Else
                        LicenseError = "Batch Program license-check fail" & vbNewLine &
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
#Region "User interface definition"
        ' Sub where the user-interface creation is done.  This is called when
        ' the add-in loaded and also if the user interface is reset.


        ' must be declared at the class level
        Private Sub AddToUserInterface()
            '' Get the part ribbon.
            'For Each Ribbon As Inventor.Ribbon In g_inventorApplication.UserInterfaceManager.Ribbons
            Dim ToolsRibbonPart As Ribbon = g_inventorApplication.UserInterfaceManager.Ribbons.Item("Part")
            Dim ToolsRibbonAssy As Ribbon = g_inventorApplication.UserInterfaceManager.Ribbons.Item("Assembly")
            Dim ToolsRibbonDraw As Ribbon = g_inventorApplication.UserInterfaceManager.Ribbons.Item("Drawing")
            Dim ToolsPart As RibbonTab = ToolsRibbonPart.RibbonTabs.Add("Batch Program", "UIBatchProgram", AddInClientID)
            Dim ToolsAssy As RibbonTab = ToolsRibbonAssy.RibbonTabs.Add("Batch Program", "UIBatchProgram", AddInClientID)
            Dim ToolsDraw As RibbonTab = ToolsRibbonDraw.RibbonTabs.Add("Batch Program", "UIBatchProgram", AddInClientID)
            '' Create a new panel.

            Dim BatchProgramPart As RibbonPanel = ToolsPart.RibbonPanels.Add("Batch Progam", "Batch Program", AddInClientID)
            Dim BatchProgramAssy As RibbonPanel = ToolsAssy.RibbonPanels.Add("Batch Progam", "Batch Program", AddInClientID)
            Dim BatchProgramDraw As RibbonPanel = ToolsDraw.RibbonPanels.Add("Batch Progam", "Batch Program", AddInClientID)
            '' Add a button.
            BatchProgramPart.CommandControls.AddButton(m_BatchProgramButton, True)
            BatchProgramAssy.CommandControls.AddButton(m_BatchProgramButton, True)
            BatchProgramDraw.CommandControls.AddButton(m_BatchProgramButton, True)
            ' Next
        End Sub

        Private Sub m_uiEvents_OnResetRibbonInterface(Context As NameValueMap) Handles m_uiEvents.OnResetRibbonInterface
            ' The ribbon was reset, so add back the add-ins user-interface.
            AddToUserInterface()
        End Sub
        ' Sample handler for the button.
        Private Sub m_BatchProgramButton_OnExecute(Context As NameValueMap) Handles m_BatchProgramButton.OnExecute
            'ActivationCheck()

            'If Fail = False Then
            MsgBox("Success")
            'Else
            'MessageBox.Show(LicenseError, "AutoSave Add-in")
            'End If
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
            Dim t As Type = GetType(BatchProgram.StandardAddInServer)
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
