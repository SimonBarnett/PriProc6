Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "Bubble")>
Public Class cplBubble : Inherits cplBase

    Public Overrides Sub LoadObject(ByRef o As Object)

        thisPanel = New cplBubblePage(o)

    End Sub

End Class
