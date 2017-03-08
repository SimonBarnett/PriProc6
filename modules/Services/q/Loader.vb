Imports System.Management
Imports System.IO
Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports Microsoft.Web.Administration
Imports PriPROC6.Interface.Cpl

<Export(GetType(svcDef))>
<ExportMetadata("Name", "loader")>
<ExportMetadata("udp", False)>
<ExportMetadata("defaultPort", 8093)>
<ExportMetadata("defaultStart", False)>
Public Class Loader
    Inherits svcbase
    Implements svcDef

    Private PriorityWeb As Dictionary(Of String, oPriWeb)

#Region "Start / Stop"

    Private Function Webs(Optional ByRef log As oMsgLog = Nothing) As Dictionary(Of String, oPriWeb)

        Dim throwLog As Boolean = False
        If log Is Nothing Then
            log = New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Verbose, LogEntryType.Information)
            throwLog = True
        End If

        Dim ret As New Dictionary(Of String, oPriWeb)
        Using sm As New ServerManager
            For Each site As Site In sm.Sites
                With site
                    If .State = ObjectState.Started Then
                        For Each app As Microsoft.Web.Administration.Application In .Applications
                            For Each virtual As Microsoft.Web.Administration.VirtualDirectory In app.VirtualDirectories
                                If String.Compare(virtual.Path, "/netfiles", True) = 0 Then

                                    Dim di As New DirectoryInfo(virtual.PhysicalPath)
                                    Dim srvpath As New FileInfo(Path.Combine(di.Parent.FullName, "srvpath"))
                                    Dim db As String = String.Empty

                                    Try
                                        If Not File.Exists(srvpath.FullName) Then
                                            Throw New Exception(String.Format("Missing svrpath file in {0}.", di.Parent.FullName))

                                        End If

                                        Try
                                            Using sr As New StreamReader(srvpath.FullName)
                                                db = sr.ReadToEnd().Replace(vbCr, "").Split(vbLf)(2)
                                            End Using

                                        Catch ex As Exception
                                            Throw New Exception(String.Format("Corrupt file: {0}", srvpath.FullName))

                                        End Try

                                        If Not AttemptConnect(log, db) Then
                                            Throw New Exception(
                                                String.Format(
                                                    "Could not connect to database {0}.",
                                                    db
                                                )
                                            )

                                        Else

                                            Dim host As String = tld(NetDef(log, db, "MARKETGATEURL"))
                                            If ret.Keys.Contains(host) Then
                                                Throw New Exception(String.Format("Duplicate Priority Installation: {0}", host))

                                            End If

                                            'f = True
                                            ret.Add(
                                                host,
                                                New oPriWeb(
                                                    db,
                                                    host,
                                                    di.Parent.FullName,
                                                    NetDef(log, db, "NETTABINI")
                                                )
                                            )
                                            With ret(host).qfolder
                                                If Not .Exists Then .Create()

                                            End With

                                            Using cmd As New prisql(log, db)
                                                Try
                                                    Dim dataReader As SqlDataReader = cmd.ExecuteReader(
                                                        "use [system] " &
                                                        "select DNAME from ENVIRONMENT " &
                                                        "where DNAME <> ''"
                                                    )

                                                    If dataReader.HasRows Then
                                                        dataReader.Read()
                                                        Do
                                                            ret(host).Environments.Add(dataReader.Item(0), New oEnv(ret(host), dataReader.Item(0)))
                                                            With ret(host).Environments(dataReader.Item(0)).qFolder
                                                                If Not .Exists Then .Create()
                                                            End With

                                                        Loop While dataReader.Read()

                                                    End If

                                                Catch ex As Exception
                                                    If Not log Is Nothing Then
                                                        log.setException(ex)
                                                    End If

                                                End Try

                                            End Using

                                            Using cmd As New prisql(log, db)
                                                Try
                                                    Dim dataReader As SqlDataReader = cmd.ExecuteReader(
                                                        "use [system]; " &
                                                        "SELECT RESTLOGIN FROM USERSB " &
                                                        "WHERE REST = 'Y'"
                                                    )

                                                    If dataReader.HasRows Then
                                                        dataReader.Read()
                                                        Do
                                                            ret(host).Users.Add(dataReader.Item(0))
                                                        Loop While dataReader.Read()

                                                    End If

                                                Catch ex As Exception
                                                    log.setException(ex)

                                                End Try

                                            End Using

                                        End If

                                    Catch ex As Exception
                                        log.setException(ex)

                                    End Try

                                End If
                            Next
                        Next
                    End If

                End With
            Next

        End Using

        If throwLog Then
            If Not log.EntryType = LogEntryType.Information Then
                LogQ.Enqueue(msgFactory.EncodeRequest("log", log))

            End If
        End If

        Return ret

    End Function

    Public Overrides Sub svcStart(ByRef log As oMsgLog)

        Dim oDataServers = Webs(log)
        If Not oDataServers.Count > 0 Then
            Throw New Exception(
                String.Format(
                    "Priority website Not found On \\{0}.",
                    Environment.MachineName.ToUpper
                )
            )
        End If

        '_PriorityUsers = New PriorityUsers(Me, log)

    End Sub

    Function tld(url As String) As String
        Return String.Format(
            "{0}//{1}",
            Split(url, "//")(0),
            Split(Split(url, "//")(1), "/")(0)
        )
    End Function

    Function AttemptConnect(ByRef log As oMsgLog, db As String) As Boolean

        Dim ret As Boolean = False
        Try
            Using cmd As New prisql(log, db)
                Select Case cmd.ExecuteScalar(
                    "Select count(name)" &
                    "FROM sysdatabases " &
                    "where name In ('system','pritempdb')"
                )
                    Case 2
                        log.LogData.Append("Connected OK.").AppendLine()
                        ret = True

                    Case Else
                        log.LogData.Append("Connection FAIL.").AppendLine()
                        ret = False

                End Select

            End Using

        Catch EX As Exception
            ret = False
            log.LogData.Append("FAIL").AppendLine()
            log.setException(EX)

        End Try

        Return ret

    End Function

    Function NetDef(ByRef log As oMsgLog, db As String, Name As String) As String
        Using cmd As New prisql(log, db)
            Return cmd.ExecuteScalar(
                    String.Format(
                        "SELECT VALUE " &
                        "FROM system.dbo.NETDEFS " &
                        "where NAME ='{0}'",
                        Name
                    )
                )
        End Using

    End Function

    Public Overrides Sub svcStop(ByRef log As oMsgLog)

    End Sub

