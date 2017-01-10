Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.Interface.Message

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "cmd")>
<ExportMetadata("verb", eVerb.Request)>
Public Class msgCommandRequest : Inherits svcRequest
    Implements msgInterface

    Public Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)
        With outputStream
            .WriteStartElement("args")
            For Each k As String In TryCast(msg.thisObject, oMsgCmd).Args.Keys
                .WriteElementString(k, TryCast(msg.thisObject, oMsgCmd).Args(k))
            Next
            .WriteEndElement()
        End With

    End Sub

    Public Overrides Sub ReadXML(ByRef msg As msgBase)
        Dim l As New oMsgCmd(msg)
        With l
            For Each sett As XmlNode In msg.msgNode.SelectSingleNode("//request/args").ChildNodes
                .Args.Add(sett.Name, sett.InnerText)
            Next
        End With
        msg.thisObject = l

    End Sub

End Class

