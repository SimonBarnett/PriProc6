Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "handler")>
Public Class cplHandler : Inherits cplBase

    Public Overrides Sub LoadObject(ByRef o As Object)
        thisPanel = New cplHanderControl(o)

    End Sub

End Class
