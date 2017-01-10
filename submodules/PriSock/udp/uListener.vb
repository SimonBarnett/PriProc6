Imports System.CommunicationFramework.Servers
Imports System.Net
Imports System.Text

Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class uListener : Inherits socketListener

    Public Delegate Sub hudpMsg(ByVal e As Byte(), RemoteEndpoint As String)
    Private msgHandler As hudpMsg
    Private Server As UdpServer

#Region "Overridden Properties"

    Public Overrides ReadOnly Property ProtocolType() As eProtocolType
        Get
            Return eProtocolType.udp
        End Get
    End Property

#End Region

#Region "Initialisation and finalisation"

    Public Sub New(ByVal Port As Integer, ByVal Name As String, ByRef StartLog As oMsgLog, ByRef logQ As Queue(Of Byte()), ByRef msgFactory As msgFactory, ByRef thisHandler As hudpMsg)
        Try
            svc_state = eSocketState.starting

            With Me
                .Port = Port
                .Name = Name
                .msgHandler = thisHandler
                .msgFactory = msgFactory
                .StartLog = StartLog
                .logq = logQ

                .Server = New UdpServer(.Tag, IPAddress.Any, .Port, 1024, 1024 * 10)
                '.Server.
            End With

            StartLog.LogData.AppendFormat("Starting UDP listener {0}...", Tag)

            With Server
                AddHandler .GeneralEvent, AddressOf ServerGeneralEvent
                AddHandler .DatagramReceived, AddressOf hDatagramReceived
                AddHandler .Started, AddressOf OnStart
                AddHandler .Stopped, AddressOf OnTerminate
                .Start()
            End With

        Catch ex As Exception
            With StartLog
                .setException(ex)
                With .LogData
                    .Append("FAILED.").AppendLine()
                End With
            End With

            svc_state = eSocketState.stopped

        End Try

    End Sub

    Public Overrides Sub disposeMe(ByRef EndLog As oMsgLog)
        Try
            Me.EndLog = EndLog
            EndLog.LogData.AppendFormat("Stopping UDP listener {0}...", Tag)
            svc_state = eSocketState.stopping
            Server.Stop()

        Catch ex As Exception
            With EndLog
                .setException(ex)
                With .LogData
                    .Append("FAILED.").AppendLine()
                End With
            End With

            svc_state = eSocketState.stopped

        End Try

    End Sub

#End Region

#Region "Private Methods"

    Sub hDatagramReceived(sender As Object, e As DatagramReceivedEventArgs)
        Try
            With e.ReceivedDatagram
                msgHandler.Invoke(.Buffer, e.ReceivedDatagram.RemoteEndPoint.ToString)
            End With

        Catch EX As Exception
            Dim msg As New oMsgLog(Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.Warning)
            msg.LogData.Append(EX.Message).AppendLine()
            logq.Enqueue(msgFactory.EncodeRequest("log", msg))

        End Try

    End Sub

#End Region

End Class
