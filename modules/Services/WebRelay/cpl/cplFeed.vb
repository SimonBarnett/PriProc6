Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl

<Export(GetType(cplInterface))>
<ExportMetadata("Name", "feed")>
Public Class cplFeed : Inherits cplBase

    Public Overrides ReadOnly Property Cpl As Windows.Forms.UserControl
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Overrides Sub LoadObject(ByRef o As Object)
        Throw New NotImplementedException()
    End Sub

End Class
