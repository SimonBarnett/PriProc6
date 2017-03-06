Public Class EndPoint

    Public Sub New(ByRef Parent As PriWeb, ByVal Name As String)
        _Parent = Parent
        _Name = Name
    End Sub

    Private _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property

    Private _Parent As PriWeb
    Public ReadOnly Property Parent As PriWeb
        Get
            Return _Parent
        End Get
    End Property

    Public ReadOnly Property URL As String
        Get
            Return String.Format("http://{0}/{1}.ashx", _Parent.Endpoint, Name)
        End Get
    End Property

End Class
