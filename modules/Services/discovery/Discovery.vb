﻿Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.svcMessage
Imports PriPROC6.PriSock
Imports System.Drawing

<Export(GetType(svcDef))>
<ExportMetadata("Name", "discovery")>
<ExportMetadata("udp", True)>
<ExportMetadata("defaultPort", 8090)>
<ExportMetadata("defaultStart", True)>
Public Class Discovery
    Inherits svcbase
    Implements svcDef

#Region "Start / Stop"

    Public Overrides Sub svcStart(ByRef log As oMsgLog)

        With thisConfig
            If .regValue(False, "broadcastdelay").length = 0 Then
                .regValue(False, "broadcastdelay") = "30"
            End If
        End With

        ' Start discovery thread.
        trdDiscovery = New System.Threading.Thread(AddressOf doDiscovery)
        With trdDiscovery
            .Name = String.Format(
                "{0}@{1}:{2}",
                Name,
                Environment.MachineName,
                Me.Port.ToString
            )
            .IsBackground = True
            log.LogData.AppendFormat(
                "Starting thread {0}.",
                .Name
            ).AppendLine()
            .Start()
        End With

    End Sub

    Public Overrides Sub svcStop(ByRef log As oMsgLog)
        With log.LogData
            .AppendFormat(
                "Closing thread {0}.",
                trdDiscovery.Name
            ).AppendLine()
        End With

    End Sub

#End Region

#Region "Discovery Messages"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)
        With outputStream
            .WriteElementString("BroadcastDelay", thisConfig.regValue(False, "broadcastdelay"))

            For Each SVR As Lazy(Of svcDef, svcDefprops) In unLoadedModules
                .WriteStartElement("dormant")
                .WriteAttributeString("name", SVR.Value.Name)
                .WriteAttributeString("svctype", "service")
                .WriteEndElement()
            Next
            For Each SVR As Lazy(Of SubscribeDef, SubscribeDefprops) In unLoadedSubscribers
                If Not SVR.Value.Console Then
                    .WriteStartElement("dormant")
                    .WriteAttributeString("name", SVR.Value.Name)
                    .WriteAttributeString("svctype", "subscriber")
                    .WriteEndElement()
                End If
            Next

        End With
    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oDiscovery(Service)
        With ret
            .BroadcastDelay = Service.SelectSingleNode("BroadcastDelay").InnerText
            For Each dormant As XmlNode In Service.SelectNodes("dormant")
                Select Case dormant.Attributes("svctype").Value
                    Case "service"
                        .Dormant.Add(dormant.Attributes("name").Value, eSvcType.Service)
                    Case Else
                        .Dormant.Add(dormant.Attributes("name").Value, eSvcType.Subscriber)

                End Select

            Next
        End With
        Return ret

    End Function

#End Region

