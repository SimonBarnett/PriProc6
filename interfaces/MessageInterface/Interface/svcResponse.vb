Public MustInherit Class svcResponse : Inherits ServiceMessage : Implements msgInterface

    Public Overrides ReadOnly Property Verb() As eVerb Implements msgInterface.verb
        Get
            Return eVerb.Response
        End Get
    End Property

    Private _errCode As Integer = 200
    Public Overrides Property ErrCde() As Integer Implements msgInterface.ErrCde
        Get
            Return _errCode
        End Get
        Set(ByVal value As Integer)
            _errCode = value
        End Set
    End Property

    Private _errMsg As String = String.Empty
    Public Overrides Property errMsg() As String Implements msgInterface.errMsg
        Get
            Return _errMsg
        End Get
        Set(ByVal value As String)
            _errMsg = value
        End Set
    End Property

    Public Overrides Function toByte(ByRef o As objBase) As Byte() Implements msgInterface.toByte
        Dim thisResponse As New msgBase()
        With thisResponse
            .msgType = Me.msgType
            .Verb = Me.Verb

            _errCode = Me.ErrCde
            _errMsg = Me.errMsg
            .thisObject = o

        End With
        Return myEncoder.GetBytes(BuildXML(thisResponse).ToString)
    End Function

    Public Overrides Sub writeErr(ByRef msg As msgBase, ByRef outputStream As System.Xml.XmlWriter)
        With outputStream
            .WriteStartElement("error")
            .WriteElementString("code", _errCode.ToString)
            .WriteElementString("message", _errMsg)
            .WriteEndElement()
        End With
    End Sub

End Class
