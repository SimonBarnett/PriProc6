Public Class cplPropertyPage : Inherits BaseCpl

    Sub New(ByRef o As Object)

        ' This call is required by the designer.
        InitializeComponent()
        Obj = o
        PropertyPage.SelectedObject = Obj

        ' Add any initialization after the InitializeComponent() call.

    End Sub

End Class
