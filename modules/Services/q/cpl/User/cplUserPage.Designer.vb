Imports PriPROC6.Interface.Cpl

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class cplUserPage
    Inherits BaseCpl 'System.Windows.Forms.UserControl

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
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("test", "userconfig.ico")
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(cplUserPage))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.lst_Users = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btn_Pass = New System.Windows.Forms.LinkLabel()
        Me.ImageList2 = New System.Windows.Forms.ImageList(Me.components)
        Me.btn_delete = New System.Windows.Forms.LinkLabel()
        Me.btn_add = New System.Windows.Forms.LinkLabel()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lst_Users)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.White
        Me.SplitContainer1.Panel2.Controls.Add(Me.btn_Pass)
        Me.SplitContainer1.Panel2.Controls.Add(Me.btn_delete)
        Me.SplitContainer1.Panel2.Controls.Add(Me.btn_add)
        Me.SplitContainer1.Size = New System.Drawing.Size(254, 170)
        Me.SplitContainer1.SplitterDistance = 96
        Me.SplitContainer1.TabIndex = 0
        '
        'lst_Users
        '
        Me.lst_Users.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.lst_Users.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lst_Users.FullRowSelect = True
        Me.lst_Users.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1})
        Me.lst_Users.LargeImageList = Me.ImageList1
        Me.lst_Users.Location = New System.Drawing.Point(0, 0)
        Me.lst_Users.MultiSelect = False
        Me.lst_Users.Name = "lst_Users"
        Me.lst_Users.Size = New System.Drawing.Size(96, 170)
        Me.lst_Users.SmallImageList = Me.ImageList1
        Me.lst_Users.TabIndex = 0
        Me.lst_Users.UseCompatibleStateImageBehavior = False
        Me.lst_Users.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Name"
        Me.ColumnHeader1.Width = 80
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "userconfig.ico")
        Me.ImageList1.Images.SetKeyName(1, "user-new.ico")
        Me.ImageList1.Images.SetKeyName(2, "user-delete.ico")
        Me.ImageList1.Images.SetKeyName(3, "password.ico")
        '
        'btn_Pass
        '
        Me.btn_Pass.AutoSize = True
        Me.btn_Pass.Dock = System.Windows.Forms.DockStyle.Top
        Me.btn_Pass.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_Pass.ImageAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.btn_Pass.ImageIndex = 2
        Me.btn_Pass.ImageList = Me.ImageList2
        Me.btn_Pass.Location = New System.Drawing.Point(0, 78)
        Me.btn_Pass.Margin = New System.Windows.Forms.Padding(0)
        Me.btn_Pass.Name = "btn_Pass"
        Me.btn_Pass.Padding = New System.Windows.Forms.Padding(35, 12, 3, 12)
        Me.btn_Pass.Size = New System.Drawing.Size(145, 39)
        Me.btn_Pass.TabIndex = 2
        Me.btn_Pass.TabStop = True
        Me.btn_Pass.Tag = "chpass"
        Me.btn_Pass.Text = "Change Password"
        Me.btn_Pass.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.btn_Pass.Visible = False
        '
        'ImageList2
        '
        Me.ImageList2.ImageStream = CType(resources.GetObject("ImageList2.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList2.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList2.Images.SetKeyName(0, "user-new.ico")
        Me.ImageList2.Images.SetKeyName(1, "user-delete.ico")
        Me.ImageList2.Images.SetKeyName(2, "password.ico")
        '
        'btn_delete
        '
        Me.btn_delete.AutoSize = True
        Me.btn_delete.Dock = System.Windows.Forms.DockStyle.Top
        Me.btn_delete.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_delete.ImageAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.btn_delete.ImageIndex = 1
        Me.btn_delete.ImageList = Me.ImageList2
        Me.btn_delete.Location = New System.Drawing.Point(0, 39)
        Me.btn_delete.Margin = New System.Windows.Forms.Padding(0)
        Me.btn_delete.Name = "btn_delete"
        Me.btn_delete.Padding = New System.Windows.Forms.Padding(35, 12, 3, 12)
        Me.btn_delete.Size = New System.Drawing.Size(110, 39)
        Me.btn_delete.TabIndex = 1
        Me.btn_delete.TabStop = True
        Me.btn_delete.Tag = "delete"
        Me.btn_delete.Text = "Delete User"
        Me.btn_delete.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.btn_delete.Visible = False
        '
        'btn_add
        '
        Me.btn_add.AutoSize = True
        Me.btn_add.Dock = System.Windows.Forms.DockStyle.Top
        Me.btn_add.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_add.ImageAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.btn_add.ImageIndex = 0
        Me.btn_add.ImageList = Me.ImageList2
        Me.btn_add.Location = New System.Drawing.Point(0, 0)
        Me.btn_add.Margin = New System.Windows.Forms.Padding(0)
        Me.btn_add.Name = "btn_add"
        Me.btn_add.Padding = New System.Windows.Forms.Padding(35, 12, 3, 12)
        Me.btn_add.Size = New System.Drawing.Size(95, 39)
        Me.btn_add.TabIndex = 0
        Me.btn_add.TabStop = True
        Me.btn_add.Tag = "add"
        Me.btn_add.Text = "Add User"
        Me.btn_add.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 500
        '
        'cplUserPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "cplUserPage"
        Me.Size = New System.Drawing.Size(254, 170)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Friend WithEvents lst_Users As Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As Windows.Forms.ColumnHeader
    Friend WithEvents ImageList1 As Windows.Forms.ImageList
    Friend WithEvents btn_add As Windows.Forms.LinkLabel
    Friend WithEvents btn_delete As Windows.Forms.LinkLabel
    Friend WithEvents ImageList2 As Windows.Forms.ImageList
    Friend WithEvents btn_Pass As Windows.Forms.LinkLabel
    Friend WithEvents Timer1 As Windows.Forms.Timer
End Class
