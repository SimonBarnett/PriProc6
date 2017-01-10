Imports PriPROC6.Interface.Cpl

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class cplBubblePage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(cplBubblePage))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.BubbleContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ReloadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.errList = New System.Windows.Forms.ListView()
        Me.DatePick = New System.Windows.Forms.DateTimePicker()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.fsw = New System.IO.FileSystemWatcher()
        Me.BubbleContextMenu.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.fsw, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "dialog-information-3.ico")
        Me.ImageList1.Images.SetKeyName(1, "dialog-warning-panel.ico")
        Me.ImageList1.Images.SetKeyName(2, "dialog-error-5.ico")
        Me.ImageList1.Images.SetKeyName(3, "procedure.ico")
        '
        'BubbleContextMenu
        '
        Me.BubbleContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ReloadToolStripMenuItem})
        Me.BubbleContextMenu.Name = "BubbleContextMenu"
        Me.BubbleContextMenu.Size = New System.Drawing.Size(111, 26)
        '
        'ReloadToolStripMenuItem
        '
        Me.ReloadToolStripMenuItem.Name = "ReloadToolStripMenuItem"
        Me.ReloadToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.ReloadToolStripMenuItem.Text = "Reload"
        '
        'errList
        '
        Me.errList.ContextMenuStrip = Me.BubbleContextMenu
        Me.errList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.errList.FullRowSelect = True
        Me.errList.Location = New System.Drawing.Point(0, 0)
        Me.errList.MultiSelect = False
        Me.errList.Name = "errList"
        Me.errList.Size = New System.Drawing.Size(150, 121)
        Me.errList.SmallImageList = Me.ImageList1
        Me.errList.TabIndex = 1
        Me.errList.UseCompatibleStateImageBehavior = False
        Me.errList.View = System.Windows.Forms.View.Details
        '
        'DatePick
        '
        Me.DatePick.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DatePick.Location = New System.Drawing.Point(0, 0)
        Me.DatePick.Name = "DatePick"
        Me.DatePick.Size = New System.Drawing.Size(150, 20)
        Me.DatePick.TabIndex = 1
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.IsSplitterFixed = True
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.DatePick)
        Me.SplitContainer3.Panel1MinSize = 10
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.errList)
        Me.SplitContainer3.Size = New System.Drawing.Size(150, 150)
        Me.SplitContainer3.SplitterDistance = 25
        Me.SplitContainer3.TabIndex = 3
        '
        'fsw
        '
        Me.fsw.EnableRaisingEvents = True
        Me.fsw.Filter = "*.xml"
        Me.fsw.SynchronizingObject = Me
        '
        'cplBubblePage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer3)
        Me.Name = "cplBubblePage"
        Me.BubbleContextMenu.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.fsw, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ImageList1 As Windows.Forms.ImageList
    Friend WithEvents BubbleContextMenu As Windows.Forms.ContextMenuStrip
    Friend WithEvents ReloadToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents errList As Windows.Forms.ListView
    Friend WithEvents DatePick As Windows.Forms.DateTimePicker
    Friend WithEvents SplitContainer3 As Windows.Forms.SplitContainer
    Friend WithEvents fsw As IO.FileSystemWatcher
End Class
