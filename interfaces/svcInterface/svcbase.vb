Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports PriPROC6.PriSock
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.regConfig
Imports PriPROC6.svcMessage
Imports PriPROC6.Interface.Base

Public MustInherit Class svcbase
    Inherits WritableXML : Implements svcDef

#Region "Listeners"

    Private _thisiListener As iListener = Nothing
    Public Property thisiListener() As iListener
        Get
            Return _thisiListener
        End Get
        Set(ByVal value As iListener)
            _thisiListener = value
        End Set
    End Property

    Private _thisuListener As uListener = Nothing
    Public Property thisuListener() As uListener
        Get
            Return _thisuListener
        End Get
        Set(ByVal value As uListener)
            _thisuListener = value
        End Set
    End Property

#End Region

#Region "Metadata Properties"

    Public Overrides ReadOnly Property SvcType As eServType
        Get
            Return eServType.Service
        End Get
    End Property

    Private _Name As String
    Public Overrides Property Name As String Implements svcDef.Name
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Public MustOverride ReadOnly Property ModuleVersion As Version Implements svcDef.ModuleVersion

    Private _defaultPort As Integer
    Public Property defaultPort As Integer Implements svcDef.defaultPort
        Get
            Return _defaultPort
        End Get
        Set(value As Integer)
            _defaultPort = value
        End Set
    End Property

    Private _udp As Boolean
    Public Property udp As Boolean Implements svcDef.udp
        Get
            Return _udp
        End Get
        Set(value As Boolean)
            _udp = value
        End Set
    End Property

    Private _defaultStart As Boolean
    Public Property defaultStart As Boolean Implements svcDef.defaultStart
        Get
            Return _defaultStart
        End Get
        Set(value As Boolean)
            _defaultStart = value
        End Set
    End Property

    Public Overrides Property MyProperties(log As Object, ParamArray Name() As String) As String
        Get
            Return thisConfig.regValue(False, Name)

        End Get
        Set(value As String)
            If Not String.Compare(thisConfig.regValue(False, Name), value, True) = 0 Then
                With TryCast(log, oMsgLog)
                    .LogData.AppendFormat("Setting property on {2}: [{0}] = {1}.", Join(Name, "\"), value, Tag).AppendLine()
                    thisConfig.regValue(False, Name) = value
                    .EntryType = LogEntryType.SuccessAudit
                End With
            End If

        End Set
    End Property

#End Region

#Region "Service Info"

    Function svc_info() As svcbase Implements svcDef.svc_info
        Return Me

    End Function

    Private _svcstate As eServiceState = eServiceState.stopped
    Public Overrides Property svc_state As eServiceState Implements svcDef.svc_state
        Get
            Return _svcstate
        End Get
        Set(value As eServiceState)
            _svcstate = value
        End Set
    End Property

    Public ReadOnly Property Start As Boolean Implements svcDef.Start
        Get
            Return String.Compare(_thisConfig.regValue(False, "start"), "1", False) = 0
        End Get
    End Property

    Public ReadOnly Property Port As Integer Implements svcDef.Port
        Get
            Return _thisConfig.regValue(False, "port")
        End Get
    End Property

    Public Overrides ReadOnly Property Tag As String Implements svcDef.Tag
        Get
            Return String.Format(
                "{0}\{1}",
                Environment.MachineName,
                Name
            )
        End Get
    End Property

    Public Sub Config(ByRef Svc As List(Of Object), ByRef Log As oMsgLog) Implements svcDef.Config
        ConfigMsg(Svc, Log)
    End Sub

    Public Overridable Sub ConfigMsg(ByRef Svc As List(Of Object), ByRef Log As oMsgLog)

    End Sub

#End Region

#Region "Parent"

    Private _ServiceHost As Object
    Private _thisConfig As svcConfig

    'Private _Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))
    'Private _Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))
    'Private _msgFactory As msgFactory    
    'Private _LogQ As Queue(Of Byte()) = Nothing
    'Private _ModulesFolder As IO.DirectoryInfo

    Public Sub setParent(ByRef ServiceHost As Object, ByVal Props As svcDefprops) Implements svcDef.setParent

        _ServiceHost = ServiceHost
        _thisConfig = New svcConfig("PriPROC6", Props.Name)

        '_Modules = CallByName(ServiceHost, "Modules", CallType.Get, Nothing)
        '_Subscribers = CallByName(ServiceHost, "Subscribers", CallType.Get, Nothing)
        '_msgFactory = CallByName(ServiceHost, "msgFactory", CallType.Get, Nothing)
        '_LogQ = CallByName(ServiceHost, "LogQ", CallType.Get, Nothing)
        '_ModulesFolder = CallByName(ServiceHost, "ModulesFolder", CallType.Get, Nothing)

        With Me
            .Name = Props.Name
            .defaultPort = Props.defaultPort
            .defaultStart = Props.defaultStart
            .udp = Props.udp
        End With

        With _thisConfig
            If .regValue(False, "start").length = 0 Then
                If defaultStart Then
                    .regValue(False, "start") = "1"
                Else
                    .regValue(False, "start") = "0"
                End If
            End If

            If .regValue(False, "port").length = 0 Then
                .regValue(False, "port") = defaultPort
            End If

        End With
    End Sub

    Public ReadOnly Property ServiceHost As Object
        Get
            Return _ServiceHost
        End Get
    End Property

    Public ReadOnly Property ModulesFolder As IO.DirectoryInfo
        Get
            Return CallByName(ServiceHost, "ModulesFolder", CallType.Get, Nothing)
        End Get
    End Property

    Public Overrides Property thisConfig() As svcConfig Implements svcDef.thisConfig
        Get
            Return _thisConfig
        End Get
        Set(value As svcConfig)
            _thisConfig = value
        End Set
    End Property

    Public ReadOnly Property msgFactory As msgFactory Implements svcDef.msgFactory
        Get
            Return CallByName(ServiceHost, "msgFactory", CallType.Get, Nothing)
        End Get
    End Property

    Public ReadOnly Property Modules As Dictionary(Of String, svcDef) 'IEnumerable(Of Lazy(Of svcDef, svcDefprops))
        Get
            Return CallByName(ServiceHost, "Modules", CallType.Get, Nothing)
        End Get
    End Property

    Public ReadOnly Property Subscribers As Dictionary(Of String, SubscribeDef) 'As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))
        Get
            Return CallByName(ServiceHost, "Subscribers", CallType.Get, Nothing)
        End Get
    End Property

    Public Property LogQ As Queue(Of Byte())
        Get
            Return CallByName(ServiceHost, "LogQ", CallType.Get, Nothing)
        End Get
        Set(value As Queue(Of Byte()))
            CallByName(ServiceHost, "LogQ", CallType.Set, value)
        End Set
    End Property

#End Region

#Region "Discovery"

    Public MustOverride Sub writeXML(ByRef outputStream As XmlWriter) Implements svcDef.writeXML

    Overrides Sub toXML(ByRef outputStream As XmlWriter)
        With outputStream
            .WriteStartElement("service")
            .WriteAttributeString("name", Name)
            .WriteAttributeString("host", Environment.MachineName)
            .WriteAttributeString("svctype", "service")
            .WriteAttributeString("port", Port)
            .WriteAttributeString("version", ModuleVersion.ToString)

            Try
                writeXML(outputStream)

            Catch ex As Exception
                LogQ.Enqueue(msgFactory.EncodeRequest("log", New oMsgLog(Me.Name, ex)))

            End Try

            .WriteEndElement()

        End With

    End Sub

#End Region

#Region "Control Panel"

    Public MustOverride ReadOnly Property thisIcon As Dictionary(Of String, Icon)
    Public Function Icon() As Dictionary(Of String, Icon) Implements svcDef.Icon
        Return thisIcon
    End Function

    Public MustOverride Function useCpl(ByRef o As Object, ParamArray args() As String) As Object Implements svcDef.useCpl
    Public MustOverride Sub DrawTree(ByRef Parent As TreeNode, ByRef MEF As Object, ByVal p As oServiceBase, ByRef IconList As Dictionary(Of String, Integer)) Implements svcDef.DrawTree
    Public MustOverride Sub ContextMenu(ByRef sender As Object, ByRef e As System.ComponentModel.CancelEventArgs, ByRef p As oServiceBase, ParamArray args() As String) Implements svcDef.ContextMenu

    Public ReadOnly Property TreeTag(p As oServiceBase) As String Implements svcDef.TreeTag
        Get
            If String.Compare(p.Name, "discovery", True) = 0 Then
                Return String.Format("{0}", p.Host)
            Else
                Return String.Format("{0}\{1}", p.Host, Name)
            End If
        End Get
    End Property

#End Region

#Region "Start / stop service"

    Public MustOverride Sub svcStart(ByRef log As oMsgLog)
    Public Overridable Sub svcStop(ByRef log As oMsgLog)

    End Sub

    Public Overrides Function svc_start(Optional ByRef Log As objBase = Nothing) As objBase Implements svcDef.svc_start

        _svcstate = eServiceState.starting
        Dim l As oMsgLog
        If IsNothing(Log) Then
            l = New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)
        Else
            l = Log
        End If

        With l
            Try
                .LogData.Append(
                    String.Format(
                        "Starting service {0}...",
                        Tag
                    )
                ).AppendLine()

                thisiListener = New iListener(
                        Port,
                        Name,
                        l,
                        LogQ,
                        msgFactory,
                        AddressOf tcpByte
                    )
                Do
                    Select Case thisiListener.svc_state
                        Case eSocketState.started
                            Exit Do

                        Case eSocketState.stopped
                            Throw New Exception("")

                        Case Else
                            Threading.Thread.Sleep(100)

                    End Select
                Loop

                If udp Then
                    thisuListener = New uListener(
                        Port,
                        Name,
                        l,
                        LogQ,
                        msgFactory,
                        AddressOf udpByte
                    )
                    Do
                        Select Case thisiListener.svc_state
                            Case eSocketState.started
                                Exit Do

                            Case eSocketState.stopped
                                Throw New Exception("")

                            Case Else
                                Threading.Thread.Sleep(100)

                        End Select
                    Loop
                End If

                Try
                    svcStart(l)
                    .LogData.Append(String.Format("Started service {0} OK.", Tag)).AppendLine()

                Catch moduleException As Exception
                    .LogData.Append(String.Format("[{0}] module load failure.", Me.Name)).AppendLine()
                    .setException(moduleException)
                    Throw moduleException

                End Try


            Catch ex As Exception
                _svcstate = eServiceState.stopped
                With .LogData
                    .Append(
                        String.Format(
                            "Start [{0}] FAILED.",
                            Tag
                        )
                    ).AppendLine()

                End With

            Finally
                If Not _svcstate = eServiceState.stopped Then
                    _svcstate = eServiceState.started

                Else
                    _thisiListener.disposeMe(l)

                End If

            End Try

        End With

        Return l

    End Function

    Public Overrides Function svc_stop(Optional ByRef Log As objBase = Nothing) As objBase Implements svcDef.svc_stop

        _svcstate = eServiceState.stopping

        Dim l As oMsgLog
        If IsNothing(Log) Then
            l = New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)
        Else
            l = Log
        End If

        With l
            Try
                .LogData.Append(
                    String.Format(
                        "Stopping {0}...",
                        Tag
                    )
                ).AppendLine()

                If Not IsNothing(_thisiListener) Then
                    If _thisiListener.svc_state = eSocketState.started Then
                        _thisiListener.disposeMe(l)
                        Do
                            Select Case thisiListener.svc_state
                                Case eSocketState.stopped
                                    Exit Do

                                Case Else
                                    Threading.Thread.Sleep(100)

                            End Select
                        Loop
                    End If
                End If

                If udp And Not IsNothing(_thisuListener) Then
                    If _thisuListener.svc_state = eSocketState.started Then
                        _thisuListener.disposeMe(l)
                        Do
                            Select Case thisiListener.svc_state
                                Case eSocketState.stopped
                                    Exit Do

                                Case Else
                                    Threading.Thread.Sleep(100)

                            End Select
                        Loop
                    End If
                End If

                Try
                    svcStop(l)
                    .LogData.Append(String.Format("Stopped service [{0}] OK.", Me.Name)).AppendLine()
                    _svcstate = eServiceState.stopped

                Catch moduleException As Exception
                    .LogData.Append(String.Format("[{0}] module stop failure.", Me.Name)).AppendLine()
                    .setException(moduleException)
                    Throw moduleException

                End Try


            Catch ex As Exception
                _svcstate = eServiceState.stopped
                With .LogData
                    .Append(
                        String.Format(
                            "Stop [{0}] FAILED.",
                            Tag
                        )
                    ).AppendLine()

                End With

            End Try
        End With

        Return l

    End Function

