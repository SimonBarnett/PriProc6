Imports PriPROC6.Interface.Message

Public Class oMsgDiscoveryRequest : Inherits objBase

    Sub New(ResponsePort As Integer)
        _ResponsePort = ResponsePort
    End Sub

    Public Sub New(ByRef msg As msgBase)
        With Me
            .Verb = msg.Verb
            .Source = msg.Source
            .msgType = msg.msgType
            .TimeStamp = msg.TimeStamp
        End With
    End Sub

    Private _ResponsePort As Integer
    Public Property ResponsePort As Integer
        Get
            Return _ResponsePort
        End Get
        Set(value As Integer)
            _ResponsePort = value
        End Set
    End Property

End Class

Public Class oMsgDiscovery : Inherits objBase

    Private _SVC As List(Of Object)
    Public Property svc As List(Of Object)
        Get
            Return _SVC
        End Get
        Set(value As List(Of Object))
            _SVC = value
        End Set
    End Property

#Region "Constructors"

    Public Sub New()
        _SVC = New List(Of Object)
    End Sub

    Public Sub New(ByRef msg As msgBase)
        _SVC = New List(Of Object)
        With Me
            .Verb = msg.Verb
            .Source = msg.Source
            .msgType = msg.msgType
            .TimeStamp = msg.TimeStamp
        End With
    End Sub

#End Region

End Class
