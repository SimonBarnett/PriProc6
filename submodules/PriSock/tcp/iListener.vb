Imports System.CommunicationFramework.Servers
Imports System.Net
Imports System.Text

Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class iListener : Inherits socketListener

    Public Delegate Function htcpMsg(ByVal e As Byte(), RemoteEndpoint As String) As Byte()
    Private msgHandler As htcpMsg
    Private server As TcpServer

#Region "Overridden Properties"

    Public Overrides ReadOnly Property ProtocolType() As eProtocolType
        Get
            Return eProtocolType.tcp
        End Get
    End Property

#End Region

#Region "Initialisation and finalisation"
    ' This server waits for a connection and then uses  asychronous operations to
    ' accept the connection, get data from the connected client, 
    ' echo that data back to the connected client.
    ' It then disconnects from the client and waits for another client. 
    Public Sub New(ByVal Port As Integer, ByVal Name As String, ByRef StartLog As oMsgLog, ByRef logQ As Queue(Of Byte()), ByRef msgFactory As msgFactory, ByRef thisHandler As htcpMsg)
        Try
            svc_state = eSocketState.starting

            With Me
                .Port = Port
                .Name = Name
                .msgFactory = msgFactory
                .msgHandler = thisHandler
                .StartLog = StartLog
                .logq = logQ

                .server = New TcpServer(Tag, IPAddress.Any, Me.Port)

            End With

            StartLog.LogData.AppendFormat("Starting TCP listener {0}...", Tag)

            With server
                AddHandler .GeneralEvent, AddressOf ServerGeneralEvent
                AddHandler .ClientConnected, AddressOf ClientConnected
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

    End Sub 'Main

    Public Overrides Sub disposeMe(ByRef EndLog As oMsgLog)
        Try
            Me.EndLog = EndLog
            EndLog.LogData.AppendFormat("Stopping TCP listener {0}...", Tag)
            svc_state = eSocketState.stopping
            server.Stop()

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

    Private Sub ClientConnected(sender As Object, e As ClientConnectedEventArgs)
        Dim trdDiscovery As New System.Threading.Thread(AddressOf ServiceConnectedClient)
        With trdDiscovery
            .Name = String.Format("{0}@{1}", e.Client.RemoteEndPoint.ToString, Tag)
            .Start(e.Client)
        End With
    End Sub

    Private Sub ServiceConnectedClient(Socket As Sockets.Socket)

        Dim bytes() As Byte
        Dim data As New Text.StringBuilder

        With Socket
            ' An incoming connection needs to be processed.
            .Send(Encoding.ASCII.GetBytes(String.Format("+PriPROC6 {0}", Tag, Environment.MachineName, Port) & vbCrLf))

            Do
                Try
                    bytes = New Byte(1024) {}
                    Dim bytesRec As Integer = .Receive(bytes)
                    data.Append(Encoding.ASCII.GetString(bytes, 0, bytesRec))

                Catch ex As Exception
                    Beep()

                End Try
            Loop Until EndTrans(data)

            .Send(
                msgHandler(
                    Encoding.ASCII.GetBytes(
                        data.ToString()
                    ),
                    Threading.Thread.CurrentThread.Name
                )
            )

            'Try


            'Catch EX As Exception

            '    Dim msg As New oMsgLog(Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.Warning)
            '    With msg
            '        .LogData.AppendFormat("Bad data from client:{0}", Socket.RemoteEndPoint.ToString).AppendLine()
            '        .LogData.Append(EX.Message).AppendLine()

            '    End With
            '    logq.Enqueue(msgFactory.EncodeRequest("log", msg))

            '    .Send(msgFactory.EncodeResponse("generic", 500, EX.Message))

            'Finally
            '    .Close()

            'End Try

        End With

    End Sub

#End Region

End Class