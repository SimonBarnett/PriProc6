
Imports System.ComponentModel
Imports System.Xml
Imports PriPROC6.svcMessage

Public Class prop_LogServer : Inherits System.ComponentModel.StringConverter

    Public Overloads Overrides Function GetStandardValuesSupported(
        ByVal context As ITypeDescriptorContext) As Boolean

        Return True 'True tells the propertygrid to display a combobox

    End Function

    Public Overloads Overrides Function GetStandardValues(
        ByVal context As System.ComponentModel.ITypeDescriptorContext
    ) As System.ComponentModel.TypeConverter.StandardValuesCollection

        Dim strItems As New List(Of String)
        strItems.Add(" ")
        For Each d As oDiscovery In share.SvcMap.Values
            strItems.Add(String.Format("{0}", d.Host, d.Port))
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
