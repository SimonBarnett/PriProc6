Imports System.IO
Imports System.Windows.Forms
Imports PriPROC6.Interface.Cpl
Imports PriPROC6.svcMessage

Public Class cplBubblePage : Inherits BaseCpl

#Region "Constructor"

    Private thd As Threading.Thread
    Private _dir As DirectoryInfo

    Sub New(ByRef o As Object)

        ' This call is required by the designer.
        InitializeComponent()
        Obj = o
        _dir = TryCast(Obj, DirectoryInfo)

        With errList
            .Clear()
            With .Columns
                .Clear()

                .Add("Date and time")
                .Item(.Count - 1).Width = 80

                .Add("Bubble ID")
                .Item(.Count - 1).Width = 250

                .Add("Source")
                .Item(.Count - 1).Width = 100

                .Add("Attempts")
                .Item(.Count - 1).Width = 100

            End With

            .ListViewItemSorter = New ListViewItemComparer(0)
            .Sorting = SortOrder.Descending

            DatePick.Value = Now()

        End With

    End Sub

#End Region

#Region "Private functions"

    Private ReadOnly Property LogFolder() As DirectoryInfo
        Get
            Return New DirectoryInfo(
                Path.Combine(
                    _dir.FullName,
                    String.Format(
                        "{0}-{1}",
                        DatePart(DateInterval.Year, DatePick.Value).ToString("D4"),
                        DatePart(DateInterval.Month, DatePick.Value).ToString("D2")
                    ),
                    DatePart(DateInterval.Day, DatePick.Value).ToString("D2")
                )
            )
        End Get
    End Property

    Private Function ldItem(ByRef ld As oMsgLoading) As ListViewItem
        Dim lvi As New ListViewItem
        With lvi
            .Text = ld.tzTime ' .LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss")  
            If IsNothing(ld.CurrentAttempt) Then
                .ImageIndex = 3
                .StateImageIndex = 3

            ElseIf Not ld.CurrentAttempt.ErrCode >= 200 Then
                .ImageIndex = 2
                .StateImageIndex = 2

            Else
                Select Case ld.CurrentAttempt.Warnings.Count
                    Case 0
                        .ImageIndex = 0
                        .StateImageIndex = 0
                    Case Else
                        .ImageIndex = 1
                        .StateImageIndex = 1
                End Select
            End If

            .SubItems.Add(ld.BubbleID)
            .SubItems.Add(ld.Source)
            .SubItems.Add(ld.LoadAttempts.Count)

            .Name = Path.Combine(LogFolder.FullName, ld.FileName)

        End With
        Return lvi
    End Function

#End Region

#Region "Refresh data"

    Public Sub RefreshData(sender As Object, e As EventArgs) ' Handles RefreshToolStripMenuItem.Click

        'If init Then
        If IsNothing(thd) Then

            thd = New Threading.Thread(AddressOf thd_Load)
            thd.Start()

        ElseIf Not (thd.IsAlive) Then
            thd = New Threading.Thread(AddressOf thd_Load)
            thd.Start()

        End If
        'End If

    End Sub

    Private Sub thd_Load()

        Dim d As New DelegateAddEntry(AddressOf AddEntry)
        Dim lv As New List(Of ListViewItem)

        With LogFolder
            If .Exists Then
                For Each f As FileInfo In .GetFiles("*.xml")

                    Using ld As New oMsgLoading(f)
                        lv.Add(ldItem(ld))
                    End Using

                    If lv.Count > 25 Then
                        Do While lv.Count > 0
                            Try
                                Me.Invoke(d, lv)
                                lv.Clear()
                            Catch
                                Threading.Thread.Sleep(1)
                            End Try
                        Loop

                    End If

                Next

                Do While lv.Count > 0
                    Try
                        Me.Invoke(d, lv)
                        lv.Clear()
                    Catch
                        Threading.Thread.Sleep(1)
                    End Try
                Loop

            End If

        End With

    End Sub

    Private Delegate Sub DelegateAddEntry(ByRef e As List(Of ListViewItem))
    Private Sub AddEntry(ByRef e As List(Of ListViewItem))
        For Each lvi As ListViewItem In e
            Me.errList.Items.Insert(0, lvi)
        Next
        errList.Sort()
    End Sub

#End Region

#Region "Control Handlers"

    Private Sub DatePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DatePick.ValueChanged
        With fsw
            errList.Items.Clear()
            If LogFolder.Exists Then
                RefreshData(Me, New EventArgs)
                .Path = LogFolder.FullName
                .EnableRaisingEvents = True
            Else
                .EnableRaisingEvents = False
            End If
        End With
    End Sub

    Private Sub fsw_Changed(sender As Object, e As FileSystemEventArgs) Handles fsw.Changed, fsw.Created, fsw.Deleted, fsw.Renamed
        With errList
            Select Case e.ChangeType
                Case WatcherChangeTypes.Created
                    .Items.Insert(0, ldItem(New oMsgLoading(New FileInfo(e.FullPath))))

                Case WatcherChangeTypes.Deleted
                    .Items(e.FullPath).Remove()

                Case WatcherChangeTypes.Changed
                    .Items(e.FullPath).Remove()
                    .Items.Insert(0, ldItem(New oMsgLoading(New FileInfo(e.FullPath))))
                    .Sort()

            End Select
        End With
    End Sub

    Private Sub errList_DoubleClick(sender As Object, e As EventArgs) Handles errList.DoubleClick
        With errList
            If .SelectedItems.Count > 0 Then

                Dim ld As New oMsgLoading(New FileInfo(.SelectedItems(0).Name))
                With ld
                    .NewAttempt(New PriorityUser("loaduser1", ""))
                    With .CurrentAttempt
                        .ErrCode = eAttemptError.Ok
                        .AddWarning("Test")
                    End With
                    .toFile(LogFolder)
                End With

                Dim _fm As New BubbleDetail(New FileInfo(.SelectedItems(0).Name))
                With Me.Controls
                    .Add(_fm)
                    With .Item(.Count - 1)
                        .Dock = DockStyle.Fill
                        .Visible = True
                        .BringToFront()
                        .Focus()
                    End With
                End With

            End If
        End With
    End Sub

    Public Overloads Overrides Sub Refresh()

        Dim i As New List(Of Integer)
        For c As Integer = Controls.Count - 1 To 0 Step -1
            If Not IsNothing(TryCast(Controls.Item(c), BubbleDetail)) Then
                i.Add(c)
            End If
        Next
        For Each c As Integer In i
            Controls.RemoveAt(c)
        Next

        With SplitContainer3
            .Dock = DockStyle.Fill
            .Visible = True
            .BringToFront()
            .Focus()
        End With

        MyBase.Refresh()

    End Sub

#End Region

End Class
