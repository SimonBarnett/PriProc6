Imports System.ComponentModel.Composition
Imports System.Xml
Imports Priproc6.Interface.Message

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "discovery")>
<ExportMetadata("verb", eVerb.Response)>
Public Class msgDiscoveryResponse : Inherits svcResponse : Implements msgInterface

    Public Overrides Sub ReadXML(ByRef msg As msgBase)
        Dim l As New oMsgDiscovery(msg)
        With l
            For Each svc As XmlNode In msg.msgNode.SelectNodes("//response/service")
                .svc.Add(svc)
            Next
        End With
        msg.thisObject = l
    End Sub

    Public Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter) 
        With TryCast(msg.thisObject, oMsgDiscovery)
            For Each thisModule As WritableXML In .svc
                thisModule.toXML(outputStream)
            Next
        End With

    End Sub

End Class
