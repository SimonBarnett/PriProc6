Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports System.IO

Imports PriPROC6.service
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Cpl
Imports PriPROC6.PriSock
Imports PriPROC6.svcMessage
Imports System.Xml

Public Class MMC

#Region "MEF objects"

    Dim _container As CompositionContainer

    <ImportMany()>
    Public Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))

    <ImportMany()>
    Public Property Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData))

    <ImportMany()>
    Public Property Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))

    <ImportMany()>
    Public Property Cpl As IEnumerable(Of Lazy(Of cplInterface, cplProps))

#End Region

#Region "Properties"

    Private iconList As New Dictionary(Of String, Integer)

    Private _msgFactory As msgFactory = Nothing
    ReadOnly Property msgFactory As msgFactory
        Get
            Return _msgFactory
        End Get
    End Property

    Private _LogQ As New Queue(Of Byte())
    Public Property LogQ As Queue(Of Byte())
        Get
            Return _LogQ
        End Get
        Set(value As Queue(Of Byte()))
            _LogQ = value
        End Set
    End Property

    Private _logListener As uListener = Nothing
    Public Property logListener() As uListener
        Get
            Return _logListener
        End Get
        Set(ByVal value As uListener)
            _logListener = value
        End Set
    End Property

    Private _svcMap As New Dictionary(Of String, oDiscovery)
    Public Property svcMap As Dictionary(Of String, oDiscovery)
        Get
            Return _svcMap
        End Get
        Set(value As Dictionary(Of String, oDiscovery))
            _svcMap = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Private formloaded As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        formloaded = True
        With My.Settings
            Me.Location = .FormTopLeft
            Me.Size = .FormSize
        End With

        Me.Icon = My.Resources.ppConsole

        Using l As New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)

            'An aggregate catalog that combines multiple catalogs
            Dim catalog = New AggregateCatalog()

            'Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(New AssemblyCatalog(GetType(MMC).Assembly))

            'IMPORTANT!
            'You will need to adjust this line to match your local path!
            Dim fi As New FileInfo(System.Reflection.Assembly.GetExecutingAssembly().ToString)
            catalog.Catalogs.Add(New DirectoryCatalog(Path.Combine(fi.DirectoryName, "modules")))

            'Create the CompositionContainer with the parts in the catalog
            _container = New CompositionContainer(catalog)

            'Fill the imports of this object

            Dim StartExecption As Exception = Nothing
            Try
                _container.ComposeParts(Me)

                _msgFactory = New msgFactory(Messages)

                For Each panel As Lazy(Of cplInterface, cplProps) In Cpl
                    With panel
                        .Value.Name = .Metadata.Name
                    End With
                Next
                HomeScreen()

                For Each subscr As Lazy(Of SubscribeDef, SubscribeDefprops) In Subscribers
                    With subscr.Value
                        .Name = subscr.Metadata.Name
                        .EntryType = subscr.Metadata.EntryType
                        .Verbosity = subscr.Metadata.Verbosity
                        .Source = subscr.Metadata.Source
                        .Console = subscr.Metadata.Console

                        For Each k As String In .Icon.Keys
                            Me.icons.Images.Add(.Icon(k))
                            iconList.Add(k, icons.Images.Count - 1)
                        Next

                    End With
                Next

                For Each svr As Lazy(Of svcDef, svcDefprops) In Modules
                    With svr.Value

                        .Name = svr.Metadata.Name
                        .defaultPort = svr.Metadata.defaultPort
                        .defaultStart = svr.Metadata.defaultStart
                        .udp = svr.Metadata.udp

                        For Each k As String In .Icon.Keys
                            Me.icons.Images.Add(.Icon(k))
                            iconList.Add(k, icons.Images.Count - 1)
                        Next

                    End With
                Next

                logListener = New uListener(
                    My.Settings.LogPort,
                    "mmc",
                    l,
                    LogQ,
                    msgFactory,
                    AddressOf logByte
                )

                ' Start log thread.
                trdLog = New System.Threading.Thread(AddressOf dolog)
                With trdLog
                    .Name = String.Format(
                        "{0}@{1}:{2}",
                        Name,
                        Environment.MachineName,
                        My.Settings.LogPort.ToString
                    )
                    .IsBackground = True
                    l.LogData.AppendFormat(
                        "Starting Log thread {0}.",
                        .Name
                    ).AppendLine()
                    .Start()
                End With

                ' Start discovery thread.
                trdDiscovery = New System.Threading.Thread(AddressOf doDiscovery)
                With trdDiscovery
                    .Name = String.Format(
                        "{0}@{1}:{2}",
                        Name,
                        Environment.MachineName,
                        My.Settings.DiscoveryPort.ToString
                    )
                    .IsBackground = True
                    l.LogData.AppendFormat(
                        "Starting dicovery thread {0}.",
                        .Name
                    ).AppendLine()
                    .Start()
                End With

            Catch ex As Exception
                l.setException(ex)

            End Try

            LogQ.Enqueue(msgFactory.EncodeRequest("log", l))

        End Using

    End Sub

