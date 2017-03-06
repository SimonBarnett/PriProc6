Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Cpl
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

Public Class cplBubble : Inherits cplBase

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Return New cplBubblePage(o)

    End Function

End Class
