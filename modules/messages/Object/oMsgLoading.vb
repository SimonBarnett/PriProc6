Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Xml
Imports PriPROC6.Interface.Message

Public Class oMsgLoading : Inherits objBase

#Region "Constructor"

    Public Sub New(FromFile As FileInfo)

        Dim thisRequest As New XmlDocument
        Try
            thisRequest.Load(FromFile.FullName)
            LoadNode(thisRequest.SelectSingleNode("load"))

        Catch ex As Exception
            Throw New Exception(String.Format("Invalid bubble. {0}", ex.Message))

        End Try

    End Sub

    Public Sub New(ByRef msg As msgBase)
        With Me
            .Verb = msg.Verb
            .Source = msg.Source
            .msgType = msg.msgType
            .TimeStamp = msg.TimeStamp
        End With
    End Sub

    Public Sub New(ByRef Request As Byte())

        Dim thisRequest As New XmlDocument
        Try
            thisRequest.LoadXml(Encoding.ASCII.GetString(Request, 0, Request.Length))

        Catch ex As Exception
            Throw New Exception(String.Format("Failed loading from BYTE: {0}", ex.Message))
            thisRequest = Nothing

        End Try

        Try
            SharedNew(thisRequest)

        Catch ex As Exception
            Throw New Exception(String.Format("{0}", ex.Message))

        End Try

    End Sub

    Public Sub New(ByRef Request As StreamReader)

        Dim thisRequest As New XmlDocument
        Try
            thisRequest.LoadXml(Request.ReadToEnd)

        Catch ex As Exception
            Throw New Exception(String.Format("Failed loading from STREAM: {0}", ex.Message))
            thisRequest = Nothing

        End Try

        Try
            SharedNew(thisRequest)

        Catch ex As Exception
            Throw New Exception(String.Format("{0}", ex.Message))

        End Try

    End Sub

    Sub SharedNew(ByRef thisRequest As XmlDocument)

        If IsNothing(thisRequest.SelectSingleNode("PriorityLoading")) Then
            Throw New Exception(String.Format("Invalid Loading: {0}", "Missing loading node"))
            Exit Sub
        End If

        Try

            Dim LN As Integer = 0
            Dim ld As XmlNode = thisRequest.SelectSingleNode("PriorityLoading")
            With Me

                .BubbleID = System.Guid.NewGuid.ToString
                .RemoteClient = HttpContext.Current.Request.UserHostAddress
                SetifExists(.Environment, ld.Attributes("ENVIRONMENT"))
                SetifExists(.Table, ld.Attributes("TABLE"))
                SetifExists(.Procedure, ld.Attributes("PROCEDURE"))
                SetifExists(.LoadTimeOut, ld.Attributes("TIMEOUT"))

                For Each Row As XmlNode In thisRequest.SelectNodes("//ROW")
                    LN += 1
                    With .Rows
                        .Add(New LoadRow(Row))
                        .Last.Line = LN
                    End With
                Next

                ' Missing Environment
                If .Environment.Length = 0 Then
                    If IsNothing(HttpContext.Current.GetSection("appSettings")("Environment")) Then
                        Throw New Exception(String.Format("Missing config: {0}", "Environment"))
                    Else
                        .Environment = HttpContext.Current.GetSection("appSettings")("Environment")
                    End If
                End If

                ' Missing Timeout
                If .LoadTimeOut = -1 Then
                    If IsNothing(HttpContext.Current.GetSection("appSettings")("LoadingTimeout")) Then
                        Throw New Exception(String.Format("Missing config: {0}", "LoadingTimeout"))
                    Else
                        .LoadTimeOut = CInt(HttpContext.Current.GetSection("appSettings")("LoadingTimeout"))
                    End If
                End If

            End With

        Catch ex As Exception
            Throw New Exception(String.Format("Exception building load message: {0}", ex.Message))

        End Try

    End Sub

    Public Sub LoadNode(ByRef load As XmlNode)

        Dim meta As XmlNode = load.SelectSingleNode("meta")
        Dim data As XmlNode = load.SelectSingleNode("data")
        Dim attempts As XmlNode = load.SelectSingleNode("attempts")

        With Me
            MyBase.TimeStamp = meta.Attributes("timestamp").Value
            MyBase.Source = meta.Attributes("source").Value
            .BubbleID = meta.Attributes("bubbleid").Value
            .RemoteClient = meta.Attributes("caller").Value
            SetifExists(.Environment, data.Attributes("environment"))
            SetifExists(.Table, data.Attributes("table"))
            SetifExists(.Procedure, data.Attributes("procedure"))
            SetifExists(.LoadTimeOut, data.Attributes("loadtimeout"))

            For Each Row As XmlNode In data.SelectNodes("//row")
                .Rows.Add(New LoadRow(Row))
            Next

            For Each attempt As XmlNode In load.SelectNodes("attempt")
                .LoadAttempts.Add(attempt.Attributes("id").Value, New LoadAttempt(attempt))
            Next

        End With

    End Sub

#End Region

