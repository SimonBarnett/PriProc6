Imports System.Windows.Forms
Imports System.Xml
Imports Priproc6.PriSock
Imports Priproc6.svcMessages
Imports Priproc6.regConfig

Public MustInherit Class svcbase : Implements svcDef

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

    Private _ServiceName As String
    Public Property ServiceName As String Implements svcDef.ServiceName
        Get
            Return _ServiceName
        End Get
        Set(value As String)
            _ServiceName = value
        End Set
    End Property

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

    Private _tcp As Boolean
    Public Property tcp As Boolean Implements svcDef.tcp
        Get
            Return _tcp
        End Get
        Set(value As Boolean)
            _tcp = value
        End Set
    End Property

#End Region

#Region "Service Info"

    Private _svcstate As eServiceState = eServiceState.stopped
    Public Property svc_state As eServiceState Implements svcDef.svc_state
        Get
            Return _svcstate
        End Get
        Set(value As eServiceState)
            _svcstate = value
        End Set
    End Property

    Private _Start As Boolean = False
    Public ReadOnly Property Start As Boolean Implements svcDef.Start
        Get
            Return _Start
        End Get
    End Property

    Private _port As Integer
    Public ReadOnly Property Port As Integer
        Get
            Return _port
        End Get
    End Property

#End Region

#Region "Parent"

    Private _p As svc_hanger
    Private _thisConfig As svcConfig

    Public Sub setParent(ByRef p As svc_hanger) Implements svcDef.setParent
        _p = p
        _thisConfig = New svcConfig("PriPROC6", ServiceName)
        With _thisConfig
            If .regValue(False, "start").length = 0 Then
                .regValue(False, "start") = "0"
            Else
                _Start = String.Compare(.regValue(False, "start"), "1", False) = 0
            End If

            If tcp Then
                If .regValue(False, "port").length = 0 Then
                    .regValue(False, "port") = defaultPort
                End If
                _port = .regValue(False, "port")
            End If

        End With
    End Sub

    Public ReadOnly Property Parent As svc_hanger Implements svcDef.Parent
        Get
            Return _p
        End Get
    End Property

    Public Property thisConfig() As svcConfig Implements svcDef.thisConfig
        Get
            Return _thisConfig
        End Get
        Set(value As svcConfig)
            _thisConfig = value
        End Set
    End Property

    Public ReadOnly Property msgFactory As msgFactory Implements svcDef.msgFactory
        Get
            Return _p.msgFactory
        End Get
    End Property

    Public ReadOnly Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))
        Get
            Return _p.modules
        End Get
    End Property

    Public ReadOnly Property LoadedModules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))
        Get
            Return _p.LoadedModules
        End Get
    End Property

    Public Property LogQ As Queue(Of Byte())
        Get
            Return _p.logq
        End Get
        Set(value As Queue(Of Byte()))
            _p.logq = value
        End Set
    End Property

    Public ReadOnly Property BroadcastPort As Integer Implements svcDef.BroadcastPort
        Get
            Return Parent.BroadcastPort
        End Get
    End Property

#End Region

#Region "Overridable methods"

    Public MustOverride Sub svc_info(ByRef wr As XmlWriter) Implements svcDef.svc_info

    Public Overridable ReadOnly Property Cpl As UserControl Implements svcDef.Cpl
        Get
            Throw New NotImplementedException()
        End Get
    End Property

#End Region