#End Region

#Region "Discovery Messages"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)

        With outputStream
            For Each pw As oPriWeb In Webs().Values
                .WriteStartElement("priweb")
                .WriteAttributeString("hostname", pw.Hostname)
                .WriteAttributeString("database", pw.PriorityDB)
                .WriteAttributeString("path", pw.Path)
                .WriteAttributeString("tabini", pw.tabini)

                For Each env As oEnv In pw.Environments.Values
                    .WriteStartElement("env")
                    .WriteAttributeString("name", env.Name)
                    .WriteEndElement() 'End row 
                Next

                For Each usr As String In pw.Users
                    .WriteStartElement("user")
                    .WriteAttributeString("name", usr)
                    .WriteEndElement() 'End row 
                Next

                .WriteEndElement() 'End row 
            Next

        End With

    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oLoader(Service)
        With ret
            For Each priweb As XmlNode In Service.SelectNodes("priweb")

                .Add(
                    priweb.Attributes("hostname").Value,
                    New oPriWeb(
                        priweb.Attributes("database").Value,
                        priweb.Attributes("hostname").Value,
                        priweb.Attributes("path").Value,
                        priweb.Attributes("tabini").Value
                    )
                )

                With TryCast(ret(priweb.Attributes("hostname").Value), oPriWeb)
                    For Each env As XmlNode In priweb.SelectNodes("env")
                        .Environments.Add(
                            env.Attributes("name").Value,
                            New oEnv(
                                ret(priweb.Attributes("hostname").Value),
                                env.Attributes("name").Value
                           )
                        )
                    Next

                End With

                With TryCast(ret(priweb.Attributes("hostname").Value), oPriWeb)
                    For Each env As XmlNode In priweb.SelectNodes("user")
                        .Users.Add(
                            env.Attributes("name").Value
                        )
                    Next

                End With

            Next

        End With
        Return ret

    End Function

#End Region

