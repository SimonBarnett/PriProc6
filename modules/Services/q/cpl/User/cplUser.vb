Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "User")>
Public Class cplUser : Inherits cplBase

    Public Overrides Sub LoadObject(ByRef o As Object)

        thisPanel = New cplUserPage(TryCast(o, oLoader))

    End Sub

End Class
