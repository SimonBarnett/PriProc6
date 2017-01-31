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

    Public MustOverride Function useCpl(ByRef o As Object, ParamArray args() As String) As Object Implements cplInterface.useCpl

End Class
