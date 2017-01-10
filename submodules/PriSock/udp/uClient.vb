Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text

Public Enum eBroadcastType
    bcPublic
    bcPrivate
    bcLocalEndPoint
End Enum

Public Class uClient : Inherits socketClient

#Region "Private Properties"

    Private Const broadcastAddress As String = "255.255.255.255" 'Sends data to all LOCAL listening clients, to send data over WAN you'll need to enter a public (external) IP address of the other client    
    Private Const LocalAddress As String = "127.0.0.1"
    Private sendingClient As UdpClient                           'Client for sending data    

#End Region

#Region "Overridden Properties"

    Public Overrides ReadOnly Property ProtocolType() As eProtocolType
        Get
            Return eProtocolType.udp
        End Get
    End Property

    Private _ConnectionError As System.Exception = Nothing
    Public Overrides ReadOnly Property ConnectionError() As System.Exception
        Get
            Return _ConnectionError
        End Get
    End Property

#End Region

#Region "Initialisation and finalisation"

    Public Sub New(ByVal Port As Integer, ByVal BroadcastType As eBroadcastType, Optional ByVal EndPt As String = "127.0.0.1")
        Try
            Select Case BroadcastType
                Case eBroadcastType.bcPrivate
                    Select Case String.Compare(EndPt, Environment.MachineName, True)
                        Case 0
                            sendingClient = New UdpClient(LocalAddress, Port)
                        Case Else
                            Select Case EndPt.ToLower
                                Case "localhost", "127.0.0.1"
                                    sendingClient = New UdpClient(LocalAddress, Port)
                                Case Else
                                    sendingClient = New UdpClient(EndPt, Port)
                            End Select
                    End Select
                '                    sendingClient = New UdpClient(EndPt, Port)
                Case eBroadcastType.bcPublic
                    sendingClient = New UdpClient(broadcastAddress, Port)
                Case eBroadcastType.bcLocalEndPoint
                    sendingClient = New UdpClient(LocalAddress, Port)
            End Select

            sendingClient.EnableBroadcast = True

        Catch ex As Exception
            _ConnectionError = ex
        End Try

    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Function Send(ByVal data() As Byte) As Byte()
        With sendingClient
            .Send(data, data.Length)               'Send bytes
            .Close()
        End With
        Return Nothing
    End Function

#End Region

End Class
