Imports System.Windows.Forms

Class ListViewItemComparer
    Implements IComparer

    Private col As Integer

#Region "Initialisation and finalisation"

    Public Sub New()
        col = 0
    End Sub

    Public Sub New(ByVal column As Integer)
        col = column
    End Sub

#End Region

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare

        Dim firstDate As Date = Date.ParseExact(CType(x, ListViewItem).SubItems(col).Text, "HH:mm:ss", Nothing)
        Dim secondDate As Date = Date.ParseExact(CType(y, ListViewItem).SubItems(col).Text, "HH:mm:ss", Nothing)

        Return DateTime.Compare(firstDate, secondDate)

    End Function

End Class
