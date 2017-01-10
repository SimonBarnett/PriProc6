Imports System.Windows.Forms
Imports PriPROC6.svcMessage

Public Class cplFeedControl

    Private _server As EndPoint

    Public Sub New(O As Object)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _server = TryCast(O, EndPoint)
        Refreshdata()
    End Sub

    Public Sub Refreshdata()
        Me.WebBrowser1.Navigate(
            String.Format(
                "{0}?{1}",
                _server.URL,
                Address.Text.Replace("?", "")
            )
        )
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Refreshdata()
    End Sub

    Private Sub Address_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Address.KeyUp
        Select Case e.KeyData
            Case Keys.Enter
                Refreshdata()
        End Select
    End Sub

End Class
