Imports System.Windows.Forms
Imports System.Xml

Public Class LoadRow : Inherits Dictionary(Of String, String)

    Public Sub New(ByVal Row As XmlNode)
        For Each atr As XmlAttribute In Row.Attributes
            Select Case atr.Name.ToUpper
                Case "RECORDTYPE"
                    RecordType = CInt(atr.Value)
                Case "LINE"
                    Line = CInt(atr.Value)
                Case "LOADED"
                    Loaded = atr.Value
                Case "KEY1"
                    Key1 = atr.Value
                Case "KEY2"
                    Key2 = atr.Value
                Case "KEY3"
                    Key3 = atr.Value
                Case Else
                    Me.Add(atr.Name, atr.Value)
            End Select
        Next
    End Sub

#Region "Public properties"

    Private _RecordType As Integer = 1
    Public Property RecordType() As Integer
        Get
            Return _RecordType
        End Get
        Set(ByVal value As Integer)
            _RecordType = value
        End Set
    End Property

    Private _Line As Integer
    Public Property Line() As Integer
        Get
            Return _Line
        End Get
        Set(ByVal value As Integer)
            _Line = value
        End Set
    End Property

    Private _Loaded As String = String.Empty
    Public Property Loaded() As String
        Get
            Return _Loaded
        End Get
        Set(ByVal value As String)
            _Loaded = value
        End Set
    End Property

#Region "Loading Keys"

    Private _Key1 As String = String.Empty
    Public Property Key1() As String
        Get
            Return _Key1
        End Get
        Set(ByVal value As String)
            _Key1 = value
        End Set
    End Property

    Private _Key2 As String = String.Empty
    Public Property Key2() As String
        Get
            Return _Key2
        End Get
        Set(ByVal value As String)
            _Key2 = value
        End Set
    End Property

    Private _Key3 As String = String.Empty
    Public Property Key3() As String
        Get
            Return _Key3
        End Get
        Set(ByVal value As String)
            _Key3 = value
        End Set
    End Property

#End Region

#End Region

#Region "Public Methods"

    Public Sub toWriter(ByRef outputStream As System.Xml.XmlWriter)
        With outputStream
            .WriteStartElement("row")
            For Each k As String In Me.Keys
                .WriteAttributeString(k, Me(k))
            Next
            .WriteAttributeString("KEY3", Me.Key3)
            .WriteAttributeString("KEY2", Me.Key2)
            .WriteAttributeString("KEY1", Me.Key1)
            .WriteAttributeString("LOADED", Me.Loaded)
            .WriteAttributeString("LINE", Me.Line)
            .WriteAttributeString("RECORDTYPE", Me.RecordType)
            .WriteEndElement()
        End With
    End Sub

    Public Sub toColumns(ByRef Grid As DataGridView)
        Grid.Columns.Add("RECORDTYPE", "RECORDTYPE")
        Grid.Columns.Add("LINE", "LINE")
        For Each k As String In Me.Keys
            Grid.Columns.Add(k, k)
        Next
    End Sub

    Public Sub toGrid(ByRef Grid As DataGridView)
        Dim Arg((Me.Count - 1) + 2)
        Dim i As Integer = 0

        Arg(i) = Me.RecordType
        i += 1
        Arg(i) = Me.Line
        i += 1

        For Each k As String In Me.Keys
            Arg(i) = Me(k)
            i += 1
        Next
        Grid.Rows.Add(Arg)
    End Sub

#End Region

End Class