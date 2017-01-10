Imports Priproc6.svcMessages

Public Interface svc_hanger

    Sub setParent(ByRef p As CompositionHost)
    Function svc_start() As Exception
    Function svc_stop() As Exception
    Property logq As Queue(Of Byte())
    ReadOnly Property msgFactory As msgFactory
    ReadOnly Property LoadedModules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))
    ReadOnly Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))
    ReadOnly Property BroadcastPort As Integer

End Interface
