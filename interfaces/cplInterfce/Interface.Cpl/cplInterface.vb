Public Interface cplInterface
    Property Name As String
    Function useCpl(ByRef o As Object, ParamArray args() As String) As Object

End Interface

Public Interface cplProps
    ReadOnly Property Name As String

End Interface