Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "feed")>
Public Class cplFeed : Inherits cplBase

    Public Overrides Sub LoadObject(ByRef o As Object)
        thisPanel = New cplFeedControl(o)

    End Sub

End Class
