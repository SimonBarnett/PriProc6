Imports System.Text
Imports System.Xml

Public Enum eVerb
    Request
    Response
End Enum

Public Class msgBase : Implements IDisposable

    Private msgDocument As New XmlDocument

    Private _byteMsg As Byte()
    Public Function toByte() As Byte()
        Return _byteMsg

    End Function

#Region "Constructors"

    Public Sub New()
        _source = Environment.MachineName
        _TimeStamp = Now.ToString("dd/MM/yyyy HH:mm:ss")
    End Sub

    Private Function verbStr(verb As eVerb) As String
        Select Case verb
            Case eVerb.Request
                Return "request"

            Case eVerb.Response
                Return "response"

            Case Else
                Throw New NotImplementedException()

        End Select
    End Function

    Public Sub New(ByVal bytes As Byte())
        Try
            _byteMsg = bytes
            With msgDocument
                .LoadXml(Encoding.ASCII.GetString(bytes))
                If Not IsNothing(.SelectSingleNode("request")) Then
                    _verb = eVerb.Request

                ElseIf Not IsNothing(.SelectSingleNode("response")) Then
                    _verb = eVerb.Response
                    _errCode = .SelectSingleNode(String.Format("{0}/error/code", verbStr(_verb))).InnerText
                    _errMsg = .SelectSingleNode(String.Format("{0}/error/message", verbStr(_verb))).InnerText

                Else
                    Throw New Exception
                End If

                _msgType = .SelectSingleNode(String.Format("{0}/type", verbStr(_verb))).InnerText
                _source = .SelectSingleNode(String.Format("{0}/source", verbStr(_verb))).InnerText
                _TimeStamp = .SelectSingleNode(String.Format("{0}/timestamp", verbStr(_verb))).InnerText
                _msgNode = .SelectSingleNode(String.Format("{0}", verbStr(_verb)))

            End With

        Catch
            Throw New Exception("Malformed message.")

        End Try

    End Sub

#End Region

#Region "properties"

    Private _verb As eVerb
    Public Property Verb As eVerb
        Get
            Return _verb
        End Get
        Set(value As eVerb)
            _verb = value
        End Set
    End Property

    Private _msgType As String
    Public Property msgType As String
        Get
            Return _msgType
        End Get
        Set(value As String)
            _msgType = value
        End Set
    End Property

    Private _source As String
    Public ReadOnly Property Source As String
        Get
            Return _source
        End Get
    End Property

    Private _TimeStamp As String
    Public ReadOnly Property TimeStamp As String
        Get
            Return _TimeStamp
        End Get
    End Property

    Private _msgNode As XmlNode
    Public ReadOnly Property msgNode() As XmlNode
        Get
            Return _msgNode
        End Get
    End Property

    Private _thisObject As Object = Nothing
    Public Property thisObject As Object
        Get
            Return _thisObject
        End Get
        Set(value As Object)
            _thisObject = value
        End Set
    End Property

#Region "Response Data"

    Private _errCode As Integer
    Public Property ErrCde As Integer
        Get
            Return _errCode

        End Get

        Set(value As Integer)
            _errCode = value

        End Set
    End Property

    Private _errMsg As String
    Public Property errMsg As String
        Get
            Return _errMsg

        End Get
        Set(value As String)
            _errMsg = value
        End Set

    End Property

#End Region

#End Region

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                _thisObject = Nothing
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
