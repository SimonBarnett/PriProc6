Imports System.ComponentModel

Public Class Prop_Timeout : Inherits System.ComponentModel.Int32Converter

    Private _tival As Integer() = New Integer() {60, 120, 180, 240, 300}

    Public Overloads Overrides Function GetStandardValuesSupported(
    ByVal context As ITypeDescriptorContext) As Boolean

        Return True 'True tells the propertygrid to display a combobox

    End Function

    Public Overloads Overrides Function GetStandardValues(
        ByVal context As System.ComponentModel.ITypeDescriptorContext
    ) As System.ComponentModel.TypeConverter.StandardValuesCollection

        Return New StandardValuesCollection(_tival)

    End Function

    Public Overloads Overrides Function _
    GetStandardValuesExclusive(ByVal context _
    As System.ComponentModel.ITypeDescriptorContext) _
    As Boolean

        Return True

    End Function
End Class
