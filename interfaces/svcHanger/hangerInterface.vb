Imports Priproc6.Interface.Message
Imports Priproc6.Interface.Service
Imports Priproc6.Interface.Subsciber

Public Interface svc_hanger

    Sub setParent(
        ByRef Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops)),
        ByRef Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData)),
        ByRef Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))
    )
    Function svc_start() As Exception
    Function svc_stop() As Exception
    Property logq As Queue(Of Byte())
    ReadOnly Property msgFactory As msgFactory
    ReadOnly Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))
    ReadOnly Property Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))
    ReadOnly Property Count() As Integer

End Interface
