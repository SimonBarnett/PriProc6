Imports System.Xml
Imports System.IO
Imports System.Web
Imports PriPROC6.PriSock
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class Definition : Implements IDisposable

#Region "Initialisation and Finalisation"

    Public Sub New()

    End Sub

    Public Sub Dispose() Implements System.IDisposable.Dispose
        LoadingColumns = Nothing
        LoadingRows = Nothing
    End Sub

#End Region

#Region "Private Variables"

    Private LoadingColumns As New Dictionary(Of Integer, List(Of LoadColumn))
    Private LoadingRows As New List(Of LoadRow)
    Private BeginData As Boolean = False

#End Region

#Region "Public Properties"

    Private _Table As String = Nothing
    Public Property Table() As String
        Set(ByVal value As String)
            _Table = value
        End Set
        Get
            Return _Table
        End Get
    End Property

    Private _Procedure As String = Nothing
    Public Property Procedure() As String
        Set(ByVal value As String)
            _Procedure = value
        End Set
        Get
            Return _Procedure
        End Get
    End Property

    Private _Environment As String = Nothing
    Public Property Environment() As String
        Set(ByVal value As String)
            _Environment = value
        End Set
        Get
            If IsNothing(_Environment) Then
                Return Nothing
            ElseIf _Environment.Length = 0 Then
                Return Nothing
            Else
                Return _Environment
            End If
        End Get
    End Property

    Private _Timeout As Integer = 0
    Public Property Timeout() As Integer
        Get
            Return _Timeout
        End Get
        Set(ByVal value As Integer)
            _Timeout = value
        End Set
    End Property

#End Region

