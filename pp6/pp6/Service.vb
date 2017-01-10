Public Class Service

    Private h As Host
    Private arg As clArg

    Protected Overrides Sub OnStart(ByVal args() As String)
        arg = New clArg(args)
        If Not arg.Keys.Contains("d") Then
            h = New Host
        End If
        If arg.Keys.Contains("m") Then
            With My.Settings
                .ModuleDir = arg("m")
                .Save()
            End With
        End If
    End Sub

    Protected Overrides Sub OnStop()
        h.Hanger.svc_stop()
    End Sub

    Protected Overrides Sub OnContinue()
        If IsNothing(h) Then
            h = New Host
        End If
    End Sub
End Class
