Imports System.Windows.Forms
Imports PriPROC6.Interface.Cpl
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class cplUserPage : Inherits BaseCpl

    Private _Loader As oLoader
    Sub New(ByRef o As Object)

        ' This call is required by the designer.
        InitializeComponent()
        Obj = o
        _Loader = TryCast(Obj, oLoader)
        With lst_Users.Items
            .Clear()
            For Each u As String In _Loader.Users
                .Add(u, 0)
            Next
        End With
    End Sub

    Private Sub btn_Click(sender As Object, a As EventArgs) Handles btn_add.Click, btn_delete.Click, btn_Pass.Click

        With TryCast(sender, Windows.Forms.LinkLabel)
            Select Case .Tag.ToString.ToLower
                Case "add"
                    Dim newuserDialog As New UserDialog
                    newuserDialog.Text = "Add User"
                    If newuserDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Dim e As CmdEventArgs = New CmdEventArgs(_Loader.Host, _Loader.Port, True)
                        e.Message = New oMsgCmd
                        With TryCast(e.Message, oMsgCmd).Args
                            .Add("user", "add")
                            .Add("username", newuserDialog.Username.Text)
                            .Add("password", newuserDialog.Pass1.Text)
                        End With
                        _Loader.SendCmd(Me, e)
                    End If

                Case "delete"
                    If MsgBox(
                        String.Format(
                            "Delete user '{0}'?",
                            Me.lst_Users.SelectedItems(0).Text
                        ), MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel
                    ) = MsgBoxResult.Ok Then

                        Dim e As CmdEventArgs = New CmdEventArgs(_Loader.Host, _Loader.Port, True)
                        e.Message = New oMsgCmd
                        With TryCast(e.Message, oMsgCmd).Args
                            .Add("user", "delete")
                            .Add("username", Me.lst_Users.SelectedItems(0).Text)
                        End With
                        _Loader.SendCmd(Me, e)
                    End If

                Case "chpass"
                    Dim dialogPassword As New UserDialog
                    dialogPassword.Text = "Change Password"
                    With dialogPassword
                        .Username.Enabled = False
                        .Username.Text = Me.lst_Users.SelectedItems(0).Text
                        If .ShowDialog = DialogResult.OK Then
                            Dim e As CmdEventArgs = New CmdEventArgs(_Loader.Host, _Loader.Port, True)
                            e.Message = New oMsgCmd
                            With TryCast(e.Message, oMsgCmd).Args
                                .Add("user", "chpass")
                                .Add("username", dialogPassword.Username.Text)
                                .Add("password", dialogPassword.Pass1.Text)
                            End With
                            _Loader.SendCmd(Me, e)
                        End If
                    End With

            End Select

        End With

    End Sub

    Private Sub lst_Users_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lst_Users.SelectedIndexChanged
        btn_Pass.Visible = lst_Users.SelectedIndices.Count > 0
        btn_delete.Visible = lst_Users.SelectedIndices.Count > 0
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Dim remove As New List(Of ListViewItem)
        Dim added As New List(Of String)

        For Each I As ListViewItem In lst_Users.Items
            If Not _Loader.Users.Contains(I.Text) Then
                remove.Add(I)
            Else
                added.Add(I.Text)
            End If
        Next

        For Each r As ListViewItem In remove
            lst_Users.Items.Remove(r)
        Next

        For Each u As String In _Loader.Users
            If Not added.Contains(u) Then
                lst_Users.Items.Add(u, 0)
            End If
        Next

    End Sub

End Class
