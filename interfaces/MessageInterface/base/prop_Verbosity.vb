Imports System.ComponentModel

Public Class Prop_Verbosity : Inherits System.ComponentModel.StringConverter

    Private _tival As String() = New String() {"Normal", "Verbose", "VeryVerbose", "Arcane"}

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
