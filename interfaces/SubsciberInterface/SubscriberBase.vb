Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports PriPROC6.Interface.Base
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.regConfig
Imports PriPROC6.svcMessage

Public MustInherit Class SubscriberBase : Inherits WritableXML : Implements SubscribeDef

#Region "Metadata Properties"

    Public Overrides ReadOnly Property SvcType As eServType
        Get
            Return eServType.Subscriber
        End Get
    End Property

    Private _SubsciberName As String
    Public Overrides Property Name As String Implements SubscribeDef.Name
        Get
            Return _SubsciberName
        End Get
        Set(value As String)
            _SubsciberName = value
        End Set
    End Property

    Public MustOverride ReadOnly Property ModuleVersion As Version Implements SubscribeDef.ModuleVersion

    Overrides ReadOnly Property Tag As String Implements SubscribeDef.Tag
        Get
            Return String.Format(
                "{0}\Subscriber:{1}",
                Environment.MachineName,
                Name
            )
        End Get
    End Property

    Private _EntryType As Integer
    Public Property EntryType As Integer Implements SubscribeDef.EntryType
        Get
            Return _EntryType
        End Get
        Set(value As Integer)
            _EntryType = value
        End Set
    End Property

    Private _Verbosity As Integer
    Public Property Verbosity As Integer Implements SubscribeDef.Verbosity
        Get
            Return _Verbosity
        End Get
        Set(value As Integer)
            _Verbosity = value
        End Set
    End Property

    Private _Source As Integer
    Public Property Source As Integer Implements SubscribeDef.Source
        Get
            Return _Source
        End Get
        Set(value As Integer)
            _Source = value
        End Set
    End Property

    Private _Console As Boolean
    Public Property Console As Boolean Implements SubscribeDef.Console
        Get
            Return _Console
        End Get
        Set(value As Boolean)
            _Console = value
        End Set
    End Property

    Private _defaultStart As Boolean
    Public Property defaultStart As Boolean Implements SubscribeDef.defaultStart
        Get
            Return _defaultStart
        End Get
        Set(value As Boolean)
            _defaultStart = value
        End Set
    End Property

    Public Overrides Property myProperties(log As Object, ParamArray Name() As String) As String
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

#Region "Discovery Messages"

    Public MustOverride Sub writeXML(ByRef outputStream As XmlWriter) Implements SubscribeDef.writeXML

    Overrides Sub toXML(ByRef outputStream As XmlWriter)
        With outputStream
            .WriteStartElement("service")
            .WriteAttributeString("name", Name)
            .WriteAttributeString("host", Environment.MachineName)
            .WriteAttributeString("version", ModuleVersion.ToString)
            .WriteAttributeString("svctype", "subscriber")

            .WriteStartElement("filter")
            .WriteAttributeString("EntryType", thisConfig.regValue(False, "EntryType"))
            .WriteAttributeString("Verbosity", thisConfig.regValue(False, "Verbosity"))
            .WriteAttributeString("Source", thisConfig.regValue(False, "Source"))

            .WriteEndElement()

            writeXML(outputStream)
            .WriteEndElement()
        End With

    End Sub

#End Region

#Region "Subscriber Info"

    Private _start As Boolean = False
    Public ReadOnly Property Start As Boolean Implements SubscribeDef.Start
        Get
            Return _start
        End Get
    End Property

    Private _svcstate As eServiceState = eServiceState.stopped
    Public Overrides Property svc_state As eServiceState Implements SubscribeDef.svc_state
        Get
            Return _svcstate
        End Get
        Set(value As eServiceState)
            _svcstate = value
        End Set
    End Property

#End Region