#Region "Message Handlers"

    Public Overrides Property myProperties(log As Object, ParamArray Name() As String) As String
        Get
            Return MyBase.MyProperties(log, Name)
        End Get
        Set(value As String)
            MyBase.MyProperties(log, Name) = value
        End Set
    End Property

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

    Public Overrides Function tcpMsg(ByRef msg As msgBase, ByRef thisLog As oMsgLog) As Byte()

        Dim ret As Byte() = Nothing

        Select Case msg.msgType
            Case "cmd"
                With TryCast(msg.thisObject, oMsgCmd)

                    Dim targetService As WritableXML = GetService(.Args)

                    If .Args.Keys.Contains("state") Then
                        Select Case .Args("state").ToLower
                            Case "start"
                                If cmdStart(targetService, thisLog) Then
                                    targetService.thisConfig.regValue(False, "start") = "1"
                                    thisLog.EntryType = LogEntryType.SuccessAudit
                                    Return msgFactory.EncodeResponse("log", 200, "Service started.", thisLog)
                                Else
                                    thisLog.EntryType = LogEntryType.FailureAudit
                                    Return msgFactory.EncodeResponse("log", 400, "Service start failed.", thisLog)
                                End If

                            Case "stop"
                                If String.Compare(.Args("service"), Me.Name, True) = 0 Then
                                    thisLog.EntryType = LogEntryType.Err
                                    Return msgFactory.EncodeResponse("log", 400, "You can't stop me, I'm the gingerbread man!", thisLog)
                                Else
                                    targetService.thisConfig.regValue(False, "start") = "0"
                                    If cmdStop(targetService, thisLog) Then
                                        thisLog.EntryType = LogEntryType.SuccessAudit
                                        Return msgFactory.EncodeResponse("log", 200, "Service stopped.", thisLog)
                                    Else
                                        thisLog.EntryType = LogEntryType.FailureAudit
                                        Return msgFactory.EncodeResponse("log", 400, "Service stop failed.", thisLog)
                                    End If
                                End If

                            Case "restart"
                                If String.Compare(.Args("service"), Me.Name, True) = 0 Then
                                    thisLog.EntryType = LogEntryType.Err
                                    Return msgFactory.EncodeResponse("log", 400, "You can't restart me, I'm the gingerbread man!", thisLog)
                                Else
                                    thisLog.LogData.AppendFormat("Attempting restart of {0}.", targetService.Tag).AppendLine()
                                    If cmdRestart(targetService, thisLog) Then
                                        thisLog.EntryType = LogEntryType.SuccessAudit
                                        Return msgFactory.EncodeResponse("log", 200, "Service restarted.", thisLog)
                                    Else
                                        thisLog.EntryType = LogEntryType.FailureAudit
                                        Return msgFactory.EncodeResponse("log", 400, "Service restart failed.", thisLog)
                                    End If
                                End If

                            Case Else
                                thisLog.EntryType = LogEntryType.Err
                                Return msgFactory.EncodeResponse(
                                    "log",
                                    400,
                                    String.Format(
                                        "Invalid State command {0}.",
                                        .Args("state")
                                    ), thisLog
                                )
                        End Select

                    End If

                    For Each k As String In .Args.Keys
                        If Not String.Compare(k, "service") = 0 Then
                            Select Case k.ToLower
                                Case "port"
                                    Select Case targetService.SvcType
                                        Case eServType.Service
                                            If Not String.Compare(targetService.thisConfig.regValue(False, k), .Args(k), True) = 0 Then
                                                thisLog.EntryType = LogEntryType.SuccessAudit
                                                thisLog.LogData.AppendFormat("Setting property {0} = {1}.", k, .Args(k)).AppendLine()
                                                targetService.thisConfig.regValue(False, k) = .Args(k)

                                                If String.Compare(.Args("service"), Me.Name, True) = 0 Then
                                                    thisLog.LogData.Append("Port set. Please restart manually.").AppendLine()
                                                    Return msgFactory.EncodeResponse("log", 400, "Port set. Please restart manually.", thisLog)
                                                Else
                                                    cmdRestart(targetService, thisLog)
                                                End If

                                            End If

                                    End Select

                                Case Else
                                    targetService.myProperties(thisLog, k) = .Args(k)

                            End Select

                        End If
                    Next

                    Select Case thisLog.EntryType
                        Case LogEntryType.Information, LogEntryType.SuccessAudit
                            Return msgFactory.EncodeResponse("log", 200,, thisLog)

                        Case Else
                            Return msgFactory.EncodeResponse("log", 400, "Warnings were issued. Please consult log.", thisLog)

                    End Select

                End With

            Case "log"
                LogQ.Enqueue(msg.toByte)
                Return msgFactory.EncodeResponse("generic", 200)

            Case Else
                Throw New Exception("Unknown Message Type.")

        End Select

    End Function

    Public Overrides Sub udpMsg(ByRef msg As msgBase, ByRef thisLog As oMsgLog)
        With msg
            Select Case .msgType
                Case "discovery"
                    Select Case .Verb
                        Case eVerb.Request
                            Dim dMsg As New oMsgDiscovery()
                            With dMsg
                                For Each SVR As Lazy(Of svcDef, svcDefprops) In LoadedModules
                                    .svc.Add(SVR.Value)
                                Next
                                For Each SVR As Lazy(Of SubscribeDef, SubscribeDefprops) In LoadedSubscribers
                                    .svc.Add(SVR.Value)
                                Next
                            End With

                            Broadcast(
                                TryCast(msg.thisObject, oMsgDiscoveryRequest).ResponsePort,
                                msgFactory.EncodeResponse(
                                    "discovery",,,
                                    dMsg
                                ),
                                PriSock.eBroadcastType.bcPrivate,
                                msg.Source
                            )

                        Case eVerb.Response
                            For Each svc As XmlNode In TryCast(msg.thisObject, oMsgDiscovery).svc
                                Dim o As oServiceBase = thisModule(svc.Attributes("name").Value).readXML(svc)
                                Select Case o.Name.ToLower
                                    Case "discovery"
                                        Dim p As oDiscovery = TryCast(o, oDiscovery)
                                        For Each SVR As Lazy(Of svcDef, svcDefprops) In LoadedModules
                                            If String.Compare(SVR.Value.Name, "webrelay", True) = 0 Then
                                                SVR.Value.Config(p, thisLog)
                                            End If
                                        Next

                                    Case "webrelay"
                                        For Each p As PriWeb In TryCast(o, oWebRelay).Values
                                            'Console.Write(p.Settings("port"))
                                        Next

                                    Case "loader"
                                        Dim p As oLoader = TryCast(o, oLoader)
                                        For Each SVR As Lazy(Of svcDef, svcDefprops) In LoadedModules
                                            If String.Compare(SVR.Value.Name, "webrelay", True) = 0 Then
                                                SVR.Value.Config(p, thisLog)
                                            End If
                                        Next

                                    Case "console"
                                        Dim P As oSubConsole = TryCast(o, oSubConsole)

                                    Case "broadcast"
                                        Dim P As oSubBroadcast = TryCast(o, oSubBroadcast)

                                End Select

                            Next

                        Case Else
                            Throw New NotImplementedException()

                    End Select

            End Select

        End With

    End Sub

