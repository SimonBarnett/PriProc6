Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports System.IO

Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Cpl

Imports PriPROC6.PriSock
Imports PriPROC6.svcMessage
Imports System.Xml
Imports PriPROC6.Services.WebRelay
Imports PriPROC6.Services.Loader

Public Class MMC

#Region "MEF objects"

    Dim _container As CompositionContainer
    Dim mef As New MEF

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

            Dim StartExecption As Exception = Nothing
            Try
                StartExecption = ReloadModules()
                If Not StartExecption Is Nothing Then Throw StartExecption

                _msgFactory = New msgFactory(mef.mefmsg)

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

    Private Function ReloadModules() As Exception

        Dim ret As Exception = Nothing

        Try
            'An aggregate catalog that combines multiple catalogs
            Dim catalog = New AggregateCatalog()

            'Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(New AssemblyCatalog(GetType(PriPROC6.service.Discovery).Assembly))
            catalog.Catalogs.Add(New AssemblyCatalog(GetType(MMC).Assembly))

            'IMPORTANT!
            'You will need to adjust this line to match your local path!
            Dim fi As New FileInfo(System.Reflection.Assembly.GetExecutingAssembly().ToString)
            catalog.Catalogs.Add(New DirectoryCatalog(Path.Combine(fi.DirectoryName, "modules")))

            'Create the CompositionContainer with the parts in the catalog
            _container = New CompositionContainer(catalog)

            'Fill the imports of this object
            _container.ComposeParts(mef)


            For Each msg As Lazy(Of msgInterface, msgInterfaceData) In mef.Messages
                If Not mef.mefmsg.Keys.Contains(String.Format("{0}:{1}", msg.Metadata.verb, msg.Metadata.msgType)) Then
                    With msg.Value
                        .msgType = msg.Metadata.msgType
                    End With
                    mef.mefmsg.Add(String.Format("{0}:{1}", msg.Metadata.verb, msg.Metadata.msgType), msg.Value)
                End If
            Next

            For Each panel As Lazy(Of cplInterface, cplProps) In mef.Cpl
                If Not mef.mefcpl.Keys.Contains(panel.Metadata.Name) Then
                    With panel
                        .Value.Name = .Metadata.Name
                        mef.mefcpl.Add(.Metadata.Name, .Value)
                    End With
                End If
            Next

            For Each subscr As Lazy(Of SubscribeDef, SubscribeDefprops) In mef.Subscribers
                If Not mef.mefsub.Keys.Contains(subscr.Metadata.Name) Then
                    With subscr
                        .Value.setParent(Me, .Metadata)
                        mef.mefsub.Add(.Metadata.Name, .Value)

                        For Each k As String In .Value.Icon.Keys
                            Me.icons.Images.Add(.Value.Icon(k))
                            iconList.Add(k, icons.Images.Count - 1)
                        Next

                    End With

                End If

                'With subscr.Value
                '    .Name = subscr.Metadata.Name
                '    .EntryType = subscr.Metadata.EntryType
                '    .Verbosity = subscr.Metadata.Verbosity
                '    .Source = subscr.Metadata.Source
                '    .Console = subscr.Metadata.Console

                '    For Each k As String In .Icon.Keys
                '        Me.icons.Images.Add(.Icon(k))
                '        iconList.Add(k, icons.Images.Count - 1)
                '    Next

                'End With
            Next

            For Each svr As Lazy(Of svcDef, svcDefprops) In mef.Modules
                If Not mef.mefsvc.Keys.Contains(svr.Metadata.Name) Then
                    With svr
                        .Value.setParent(Me, .Metadata)
                        mef.mefsvc.Add(.Metadata.Name, .Value)
                        For Each k As String In .Value.Icon.Keys
                            Me.icons.Images.Add(.Value.Icon(k))
                            iconList.Add(k, icons.Images.Count - 1)
                        Next
                    End With
                End If

                'With svr.Value

                '    .Name = svr.Metadata.Name
                '    .defaultPort = svr.Metadata.defaultPort
                '    .defaultStart = svr.Metadata.defaultStart
                '    .udp = svr.Metadata.udp

                '    For Each k As String In .Icon.Keys
                '        Me.icons.Images.Add(.Icon(k))
                '        iconList.Add(k, icons.Images.Count - 1)
                '    Next

                'End With
            Next

        Catch ex As Exception
            ret = ex

        End Try

        Return ret

    End Function
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
            While LogQ.Count > 0 And Not PauseToolStripMenuItem.Checked
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

        ReloadModules()

        If mef.mefsvc.Keys.Contains(Named) Then
            Return mef.mefsvc(Named)
        End If

        If mef.mefsub.Keys.Contains(Named) Then
            Return mef.mefsub(Named)
        End If

        Throw New Exception(String.Format("Module name {0} not found.", Named))

    End Function

    Private Function thisModule2(Named As String) As svcDef

        ReloadModules()

        For Each svr As svcDef In mef.mefsvc.Values
            If String.Compare(svr.Name, Named, True) = 0 Then
                Return svr
            End If
        Next
        For Each svr As SubscribeDef In mef.mefsub.Values
            If String.Compare(svr.Name, Named, True) = 0 Then
                Return TryCast(svr, svcDef)
            End If
        Next
        Return Nothing

    End Function

    Private Delegate Sub rcvDiscoveryInvoker(ByRef msg As msgBase)
    Sub rcvDiscovery(ByRef msg As msgBase)

        If Me.InvokeRequired Then
            Dim args(0) As Object
            args(0) = msg
            Me.Invoke(New rcvDiscoveryInvoker(AddressOf rcvDiscovery), args)

        Else

            If ShowBroadcastsToolStripMenuItem.Checked Then
                Dim l As New oMsgLog(Me.Name, EvtLogSource.SYSTEM)
                l.LogData.Append(msg.msgNode.OuterXml).AppendLine()
                LogQ.Enqueue(msgFactory.EncodeRequest("log", l))
            End If

            Dim loaders As New List(Of XmlNode)
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

                        If String.Compare(o.Name, "loader", True) = 0 Then
                            For Each priweb As XmlNode In svc.SelectNodes("priweb")
                                loaders.Add(priweb)
                            Next

                        End If

                End Select

            Next

            Dim delDiscovery As New List(Of String)
            Dim network As TreeNode = Browser.Nodes("network")

            For Each k As String In svcMap.Keys
                thisModule2("discovery").DrawTree(network, mef, svcMap(k), iconList)
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

            For Each h As String In svcMap.Keys
                For Each s As oServiceBase In svcMap(h).values
                    If String.Compare(s.Name, "webrelay", True) = 0 Then
                        For Each web As PriWeb In s.values
                            web.SetoDataServers(loaders, svcMap)
                        Next
                    End If
                Next
            Next

            propSvcMap.svcMap = svcMap

        End If

    End Sub

