Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "home")>
Public Class cplHome : Inherits cplBase

    Public Overrides Sub LoadObject(ByRef o As Object)
        thisPanel = New cplHtmlPage
        With TryCast(thisPanel, cplHtmlPage)
            .WebBrowser.DocumentText = My.Resources.home
        End With


    End Sub

End Class

