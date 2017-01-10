Imports System.Xml
Imports PriPROC6.regConfig
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Base
Imports System.Drawing

Public Interface SubscribeDef : Inherits svcDefintion
    Sub setParent(
        ByRef msgFactory As msgFactory,
        ByRef logQ As Queue(Of Byte())
    )

    Property EntryType As Integer
    Property Verbosity As Integer
    Property Source As Integer
    Property Console As Boolean

    Property LogQ As Queue(Of Byte())
    Sub NewEntry(Entry As msgBase)
    Sub NewMessage(data As Byte())

End Interface

Public Interface SubscribeDefprops : Inherits svcProps
    ReadOnly Property EntryType As Integer
    ReadOnly Property Verbosity As Integer
    ReadOnly Property Source As Integer
    ReadOnly Property Console As Boolean

End Interface