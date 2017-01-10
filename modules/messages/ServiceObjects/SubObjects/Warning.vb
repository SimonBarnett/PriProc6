Imports System.Windows.Forms
Imports System.Xml

Public Class Warning

#Region "Initialisation and finalisation"

    Public Sub New(ByVal Line As Integer, ByVal Message As String)
        _Line = Line
        _Message = Message
    End Sub

    Public Sub New(ByRef WarningNode As XmlNode)
        _Line = WarningNode.Attributes("line").Value
        _Message = WarningNode.Attributes("message").Value
    End Sub

#End Region

#Region "public properties"

    Private _Line As Integer
    Public Property Line() As Integer
        Get
            Return _Line
        End Get
        Set(ByVal value As Integer)
            _Line = value
        End Set
    End Property

    Private _Message As String
    Public Property Mesage() As String
        Get
            Return _Message
        End Get
        Set(ByVal value As String)
            _Message = value
        End Set
    End Property

#End Region

#Region "puiblic properties"

    Public Sub toWriter(ByRef outputStream As System.Xml.XmlWriter)
        With outputStream
            .WriteStartElement("warning")
            .WriteAttributeString("line", Line)
            .WriteAttributeString("message", Mesage)
            .WriteEndElement()
        End With
    End Sub

    Public Sub toColumns(ByRef lv As ListView)
        With lv
            .Columns.Clear()
            .Columns.Add("Line", "Line")
            .Columns(0).Width = 100
            .Columns.Add("Message", "Message")
            .Columns(1).Width = 400
        End With
    End Sub

    Public Function lvItem() As ListViewItem
        Dim lvi As New ListViewItem
        With lvi
            .Text = Me.Line ' .LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss")  
            .SubItems.Add(Me._Message)

        End With
        Return lvi
    End Function

#End Region

End Class