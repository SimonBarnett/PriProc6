Public Class PriorityUser

    Private _Username As String
    Public Property Username() As String
        Get
            Return _Username
        End Get
        Set(ByVal value As String)
            _Username = value
        End Set
    End Property

    Private _Password As String
    Public Property Password() As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property

    Private _Remove As Boolean = False
    Public Property Remove() As Boolean
        Get
            Return _Remove
        End Get
        Set(ByVal value As Boolean)
            _Remove = value
        End Set
    End Property

    Public Sub New(ByVal Username As String, ByVal Password As String)
        _Username = Username
        _Password = Password
    End Sub

End Class