#End Region

#Region "Message Handler"

    Public Sub udpByte(ByVal e As Byte(), RemoteEndpoint As String) Implements svcDef.udpByte

        Using thislog As New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.Information)
            Try
                thislog.LogData.AppendFormat("Inbound UDP msg from {0}.", RemoteEndpoint).AppendLine()
                udpMsg(msgFactory.Decode(e), thislog)

            Catch ex As Exception
                thislog.setException(ex)

            Finally
                If Not thislog.EntryType = LogEntryType.Information Then
                    LogQ.Enqueue(msgFactory.EncodeRequest("log", thislog))
                End If

            End Try

        End Using

    End Sub

    Public Function tcpByte(ByVal e As Byte(), RemoteEndpoint As String) As Byte() Implements svcDef.tcpByte
        Using thislog As New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.Information)
            Try
                thislog.LogData.AppendFormat("Inbound TCP msg from {0}.", RemoteEndpoint).AppendLine()
                Return tcpMsg(msgFactory.Decode(e), thislog)

            Catch ex As Exception
                thislog.setException(ex)
                Return msgFactory.EncodeResponse(
                    "log",
                    400,
                    ex.Message,
                    thislog
                )

            Finally
                If Not thislog.EntryType = LogEntryType.Information Then
                    LogQ.Enqueue(msgFactory.EncodeRequest("log", thislog))
                End If

            End Try

        End Using
    End Function

    Public Overridable Sub udpMsg(ByRef msg As msgBase, ByRef thisLog As oMsgLog)
        Throw New NotImplementedException()
    End Sub

    Public Overridable Function tcpMsg(ByRef msg As msgBase, ByRef thisLog As oMsgLog) As Byte()
        Throw New NotImplementedException()
    End Function

