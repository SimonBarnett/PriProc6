Imports Priproc6.Interface.Cpl
Imports System.Windows.Forms
Imports Priproc6.svcMessage

Public Class cplDiscovery : Inherits BaseCpl

    Private _Server As String
    Sub New(ByRef o As Object)

        ' This call is required by the designer.
        InitializeComponent()
        Obj = o
        PropertyGrid.SelectedObject = Obj
        WebBrowser.SetXML(TryCast(Obj, oDiscovery).BroacastXML)

    End Sub

End Class