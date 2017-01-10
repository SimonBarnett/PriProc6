Imports System.Xml
Imports System.IO
Imports System.Web

Public Enum tColumnType
    typeCHAR = 1
    typeDATE = 2
    typeREAL = 3
    typeINT = 4
    typeBOOL = 5
    typeMONEY = 6
End Enum

Public Class LoadColumn

    Public Sub New(ByVal ColumnName As String, ByVal ColumnType As tColumnType, ByVal Length As Integer)
        _ColumnName = ColumnName
        _ColumnType = ColumnType
        _Length = Length
    End Sub

    Public Sub New(ByVal ColumnName As String, ByVal ColumnType As tColumnType)
        _ColumnName = ColumnName
        _ColumnType = ColumnType
        _Length = -1
    End Sub

    Private _ColumnName As String
    Public Property ColumnName() As String
        Get
            Return _ColumnName
        End Get
        Set(ByVal value As String)
            _ColumnName = value
        End Set
    End Property

    Private _ColumnType As tColumnType
    Public Property ColumnType() As tColumnType
        Get
            Return _ColumnType
        End Get
        Set(ByVal value As tColumnType)
            _ColumnType = value
        End Set
    End Property

    Private _Length As Integer
    Public Property Length() As Integer
        Get
            Return _Length
        End Get
        Set(ByVal value As Integer)
            _Length = value
        End Set
    End Property

End Class