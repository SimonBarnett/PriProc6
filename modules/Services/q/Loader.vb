Imports System
Imports System.Management
Imports System.IO
Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.svcMessage
Imports PriPROC6.PriSock
Imports System.Data.SqlClient
Imports System.Drawing

<Export(GetType(svcDef))>
<ExportMetadata("Name", "loader")>
<ExportMetadata("udp", False)>
<ExportMetadata("defaultPort", 8093)>
<ExportMetadata("defaultStart", False)>
Public Class Loader : Inherits svcbase
    Implements svcDef


#Region "Start / Stop"

    Public Overrides Sub svcStart(ByRef log As oMsgLog)

        If Not FindPriority(log) Then
            Throw New Exception(
                String.Format(
                    "A Priorty share was not located on {0}.",
                    Environment.MachineName
                )
            )
        End If

        If Not AttemptConnect(log) Then
            Throw New Exception(
                String.Format(
                    "Could not connect to database {0}.",
                    thisConfig.regValue(False, "PriorityDB")
                )
            )
        End If

        _PriorityUsers = New PriorityUsers(Me, log)

    End Sub

    Function AttemptConnect(ByRef log As oMsgLog) As Boolean

        Dim ret As Boolean = False
        Try
            Using cmd As New prisql(log, thisConfig)
                Select Case cmd.ExecuteScalar(
                    "SELECT count(name)" &
                    "FROM sysdatabases " &
                    "where name in ('system','pritempdb')"
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

    Public Overrides Sub svcStop(ByRef log As oMsgLog)

    End Sub

#End Region

#Region "Discovery Messages"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)

        With outputStream

            .WriteElementString("PriorityShare", thisConfig.regValue(False, "PriorityShare"))
            .WriteElementString("PriorityPath", thisConfig.regValue(False, "PriorityPath"))
            .WriteElementString("PriorityDB", thisConfig.regValue(False, "PriorityDB"))

            Dim log As New oMsgLog(Me.Name, EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.Information)
            Using cmd As New prisql(log, thisConfig)
                Try
                    Dim dataReader As SqlDataReader = cmd.ExecuteReader(
                    "use [system] " &
                    "select DNAME from ENVIRONMENT " &
                    "where DNAME <> ''"
                )

                    If dataReader.HasRows Then
                        dataReader.Read()
                        Do
                            Dim d As New DirectoryInfo(
                                IO.Path.Combine(
                                    Qfolder.FullName,
                                    dataReader.Item(0).ToString)
                                )
                            If Not d.Exists Then d.Create()

                            .WriteStartElement("env")
                            .WriteAttributeString("name", dataReader.Item(0).ToString)
                            .WriteEndElement() 'End row 
                        Loop While dataReader.Read()

                    End If
                Catch : End Try

                Dim dic As Dictionary(Of String, String) = thisConfig.regDictionary(True, "users")
                Dim f As Boolean = False
                For Each u As String In dic.Keys
                    .WriteStartElement("user")
                    .WriteAttributeString("name", u.ToString)
                    .WriteEndElement() 'End row 
                Next
            End Using

            If Not log.EntryType = LogEntryType.Information Then
                LogQ.Enqueue(msgFactory.EncodeRequest("log", log))
            End If

        End With
    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oLoader(Service)
        With ret
            .PriorityShare = Service.SelectSingleNode("PriorityShare").InnerText
            .PriorityPath = Service.SelectSingleNode("PriorityPath").InnerText
            .PriorityDB = Service.SelectSingleNode("PriorityDB").InnerText

            For Each env As XmlNode In Service.SelectNodes("env")
                .Add(env.Attributes("name").Value, New PriEnv(env.Attributes("name").Value))
            Next
            For Each usr As XmlNode In Service.SelectNodes("user")
                .Users.Add(usr.Attributes("name").Value)
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
                With TryCast(msg.thisObject, oMsgCmd)
                    Try
                        If .Args.Keys.Contains("user") Then
                            Select Case .Args("user").ToLower
                                Case "add"
                                    thisLog.LogData.AppendFormat("Adding user [{0}].", .Args("username")).AppendLine()
                                    _PriorityUsers.AddUser(
                                        New PriorityUser(
                                            .Args("username"),
                                            .Args("password")
                                        ), thisLog
                                    )
                                    thisLog.EntryType = LogEntryType.SuccessAudit
                                    Return msgFactory.EncodeResponse("generic", 200)

                                Case "delete"
                                    thisLog.LogData.AppendFormat("Deleting user [{0}].", .Args("username")).AppendLine()
                                    _PriorityUsers.DeleteUser(
                                        .Args("username"),
                                        thisLog
                                    )
                                    thisLog.EntryType = LogEntryType.SuccessAudit
                                    Return msgFactory.EncodeResponse("generic", 200)

                                Case "chpass"
                                    thisLog.LogData.AppendFormat("Chpass user [{0}].", .Args("username")).AppendLine()
                                    _PriorityUsers.chPass(
                                        New PriorityUser(
                                            .Args("username"),
                                            .Args("password")
                                        ), thisLog
                                    )
                                    thisLog.EntryType = LogEntryType.SuccessAudit
                                    Return msgFactory.EncodeResponse("generic", 200)

                                Case Else
                                    Return msgFactory.EncodeResponse("generic", 400, "Invalid user command.")

                            End Select

                        Else
                            Return msgFactory.EncodeResponse("generic", 400, "Unknown command.")

                        End If

                    Catch ex As Exception
                        thisLog.setException(ex)
                        Return msgFactory.EncodeResponse("generic", 400, ex.Message)

                    End Try

                End With

            Case "loading"
                With TryCast(msg.thisObject, oMsgLoading)
                    .toFile(New DirectoryInfo(Path.Combine(Qfolder.FullName, .SaveAs())))
                    Return msgFactory.EncodeResponse("generic", 200)

                End With

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

#Region "user"

    Private _PriorityUsers As PriorityUsers
    Public Property PriorityUsers As PriorityUsers
        Get
            Return _PriorityUsers
        End Get
        Set(value As PriorityUsers)
            _PriorityUsers = value
        End Set
    End Property

#End Region

#Region "Queues"

    Private _Queues As New Dictionary(Of String, LoadQ)
    Public Property Queues As Dictionary(Of String, LoadQ)
        Get
            Return _Queues
        End Get
        Set(value As Dictionary(Of String, LoadQ))
            _Queues = value
        End Set
    End Property

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.ppLoader)
            ret.Add("environment", My.Resources.priority)
            ret.Add("user", My.Resources.userconfig)
            ret.Add("procedure", My.Resources.procedure)
            Return ret
        End Get
    End Property

    Public Overrides ReadOnly Property ModuleVersion As Version
        Get
            Return Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property

#End Region

End Class
