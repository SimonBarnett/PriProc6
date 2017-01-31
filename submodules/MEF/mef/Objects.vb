Imports System.ComponentModel.Composition

Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Cpl

Public Class MEF

#Region "MEF objects"

    <ImportMany()>
    Public Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))

    <ImportMany()>
    Public Property Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData))

    <ImportMany()>
    Public Property Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))

    <ImportMany()>
    Public Property Cpl As IEnumerable(Of Lazy(Of cplInterface, cplProps))

    Public mefsvc As New Dictionary(Of String, svcDef)
    Public mefsub As New Dictionary(Of String, SubscribeDef)
    Public mefmsg As New Dictionary(Of String, msgInterface)
    Public mefcpl As New Dictionary(Of String, cplInterface)

#End Region

End Class