#End Region

#Region "Service Commands"

    Private Function GetService(ByRef args As Dictionary(Of String, String)) As WritableXML

        Dim targetService As WritableXML = Nothing
        With args
            If Not .Keys.Contains("service") Then
                Throw New Exception("Service not specified.")
            End If

            For Each SVR As Lazy(Of svcDef, svcDefprops) In Modules
                If String.Compare(SVR.Value.Name, args("service"), True) = 0 Then
                    Return SVR.Value
                End If
            Next

            For Each SVR As Lazy(Of SubscribeDef, SubscribeDefprops) In Subscribers
                If String.Compare(SVR.Value.Name, args("service"), True) = 0 Then
                    Return SVR.Value
                End If
            Next

        End With

        Throw New Exception(
            String.Format(
                "Unknown service specifier {0}.",
                args("service")
            )
        )

    End Function

    Private Function cmdStart(ByRef targetService As WritableXML, ByRef Thislog As oMsgLog) As Boolean

        Select Case targetService.svc_state
            Case eServiceState.stopped
                targetService.svc_start(Thislog)
                Do
                    Select Case targetService.svc_state
                        Case eServiceState.started
                            Return True
                        Case eServiceState.stopped
                            Return False
                    End Select
                    Threading.Thread.Sleep(100)
                Loop

            Case eServiceState.started
                Throw New Exception(
                    String.Format(
                        "Service {0} already started on {1}.",
                        targetService.Tag,
                        Environment.MachineName
                    )
                )

            Case eServiceState.starting, eServiceState.stopping
                Throw New Exception(
                    String.Format(
                        "Service {0} on {1} has pending start/stop.",
                        targetService.Tag,
                        Environment.MachineName
                        )
                    )

            Case Else
                Throw New Exception("")

        End Select

    End Function

    Private Function cmdStop(ByRef targetService As WritableXML, ByRef Thislog As oMsgLog) As Boolean

        Select Case targetService.svc_state
            Case eServiceState.started
                targetService.svc_stop(Thislog)
                Do Until targetService.svc_state = eServiceState.stopped
                    Threading.Thread.Sleep(100)
                Loop
                Return True

            Case eServiceState.stopped
                Throw New Exception(
                    String.Format(
                        "Service {0} already stopped on {1}.",
                        targetService.Tag,
                        Environment.MachineName
                    )
                )

            Case eServiceState.starting, eServiceState.stopping
                Throw New Exception(
                    String.Format(
                        "Service {0} on {1} has pending start/stop.",
                        targetService.Tag,
                        Environment.MachineName
                        )
                    )

            Case Else
                Throw New Exception("")

        End Select

    End Function

    Private Function cmdRestart(ByRef targetService As WritableXML, ByRef Thislog As oMsgLog) As Boolean
        Thislog.LogData.AppendFormat("Attempting restart of {0}.", targetService.Tag).AppendLine()
        If Not cmdStop(targetService, Thislog) Then
            Return False
        Else
            If Not cmdStart(targetService, Thislog) Then
                Return False
            End If
        End If
        Return True
    End Function