#End Region

#Region "Panels"

    Private Sub Browser_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Browser.AfterSelect

        Dim params As String() = e.Node.Name.Split("\")
        Dim useCpl As Object = Nothing

        Select Case params(0).ToLower
            Case "network"
                useCpl = SetPanel("home")

            Case Else
                With svcMap(params(0))
                    Select Case UBound(params)
                        Case 0
                            useCpl = thisModule2("discovery").useCpl(
                                    PropertyObject(params(0)),
                                    params
                            )

                        Case Else
                            If UBound(params) = 1 And String.Compare(params(1), "Subscribers", True) = 0 Then
                                TabPage1.Controls.Clear()

                            Else

                                Dim thisSvc As Object = PropertyObject(params(0), params(1))
                                If Not thisSvc Is Nothing Then
                                    For Each svr As svcDef In mef.mefsvc.Values
                                        If String.Compare(svr.Name, params(1), True) = 0 Then
                                            useCpl = svr.useCpl(thisSvc, params)
                                            Exit For
                                        End If
                                    Next

                                    If useCpl Is Nothing Then
                                        For Each svr As SubscribeDef In mef.mefsub.Values
                                            If String.Compare(svr.Name, params(1), True) = 0 Then
                                                useCpl = svr.useCpl(thisSvc, params)
                                                Exit For
                                            End If
                                        Next

                                    End If

                                End If

                            End If

                    End Select

                End With

        End Select

        If Not useCpl Is Nothing Then
            With TabPage1.Controls
                .Clear()
                .Add(useCpl)
                .Item(0).Dock = DockStyle.Fill
            End With

        Else
            TabPage1.Controls.Clear()

        End If

    End Sub

    Private Function PropertyObject(ServerName As String, Optional ObjectType As String = "discovery") As oServiceBase

        If String.Compare(ObjectType, "discovery", True) = 0 Then
            Return svcMap(ServerName)

        Else
            With svcMap(ServerName)
                For Each svc As oServiceBase In .values
                    If String.Compare(svc.Name, ObjectType, True) = 0 Then
                        Return svc
                    End If
                Next
            End With

        End If
        Return Nothing

    End Function

    Private Function SetPanel(PanelName As String) As Object
        Dim Cpl As Object = Nothing
        For Each panel As cplInterface In mef.mefcpl.Values
            If String.Compare(panel.Name, PanelName, True) = 0 Then
                Return panel.useCpl(Nothing, Nothing)
            End If
        Next
        Return Nothing

    End Function

    Private Sub SaveProperty(Sender As Object, e As CmdEventArgs)
        With e
            Using cli As New iClient(.Server, .Port)
                Dim resp = msgFactory.Decode(cli.Send(msgFactory.EncodeRequest("cmd", .Message)))
                e.errCode = resp.ErrCde
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

        e.Cancel = True
        Dim params As String() = Browser.SelectedNode.Name.Split("\")
        Dim p As oServiceBase

        If String.Compare(params(0), "network", True) = 0 Then
            Exit Sub
        End If

        With svcMap(params(0))
            Select Case UBound(params)
                Case 0
                    e.Cancel = False
                    p = PropertyObject(params(0))

                Case Else
                    If UBound(params) = 1 And String.Compare(params(1), "Subscribers", True) = 0 Then
                        e.Cancel = False
                        p = PropertyObject(params(0))

                    Else
                        e.Cancel = False
                        p = PropertyObject(params(0), params(1))
                    End If

            End Select

        End With

        If Not e.Cancel Then
            For Each svr As svcDef In mef.mefsvc.Values
                If String.Compare(svr.Name, p.Name, True) = 0 Then
                    svr.ContextMenu(sender, e, p, params)
                    Exit Sub
                End If
            Next

            For Each svr As SubscribeDef In mef.mefsub.Values
                If String.Compare(svr.Name, params(1), True) = 0 Then
                    svr.ContextMenu(sender, e, p, params)
                    Exit Sub
                End If
            Next

        End If

    End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        Console.Clear()

    End Sub

    Private Sub ShowBroadcastsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowBroadcastsToolStripMenuItem.Click
        ShowBroadcastsToolStripMenuItem.Checked = Not ShowBroadcastsToolStripMenuItem.Checked

    End Sub

    Private Sub PauseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PauseToolStripMenuItem.Click
        PauseToolStripMenuItem.Checked = Not PauseToolStripMenuItem.Checked

    End Sub

#End Region

End Class
