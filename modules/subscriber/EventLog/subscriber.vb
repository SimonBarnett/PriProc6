Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports System.Xml
Imports System.Drawing
Imports System.IO

<Export(GetType(SubscribeDef))>
<ExportMetadata("Name", "EventLog")>
<ExportMetadata("defaultStart", False)>
<ExportMetadata("Console", False)>
<ExportMetadata("EntryType", 31)>
<ExportMetadata("Verbosity", 99)>
<ExportMetadata("Source", 7)>
Public Class Subscriber : Inherits SubscriberBase

    Public Overrides Sub NewEntry(Entry As msgBase)
        Try

            With TryCast(Entry.thisObject, oMsgLog)
                Dim objEventLog As New EventLog()
                Try
                    Dim myEvent As EventInstance
                    Select Case .LogSource
                        Case EvtLogSource.SYSTEM
                            myEvent = New EventInstance(1000, 1)
                        Case EvtLogSource.APPLICATION
                            myEvent = New EventInstance(1001, 2)
                        Case Else
                            myEvent = New EventInstance(1002, 3)
                    End Select

                    Select Case .EntryType
                        Case LogEntryType.Err
                            myEvent.EntryType = EventLogEntryType.Error

                        Case LogEntryType.FailureAudit
                            myEvent.EntryType = EventLogEntryType.FailureAudit

                        Case LogEntryType.Information
                            myEvent.EntryType = EventLogEntryType.Information

                        Case LogEntryType.SuccessAudit
                            myEvent.EntryType = EventLogEntryType.SuccessAudit

                        Case LogEntryType.Warning
                            myEvent.EntryType = EventLogEntryType.Warning

                    End Select

                    EventLog.WriteEvent(
                        SysLogName,
                        myEvent,
                        vbCrLf & .LogData.ToString
                    )

                Catch Ex As Exception

                End Try

            End With

        Catch ex As Exception

        End Try

    End Sub

#Region "Start / stop"

    Public Overrides Sub svcStart(ByRef Log As objBase)
        With TryCast(Log, oMsgLog)
            Try
                Dim f As New FileInfo(Reflection.Assembly.GetExecutingAssembly.FullName)
                Dim messageFile As String = IO.Path.Combine(f.DirectoryName, "modules\resources\eventlogmsgs.dll")

                'Register the SysLog as an Event Source
                'EventLog.Delete(SysLogName)
                If Not EventLog.SourceExists(SysLogName) Then
                    .LogData.AppendFormat("Log [{0}] does not exist. Creating.", SysLogName).AppendLine()
                    Dim e As New EventSourceCreationData(SysLogName, SysLogName)
                    With e
                        .MessageResourceFile = messageFile
                        .CategoryResourceFile = messageFile
                        .CategoryCount = 3
                        .ParameterResourceFile = messageFile
                    End With
                    EventLog.CreateEventSource(e)
                End If

                Dim myEventLog As EventLog = New EventLog(SysLogName, ".", SysLogName)
                If messageFile.Length > 0 Then
                    myEventLog.RegisterDisplayName(messageFile, 5001)
                End If

            Catch ex As Exception
                Throw New Exception(String.Format("Failed to create {0} log. {1}", SysLogName, ex.Message))
            End Try

        End With

    End Sub

#End Region

#Region "Discovery Message"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)
        Using myEventLog As EventLog = New EventLog(SysLogName, ".", SysLogName)
            With outputStream
                .WriteElementString("MaximumKilobytes", myEventLog.MaximumKilobytes)
                .WriteElementString("MinimumRetentionDays", myEventLog.MinimumRetentionDays)
            End With
        End Using
    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oSubEventLog(Service)
        With ret
            ret.MinimumRetentionDays = Service.SelectSingleNode("MinimumRetentionDays").InnerText
            ret.MaximumKilobytes = Service.SelectSingleNode("MaximumKilobytes").InnerText
        End With
        Return ret
    End Function

#End Region

#Region "Properties"

    Overrides Property MyProperties(log As Object, ParamArray Name() As String) As String
        Get
            Return MyBase.myProperties(log, Name)

        End Get
        Set(value As String)
            Using myEventLog As EventLog = New EventLog(SysLogName, ".", SysLogName)
                Select Case Name(0).ToLower
                    Case "maximumkilobytes"
                        With TryCast(log, oMsgLog)
                            If Not myEventLog.MaximumKilobytes = CInt(value) Then
                                .LogData.AppendFormat("Setting property on {2}: [{0}] = {1}.", Join(Name, "\"), value, Tag).AppendLine()
                                myEventLog.MaximumKilobytes = CInt(value)
                                .EntryType = LogEntryType.SuccessAudit

                            End If
                        End With

                    Case Else
                        MyBase.myProperties(log, Name) = value

                End Select
            End Using
        End Set
    End Property

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.events)
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
