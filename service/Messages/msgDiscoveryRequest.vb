Imports System.ComponentModel.Composition
Imports System.Xml
Imports Priproc6.svcMessages

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "discovery")>
<ExportMetadata("verb", eVerb.Request)>
Public Class msgDiscoveryRequest : Inherits svcRequest
    Implements msgInterface

    Public Overrides Sub fromObject(ByRef msg As msgBase, ByRef ob As objBase)

    End Sub

    Public Overrides Sub ReadXML(ByRef msg As msgBase)

    End Sub

End Class