#End Region

#Region "Message handlers"

    Public Sub logByte(ByVal e As Byte(), RemoteEndpoint As String)
        Dim msg As msgBase = msgFactory.Decode(e)
        Select Case msg.msgType.ToLower
            Case "log"
                LogQ.Enqueue(e)

            Case "discovery"
                rcvDiscovery(msg)

        End Select

    End Sub

#End Region

#Region "Console log"

    Private trdLog As System.Threading.Thread
    Private Sub dolog()

        Do
            While LogQ.Count > 0
                NewLogEntry(TryCast(msgFactory.Decode(LogQ.Dequeue).thisObject, oMsgLog))
                Threading.Thread.Sleep(2)
            End While
            Threading.Thread.Sleep(100)
        Loop

    End Sub

    Private Delegate Sub NewLogEntryInvoker(ByRef logEntry As oMsgLog)
    Private Sub NewLogEntry(ByRef logEntry As oMsgLog)
        If Me.InvokeRequired Then
            Dim args(0) As Object
            args(0) = logEntry
            Me.Invoke(New NewLogEntryInvoker(AddressOf NewLogEntry), args)
        Else

            Try
                Dim bg As Color
                Dim fg As Color

                With logEntry
                    Select Case .EntryType
                        Case LogEntryType.Err, LogEntryType.FailureAudit
                            bg = Color.Red
                        Case LogEntryType.Warning
                            bg = Color.Yellow
                        Case LogEntryType.SuccessAudit, LogEntryType.Information
                            bg = Color.Green
                    End Select

                    Select Case .Verbosity
                        Case EvtLogVerbosity.Normal
                            fg = Color.Black
                        Case EvtLogVerbosity.Verbose
                            fg = Color.DarkBlue
                        Case EvtLogVerbosity.VeryVerbose
                            fg = Color.DarkGray
                        Case EvtLogVerbosity.Arcane
                            fg = Color.Gray

                    End Select

                    Dim HDR As String = String.Format(
                            "{0}{1}{2}{3}",
                            Pad(.TimeStamp, 22),
                            Pad(.LogSource, 12),
                            Pad(.Source.ToUpper, 25),
                            Pad(.svcType.ToUpper, 10)
                        )

                    With Console
                        .AppendText(HDR)
                        .Select(.TextLength - HDR.Length, .TextLength)
                        .SelectionColor = fg
                        .SelectionBackColor = bg
                        .SelectionFont = New Font(.Font, FontStyle.Bold)
                        .AppendText(vbCrLf)
                        .AppendText(logEntry.LogData.ToString)
                        .AppendText(vbCrLf)
                        .SelectionStart = .Text.Length
                        .SelectionLength = 1
                        .ScrollToCaret()

                    End With
                End With

            Catch ex As Exception

            End Try
        End If

    End Sub

    Private Function Pad(ByVal Str As String, ByVal Width As Integer) As String
        Return String.Format("{0}{1}", Str, New String(" ", Width)).Substring(0, Width)
    End Function

    Private Function Pad(ByVal logSource As EvtLogSource, ByVal width As Integer)
        Select Case logSource
            Case EvtLogSource.APPLICATION
                Return String.Format("{0}{1}", "APPLICATION", New String(" ", width)).Substring(0, width)
            Case EvtLogSource.WEB
                Return String.Format("{0}{1}", "WEB", New String(" ", width)).Substring(0, width)
            Case Else
                Return String.Format("{0}{1}", "SYSTEM", New String(" ", width)).Substring(0, width)
        End Select
    End Function

