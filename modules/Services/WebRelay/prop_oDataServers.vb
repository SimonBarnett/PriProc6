Imports System.ComponentModel
Imports System.Xml

Public Class Prop_oDataServers : Inherits System.ComponentModel.StringConverter

    Public Overloads Overrides Function GetStandardValuesSupported(
    ByVal context As ITypeDescriptorContext) As Boolean

        Return True 'True tells the propertygrid to display a combobox

    End Function

    Public Overloads Overrides Function GetStandardValues(
        ByVal context As System.ComponentModel.ITypeDescriptorContext
    ) As System.ComponentModel.TypeConverter.StandardValuesCollection

        Dim strItems As New List(Of String)
        strItems.Add(" ")
        For Each oDataServer As XmlNode In oDataServers
            strItems.Add(oDataServer.Attributes("hostname").Value)
        Next
        Return New StandardValuesCollection(strItems)

    End Function

    Public Overloads Overrides Function _
    GetStandardValuesExclusive(ByVal context _
    As System.ComponentModel.ITypeDescriptorContext) _
    As Boolean

        Return True

    End Function

End Class
