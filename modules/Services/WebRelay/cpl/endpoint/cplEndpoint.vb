Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "endpoint")>
Public Class cplEndpoint : Inherits cplBase

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Return New cplPropertyPage(TryCast(o, PriWeb))
    End Function

End Class