#Region "Private Properties"

    Private _DefEnvironment As String = String.Empty
    Private _DefLoadTimeOut As Integer = 60

    Private Sub SetifExists(ByRef Value As String, ByRef Attrib As XmlAttribute)
        If Not IsNothing(Attrib) Then
            If Attrib.Value.Length > 0 Then
                Value = Attrib.Value
            End If
        End If
    End Sub

#End Region

#Region "Public properties"

    Private _BubbleID As String
    <Browsable(False)>
    Public Property BubbleID() As String
        Get
            Return _BubbleID
        End Get
        Set(ByVal value As String)
            _BubbleID = value
        End Set
    End Property

    Private _Table As String = String.Empty
    <CategoryAttribute("Loading"),
    DescriptionAttribute("The interim table in Priority."),
    [ReadOnly](True)>
    Public Property Table() As String
        Set(ByVal value As String)
            _Table = value
        End Set
        Get
            Return _Table
        End Get
    End Property

    Private _Procedure As String = String.Empty
    <CategoryAttribute("Loading"),
    DescriptionAttribute("The Priority Loading Procedure that loads the data."),
    [ReadOnly](True)>
    Public Property Procedure() As String
        Set(ByVal value As String)
            _Procedure = value
        End Set
        Get
            Return _Procedure
        End Get
    End Property

    Private _Environment As String = String.Empty
    <CategoryAttribute("Loading"),
    DescriptionAttribute("The Priority company into which to load the data."),
    [ReadOnly](True)>
    Public Property Environment() As String
        Set(ByVal value As String)
            _Environment = value
        End Set
        Get
            Return _Environment
        End Get
    End Property

    Private _LoadTimeOut As Integer = -1
    <TypeConverter(GetType(Prop_Timeout)),
    CategoryAttribute("Loading"),
    DescriptionAttribute("How long (in seconds) to wait for a procedure to finish.")>
    Public Property LoadTimeOut() As Integer
        Get
            Return _LoadTimeOut
        End Get
        Set(ByVal value As Integer)
            _LoadTimeOut = value
        End Set
    End Property

    Private _RemoteClient As String
    <CategoryAttribute("Source"),
    DescriptionAttribute("The address of the caller."),
    [ReadOnly](True)>
    Public Property RemoteClient() As String
        Get
            Return _RemoteClient
        End Get
        Set(ByVal value As String)
            _RemoteClient = value
        End Set
    End Property

    Private _Rows As New List(Of LoadRow)
    <Browsable(False)>
    Public Property Rows() As List(Of LoadRow)
        Get
            Return _Rows
        End Get
        Set(ByVal value As List(Of LoadRow))
            _Rows = value
        End Set
    End Property

    Private _LoadAttempts As New Dictionary(Of Integer, LoadAttempt)
    <Browsable(False)>
    Public Property LoadAttempts() As Dictionary(Of Integer, LoadAttempt)
        Get
            Return _LoadAttempts
        End Get
        Set(ByVal value As Dictionary(Of Integer, LoadAttempt))
            _LoadAttempts = value
        End Set
    End Property

#End Region

