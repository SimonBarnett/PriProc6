'https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx

Public Module Main

    Sub Main()
        Using h As New Host()
            While h.Hanger.Count > 0
                Threading.Thread.Sleep(100)
            End While
            h.Hanger.svc_stop()
        End Using

    End Sub

End Module
