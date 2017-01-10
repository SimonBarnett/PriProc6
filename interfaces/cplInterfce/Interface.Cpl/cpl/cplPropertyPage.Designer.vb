<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cplPropertyPage
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
        Me.PropertyPage = New System.Windows.Forms.PropertyGrid()
        Me.SuspendLayout()
        '
        'PropertyPage
        '
        Me.PropertyPage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyPage.Location = New System.Drawing.Point(0, 0)
        Me.PropertyPage.Name = "PropertyPage"
        Me.PropertyPage.Size = New System.Drawing.Size(150, 150)
        Me.PropertyPage.TabIndex = 0
        '
        'cplPropertyPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PropertyPage)
        Me.Name = "cplPropertyPage"
        Me.ResumeLayout(False)

    End Sub

    Public WithEvents PropertyPage As Windows.Forms.PropertyGrid
End Class
