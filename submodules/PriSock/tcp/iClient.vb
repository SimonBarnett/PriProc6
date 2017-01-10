Imports System.Net.Sockets
Imports System.Text
Imports System.Xml

Public Class iClient : Inherits socketClient

#Region "Private Properties"

    Private ClientSocket As New TcpClient

#End Region

#Region "Overridden Properties"

    Public Overrides ReadOnly Property ProtocolType() As eProtocolType
        Get
            Return eProtocolType.tcp
        End Get
    End Property

    Private _ConnectionError As Exception = Nothing
    Public Overrides ReadOnly Property ConnectionError() As System.Exception
        Get
            Return _ConnectionError
        End Get
    End Property

#End Region

#Region "Initialisation and finalisation"

    Public Sub New(ByVal ServerAddress As String, ByVal PortNumber As Integer, Optional retry As Integer = 3)

        Dim attempt As Integer = 1
        Do
            Try
                _ConnectionError = Nothing
                ClientSocket.Connect(ServerAddress, PortNumber)

            Catch ex As Exception
                _ConnectionError = ex
                attempt += 1
                If attempt < retry Then Threading.Thread.Sleep(100)

            End Try

        Loop Until IsNothing(_ConnectionError) Or attempt > retry

        If Not (IsNothing(_ConnectionError)) Then
            Throw New Exception(
                String.Format(
                    "Could not connect to {0}:{1}",
                    ServerAddress,
                    PortNumber
                )
            )
        End If

    End Sub

#End Region

#Region "Overridden Methods"

    Public Overrides Function Send(ByVal data() As Byte) As Byte()

        Dim sb As New System.Text.StringBuilder
        Using ServerStream As NetworkStream = ClientSocket.GetStream()
            With ServerStream
                .Write(data, 0, data.Length)
                .Flush()

                Do
                    Dim inStream(1024) As Byte
                    sb.Capacity += inStream.Length

                    .Read(inStream, 0, inStream.Length)
                    sb.Append(Encoding.ASCII.GetString(inStream, 0, inStream.Length))
                    Threading.Thread.Sleep(1)
                    ' Check for end-of-file tag. If it is not there, read 
                    ' more data.
                Loop Until EndTrans(sb)

            End With
            ClientSocket.Close()

            Dim clean As String = sb.ToString
            While Not String.Compare(clean.Substring(0, 1), "<") = 0
                clean = clean.Substring(1)
            End While
            Return Encoding.ASCII.GetBytes(clean)

        End Using
    End Function

#End Region

End Class
