Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms

Public Class cplLoader : Inherits BaseCpl

    Friend WithEvents PropertyGrid As PropertyGrid

    Sub New(ByRef o As Object)

        ' This call is required by the designer.
        InitializeComponent()
        Obj = o
        PropertyGrid.SelectedObject = Obj

    End Sub

    Private Sub InitializeComponent()
        Me.PropertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.SuspendLayout()
        '
        'PropertyGrid
        '
        Me.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid.Name = "PropertyGrid"
        Me.PropertyGrid.Size = New System.Drawing.Size(150, 150)
        Me.PropertyGrid.TabIndex = 0
        '
        'cplLoader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.Controls.Add(Me.PropertyGrid)
        Me.Name = "cplLoader"
        Me.ResumeLayout(False)

    End Sub
End Class