#End Region

#Region "Discovery thread"

    Private trdDiscovery As System.Threading.Thread

    Private Sub doDiscovery()

        Threading.Thread.Sleep(2000)
        Do
            Try
                Broadcast(msgFactory.EncodeRequest("discovery", New oMsgDiscoveryRequest(Me.Port)))

                ''Configure website settings
                'Dim thisCmd As New oMsgCmd
                'With thisCmd.Args
                '    .Add("endpoint", "MARCHHARE:8080")
                '    .Add("Service", "test")
                '    .Add("Environment", "demo")
                '    .Add("LoadingTimeout", "180")
                'End With

                'Dim resp = Send("MARCHHARE", 8092, msgFactory.EncodeRequest("cmd", thisCmd))
                'If Not resp.ErrCde = 200 Then
                '    MsgBox(resp.errMsg)
                'End If

                ''Start / stop service
                'Dim thisCmd As New oMsgCmd
                'With thisCmd.Args
                '    .Add("service", "discovery")
                '    .Add("broadcastdelay", "15")
                'End With

                'Dim resp = Send("MARCHHARE", 8090, msgFactory.EncodeRequest("cmd", thisCmd))
                'If Not resp.ErrCde = 200 Then
                '    MsgBox(resp.errMsg)
                'End If
                ''MsgBox(TryCast(resp.thisObject, oMsgLog).LogData.ToString)

                ''Set start
                'Dim thisCmd As New oMsgCmd
                'With thisCmd.Args
                '    .Add("service", "loader")
                '    .Add("start", "1")
                'End With

                'Dim resp = Send("MARCHHARE", 8090, msgFactory.EncodeRequest("cmd", thisCmd))
                'If Not resp.ErrCde = 200 Then
                '    MsgBox(resp.errMsg)
                'End If

            Catch ex As Exception
                LogQ.Enqueue(msgFactory.EncodeRequest("log", New oMsgLog(Name, ex)))

            Finally
                Pause(thisConfig.regValue(False, "broadcastdelay"))

            End Try

        Loop While Not eServiceState.stopping

        svc_state = eServiceState.stopped

    End Sub

    Private Sub Pause(Seconds As Integer)
        For sec = 1 To Seconds * 10
            Threading.Thread.Sleep(100)
            If svc_state = eServiceState.stopping Then Exit For
        Next
    End Sub

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.ppDiscovery)
            ret.Add("subscribers", My.Resources.ppLog)
            Return ret
        End Get
    End Property

    Public Overrides ReadOnly Property ModuleVersion As Version
        Get
            Return Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property

#End Region

End Class
