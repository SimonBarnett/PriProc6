Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.MessageInterface

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "log")>
<ExportMetadata("verb", eVerb.Request)>
Public Class msgLog : Inherits svcRequest : Implements msgInterface

    Overrides Sub fromObject(ByRef msg As msgBase, ByRef ob As objBase)

        Dim thisMsg As oMsgLog = TryCast(msg.thisObject, oMsgLog)
        Dim thisOb As oMsgLog = TryCast(ob, oMsgLog)

        With thisMsg
            .svcType = thisOb.svcType
            .LogSource = thisOb.LogSource
            .Verbosity = thisOb.Verbosity
            .EntryType = thisOb.EntryType
            .EventOnly = thisOb.EventOnly
            .LogData = thisOb.LogData
        End With

    End Sub

    Overrides Sub readXML(ByRef msg As msgBase) Implements msgInterface.readXML

        msg.thisObject = New oMsgLog()
        With TryCast(msg.thisObject, oMsgLog)
            .Verb = msg.Verb
            .Source = msg.Source
            .TimeStamp = msg.TimeStamp
            .msgType = msg.msgType
        End With

        Dim thisMsg As oMsgLog = TryCast(msg.thisObject, oMsgLog)
        With thisMsg
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
