Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Text
Imports System.IO

Public Class cplHtmlPage

    Private loading As Boolean = False

    Private Sub WebBrowser_DocumentCompleted(sender As Object, e As Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebBrowser.DocumentCompleted
        loading = False
    End Sub

    Public Sub SetHTML(html As String)
        WebBrowser.DocumentText = html
        loading = True
        While loading
            Application.DoEvents()
        End While
    End Sub

    Sub SetXML(ByRef Node As XmlNode)

        Dim doc As New XmlDocument
        doc.AppendChild(doc.ImportNode(Node, True))

        Dim xr As XmlReader =
            XmlReader.Create(
                New MemoryStream(
                    Encoding.UTF8.GetBytes(
                        My.Resources.ResourceManager.GetObject("defaultss").ToString
                    )
                )
            )

        Dim xct As New XslCompiledTransform()
        xct.Load(xr)

        Dim sb As New StringBuilder()
        Dim xw As XmlWriter = XmlWriter.Create(sb)
        xct.Transform(New XmlNodeReader(doc), xw)

        WebBrowser.DocumentText = sb.ToString()
        loading = True
        While loading
            Application.DoEvents()
        End While

    End Sub

End Class
