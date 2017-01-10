Imports System.Xml
Imports System.Text
Imports service

Public MustInherit Class ServiceMessage : Implements IDisposable, msgInterface

#Region "Private Variables"

    Public myEncoder As New System.Text.ASCIIEncoding

#End Region

#Region "Public Properties"

    Public MustOverride ReadOnly Property Verb As eVerb Implements msgInterface.verb

    Private _msgType As String
    Public Property msgType As String Implements msgInterface.msgType
        Get
            Return _msgType
        End Get
        Set(value As String)
            _msgType = value
        End Set
    End Property

    Public MustOverride Property ErrCde As Integer Implements msgInterface.ErrCde
    Public MustOverride Property errMsg As String Implements msgInterface.errMsg

#End Region

#Region "Overridable Methods"

    Public MustOverride Sub ReadXML(ByRef msg As msgBase) Implements msgInterface.readXML
    Public MustOverride Sub fromObject(ByRef msg As msgBase, ByRef ob As objBase) Implements msgInterface.fromObject

    Public Overridable Function toByte(ByRef o As objBase) As Byte() Implements msgInterface.toByte
        Dim thisrequest As New msgBase()
        With thisrequest
            .msgType = Me.msgType
            .Verb = Me.Verb
            .thisObject = o

        End With
        fromObject(thisrequest, o)
        Return myEncoder.GetBytes(BuildXML(thisrequest).ToString)
    End Function

    Public Overridable Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter) Implements msgInterface.writeXML
    End Sub

    Public Overridable Sub writeErr(ByRef msg As msgBase, ByRef outputStream As XmlWriter) Implements msgInterface.writeErr
    End Sub

#End Region

#Region "Private Methods"

    Public Function BuildXML(o As msgBase) As System.Text.StringBuilder
        Dim str As New System.Text.StringBuilder
        Dim xw As XmlWriter = XmlWriter.Create(str)
        With xw
            .WriteStartDocument()
            Select Case o.Verb
                Case eVerb.Request
                    .WriteStartElement("request")
                Case eVerb.Response
                    .WriteStartElement("response")
                Case Else
                    Throw New NotImplementedException()
            End Select
            .WriteElementString("type", o.msgType)
            .WriteElementString("source", o.Source)
            .WriteElementString("timestamp", o.TimeStamp)
            writeErr(o, xw)
            writeXML(o, xw)
            .WriteEndDocument()
            .Flush()
        End With
        Return str
    End Function

#End Region

#Region " IDisposable Support "

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed msgBases).
            End If

            ' TODO: free your own state (unmanaged msgBases).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
