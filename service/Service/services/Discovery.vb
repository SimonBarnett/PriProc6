Imports System.ComponentModel.Composition
Imports System.Xml
Imports Priproc6.svcInterface
Imports Priproc6.svcMessages
Imports Priproc6.Messages

<Export(GetType(svcDef))>
<ExportMetadata("serviceName", "discovery")>
<ExportMetadata("tcp", True)>
<ExportMetadata("udp", True)>
<ExportMetadata("defaultPort", 8090)>
Public Class Discovery : Inherits svcbase
    Implements svcDef

#Region "overriden methods "

    Public Overrides Sub svcStart(ByRef log As oMsgLog)

        'Console.Write(ServiceName)
        'With msgFactory

        '    Dim t As Byte() = newMessage(.EncodeRequest("discovery", Nothing))
        '    Dim req As Byte() = .EncodeRequest("log", New oMsgLog("SVC"))
        '    Dim resp As Byte() = .EncodeResponse("generic", 400, "An error")

        '    Dim q As objBase = TryCast(.Decode(req).thisObject, oMsgLog)
        '    Dim p As msgBase = .Decode(resp)

        '    Parent.logq.Enqueue(.EncodeResponse("generic", 400, "An error"))
        'End With

        trdDiscovery = New System.Threading.Thread(AddressOf doDiscovery)
        With trdDiscovery
            .Name = String.Format("{0}_{1}", Environment.MachineName, ServiceName)
            .IsBackground = True
            log.LogData.AppendFormat("Starting thread [{0}].", .Name).AppendLine()
            .Start()
        End With

    End Sub

    Public Overrides Sub svcStop(ByRef log As oMsgLog)
        ' Wait for thread close
        With log.LogData
            .AppendFormat("Closing thread [{0}]...", trdDiscovery.Name).AppendLine()
            While Not IsNothing(trdDiscovery)
                Threading.Thread.Sleep(100)
            End While
        End With

    End Sub

    Public Overrides Sub svc_info(ByRef wr As XmlWriter) Implements svcDef.svc_info
        With wr
            .WriteStartElement("service")
            .WriteElementString("name", ServiceName)
            .WriteElementString("host", Environment.MachineName)
            If tcp Then
                .WriteElementString("tcp", Port.ToString)
            End If
            If udp Then
                .WriteElementString("udp", BroadcastPort.ToString)
            End If
            .WriteEndElement()
        End With

    End Sub

    Public Overrides Function tcpMsg(ByRef msg As msgBase) As Byte()
        With msg
            Select Case .msgType
                Case "discovery"
                    'Return msgFactory.EncodeResponse("discovery", 200, "", New oMsgDiscovery(LoadedModules))

                Case Else
                    Return msgFactory.EncodeResponse("generic", 400, "An error")

            End Select

        End With
    End Function

    Public Overrides Sub udpMsg(ByRef msg As msgBase)
        With msg
            Select Case .msgType
                Case "discovery"
                    Select Case .Verb
                        Case eVerb.Request
                            Broadcast(
                                BroadcastPort,
                                msgFactory.EncodeResponse(
                                    "discovery",
,,
                                    New oMsgDiscovery()
                                ),
                                PriSock.eBroadcastType.bcPrivate,
                                msg.Source
                            )

                        Case eVerb.Response
                            With TryCast(msg.thisObject, oMsgDiscovery)

                            End With

                        Case Else
                            Throw New NotImplementedException()

                    End Select

            End Select

        End With

    End Sub

#End Region

#Region "Discovery thread"

    Private trdDiscovery As System.Threading.Thread
    Private Sub doDiscovery()

        Threading.Thread.Sleep(10000)
        Do
            Try
                Broadcast(msgFactory.EncodeRequest("discovery", Nothing))
            Catch : End Try

            For sec = 0 To 59
                Threading.Thread.Sleep(1000)
                If svc_state = eServiceState.stopping Then Exit For
            Next

        Loop While Not eServiceState.stopping

        trdDiscovery = Nothing

    End Sub

#End Region

End Class
