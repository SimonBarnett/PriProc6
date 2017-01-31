Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "home")>
Public Class cplHome : Inherits cplBase

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Dim cplHtmlPage = New cplHtmlPage
        cplHtmlPage.SetHTML(My.Resources.home.ToString)
        Return cplHtmlPage
    End Function

End Class

