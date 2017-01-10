Imports System.Windows.Forms
Imports PriPROC6.Interface.Cpl
Imports PriPROC6.svcMessage

Public Class cplEventLogPage : Inherits BaseCpl

    Private thd As Threading.Thread
    Private lastEntry As Integer = 0
    Public init As Boolean = False

    Private _Server As String
    Sub New(ByRef o As Object)

        ' This call is required by the designer.
        InitializeComponent()
        Obj = o
        PropertyGrid.SelectedObject = Obj
        _Server = TryCast(Obj, oSubEventLog).Host

        ' Add any initialization after the InitializeComponent() call.
        DatePick.Value = Now()
        With errList
            .Clear()
            With .Columns
                .Clear()
                .Add("Level")
                .Add("Category")
                .Add("Date and time")
                .Item(0).Width = 150
                .Item(1).Width = 80
                .Item(2).Width = 150
            End With
        End With
        RefreshData(Me, Nothing)

    End Sub

    Private ReadOnly Property EventItem(e As EventLogEntry) As ListViewItem
        Get
            Dim lvi As New ListViewItem
            With lvi
                .Name = e.Index 'Guid.NewGuid.ToString

                Select Case e.EntryType
                    Case EventLogEntryType.Information
                        .Text = "Information"
                        .ImageIndex = 0
                        .StateImageIndex = 0

                    Case EventLogEntryType.SuccessAudit
                        .Text = "SuccessAudit"
                        .ImageIndex = 0
                        .StateImageIndex = 0

                    Case EventLogEntryType.FailureAudit
                        .Text = "FailureAudit"
                        .ImageIndex = 2
                        .StateImageIndex = 2

                    Case EventLogEntryType.Warning
                        .Text = "Warning"
                        .ImageIndex = 1
                        .StateImageIndex = 1

                    Case EventLogEntryType.Error
                        .Text = "Error"
                        .ImageIndex = 2
                        .StateImageIndex = 2

                End Select

                .SubItems.Add(e.Category)
                .SubItems.Add(e.TimeGenerated.ToString("dd/MM/yyyy HH:mm:ss"))
                .Tag = e.Message

            End With
            Return lvi

        End Get
    End Property

    Private ReadOnly Property StartDate As Integer
        Get
            Return DateDiff(
                DateInterval.Minute,
                New Date(
                    1988,
                    1,
                    1
                ),
                New Date(
                    DatePart(
                        DateInterval.Year,
                        DatePick.Value
                    ),
                    DatePart(
                        DateInterval.Month,
                        DatePick.Value
                    ),
                    DatePart(
                        DateInterval.Day,
                        DatePick.Value
                    )
                )
            )
        End Get
    End Property

    Private loadedid As New List(Of Integer)
    Private Sub thd_Load()

        Dim d As New DelegateAddEntry(AddressOf AddEntry)
        Dim lv As New List(Of ListViewItem)

        With EventLog
            .MachineName = _Server
            .Log = SysLogName

            For Each e As EventLogEntry In .Entries
                Dim eDate As Integer = DateDiff(DateInterval.Minute, #1/1/1988#, e.TimeGenerated)
                If eDate >= StartDate And eDate <= StartDate + 1440 Then
                    If Not loadedid.Contains(e.Index) Then
                        lv.Add(EventItem(e))
                        loadedid.Add(e.Index)
                    End If
                End If

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

            If lv.Count > 0 Then
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

    Public Sub RefreshData(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click

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

    Private Delegate Sub DelegateAddEntry(ByRef e As List(Of ListViewItem))
    Private Sub AddEntry(ByRef e As List(Of ListViewItem))
        For Each lvi As ListViewItem In e
            Me.errList.Items.Insert(0, lvi)
        Next
    End Sub

    Private Sub DatePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DatePick.ValueChanged
        Clear()
        RefreshData(Me, Nothing)
    End Sub

    Private Sub errList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles errList.SelectedIndexChanged
        If errList.SelectedItems.Count > 0 Then
            errDetail.Text = errList.SelectedItems(0).Tag
        End If
    End Sub

    Public Sub Clear()
        lastEntry = 0
        Me.errList.Items.Clear()
        Me.errDetail.Text = ""
        loadedid.Clear()

        If Not IsNothing(thd) Then
            While thd.IsAlive
                thd.Abort()
            End While
        End If
    End Sub

End Class
