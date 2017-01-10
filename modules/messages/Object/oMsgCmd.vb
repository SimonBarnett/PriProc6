Imports PriPROC6.Interface.Message

Public Class oMsgCmd : Inherits objBase

    Public Sub New(ByRef msg As msgBase)
        _Args = New Dictionary(Of String, String)
        With Me
            .Verb = msg.Verb
            .Source = msg.Source
            .msgType = msg.msgType
            .TimeStamp = msg.TimeStamp
        End With
    End Sub

    Sub New()
        _Args = New Dictionary(Of String, String)
    End Sub

    Private _Args As Dictionary(Of String, String)
    Public Property Args As Dictionary(Of String, String)
        Get
            Return _Args
        End Get
        Set(value As Dictionary(Of String, String))
            _Args = value
        End Set
    End Property

End Class