#Region "Message Handlers"

    Overrides Property MyProperties(log As Object, ParamArray Name() As String) As String
        Get
            Select Case Name(0).ToLower
                Case "db"
                    Return thisConfig.regValue(False, "PriorityDB")

                Case Else
                    Return MyBase.MyProperties(log, Name)

            End Select

        End Get
        Set(value As String)
            Select Case Name(0).ToLower
                Case "db"
                    'Read only from here

                Case Else
                    MyBase.MyProperties(log, Name) = value

            End Select

        End Set
    End Property

    Public Overrides Function tcpMsg(ByRef msg As msgBase, ByRef thisLog As oMsgLog) As Byte()

        Dim ret As Byte() = Nothing

        Select Case msg.msgType
            Case "cmd"
                '    With TryCast(msg.thisObject, oMsgCmd)
                '        Try
                '            If .Args.Keys.Contains("user") Then
                '                Select Case .Args("user").ToLower
                '                    Case "add"
                '                        thisLog.LogData.AppendFormat("Adding user [{0}].", .Args("username")).AppendLine()
                '                        _PriorityUsers.AddUser(
                '                            New PriorityUser(
                '                                .Args("username"),
                '                                .Args("password")
                '                            ), thisLog
                '                        )
                '                        thisLog.EntryType = LogEntryType.SuccessAudit
                '                        Return msgFactory.EncodeResponse("generic", 200)

                '                    Case "delete"
                '                        thisLog.LogData.AppendFormat("Deleting user [{0}].", .Args("username")).AppendLine()
                '                        _PriorityUsers.DeleteUser(
                '                            .Args("username"),
                '                            thisLog
                '                        )
                '                        thisLog.EntryType = LogEntryType.SuccessAudit
                '                        Return msgFactory.EncodeResponse("generic", 200)

                '                    Case "chpass"
                '                        thisLog.LogData.AppendFormat("Chpass user [{0}].", .Args("username")).AppendLine()
                '                        _PriorityUsers.chPass(
                '                            New PriorityUser(
                '                                .Args("username"),
                '                                .Args("password")
                '                            ), thisLog
                '                        )
                '                        thisLog.EntryType = LogEntryType.SuccessAudit
                '                        Return msgFactory.EncodeResponse("generic", 200)

                '                    Case Else
                '                        Return msgFactory.EncodeResponse("generic", 400, "Invalid user command.")

                '                End Select

                '            Else
                '                Return msgFactory.EncodeResponse("generic", 400, "Unknown command.")

                '            End If

                '        Catch ex As Exception
                '            thisLog.setException(ex)
                '            Return msgFactory.EncodeResponse("generic", 400, ex.Message)

                '        End Try

                '    End With

                'Case "loading"
                '    With TryCast(msg.thisObject, oMsgLoading)
                '        .toFile(New DirectoryInfo(Path.Combine(Qfolder.FullName, .SaveAs())))
                '        Return msgFactory.EncodeResponse("generic", 200)

                '    End With

            Case Else
                Throw New Exception("Unknown Message Type.")

        End Select

    End Function

#End Region

