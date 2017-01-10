Imports System.Xml
Imports System.IO
Imports System.Web

Public Class LoadRow

    Private _Data() As String = Nothing

    Sub New(ByVal ParamArray args As String())
        _Data = args
    End Sub

    Public Property Data() As String()
        Get
            Return _Data
        End Get
        Set(ByVal value As String())
            _Data = value
        End Set
    End Property

    Public ReadOnly Property Length()
        Get
            Return UBound(_Data)
        End Get
    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return UBound(_Data) + 1
        End Get
    End Property

    Private _RecordType As Integer = 1
    Public Property RecordType() As Integer
        Get
            Return _RecordType
        End Get
        Set(ByVal value As Integer)
            _RecordType = value
        End Set
    End Property

End Class