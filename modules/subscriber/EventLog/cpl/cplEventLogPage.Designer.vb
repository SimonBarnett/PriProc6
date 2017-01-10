Imports PriPROC6.Interface.Cpl

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class cplEventLogPage
    Inherits BaseCpl 'System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(cplEventLogPage))
        Me.Tabs = New System.Windows.Forms.TabControl()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.DatePick = New System.Windows.Forms.DateTimePicker()
        Me.errList = New System.Windows.Forms.ListView()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.errDetail = New System.Windows.Forms.TextBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.PropertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.EventLog = New System.Diagnostics.EventLog()
        Me.Tabs.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.EventLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Tabs
        '
        Me.Tabs.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Tabs.Controls.Add(Me.TabPage1)
        Me.Tabs.Controls.Add(Me.TabPage2)
        Me.Tabs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Tabs.Location = New System.Drawing.Point(0, 0)
        Me.Tabs.Name = "Tabs"
        Me.Tabs.SelectedIndex = 0
        Me.Tabs.Size = New System.Drawing.Size(150, 150)
        Me.Tabs.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RefreshToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(133, 26)
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(142, 124)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Log"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.errDetail)
        Me.SplitContainer1.Size = New System.Drawing.Size(136, 118)
        Me.SplitContainer1.SplitterDistance = 59
        Me.SplitContainer1.TabIndex = 1
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.DatePick)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.errList)
        Me.SplitContainer2.Size = New System.Drawing.Size(136, 59)
        Me.SplitContainer2.SplitterDistance = 25
        Me.SplitContainer2.TabIndex = 0
        '
        'DatePick
        '
        Me.DatePick.ContextMenuStrip = Me.ContextMenuStrip1
        Me.DatePick.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DatePick.Location = New System.Drawing.Point(0, 0)
        Me.DatePick.Name = "DatePick"
        Me.DatePick.Size = New System.Drawing.Size(136, 20)
        Me.DatePick.TabIndex = 0
        '
        'errList
        '
        Me.errList.ContextMenuStrip = Me.ContextMenuStrip1
        Me.errList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.errList.FullRowSelect = True
        Me.errList.Location = New System.Drawing.Point(0, 0)
        Me.errList.MultiSelect = False
        Me.errList.Name = "errList"
        Me.errList.Size = New System.Drawing.Size(136, 30)
        Me.errList.SmallImageList = Me.ImageList1
        Me.errList.TabIndex = 0
        Me.errList.UseCompatibleStateImageBehavior = False
        Me.errList.View = System.Windows.Forms.View.Details
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "dialog-information-3.ico")
        Me.ImageList1.Images.SetKeyName(1, "dialog-warning-panel.ico")
        Me.ImageList1.Images.SetKeyName(2, "dialog-error-5.ico")
        '
        'errDetail
        '
        Me.errDetail.BackColor = System.Drawing.SystemColors.InactiveBorder
        Me.errDetail.ContextMenuStrip = Me.ContextMenuStrip1
        Me.errDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.errDetail.Location = New System.Drawing.Point(0, 0)
        Me.errDetail.Multiline = True
        Me.errDetail.Name = "errDetail"
        Me.errDetail.ReadOnly = True
        Me.errDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.errDetail.Size = New System.Drawing.Size(136, 55)
        Me.errDetail.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.PropertyGrid)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(142, 124)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Settings"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'PropertyGrid
        '
        Me.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid.Location = New System.Drawing.Point(3, 3)
        Me.PropertyGrid.Name = "PropertyGrid"
        Me.PropertyGrid.Size = New System.Drawing.Size(136, 118)
        Me.PropertyGrid.TabIndex = 0
        '
        'EventLog
        '
        Me.EventLog.SynchronizingObject = Me
        '
        'cplEventLogPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Tabs)
        Me.Name = "cplEventLogPage"
        Me.Tabs.ResumeLayout(False)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        CType(Me.EventLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Tabs As Windows.Forms.TabControl
    Friend WithEvents TabPage1 As Windows.Forms.TabPage
    Friend WithEvents TabPage2 As Windows.Forms.TabPage
    Friend WithEvents PropertyGrid As Windows.Forms.PropertyGrid
    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As Windows.Forms.SplitContainer
    Friend WithEvents DatePick As Windows.Forms.DateTimePicker
    Friend WithEvents errList As Windows.Forms.ListView
    Friend WithEvents ImageList1 As Windows.Forms.ImageList
    Friend WithEvents errDetail As Windows.Forms.TextBox
    Public WithEvents EventLog As EventLog
    Friend WithEvents ContextMenuStrip1 As Windows.Forms.ContextMenuStrip
    Friend WithEvents RefreshToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class
