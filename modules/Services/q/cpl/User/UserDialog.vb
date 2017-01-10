Imports System.Windows.Forms

Public Class UserDialog

    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Timer1.Enabled = True

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        With Me
            If .Pass1.Text = .Pass2.Text Then
                If Username.Text.Length > 3 Then
                    .DialogResult = System.Windows.Forms.DialogResult.OK
                    .Close()
                Else
                    MsgBox("Username must be more than 3 characters.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Error.")
                End If
            Else
                MsgBox("Passwords do not match", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Error.")
            End If
        End With
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If Me.Username.Enabled Then
            Username.Focus()
        Else
            Pass1.Focus()
        End If
        Timer1.Enabled = False
    End Sub

End Class
