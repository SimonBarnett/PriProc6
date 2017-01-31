Imports Priproc6.Interface.Message
Imports Priproc6.Interface.Service
Imports Priproc6.Interface.Subsciber

Public Interface svc_hanger

    Function svc_start() As Exception
    Function svc_stop() As Exception
    Property logq As Queue(Of Byte())
    ReadOnly Property msgFactory As msgFactory
    ReadOnly Property Modules As Dictionary(Of String, svcDef) 'IEnumerable(Of Lazy(Of svcDef, svcDefprops))
    ReadOnly Property Subscribers As Dictionary(Of String, SubscribeDef) 'IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))
    ReadOnly Property Count() As Integer

End Interface
