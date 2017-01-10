Imports System.Windows.Forms
Imports PriPROC6.Interface.Cpl

Public MustInherit Class cplBase
    Implements cplInterface

    Private _name As String
    Public Property Name As String Implements cplInterface.Name
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    Private _thisPanel As UserControl
    Public Property thisPanel As UserControl
        Get
            Return _thisPanel
        End Get
        Set(value As UserControl)
            _thisPanel = value
        End Set
    End Property

    Public MustOverride Sub LoadObject(ByRef o As Object) Implements cplInterface.LoadObject

    Public ReadOnly Property Cpl As System.Windows.Forms.UserControl Implements cplInterface.Cpl
        Get
            Return _thisPanel
        End Get
    End Property

End Class
