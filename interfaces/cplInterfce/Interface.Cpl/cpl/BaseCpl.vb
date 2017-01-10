Public Class BaseCpl

    Private _obj As Object
    Public Property Obj As Object
        Get
            Return _obj
        End Get
        Set(value As Object)
            _obj = value
        End Set
    End Property

End Class