#End Region

#Region "Discovery thread"

    Private trdDiscovery As System.Threading.Thread
    Private Sub doDiscovery()
        Do
            Try
                discovery(Me, Nothing)

            Catch ex As Exception
                LogQ.Enqueue(msgFactory.EncodeRequest("log", New oMsgLog(Name, ex)))

            Finally
                Pause(My.Settings.BroadcastDelay)

            End Try
        Loop
    End Sub

    Private Sub discovery(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Using uclient As New uClient(My.Settings.DiscoveryPort, eBroadcastType.bcPublic)
            uclient.Send(msgFactory.EncodeRequest("discovery", New oMsgDiscoveryRequest(My.Settings.LogPort)))
        End Using
    End Sub

    Private Sub Pause(Seconds As Integer)
        For sec = 1 To Seconds * 10
            Threading.Thread.Sleep(100)
        Next
    End Sub

    Private Function thisModule(Named As String) As WritableXML
        For Each svr As Lazy(Of svcDef, svcDefprops) In Me.Modules
            If String.Compare(svr.Value.Name, Named, True) = 0 Then
                Return svr.Value
            End If
        Next
        For Each svr As Lazy(Of SubscribeDef, SubscribeDefprops) In Me.Subscribers
            If String.Compare(svr.Value.Name, Named, True) = 0 Then
                Return svr.Value
            End If
        Next
        Throw New Exception(String.Format("Module name {0} not found.", Named))

    End Function

    Private Delegate Sub rcvDiscoveryInvoker(ByRef msg As msgBase)
    Sub rcvDiscovery(ByRef msg As msgBase)

        If Me.InvokeRequired Then
            Dim args(0) As Object
            args(0) = msg
            Me.Invoke(New rcvDiscoveryInvoker(AddressOf rcvDiscovery), args)

        Else

            Dim l As New oMsgLog(Me.Name, EvtLogSource.SYSTEM)
            l.LogData.Append(msg.msgNode.OuterXml).AppendLine()
            LogQ.Enqueue(msgFactory.EncodeRequest("log", l))

            For Each svc As XmlNode In TryCast(msg.thisObject, oMsgDiscovery).svc
                Dim o As oServiceBase = thisModule(svc.Attributes("name").Value).readXML(svc)
                AddHandler o.onSendCmd, AddressOf SaveProperty
                o.mmc = True
                Select Case o.Name.ToLower
                    Case "discovery"
                        Dim p As oDiscovery = TryCast(o, oDiscovery)
                        If Not svcMap.Keys.Contains(p.Host) Then
                            svcMap.Add(p.Host, p)
                        Else
                            svcMap(p.Host).Update(p)
                        End If

                    Case Else
                        If svcMap.Keys.Contains(o.Host) Then
                            If Not svcMap(o.Host).Keys.Contains(o.Name.ToLower) Then
                                svcMap(o.Host).Add(o.Name.ToLower, o)
                                TryCast(svcMap(o.Host).Item(o.Name.ToLower), oServiceBase).Parent = svcMap(o.Host)
                            Else
                                TryCast(svcMap(o.Host).Item(o.Name.ToLower), oServiceBase).Update(o)
                            End If

                        End If

                End Select

            Next

            Dim delDiscovery As New List(Of String)
            Dim network As TreeNode = Browser.Nodes("network")

            For Each k As String In svcMap.Keys
                svcMap(k).DrawTree(network, iconList)
                If svcMap(k).IsTimedOut Then
                    delDiscovery.Add(k)
                Else
                    Dim delSvc As New List(Of String)
                    For Each sk As String In svcMap(k).Keys
                        If TryCast(svcMap(k).Item(sk), oServiceBase).IsTimedOut Then
                            delSvc.Add(sk)
                        End If
                    Next
                    For Each d In delSvc
                        svcMap(k).Remove(d)
                    Next
                End If
            Next
            For Each d As String In delDiscovery
                svcMap.Remove(d)
            Next

            propSvcMap.svcMap = svcMap

        End If

    End Sub

#End Region

#Region "Panels"

    Private Sub HomeScreen()
        SetPanel("home", Nothing)
    End Sub

    Private Sub Browser_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Browser.AfterSelect

        Dim params As String() = e.Node.Name.Split("\")
        Select Case params(0).ToLower
            Case "network"
                HomeScreen()

            Case Else
                With svcMap(params(0))
                    Select Case UBound(params)
                        Case 0
                            Dim pnlName As String = String.Empty
                            Dim useCpl As Object = TryCast(svcMap(params(0)), oDiscovery).useCpl(pnlName, params)
                            If Not IsNothing(useCpl) Then
                                SetPanel(pnlName, useCpl)
                            Else
                                TabPage1.Controls.Clear()

                            End If

                        Case Else
                            If UBound(params) = 1 And String.Compare(params(1), "Subscribers", True) = 0 Then
                                TabPage1.Controls.Clear()

                            Else
                                For Each svc As Object In .values
                                    If String.Compare(TryCast(svc, oServiceBase).Name, params(1), True) = 0 Then

                                        Dim pnlName As String = String.Empty
                                        Dim useCpl As Object = TryCast(svc, oServiceBase).useCpl(pnlName, params)
                                        If Not IsNothing(useCpl) Then
                                            SetPanel(pnlName, useCpl)
                                        Else
                                            TabPage1.Controls.Clear()

                                        End If
                                    End If
                                Next
                            End If
                    End Select

                End With

        End Select

    End Sub

    Sub SetPanel(PanelName As String, useCpl As Object)
        For Each panel As Lazy(Of cplInterface, cplProps) In Cpl
            If String.Compare(panel.Value.Name, PanelName, True) = 0 Then
                panel.Value.LoadObject(useCpl)
                With TabPage1.Controls
                    .Clear()
                    .Add(panel.Value.Cpl)
                    .Item(0).Dock = DockStyle.Fill
                End With

            End If
        Next
    End Sub

    Private Sub SaveProperty(Sender As Object, e As CmdEventArgs)
        With e
            Using cli As New iClient(.Server, .Port)
                Dim resp = msgFactory.Decode(cli.Send(msgFactory.EncodeRequest("cmd", .Message)))
                If Not resp.ErrCde = 200 Then
                    MsgBox(resp.errMsg)
                Else
                    If e.Refresh Then discovery(Me, Nothing)
                End If
            End Using
        End With
    End Sub

#End Region

#Region "Form events"

    Private Sub MMC_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If formloaded Then
            With My.Settings
                .FormSize = Me.Size
                .Save()
            End With
        End If
    End Sub

    Private Sub MMC_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
        With My.Settings
            .FormTopLeft = Me.Location
            .Save()
        End With
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub Browser_Click(sender As Object, e As EventArgs) Handles Browser.Click
        tabs.SelectTab(0)
        If TabPage1.Controls.Count > 0 Then
            TabPage1.Controls(0).Refresh()
        End If
    End Sub

    Private Sub ContextMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenu.Opening
        Dim params As String() = Browser.SelectedNode.Name.Split("\")
        Select Case params(0).ToLower
            Case "network"
                e.Cancel = True

            Case Else
                With svcMap(params(0))
                    Select Case UBound(params)
                        Case 0
                            .ContextMenu(sender, e, params)

                        Case Else
                            If UBound(params) = 1 And String.Compare(params(1), "Subscribers", True) = 0 Then
                                .ContextMenu(sender, e, params)

                            Else
                                For Each svc As Object In .values
                                    If String.Compare(TryCast(svc, oServiceBase).Name, params(1), True) = 0 Then
                                        TryCast(svc, oServiceBase).ContextMenu(sender, e, params)
                                        Exit For
                                    End If
                                Next
                            End If

                    End Select

                End With

        End Select
    End Sub

#End Region

End Class
