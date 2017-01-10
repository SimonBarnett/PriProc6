Imports System.Windows.Forms
Imports System.Xml

Public Enum eAttemptError As Integer

    Ok = 200
    DBFail = 401
    TimeOut = 402
    Licence = 403
    NoWinActiv = 404

End Enum

Public Class LoadAttempt

#Region "Initialisation and finalisation"

    Public Sub New(ByRef AttemptNode As XmlNode)
        With Me
            .ID = AttemptNode.Attributes("id").Value
            .User = AttemptNode.Attributes("user").Value
            .LoadDate = AttemptNode.Attributes("loaddate").Value
            .ErrCode = AttemptNode.Attributes("errcode").Value
            For Each warn As XmlNode In AttemptNode.SelectNodes("warning")
                .Warnings.Add(New Warning(warn))
            Next
        End With
    End Sub

    Sub New(ByVal ID As Integer, ByVal User As String)
        With Me
            .ID = ID
            .User = User
            .LoadDate = Now.ToString("dd/MM/yyyy HH:mm:ss")
        End With
    End Sub

#End Region

#Region "Public Properties"

    Private _id As Integer
    Public Property ID() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property

    Private _User As String
    Public Property User() As String
        Get
            Return _User
        End Get
        Set(ByVal value As String)
            _User = value
        End Set
    End Property

    Private _LoadDate As String
    Public Property LoadDate() As String
        Get
            Return _LoadDate
        End Get
        Set(ByVal value As String)
            _LoadDate = value
        End Set
    End Property

    Private _ErrCode As eAttemptError
    Public Property ErrCode() As eAttemptError
        Get
            Return _ErrCode
        End Get
        Set(ByVal value As eAttemptError)
            _ErrCode = value
        End Set
    End Property

    Private _Warnings As New List(Of Warning)
    Public Property Warnings() As List(Of Warning)
        Get
            Return _Warnings
        End Get
        Set(ByVal value As List(Of Warning))
            _Warnings = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Sub AddWarning(ByVal strFormat As String, ByVal ParamArray Args() As String)
        ' Add errors to the bubble
        Me.Warnings.Add(
            New Warning(
                0,
                String.Format(
                    strFormat,
                    Args
                )
            )
        )
    End Sub

    Public Sub AddWarning(Line As Integer, ByVal strFormat As String, ByVal ParamArray Args() As String)
        ' Add errors to the bubble
        Me.Warnings.Add(
            New Warning(
                Line,
                String.Format(
                    strFormat,
                    Args
                )
            )
        )
    End Sub

    Public Sub AddWarning(ByVal strFormat As String)
        ' Add errors to the bubble
        Me.Warnings.Add(
            New Warning(
                0,
                strFormat
            )
        )
    End Sub

    Public Sub AddWarning(Line As Integer, ByVal strFormat As String)
        ' Add errors to the bubble
        Me.Warnings.Add(
            New Warning(
                Line,
                strFormat
            )
        )
    End Sub

    Public Sub toWriter(ByRef outputStream As System.Xml.XmlWriter)
        With outputStream
            .WriteStartElement("attempt")
            .WriteAttributeString("id", ID)
            .WriteAttributeString("user", User)
            .WriteAttributeString("loaddate", LoadDate)
            .WriteAttributeString("errcode", ErrCode)

            For Each Warn As Warning In Warnings
                Warn.toWriter(outputStream)
            Next

            .WriteEndElement()

        End With
    End Sub

    Public Sub toColumns(ByRef lv As ListView)
        With lv.Columns
            .Clear()
            .Add("Date / Time")
            .Item(0).Width = 150
            .Add("User")
            .Item(1).Width = 100
            .Add("Error")
            .Item(1).Width = 150
        End With
    End Sub

    Public Function lvItem() As ListViewItem
        Dim lvi As New ListViewItem
        With lvi
            .Text = Me.LoadDate ' .LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss")  
            If Not ErrCode >= 200 Then
                .ImageIndex = 2
                .StateImageIndex = 2

            Else
                Select Case Warnings.Count
                    Case 0
                        .ImageIndex = 0
                        .StateImageIndex = 0
                    Case Else
                        .ImageIndex = 1
                        .StateImageIndex = 1
                End Select
            End If

            .SubItems.Add(Me.User)

            Select Case Me.ErrCode
                Case eAttemptError.Ok
                    .SubItems.Add("Ok")

                Case eAttemptError.DBFail
                    .SubItems.Add("DBFail")

                Case eAttemptError.Licence
                    .SubItems.Add("Licence")

                Case eAttemptError.NoWinActiv
                    .SubItems.Add("NoWinActiv")

                Case eAttemptError.TimeOut
                    .SubItems.Add("TimeOut")

                Case Else
                    .SubItems.Add("Unknown")

            End Select

            .Name = Me.ID

        End With
        Return lvi
    End Function

#End Region

End Class