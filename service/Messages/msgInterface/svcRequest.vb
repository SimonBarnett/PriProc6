Imports System.Xml
Imports service

Public MustInherit Class svcRequest : Inherits ServiceMessage : Implements msgInterface

    Public Overrides Property ErrCde As Integer Implements msgInterface.ErrCde
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As Integer)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Overrides Property errMsg As String Implements msgInterface.errMsg
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Overrides ReadOnly Property Verb As eVerb Implements msgInterface.verb
        Get
            Return eVerb.Request
        End Get
    End Property

End Class
