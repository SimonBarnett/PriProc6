Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "subconsole")>
Public Class cplLoader : Inherits cplBase

    Public Overrides Sub LoadObject(ByRef o As Object)
        thisPanel = New cplPropertyPage(TryCast(o, oSubConsole))

    End Sub

End Class
