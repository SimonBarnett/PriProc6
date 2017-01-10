Imports System.ComponentModel.Composition
Imports System.Xml
Imports Priproc6.Interface.Message

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "log")>
<ExportMetadata("verb", eVerb.Request)>
Public Class msgLogRequest : Inherits svcRequest : Implements msgInterface

    Overrides Sub readXML(ByRef msg As msgBase)

        Dim l As New oMsgLog(msg)
        With l
            .TimeStamp = msg.TimeStamp
            .svcType = msg.msgNode.SelectSingleNode("svctype").InnerText
            Select Case msg.msgNode.SelectSingleNode("logSource").InnerText.ToLower
                Case "application"
                    .LogSource = EvtLogSource.APPLICATION
                Case "web"
                    .LogSource = EvtLogSource.WEB
                Case Else
                    .LogSource = EvtLogSource.SYSTEM
            End Select
            .Verbosity = msg.msgNode.SelectSingleNode("Verbosity").InnerText
            .EntryType = msg.msgNode.SelectSingleNode("EntryType").InnerText
            .EventOnly = CInt(msg.msgNode.SelectSingleNode("EventOnly").InnerText)
            .LogData.Append(msg.msgNode.SelectSingleNode("LogData").InnerText)
        End With
        msg.thisObject = l

    End Sub

    Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)
        With TryCast(msg.thisObject, oMsgLog)
            outputStream.WriteElementString("svctype", .svcType)
            Select Case .LogSource
                Case EvtLogSource.APPLICATION
                    outputStream.WriteElementString("logSource", "application")
                Case EvtLogSource.WEB
                    outputStream.WriteElementString("logSource", "web")
                Case Else
                    outputStream.WriteElementString("logSource", "system")
            End Select
            outputStream.WriteElementString("Verbosity", .Verbosity)
            outputStream.WriteElementString("EntryType", .EntryType)
            outputStream.WriteElementString("LogData", .LogData.ToString)
            outputStream.WriteElementString("EventOnly", .EventOnly.ToString)
        End With

    End Sub

End Class

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "log")>
<ExportMetadata("verb", eVerb.Response)>
Public Class msgLogResponse : Inherits svcResponse : Implements msgInterface

    Overrides Sub readXML(ByRef msg As msgBase)

        Dim l As New oMsgLog(msg)
        With l
            .TimeStamp = msg.TimeStamp
            .svcType = msg.msgNode.SelectSingleNode("svctype").InnerText
            Select Case msg.msgNode.SelectSingleNode("logSource").InnerText
                Case "application"
                    .LogSource = EvtLogSource.APPLICATION
                Case Else
                    .LogSource = EvtLogSource.SYSTEM
            End Select
            .Verbosity = msg.msgNode.SelectSingleNode("Verbosity").InnerText
            .EntryType = msg.msgNode.SelectSingleNode("EntryType").InnerText
            .EventOnly = CInt(msg.msgNode.SelectSingleNode("EventOnly").InnerText)
            .LogData.Append(msg.msgNode.SelectSingleNode("LogData").InnerText)
        End With
        msg.thisObject = l

    End Sub

    Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)
        With TryCast(msg.thisObject, oMsgLog)
            outputStream.WriteElementString("svctype", .svcType)
            Select Case .LogSource
                Case EvtLogSource.APPLICATION
                    outputStream.WriteElementString("logSource", "application")
                Case Else
                    outputStream.WriteElementString("logSource", "system")
            End Select
            outputStream.WriteElementString("Verbosity", .Verbosity)
            outputStream.WriteElementString("EntryType", .EntryType)
            outputStream.WriteElementString("LogData", .LogData.ToString)
            outputStream.WriteElementString("EventOnly", .EventOnly.ToString)
        End With

    End Sub

End Class