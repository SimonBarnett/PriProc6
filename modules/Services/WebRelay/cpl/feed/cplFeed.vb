Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms

Public Class cplFeed : Inherits cplBase

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Return New cplFeedControl(o)

    End Function

End Class