#Region "Control Panel"

    Public MustOverride ReadOnly Property thisIcon As Dictionary(Of String, Icon)
    Public Function Icon() As Dictionary(Of String, Icon) Implements SubscribeDef.Icon
        Return thisIcon
    End Function

    Public MustOverride Function useCpl(ByRef o As Object, ParamArray args() As String) As Object Implements SubscribeDef.useCpl
    Public MustOverride Sub DrawTree(ByRef Parent As TreeNode, ByRef MEF As Object, ByVal p As oServiceBase, ByRef IconList As Dictionary(Of String, Integer)) Implements SubscribeDef.DrawTree
    Public MustOverride Sub ContextMenu(ByRef sender As Object, ByRef e As System.ComponentModel.CancelEventArgs, ByRef p As oServiceBase, ParamArray args() As String) Implements SubscribeDef.ContextMenu

    Public ReadOnly Property TreeTag(p As oServiceBase) As String Implements SubscribeDef.TreeTag
        Get
            Return String.Format("{0}\{1}", p.Host, Name)
        End Get
    End Property

#End Region

#Region "Start / Stop subscriber"

    Overridable Sub svcStart(ByRef Log As objBase)

    End Sub

    Overridable Sub svcStop(ByRef Log As objBase)

    End Sub

    Overrides Function svc_start(Optional ByRef Log As objBase = Nothing) As objBase Implements SubscribeDef.svc_start

        svc_state = eServiceState.starting

        Dim l As oMsgLog
        If IsNothing(Log) Then
            l = New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)
        Else
            l = Log
        End If

        Try
            With thisConfig
                If .regValue(False, "EntryType").ToString.Length = 0 Then
                    .regValue(False, "EntryType") = Me.EntryType
                End If
                If .regValue(False, "Verbosity").ToString.Length = 0 Then
                    .regValue(False, "Verbosity") = Me.Verbosity
                End If
                If .regValue(False, "Source").ToString.Length = 0 Then
                    .regValue(False, "Source") = Me.Source
                End If
            End With

            trdSubscriber = New System.Threading.Thread(AddressOf Subscription)
            With trdSubscriber
                .Name = Tag
                .IsBackground = True
                l.LogData.AppendFormat(
                    "Starting subscriber thread {0}...",
                    .Name
                ).AppendLine()
                .Start()
            End With

            svcStart(l)
            svc_state = eServiceState.started

        Catch ex As Exception
            svc_state = eServiceState.stopped
            l.setException(ex)

        End Try

        Return l

    End Function

    Public Overrides Function svc_stop(Optional ByRef Log As objBase = Nothing) As objBase Implements SubscribeDef.svc_stop

        Dim l As oMsgLog
        If IsNothing(Log) Then
            l = New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)
        Else
            l = Log
        End If

        Try
            With trdSubscriber
                l.LogData.AppendFormat(
                    "Stopping thread [{0}] on {1}...",
                    .Name,
                    Environment.MachineName
                ).AppendLine()
            End With

            svcStop(l)

        Catch ex As Exception
            l.setException(ex)

        Finally
            svc_state = eServiceState.stopping

        End Try

        Return l

    End Function

#End Region

#Region "Parent"

    Private _msgFactory As msgFactory
    Private _thisConfig As svcConfig
    Private _LogQ As Queue(Of Byte()) = Nothing

    Public Sub setParent(ByRef ServiceHost As Object, ByVal Props As SubscribeDefprops) Implements SubscribeDef.setParent

        _thisConfig = New svcConfig("PriPROC6", Props.Name)
        _msgFactory = CallByName(ServiceHost, "msgFactory", CallType.Get, Nothing)
        _LogQ = CallByName(ServiceHost, "LogQ", CallType.Get, Nothing)

        With Me
            .Name = Props.Name
            .defaultStart = Props.defaultStart
            .EntryType = Props.EntryType
            .Verbosity = Props.Verbosity
            .Source = Props.Source
            .Console = Props.Console
        End With

        With _thisConfig
            If .regValue(False, "start").length = 0 Then
                Select Case defaultStart
                    Case True
                        .regValue(False, "start") = "1"
                    Case Else
                        .regValue(False, "start") = "0"
                End Select

            Else
                _start = String.Compare(.regValue(False, "start"), "1", False) = 0
            End If

        End With
    End Sub

    Public ReadOnly Property msgFactory As msgFactory Implements SubscribeDef.msgFactory
        Get
            Return _msgFactory
        End Get
    End Property

    Public Property LogQ As Queue(Of Byte()) Implements SubscribeDef.LogQ
        Get
            Return _LogQ
        End Get
        Set(value As Queue(Of Byte()))
            _LogQ = value
        End Set
    End Property

    Public Overrides Property thisConfig As svcConfig Implements SubscribeDef.thisConfig
        Get
            Return _thisConfig
        End Get
        Set(value As svcConfig)
            _thisConfig = value
        End Set
    End Property

