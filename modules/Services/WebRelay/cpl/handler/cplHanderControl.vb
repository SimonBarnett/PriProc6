Imports System.IO
Imports System.Windows.Forms
Imports System.Xml
Imports PriPROC6.svcMessage

Public Class cplHanderControl

    Private _File As String
    Private _server As EndPoint

    Public Sub New(O As Object)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _server = TryCast(O, EndPoint)

    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        With OpenFileDialog1
            .DefaultExt = "xml"
            .FileName = ""
            If .ShowDialog = DialogResult.OK Then
                Dim fn As New FileInfo(.FileName)
                If fn.Exists And String.Compare(fn.Extension, ".XML", True) = 0 Then
                    _File = .FileName
                    WebBrowser.Navigate(_File)
                    SendToolStripMenuItem.Enabled = True
                Else
                    MsgBox("Invalid File.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Error.")
                    SendToolStripMenuItem.Enabled = False
                End If
            End If
        End With
    End Sub

    Private Sub SendToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendToolStripMenuItem.Click

        Dim ret As Boolean = True
        Dim requestStream As Stream = Nothing
        Dim uploadResponse As Net.HttpWebResponse = Nothing
        Dim myEncoder As New System.Text.ASCIIEncoding
        Dim bytes As Byte()
        Using sr As New StreamReader(_File)
            bytes = myEncoder.GetBytes(sr.ReadToEnd)
        End Using
        Dim ms As MemoryStream = New MemoryStream(bytes)

        Try

            Dim uploadRequest As Net.HttpWebRequest = CType(Net.HttpWebRequest.Create(_server.URL), Net.HttpWebRequest)
            uploadRequest.Method = Net.WebRequestMethods.Http.Post
            uploadRequest.Proxy = Nothing
            requestStream = uploadRequest.GetRequestStream()

            ' Upload the XML
            Dim buffer(1024) As Byte
            Dim bytesRead As Integer
            While True
                bytesRead = ms.Read(buffer, 0, buffer.Length)
                If bytesRead = 0 Then
                    Exit While
                End If
                requestStream.Write(buffer, 0, bytesRead)
            End While

            ' The request stream must be closed before getting the response.
            requestStream.Close()

            uploadResponse = uploadRequest.GetResponse()

            Dim thisRequest As New XmlDocument
            Dim reader As New StreamReader(uploadResponse.GetResponseStream)
            With thisRequest
                .LoadXml(reader.ReadToEnd)
                Dim n As XmlNode = .SelectSingleNode("response")
                MsgBox(
                    n.Attributes("message").Value,
                    MsgBoxStyle.Information + MsgBoxStyle.OkOnly,
                    n.Attributes("status").Value
                )
            End With

        Catch uex As UriFormatException
            ret = False

            Throw New Exception(
                String.Format(
                    "Malformed URL [{0}]. {1}",
                    _server.URL,
                    uex.Message
                )
            )

        Catch uex As Net.WebException
            ret = False

            Throw New Exception(
                String.Format(
                    "Web Error connecting to [{0}]. {1}",
                    _server.URL,
                    uex.Message
                )
            )


        Finally
            If uploadResponse IsNot Nothing Then
                uploadResponse.Close()
            End If
            If requestStream IsNot Nothing Then
                requestStream.Close()
            End If

        End Try

    End Sub

End Class