#Region "Private methods"

    ReadOnly Property shareName(share As ManagementObject) As String
        Get
            Return String.Format(
                "\\{0}\{1}",
                Environment.MachineName,
                share.Item("name")
            )
        End Get
    End Property

    ReadOnly Property shareContains(share As ManagementObject, dir As String) As Boolean
        Get
            Return Directory.Exists(
                String.Format(
                    "{0}\{1}",
                    shareName(share),
                    dir
                )
            )
        End Get
    End Property

    ReadOnly Property Shares As ManagementObjectCollection
        Get
            Dim Path As New ManagementPath

            With Path
                .Server = Environment.MachineName ' use . for local server, else server name
                .NamespacePath = "root\CIMV2"
                .RelativePath = "Win32_Share"
            End With

            Dim sh As New ManagementClass(
                New ManagementScope(Path),
                Path,
                New ObjectGetOptions(
                    Nothing,
                    New TimeSpan(0, 0, 0, 5),
                    True
                )
            )

            Return sh.GetInstances()

        End Get
    End Property

    Private Function FindPriority(ByRef log As oMsgLog) As Boolean

        Dim f As Boolean = False

        For Each MO As ManagementObject In Shares
            If MO.Item("Type") = 0 Then
                log.LogData.AppendFormat("Looking for Priority installation in {0}.", shareName(MO)).AppendLine()
                If shareContains(MO, "BIN.95") And shareContains(MO, "SYSTEM") Then
                    log.LogData.AppendFormat("Found Priority Installation at {0}.", shareName(MO)).AppendLine()
                    If Not File.Exists(String.Format("{0}\nofollow.soap", shareName(MO))) Then
                        If File.Exists(Path.Combine(MO.Item("Path"), "srvpath")) Then

                            Try
                                Using sr As New StreamReader(Path.Combine(MO.Item("Path"), "srvpath"))
                                    thisConfig.regValue(False, "PriorityDB") = sr.ReadToEnd().Replace(vbCr, "").Split(vbLf)(2)
                                End Using

                                f = True
                                thisConfig.regValue(False, "PriorityShare") = shareName(MO)
                                thisConfig.regValue(False, "PriorityPath") = MO.Item("Path")

                            Catch ex As Exception
                                log.LogData.AppendFormat("Invalid svrpath file in {0}.", shareName(MO)).AppendLine()

                            End Try

                            Exit For
                        Else
                            log.LogData.AppendFormat("No svrpath file in {0}.", shareName(MO)).AppendLine()
                        End If

                    Else
                        log.LogData.AppendFormat("Found nofollow.soap file in {0}.", shareName(MO)).AppendLine()
                    End If
                End If
            End If
        Next
        Return f

    End Function

    ReadOnly Property Qfolder As DirectoryInfo
        Get
            Dim d As DirectoryInfo
            d = New DirectoryInfo(IO.Path.Combine(thisConfig.regValue(False, "PriorityPath"), "system\queue"))
            If Not d.Exists Then
                d.Create()
            End If
            Return d
        End Get
    End Property

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property ModuleVersion As Version
        Get
            Return Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Select Case UBound(args)
            Case 1
                Return New cplLoader(TryCast(o, oLoader))
            Case 2
                Return New cplPropertyPage(TryCast(o(args(2)), oPriWeb))

            Case 3
                Return New cplPropertyPage(TryCast(TryCast(o(args(2)), oPriWeb).Environments(args(3)), oEnv))

            Case 4
                Return New cplBubblePage(TryCast(TryCast(o(args(2)), oPriWeb).Environments(args(3)), oEnv).qFolder)

            Case Else
                Return Nothing

        End Select

    End Function

#Region "Tree"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.ppLoader)
            ret.Add("environment", My.Resources.priority)
            ret.Add("user", My.Resources.userconfig)
            ret.Add("priority", My.Resources.priority)
            ret.Add("procedure", My.Resources.procedure)
            Return ret
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef MEF As Object, ByVal p As oServiceBase, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag(p))
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag(p), Name, IconList(Name), IconList(Name))
            Else
                If p.IsTimedOut Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            Dim del As New List(Of String)
            For Each PriorityWeb As Object In p.values
                With TryCast(PriorityWeb, oPriWeb)
                    del.Add(String.Format("{0}\{1}", TreeTag(p), .Hostname))
                    Dim pweb As TreeNode = this.Nodes(String.Format("{0}\{1}", TreeTag(p), .Hostname))
                    If IsNothing(pweb) Then
                        pweb = this.Nodes.Add(String.Format("{0}\{1}", TreeTag(p), .Hostname), .Hostname, IconList("priority"), IconList("priority"))
                    End If

                    .DrawTree(TryCast(PriorityWeb, oPriWeb), pweb, IconList)

                End With
            Next

            For Each tv As TreeNode In this.Nodes
                If Not tv Is Nothing Then
                    If Not del.Contains(tv.Name) Then
                        tv.Remove()
                    End If
                End If
            Next

        End With

    End Sub

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ByRef p As oServiceBase, ParamArray args() As String)
        Select Case UBound(args)
            Case 1
                With TryCast(sender, ContextMenuStrip).Items
                    .Clear()
                    .Add("Stop service", Nothing, AddressOf TryCast(p, oLoader).hStopClick)
                    .Add("Restart service", Nothing, AddressOf TryCast(p, oLoader).hRestartClick)
                End With

            Case Else
                e.Cancel = True

        End Select

    End Sub

#End Region

#End Region

End Class
