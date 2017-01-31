Imports System.ComponentModel
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports PriPROC6.Services.Loader

Public Class Prop_Env : Inherits System.ComponentModel.StringConverter

    Public Overloads Overrides Function GetStandardValuesSupported(
    ByVal context As ITypeDescriptorContext) As Boolean

        Return True 'True tells the propertygrid to display a combobox

    End Function

    Public Overloads Overrides Function GetStandardValues(
        ByVal context As System.ComponentModel.ITypeDescriptorContext
    ) As System.ComponentModel.TypeConverter.StandardValuesCollection

        Dim strItems As New List(Of String)
        'For Each discovery As oDiscovery In propSvcMap.svcMap.Values
        '    For Each svc As oServiceBase In discovery.values
        '        If Not IsNothing(TryCast(svc, oLoader)) Then
        '            With TryCast(svc, oLoader)
        '                For Each env As PriEnv In .values
        '                    strItems.Add(String.Format("{0}\{1}", .Host, env.Name))
        '                Next
        '            End With
        '        End If
        '    Next
        'Next
        Return New StandardValuesCollection(strItems)

    End Function

    Public Overloads Overrides Function _
    GetStandardValuesExclusive(ByVal context _
    As System.ComponentModel.ITypeDescriptorContext) _
    As Boolean

        Return True

    End Function
End Class
