Imports System.IO
Imports System.Windows.Forms
Imports System.Xml
Imports PriPROC6.svcMessage

Public Class BubbleDetail

    Public Event CloseForm()

    'Private ld As oMsgLoading
    Private _Bubble As FileInfo

    Public Sub New(Bubble As FileInfo)

        ' This call is required by the designer.
        InitializeComponent()

        '' Add any initialization after the InitializeComponent() call.
        '_Bubble = Bubble
        'ld = New oMsgLoading(Bubble)

        'PropertyPage.SelectedObject = ld

        'Dim firstrow As Boolean = True
        'For Each row As LoadRow In ld.Rows
        '    If firstrow Then
        '        firstrow = Not firstrow
        '        row.toColumns(Me.DataGrid)
        '    End If
        '    row.toGrid(DataGrid)
        'Next

        'firstrow = True
        'For Each attempt As LoadAttempt In ld.LoadAttempts.Values
        '    If firstrow Then
        '        firstrow = Not firstrow
        '        attempt.toColumns(Me.historyList)
        '    End If
        '    Me.historyList.Items.Add(attempt.lvItem)
        'Next

    End Sub

#Region "Form Control Events"

    'Private Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl.SelectedIndexChanged
    '    Select Case TabControl.SelectedIndex
    '        Case 3
    '            Me.WebBrowser.Navigate(_Bubble.FullName)


    '    End Select
    'End Sub

    'Private Sub historyList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles historyList.SelectedIndexChanged
    '    Dim firstrow As Boolean = True
    '    lstWarnings.Clear()
    '    If historyList.SelectedItems.Count > 0 Then
    '        For Each warn As Warning In ld.LoadAttempts(historyList.SelectedItems(0).Name).Warnings
    '            If firstrow Then
    '                firstrow = Not firstrow
    '                warn.toColumns(Me.lstWarnings)
    '            End If
    '            Me.lstWarnings.Items.Add(warn.lvItem)
    '        Next
    '    End If
    'End Sub

#End Region

End Class
