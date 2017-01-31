'https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx

Imports System.IO
Imports System.Reflection

Public Module Main

    Sub Main()

        Dim fi As New FileInfo(Assembly.GetExecutingAssembly().ToString)
        Using h As New ServiceHost.Hanger(
            GetType(Priproc6.service.Discovery).Assembly,
            New DirectoryInfo(
                Path.Combine(fi.DirectoryName, "modules")
            )
        )
            While h.Count > 0
                Threading.Thread.Sleep(100)
            End While
            h.svc_stop()
        End Using

    End Sub

End Module
