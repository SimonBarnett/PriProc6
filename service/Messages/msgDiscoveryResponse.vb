Imports System.ComponentModel.Composition
Imports System.Xml
Imports Priproc6.svcMessages

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "discovery")>
<ExportMetadata("verb", eVerb.Response)>
Public Class msgDiscoveryResponse : Inherits svcResponse : Implements msgInterface

    Public Overrides Sub fromObject(ByRef msg As msgBase, ByRef ob As objBase)

    End Sub

    Public Overrides Sub ReadXML(ByRef msg As msgBase)

    End Sub

    Public Overrides Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)

        With TryCast(msg.thisObject, oMsgDiscovery)
            For Each svr As Lazy(Of svcDef, svcDefprops) In .Modules
                svr.Value.svc_info(outputStream)
            Next
        End With

    End Sub

End Class