#Region "Write Only Properties"

    Public WriteOnly Property AddColumn(ByVal RecordType As Integer) As LoadColumn
        Set(ByVal value As LoadColumn)
            If Not BeginData Then
                If Not LoadingColumns.Keys.Contains(RecordType) Then
                    LoadingColumns.Add(RecordType, New List(Of LoadColumn))
                End If
                LoadingColumns(RecordType).Add(value)
            Else
                Throw New Exception("Cannot add more columns after AddRecordType(x) directive.")
            End If
        End Set
    End Property

    Public WriteOnly Property AddRecordType(ByVal RecordType As Integer) As LoadRow
        Set(ByVal Row As LoadRow)
            BeginData = True

            If Not LoadingColumns.Keys.Contains(RecordType) Then
                Throw New Exception(String.Format("Invalid record type [{0}]. Record type [{0}] was not defined.", RecordType))
            Else
                Row.RecordType = RecordType
            End If

            If Not LoadingColumns(RecordType).Count = Row.Count Then
                Throw New Exception(
                    String.Format(
                        "Column mismatch. {0} record type {1} columns were defined, but current row contains {2} columns.",
                        LoadingColumns(RecordType).Count, RecordType, Row.Count
                        )
                    )
            End If

            For i As Integer = 0 To Row.Length
                If LoadingColumns(RecordType)(i).Length > -1 Then
                    If Row.Data(i).Length > LoadingColumns(RecordType)(i).Length Then
                        Throw New Exception(
                            String.Format(
                                "Column {0} of record type {1} must be less than {2} characters in length.",
                                LoadingColumns(RecordType)(i).ColumnName,
                                RecordType,
                                LoadingColumns(RecordType)(i).Length
                            )
                        )
                    End If
                End If

                Select Case LoadingColumns(RecordType)(i).ColumnType
                    Case tColumnType.typeCHAR
                        Row.Data(i) = String.Format("'{0}'", Row.Data(i).Replace("'", "' + char(39) + '"))

                    Case tColumnType.typeINT
                        If Not IsNumeric(Row.Data(i)) Then _
                            Throw New Exception(
                                String.Format(
                                    "Invalid Data [{0}]. Record Type {1}, column [{2}] is declared as INT.",
                                    Row.Data(i),
                                    RecordType,
                                    LoadingColumns(RecordType)(i).ColumnName
                                 )
                            )
                        Row.Data(i) = String.Format("dbo.INTQUANT({0})", Row.Data(i))

                    Case tColumnType.typeREAL
                        If Not IsNumeric(Row.Data(i)) Then _
                            Throw New Exception(
                                String.Format(
                                    "Invalid Data [{0}]. Record Type {1}, column [{2}] is declared as REAL.",
                                    Row.Data(i),
                                    RecordType,
                                    LoadingColumns(RecordType)(i).ColumnName
                                 )
                            )
                        Row.Data(i) = String.Format("dbo.REALQUANT({0})", Row.Data(i))

                    Case tColumnType.typeMONEY
                        If Not IsNumeric(Row.Data(i)) Then _
                            Throw New Exception(
                                String.Format(
                                    "Invalid Data [{0}]. Record Type {1}, column [{2}] is declared as MONEY.",
                                    Row.Data(i),
                                    RecordType,
                                    LoadingColumns(RecordType)(i).ColumnName
                                 )
                            )
                        Row.Data(i) = String.Format("{0}", Row.Data(i))

                    Case tColumnType.typeDATE
                        If IsDate(Row.Data(i)) Then
                            Row.Data(i) = String.Format("{0}", DateDiff(DateInterval.Minute, #1/1/1988#, CDate(Row.Data(i))))
                        ElseIf String.Compare(Row.Data(i), "%NOW%", True) = 0 Then
                            Row.Data(i) = String.Format("{0}", DateDiff(DateInterval.Minute, #1/1/1988#, Now()))
                        ElseIf String.Compare(Row.Data(i), "%DATE%", True) = 0 Then
                            Row.Data(i) = String.Format("{0}", DateDiff(DateInterval.Minute, #1/1/1988#, Now()))
                        ElseIf IsNumeric(Row.Data(i)) Then
                            Row.Data(i) = String.Format("{0}", Row.Data(i))
                        Else
                            Throw New Exception(
                                String.Format(
                                    "Invalid Data [{0}]. Record Type {1}, column [{2}] is declared as DATE.",
                                    Row.Data(i),
                                    RecordType,
                                    LoadingColumns(RecordType)(i).ColumnName
                                 )
                            )
                        End If

                    Case tColumnType.typeBOOL
                        Select Case Row.Data(i).ToUpper
                            Case "Y", "N", ""
                                Row.Data(i) = String.Format("'{0}'", Row.Data(i).ToUpper)
                            Case Else
                                Throw New Exception(
                                    String.Format(
                                        "Invalid Data [{0}]. Record Type {1}, column [{2}] is declared as BOOLEAN.",
                                        Row.Data(i),
                                        RecordType,
                                        LoadingColumns(RecordType)(i).ColumnName
                                     )
                                )
                        End Select

                End Select
            Next
            LoadingRows.Add(Row)
        End Set
    End Property

#End Region

#Region "Private Properties"

    Private ReadOnly Property toByte() As Byte()
        Get
            Dim myEncoder As New System.Text.ASCIIEncoding
            Dim str As New System.Text.StringBuilder
            Dim xw As XmlWriter = XmlWriter.Create(str)
            With xw

                .WriteStartDocument()
                .WriteStartElement("PriorityLoading")

                If Not IsNothing(Table) Then .WriteAttributeString("TABLE", Me.Table)
                If Not IsNothing(Procedure) Then .WriteAttributeString("PROCEDURE", Me.Procedure)
                If Not IsNothing(Environment) Then .WriteAttributeString("ENVIRONMENT", Me.Environment)
                If Timeout > 0 Then .WriteAttributeString("TIMEOUT", Me.Timeout.ToString)

                For Each row As LoadRow In LoadingRows
                    .WriteStartElement("ROW")
                    .WriteAttributeString("RECORDTYPE", row.RecordType.ToString)
                    For Each rt As Integer In LoadingColumns.Keys
                        If rt = row.RecordType Then
                            For i As Integer = 0 To UBound(row.Data)
                                .WriteAttributeString(LoadingColumns(rt)(i).ColumnName, row.Data(i))
                            Next
                        Else
                            For Each col As LoadColumn In LoadingColumns(rt)
                                Select Case col.ColumnType
                                    Case tColumnType.typeBOOL, tColumnType.typeCHAR
                                        .WriteAttributeString(col.ColumnName, "''")
                                    Case tColumnType.typeDATE, tColumnType.typeINT, tColumnType.typeREAL, tColumnType.typeMONEY
                                        .WriteAttributeString(col.ColumnName, "0")
                                End Select
                            Next
                        End If
                    Next
                    .WriteEndElement()
                Next
                .WriteEndDocument()
                .Flush()

            End With

            Return myEncoder.GetBytes(str.ToString)
        End Get
    End Property

    Private ReadOnly Property DefaultEnv() As String
        Get
            Return HttpContext.Current.GetSection("appSettings")("Environment")
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Sub Clear()
        LoadingColumns = New Dictionary(Of Integer, List(Of LoadColumn))
        LoadingRows = New List(Of LoadRow)
        With Me
            .Environment = Nothing
            .Table = Nothing
            .Procedure = Nothing
            .Timeout = Nothing
            .BeginData = False
        End With
    End Sub

    Public Function Post(ByRef Ex As Exception) As Boolean
        Return Post("http://localhost:8080/loadhandler.ashx", Ex)
    End Function

    Public Function Post(ByRef context As HttpContext, ByRef msgFactory As msgFactory, ByRef Ex As Exception) As Boolean

        Dim ret As Boolean = False
        With context
            Try
                Using cli As New iClient(
                    .GetSection("appSettings")("Service"),
                    .GetSection("appSettings")("loadport")
                )

                    Dim resp = msgFactory.Decode(
                        cli.Send(
                            msgFactory.EncodeRequest(
                                "loading",
                                New oMsgLoading(toByte)
                            )
                        )
                    )

                    If Not resp.ErrCde = 200 Then
                        Ex = New Exception(resp.errMsg)
                    Else
                        ret = True
                    End If

                End Using

            Catch exep As Exception
                Ex = exep

            End Try

        End With
        Return ret

    End Function

    Public Function Post(ByVal url As String, ByRef Ex As Exception) As Boolean

        Dim uploadResponse As Net.HttpWebResponse
        Dim requestStream As Stream
        Dim posted As Boolean = False
        Ex = Nothing

        Try

            ' Add script handler name if not specified by the request
            ' Defaults to loadhandler.ashx if ommited
            Try
                If Not String.Compare(url.Split("/").Last.Split(".").Last, "ashx", True) = 0 Then
                    If Not String.Compare(url.Last, "/") = 0 Then
                        url += "/"
                    End If
                    url += "loadHandler.ashx"
                End If
            Catch
                Throw New Exception("Invalid URL specified.")
            End Try

            ' Validate Procedure and Table are present
            If IsNothing(Me.Procedure) And IsNothing(Me.Table) Then
                Throw New Exception(
                    String.Format(
                        "Procedure or loading table not specified."
                     )
                )
            End If

            ' Validate the loading contains some rows
            If Not IsNothing(Me.Table) And Not LoadingRows.Count > 0 Then
                Throw New Exception(
                    String.Format(
                        "Loading contains no data."
                     )
                )
            End If

            requestStream = Nothing
            uploadResponse = Nothing

            Dim ms As MemoryStream = New MemoryStream(toByte)
            Dim uploadRequest As Net.HttpWebRequest = CType(Net.HttpWebRequest.Create(url), Net.HttpWebRequest)

            uploadRequest.Method = "POST"
            uploadRequest.Proxy = Nothing
            uploadRequest.SendChunked = True
            requestStream = uploadRequest.GetRequestStream()

            ' Upload the XML
            Dim buffer(1024) As Byte
            Dim bytesRead As Integer
            While True
                bytesRead = ms.Read(buffer, 0, buffer.Length)
                If bytesRead = 0 Then
                    Exit While
                End If
                requestStream.Write(buffer, 0, bytesRead)
            End While

            ' The request stream must be closed before getting the response.
            requestStream.Close()
            uploadResponse = uploadRequest.GetResponse()

            Dim thisRequest As New XmlDocument
            Dim reader As New StreamReader(uploadResponse.GetResponseStream)
            With thisRequest
                .LoadXml(reader.ReadToEnd)
                Dim n As XmlNode = .SelectSingleNode("response")
                Dim er As Boolean = False
                For Each attrib As XmlAttribute In n.Attributes
                    If attrib.Name = "status" Then
                        If Not attrib.Value = "200" Then er = True
                    End If
                    If attrib.Name = "message" Then
                        If er Then
                            Throw New Exception(attrib.Value)
                        End If
                    End If
                Next
            End With

            posted = True

        Catch exep As UriFormatException
            Ex = New Exception(String.Format("Invalid URL: {0}", exep.Message))
        Catch exep As Net.WebException
            Ex = New Exception(String.Format("Connection Error: {0}", exep.Message))
        Catch exep As Exception
            Ex = New Exception(String.Format("Posting failed: {0}", exep.Message))
        Finally
            ' Clean up the streams
            If Not IsNothing(uploadResponse) Then
                uploadResponse.Close()
            End If
            If Not IsNothing(requestStream) Then
                requestStream.Close()
            End If
        End Try

        Return posted

    End Function

#End Region

End Class