#End Region

#Region "Broadcast"

    Public Function Send(ByVal ServerAddress As String, ByVal PortNumber As Integer, ByRef data() As Byte) As msgBase
        Try
            If String.Compare(ServerAddress, Environment.MachineName) = 0 Then
                For Each m As svcDef In Modules.Values
                    If (m.Port = PortNumber) And m.svc_state = eServiceState.started Then
                        Return msgFactory.Decode(m.tcpByte(data, String.Format("{0}", Me.Tag)))
                    End If
                Next
                Throw New Exception("Local message delivery failed.")

            Else
                Using cli As New iClient(ServerAddress, PortNumber)
                    Return msgFactory.Decode(cli.Send(data))

                End Using
            End If

        Catch ex As Exception
            LogQ.Enqueue(msgFactory.EncodeRequest("log", New oMsgLog(Name, ex)))
            Return Nothing

        End Try

    End Function

    Public Sub Broadcast(ByVal message As Byte(), Optional ByVal BroadcastType As eBroadcastType = eBroadcastType.bcPublic)
        Using cli As New uClient(Me.Port, BroadcastType)
            cli.Send(message)
        End Using
    End Sub

    Public Sub Broadcast(ByVal Port As Integer, ByVal message As Byte(), Optional ByVal BroadcastType As eBroadcastType = eBroadcastType.bcPublic)
        Using cli As New uClient(Port, BroadcastType)
            cli.Send(message)
        End Using
    End Sub

    Public Sub Broadcast(ByVal Port As Integer, ByVal message As Byte(), ByVal BroadcastType As eBroadcastType, ByVal EndPoint As String)
        Using cli As New uClient(Port, BroadcastType, EndPoint)
            cli.Send(message)
        End Using
    End Sub

#End Region

End Class
