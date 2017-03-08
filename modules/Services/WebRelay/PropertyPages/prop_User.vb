Imports System.ComponentModel
Imports System.Xml

Public Class prop_User : Inherits System.ComponentModel.StringConverter

        Public Overloads Overrides Function GetStandardValuesSupported(
    ByVal context As ITypeDescriptorContext) As Boolean

            Return True 'True tells the propertygrid to display a combobox

        End Function

        Public Overloads Overrides Function GetStandardValues(
        ByVal context As System.ComponentModel.ITypeDescriptorContext
    ) As System.ComponentModel.TypeConverter.StandardValuesCollection

        Dim strItems As New List(Of String)
        For Each oDataServer As XmlNode In oDataServers
                If String.Compare(
                oDataServer.Attributes("hostname").Value,
                TryCast(context.Instance, PriWeb).Service,
                True) = 0 Then

                For Each e As XmlNode In oDataServer.SelectNodes("user")
                    strItems.Add(e.Attributes("name").Value)
                Next

            End If
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

