Imports System.ComponentModel.Composition
Imports Priproc6.svcMessages

<Export(GetType(msgInterface))>
<ExportMetadata("msgType", "generic")>
<ExportMetadata("verb", eVerb.Response)>
Public Class msgGenericResponse : Inherits svcResponse : Implements msgInterface

    Public Overrides Sub fromObject(ByRef msg As msgBase, ByRef ob As objBase)

    End Sub

    Public Overrides Sub ReadXML(ByRef msg As msgBase)

    End Sub

End Class
