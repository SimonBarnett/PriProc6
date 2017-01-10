Imports System.ComponentModel.Composition
Imports System.Xml
Imports PriPROC6.Interface.Message

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "loading")>
<ExportMetadata("verb", eVerb.Request)>
Public Class msgLoadingRequest : Inherits svcRequest : Implements msgInterface

    Overrides Sub readXML(ByRef msg As msgBase)
        Dim l As New oMsgLoading(msg)
        l.LoadNode(msg.msgNode.SelectSingleNode("load"))
        msg.thisObject = l

    End Sub

    Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)
        Dim o As oMsgLoading = TryCast(msg.thisObject, oMsgLoading)
        o.toWriter(outputStream)

    End Sub

End Class