#Region "Start / stop service"

    Public MustOverride Sub svcStart(ByRef log As oMsgLog)
    Public Overridable Sub svcStop(ByRef log As oMsgLog)

    End Sub

    Public Function svc_start() As Byte() Implements svcDef.svc_start

        Dim retry As Integer
        Dim l As New oMsgLog(Me.ServiceName, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)

        With l
            Try
                _svcstate = eServiceState.starting
                With .LogData
                    .Append(String.Format("Starting [{0}] service...", Me.ServiceName)).AppendLine()
                    svcStart(l)

                    If tcp Then
                        retry = 0
                        Do Until Not IsNothing(thisiListener) Or retry > 4
                            Try
                                thisiListener = New iListener(
                                    Port,
                                    AddressOf newMessage
                                )
                                .AppendFormat(
                                    "Started TCP Listener on {0}:{1}.",
                                    Environment.MachineName,
                                    Port
                                ).AppendLine()

                            Catch ex As Exception
                                retry += 1
                                Threading.Thread.Sleep(3000)

                            End Try
                        Loop

                        If IsNothing(thisiListener) Then
                            Throw New Exception(
                                String.Format(
                                    "Could not start TCP Listener on {0}. " &
                                    "Another instance may still be running.",
                                    Port
                                )
                            )
                        End If
                    End If


                    If udp Then
                        retry = 0
                        Do Until Not IsNothing(thisuListener) Or retry > 4
                            Try
                                thisuListener = New uListener(
                                    Parent.BroadcastPort,
                                    AddressOf newMessage
                                )
                                .AppendFormat(
                                    "Started UDP Listener on {0}:{1}.",
                                    Environment.MachineName,
                                    Port
                                ).AppendLine()

                            Catch ex As Exception
                                retry += 1
                                Threading.Thread.Sleep(3000)

                            End Try

                        Loop
                        If IsNothing(thisuListener) Then
                            Throw New Exception(
                                String.Format(
                                    "Could not start UDP Listener on {0}. " &
                                    "Another instance may still be running.",
                                    Port
                                )
                            )
                        End If
                    End If

                    .Append(String.Format("Starting [{0}] service...OK.", Me.ServiceName)).AppendLine()

                End With

            Catch ex As Exception
                _svcstate = eServiceState.stopped
                .EntryType = LogEntryType.FailureAudit
                With .LogData
                    .Append(String.Format("Starting [{0}] service FAILED..", Me.ServiceName)).AppendLine()
                    .Append(ex.Message).AppendLine()
                    .AppendLine(ex.StackTrace).AppendLine()

                End With

            Finally
                If Not _svcstate = eServiceState.stopped Then
                    _svcstate = eServiceState.started

                End If

            End Try

        End With

        Return msgFactory.EncodeRequest("log", l)

    End Function

    Public Function svc_stop() As Byte() Implements svcDef.svc_stop

        Dim l As New oMsgLog(Me.ServiceName, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.SuccessAudit)
        With l
            Try
                With .LogData
                    .Append(String.Format("Stopping [{0}] service...", Me.ServiceName))
                    _svcstate = eServiceState.stopping
                    svcStop(l)
                    .Append("Ok").AppendLine()
                End With

            Catch ex As Exception
                .EntryType = LogEntryType.FailureAudit
                With .LogData
                    .Append("Failed").AppendLine()
                    .Append(ex.Message).AppendLine()
                    .AppendLine(ex.StackTrace).AppendLine()

                End With

            Finally
                _svcstate = eServiceState.stopped

            End Try
        End With

        Return msgFactory.EncodeRequest("log", l)

    End Function

#End Region

#Region "Message Handler"

    Public Overridable Sub udpMsg(ByRef msg As msgBase)
        Throw New NotImplementedException()
    End Sub

    Public Overridable Function tcpMsg(ByRef msg As msgBase) As Byte()
        Throw New NotImplementedException()
    End Function

    Public Overridable Sub newMessage(ByVal sender As Object, ByVal e As byteMsg) Implements svcDef.newMessage
        Try
            Select Case e.ProtocolType
                Case eProtocolType.tcp
                    With TryCast(sender, iListener)
                        .dResponse.Add(e.msgID, tcpMsg(msgFactory.Decode(e.Message)))
                    End With

                Case eProtocolType.udp
                    udpMsg(msgFactory.Decode(e.Message))

                Case Else
                    Throw New NotImplementedException()

            End Select

        Catch ex As Exception
            Dim er As New oMsgLog(ServiceName, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.FailureAudit)
            With er.LogData
                .Append(ex.Message).AppendLine()
                .Append(ex.StackTrace).AppendLine()
            End With
            LogQ.Enqueue(msgFactory.EncodeRequest("log", er))

        End Try

    End Sub

#End Region

#Region "Broadcast"

    Public Sub Broadcast(ByVal message As Byte(), Optional ByVal BroadcastType As eBroadcastType = eBroadcastType.bcPublic)
        Using cli As New uClient(BroadcastPort, BroadcastType)
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
