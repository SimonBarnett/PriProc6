Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.Interface.Message

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "discovery")>
<ExportMetadata("verb", eVerb.Request)>
Public Class msgDiscoveryRequest : Inherits svcRequest
    Implements msgInterface

    Public Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)
        With outputStream
            .WriteElementString("reponseport", TryCast(msg.thisObject, oMsgDiscoveryRequest).ResponsePort)

        End With

    End Sub

    Public Overrides Sub ReadXML(ByRef msg As msgBase)
        Dim l As New oMsgDiscoveryRequest(msg)
        With l
            l.ResponsePort = msg.msgNode.SelectSingleNode("reponseport").InnerText
        End With
        msg.thisObject = l

    End Sub

End Class
