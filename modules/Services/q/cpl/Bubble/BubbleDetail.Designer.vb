<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BubbleDetail
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BubbleDetail))
        Me.TabControl = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.PropertyPage = New System.Windows.Forms.PropertyGrid()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.DataGrid = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.historyList = New System.Windows.Forms.ListView()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.WebBrowser = New System.Windows.Forms.WebBrowser()
        Me.lstWarnings = New System.Windows.Forms.ListView()
        Me.TabControl.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.DataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl
        '
        Me.TabControl.Controls.Add(Me.TabPage1)
        Me.TabControl.Controls.Add(Me.TabPage2)
        Me.TabControl.Controls.Add(Me.TabPage3)
        Me.TabControl.Controls.Add(Me.TabPage4)
        Me.TabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl.Location = New System.Drawing.Point(0, 0)
        Me.TabControl.Name = "TabControl"
        Me.TabControl.SelectedIndex = 0
        Me.TabControl.Size = New System.Drawing.Size(150, 150)
        Me.TabControl.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.PropertyPage)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(142, 124)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Loading"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'PropertyPage
        '
        Me.PropertyPage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyPage.Location = New System.Drawing.Point(3, 3)
        Me.PropertyPage.Name = "PropertyPage"
        Me.PropertyPage.Size = New System.Drawing.Size(136, 118)
        Me.PropertyPage.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.DataGrid)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(142, 124)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Data"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.SplitContainer1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(142, 124)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "History"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'DataGrid
        '
        Me.DataGrid.AllowUserToAddRows = False
        Me.DataGrid.AllowUserToDeleteRows = False
        Me.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGrid.Location = New System.Drawing.Point(3, 3)
        Me.DataGrid.Name = "DataGrid"
        Me.DataGrid.ReadOnly = True
        Me.DataGrid.Size = New System.Drawing.Size(136, 118)
        Me.DataGrid.TabIndex = 2
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.historyList)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lstWarnings)
        Me.SplitContainer1.Size = New System.Drawing.Size(142, 124)
        Me.SplitContainer1.SplitterDistance = 47
        Me.SplitContainer1.TabIndex = 0
        '
        'historyList
        '
        Me.historyList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.historyList.FullRowSelect = True
        Me.historyList.LargeImageList = Me.ImageList1
        Me.historyList.Location = New System.Drawing.Point(0, 0)
        Me.historyList.MultiSelect = False
        Me.historyList.Name = "historyList"
        Me.historyList.Size = New System.Drawing.Size(142, 47)
        Me.historyList.SmallImageList = Me.ImageList1
        Me.historyList.TabIndex = 2
        Me.historyList.UseCompatibleStateImageBehavior = False
        Me.historyList.View = System.Windows.Forms.View.Details
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
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.WebBrowser)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(142, 124)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Source"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'WebBrowser
        '
        Me.WebBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebBrowser.Location = New System.Drawing.Point(3, 3)
        Me.WebBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser.Name = "WebBrowser"
        Me.WebBrowser.Size = New System.Drawing.Size(136, 118)
        Me.WebBrowser.TabIndex = 0
        '
        'lstWarnings
        '
        Me.lstWarnings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstWarnings.FullRowSelect = True
        Me.lstWarnings.Location = New System.Drawing.Point(0, 0)
        Me.lstWarnings.MultiSelect = False
        Me.lstWarnings.Name = "lstWarnings"
        Me.lstWarnings.Size = New System.Drawing.Size(142, 73)
        Me.lstWarnings.TabIndex = 0
        Me.lstWarnings.UseCompatibleStateImageBehavior = False
        Me.lstWarnings.View = System.Windows.Forms.View.Details
        '
        'BubbleDetail
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TabControl)
        Me.Name = "BubbleDetail"
        Me.TabControl.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        CType(Me.DataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl As Windows.Forms.TabControl
    Friend WithEvents TabPage1 As Windows.Forms.TabPage
    Friend WithEvents PropertyPage As Windows.Forms.PropertyGrid
    Friend WithEvents TabPage2 As Windows.Forms.TabPage
    Friend WithEvents TabPage3 As Windows.Forms.TabPage
    Public WithEvents DataGrid As Windows.Forms.DataGridView
    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Public WithEvents historyList As Windows.Forms.ListView
    Friend WithEvents ImageList1 As Windows.Forms.ImageList
    Friend WithEvents TabPage4 As Windows.Forms.TabPage
    Friend WithEvents WebBrowser As Windows.Forms.WebBrowser
    Friend WithEvents lstWarnings As Windows.Forms.ListView
End Class
