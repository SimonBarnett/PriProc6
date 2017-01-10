Imports System.CommunicationFramework.Common
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Enum eSocketState
    stopped
    starting
    started
    stopping
    notresponding
End Enum

Public MustInherit Class socketListener : Inherits protocol

    Public MustOverride Sub disposeMe(ByRef EndLog As oMsgLog)

    Private _svcstate As eSocketState = eSocketState.stopped
    Public Property svc_state As eSocketState
        Get
            Return _svcstate
        End Get
        Set(value As eSocketState)
            _svcstate = value
        End Set
    End Property

    Private _msgFactory As msgFactory
    Public Property msgFactory As msgFactory
        Get
            Return _msgFactory
        End Get
        Set(value As msgFactory)
            _msgFactory = value
        End Set
    End Property

    Private _logq As Queue(Of Byte())
    Public Property logq As Queue(Of Byte())
        Get
            Return _logq
        End Get
        Set(value As Queue(Of Byte()))
            _logq = value
        End Set
    End Property

    Private _Port As Integer
    Public Property Port As Integer
        Get
            Return _Port
        End Get
        Set(value As Integer)
            _Port = value
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Public ReadOnly Property Tag As String
        Get
            Return String.Format(
                "{0}\{1}:{2}",
                Environment.MachineName,
                Name,
                Port.ToString
            )
        End Get
    End Property

    Private _StartLog As oMsgLog
    Public Property StartLog As oMsgLog
        Get
            Return _StartLog
        End Get
        Set(value As oMsgLog)
            _StartLog = value
        End Set
    End Property

    Private _EndLog As oMsgLog
    Public Property EndLog As oMsgLog
        Get
            Return _EndLog
        End Get
        Set(value As oMsgLog)
            _EndLog = value
        End Set
    End Property

    Public Sub OnStart(Sender As Object, e As EventArgs)
        StartLog.LogData.Append("OK.").AppendLine()
        svc_state = eSocketState.started
    End Sub

    Public Sub OnTerminate(Sender As Object, e As EventArgs)
        EndLog.LogData.Append("OK.").AppendLine()
        svc_state = eSocketState.stopped
    End Sub

    Public Sub ServerGeneralEvent(sender As Object, e As CancellableMethodManagerEventArgs)
        Try
            Throw New Exception(String.Format("{0}", e.Message))

        Catch ex As Exception
            Dim er As New oMsgLog("Sockets", ex)
            logq.Enqueue(msgFactory.EncodeRequest("log", er))

        End Try

    End Sub


End Class
