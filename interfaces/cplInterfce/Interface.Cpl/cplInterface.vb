Imports System.Windows.Forms

Public Interface cplInterface
    Property Name As String
    Sub LoadObject(ByRef o As Object)
    ReadOnly Property Cpl As UserControl

End Interface

Public Interface cplProps
    ReadOnly Property Name As String

End Interface