#Region "Public methods"

    Public Function SQL(Optional NOEXEC As Boolean = False) As String

        Dim thissql As New Text.StringBuilder
        With Me
            thissql.AppendFormat("use [{0}]; ", .Environment).AppendLine()

            If NOEXEC Then

                thissql.Append("declare @c int").AppendLine()
                thissql.AppendFormat("select @c = count(*) from system.dbo.T$EXEC where TYPE = 'P' and ENAME = '{0}'", .Procedure).AppendLine()
                thissql.Append("if @c = 0 ").AppendLine()
                thissql.Append("begin ").AppendLine()
                thissql.AppendFormat("	RAISERROR('Priority Procedure [{0}] does not exist.',16,1);", Procedure).AppendLine()
                thissql.Append("end ").AppendLine()

                thissql.AppendFormat("select @c = count(*) from sys.tables where name = '{0}'", .Table).AppendLine()
                thissql.Append("if @c = 0 ").AppendLine()
                thissql.Append("begin ").AppendLine()
                thissql.AppendFormat("	RAISERROR('Loading Table [{0}] does not exist.',16,1);", Procedure).AppendLine()
                thissql.Append("end ").AppendLine()

                thissql.Append("select count(name) from sys.columns").AppendLine()
                thissql.AppendFormat("where object_id = (select OBJECT_ID from sys.tables where name = '{0}')", .Table).AppendLine()
                thissql.Append("and name in ('BUBBLEID','RECORDTYPE','LINE','LOADED','KEY1','KEY2','KEY3')").AppendLine()
                thissql.Append("if @c <> 7 ").AppendLine()
                thissql.Append("begin ").AppendLine()
                thissql.AppendFormat("	RAISERROR('Table [{0}] has missing loading columns.',16,1);", Procedure).AppendLine()
                thissql.Append("end ").AppendLine()

                thissql.Append("SET FMTONLY ON;").AppendLine()

            End If

            thissql.AppendFormat("DELETE FROM {0} WHERE LINE > 0", .Table).AppendLine()

            Dim first As Boolean = True
            For Each Row As LoadRow In Me.Rows
                If first Then
                    first = False
                    thissql.AppendFormat(
                            "insert into {0} (BUBBLEID, LINE, RECORDTYPE, LOADED, KEY1, KEY2, KEY3, ",
                             .Table
                        )
                    For i As Integer = 0 To Row.Count - 1
                        thissql.Append(Row.Keys(i))
                        If i < Row.Count - 1 Then
                            thissql.Append(", ")
                        Else
                            thissql.Append(") ").AppendLine()
                        End If
                    Next
                Else
                    thissql.Append("UNION ALL ").AppendLine()
                End If

                thissql.AppendFormat(
                        "select '{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}', ",
                            .BubbleID,
                            Row.Line,
                            Row.RecordType,
                            Row.Loaded,
                            Row.Key1,
                            Row.Key2,
                            Row.Key3
                    )
                For i As Integer = 0 To Row.Count - 1
                    thissql.Append(Row.Values(i))
                    If i < Row.Count - 1 Then
                        thissql.Append(", ")
                    Else
                        thissql.AppendLine()
                    End If
                Next
            Next

            If NOEXEC Then thissql.Append("SET FMTONLY OFF;").AppendLine()

        End With

        Return thissql.ToString

    End Function

    Public Sub toFile(Dir As DirectoryInfo)

        If Not Dir.Exists Then Dir.Create()
        Dim fn As String = String.Format("{0}\{1}", Dir.FullName, FileName)
        While File.Exists(fn)
            Try
                File.Delete(fn)
            Catch
                ' Wait 5 seconds and retry
                Threading.Thread.Sleep(5000)
            End Try
        End While

        Using xmlWr As XmlWriter = XmlWriter.Create(fn)
            toWriter(xmlWr)
            xmlWr.Flush()
        End Using

    End Sub

    Public Sub toWriter(ByRef outputStream As System.Xml.XmlWriter)

        With outputStream

            .WriteStartElement("load")

            .WriteStartElement("meta")
            .WriteAttributeString("bubbleid", BubbleID)
            .WriteAttributeString("source", Source)
            .WriteAttributeString("caller", RemoteClient)
            .WriteAttributeString("timestamp", TimeStamp)
            .WriteEndElement()

            .WriteStartElement("data")
            .WriteAttributeString("environment", Environment)
            .WriteAttributeString("table", Table)
            .WriteAttributeString("procedure", Procedure)
            .WriteAttributeString("loadtimeout", LoadTimeOut)

            For Each Row As LoadRow In Rows
                Row.toWriter(outputStream)
            Next
            .WriteEndElement()

            For Each id As Integer In LoadAttempts.Keys
                LoadAttempts(id).toWriter(outputStream)
            Next

        End With

    End Sub

    <CategoryAttribute("Location"),
    DescriptionAttribute("The directory containing the bubble.")>
    Public ReadOnly Property SaveAs() As String
        Get
            Dim tstamp As Date = Date.ParseExact(
                MyBase.TimeStamp,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture
            )
            Return String.Format(
                "{0}\{1}\{2}\{3}",
                Me.Environment,
                Me.Procedure,
                String.Format(
                    "{0}-{1}",
                    tstamp.Year.ToString,
                    Right("00" & tstamp.Month.ToString, 2)
                ),
                Right("00" & tstamp.Day.ToString, 2)
            )
        End Get
    End Property

    <CategoryAttribute("Location"),
    DescriptionAttribute("The filename of the bubble.")>
    Public ReadOnly Property FileName As String
        Get
            Return String.Format("{0}.xml", BubbleID)
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property tzTime As String
        Get
            Dim tstamp As Date = Date.ParseExact(
                MyBase.TimeStamp,
                "dd/MM/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture
            )
            Return String.Format(
                "{0}:{1}:{2}",
                Right("00" & tstamp.Hour.ToString, 2),
                Right("00" & tstamp.Minute.ToString, 2),
                Right("00" & tstamp.Second.ToString, 2)
            )
        End Get
    End Property

#End Region

#Region "Attempts"

    Public Sub NewAttempt(ByRef thisUser As PriorityUser)
        ' Create a new loading attempt
        With Me
            .LoadAttempts.Add(
            .LoadAttempts.Count + 1,
                New LoadAttempt(
                    .LoadAttempts.Count + 1,
                    thisUser.Username
                )
            )
        End With
    End Sub

    Public Sub RemoveLastAttempt()
        With Me
            If LoadAttempts.Count > 0 Then
                With .LoadAttempts
                    ' Remove the last attempt
                    .Remove(.Last.Key)
                End With
            End If
        End With
    End Sub

    Public Function CurrentAttempt() As LoadAttempt
        If LoadAttempts.Count = 0 Then
            Return Nothing
        Else
            Return LoadAttempts.Last.Value
        End If
    End Function

#End Region

End Class
