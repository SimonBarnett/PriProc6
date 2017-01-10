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
Imports System.Data.Sql
Imports System.Data.SqlClient

<Export(GetType(svcDef))>
<ExportMetadata("Name", "prisql")>
<ExportMetadata("udp", False)>
<ExportMetadata("defaultPort", 8094)>
<ExportMetadata("defaultStart", False)>
Public Class priSQL : Inherits svcbase
    Implements svcDef

#Region "Start / Stop"

    Public Overrides Sub svcStart(ByRef log As oMsgLog)

        Dim db As ServerInstance = DataSource
        If IsNothing(db) Then
            Throw New Exception(
                String.Format(
                    "Priority Database not found on [{0}].",
                    Environment.MachineName
                )
            )
        Else
            log.LogData.AppendFormat("Found Priority Database at [{0}].", db.ServerInstance).AppendLine()
            CheckEnvironment(db)
            Using cn As New SqlConnection(DataSource.ConnectionString)
                cn.Open()
                Dim cmd As SqlCommand = cn.CreateCommand
                For Each env As String In PriorityEnviroments
                    log.LogData.AppendFormat("Found Priority Environment: [{0}].", env).AppendLine()
                    'cmd.CommandText = CountERRMSGTrigger(env).ToString
                    'If cmd.ExecuteScalar > 0 Then
                    '    log.LogData.AppendFormat("Found triggers on: [{0}].[ERRMSGS], removing.", env).AppendLine()
                    '    cmd.CommandText = RemoveERRMSGTrigger(env, cmd).ToString
                    '    cmd.ExecuteNonQuery()
                    'End If
                Next
            End Using

        End If

    End Sub

    Public Overrides Sub svcStop(ByRef log As oMsgLog)

    End Sub

#End Region

#Region "Discovery Messages"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)
        With outputStream
            .WriteElementString(
                "datasource",
                String.Format(
                    "{0}\{1}",
                    thisConfig.regValue(False, "Datasource", "Server"),
                    thisConfig.regValue(False, "Datasource", "Instance")
                )
            )
            Using db As New SqlConnection(DataSource.ConnectionString)
                db.Open()

                Dim cmd As SqlCommand = db.CreateCommand
                cmd.CommandText =
                    "use [system] " &
                    "select DNAME from ENVIRONMENT " &
                    "where DNAME <> ''"

                Dim dataReader As SqlDataReader =
                    cmd.ExecuteReader()

                If dataReader.HasRows Then
                    dataReader.Read()
                    Do
                        .WriteStartElement("env")
                        .WriteAttributeString("name", dataReader.Item(0).ToString)
                        .WriteEndElement() 'End row 
                    Loop While dataReader.Read()

                End If

            End Using

        End With
    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oService
        Dim ret As New oPriSQL(Service)
        With ret
            .datasource = Service.SelectSingleNode("datasource").InnerText
            For Each env As XmlNode In Service.SelectNodes("env")
                ret.Add(env.Attributes("name").Value, New PriEnv(env.Attributes("name").Value))
            Next
        End With
        Return ret

    End Function

#End Region

#Region "Message Handlers"

    Public Overrides Function tcpMsg(ByRef msg As msgBase, ByRef thisLog As oMsgLog) As Byte()

        Dim ret As Byte() = Nothing

        Select Case msg.msgType
            Case "cmd"
                With TryCast(msg.thisObject, oMsgCmd)
                    Throw New Exception("Unknown Message Type.")
                End With

            Case Else
                Throw New Exception("Unknown Message Type.")

        End Select

    End Function

#End Region

#Region "Service Properties"

    Private ReadOnly Property DataSource() As ServerInstance
        Get
            With thisConfig

                Dim db As String = .regValue(False, "Datasource", "Server")
                Dim inst As String = .regValue(False, "Datasource", "Instance")

                If Not (db.Length = 0) Then
                    Dim testInst As New ServerInstance(db, inst)
                    If Not (AttemptConnect(testInst)) Then
                        db = String.Empty
                    End If
                End If

                If db.Length = 0 Then
                    For Each s As ServerInstance In EnumerateServers()
                        If AttemptConnect(s) Then
                            .regValue(False, "Datasource", "Server") = s.Server
                            .regValue(False, "Datasource", "Instance") = s.Instance
                            Return s
                            Exit For
                        End If
                    Next
                    Return Nothing
                Else
                    Return New ServerInstance(db, inst)
                End If

            End With
        End Get
    End Property

#End Region


End Class