#End Region

#Region "Subscriber thread"

    Private _Buffer As New Queue(Of Byte())
    Public Sub NewMessage(data() As Byte) Implements SubscribeDef.NewMessage
        _Buffer.Enqueue(data)
    End Sub

    Private trdSubscriber As System.Threading.Thread
    Private Sub Subscription()
        Do
            Try
                If _Buffer.Count > 0 Then
                    Dim msg As msgBase = msgFactory.Decode(_Buffer.Dequeue)
                    If FilterMatch(TryCast(msg.thisObject, oMsgLog)) Then
                        NewEntry(msg)
                    End If
                Else
                    Threading.Thread.Sleep(100)
                End If

            Catch ex As Exception
                _LogQ.Enqueue(msgFactory.EncodeRequest("log", New oMsgLog(Name, ex)))

            Finally

            End Try

        Loop Until svc_state = eServiceState.stopping And _Buffer.Count = 0

        trdSubscriber = Nothing
        svc_state = eServiceState.stopped

    End Sub

    Public MustOverride Sub NewEntry(Entry As msgBase) Implements SubscribeDef.NewEntry

    Private Function FilterMatch(ByRef Log As oMsgLog) As Boolean
        With thisConfig
            Dim et As Integer = .regValue(False, "EntryType")

            Dim f As Boolean = False
            Do
                If et >= LogEntryType.FailureAudit Then
                    et -= LogEntryType.FailureAudit
                    If Log.EntryType = LogEntryType.FailureAudit Then
                        f = True
                        Exit Do
                    End If
                End If

                If et >= LogEntryType.SuccessAudit Then
                    et -= LogEntryType.SuccessAudit
                    If Log.EntryType = LogEntryType.SuccessAudit Then
                        f = True
                        Exit Do
                    End If
                End If

                If et >= LogEntryType.Information Then
                    et -= LogEntryType.Information
                    If Log.EntryType = LogEntryType.Information Then
                        f = True
                        Exit Do
                    End If
                End If

                If et >= LogEntryType.Warning Then
                    et -= LogEntryType.Warning
                    If Log.EntryType = LogEntryType.Warning Then
                        f = True
                        Exit Do
                    End If
                End If

                If et >= LogEntryType.Err Then
                    et -= LogEntryType.Err
                    If Log.EntryType = LogEntryType.Err Then
                        f = True
                        Exit Do
                    End If
                End If

                Exit Do
            Loop

            Dim so As Integer = .regValue(False, "Source")
            Dim fso As Boolean = False
            Do
                If so >= EvtLogSource.WEB Then
                    so -= EvtLogSource.WEB
                    If Log.LogSource = EvtLogSource.WEB Then
                        fso = True
                        Exit Do
                    End If
                End If

                If so >= EvtLogSource.SYSTEM Then
                    so -= EvtLogSource.SYSTEM
                    If Log.LogSource = EvtLogSource.SYSTEM Then
                        fso = True
                        Exit Do
                    End If
                End If

                If so >= EvtLogSource.APPLICATION Then
                    so -= EvtLogSource.APPLICATION
                    If Log.LogSource = EvtLogSource.APPLICATION Then
                        fso = True
                        Exit Do
                    End If
                End If

                Exit Do
            Loop

            Return CInt(.regValue(False, "Verbosity")) >= Log.Verbosity And f And fso

        End With

        Return True

    End Function

#End Region

End Class
