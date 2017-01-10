Imports System.Xml

Public Interface msgInterface

    ReadOnly Property verb As eVerb
    Property msgType As String
    Property ErrCde As Integer
    Property errMsg As String

    Function toByte(ByRef o As objBase) As Byte()

    'Sub fromObject(ByRef msg As msgBase, ByRef ob As objBase)
    Sub readXML(ByRef msg As msgBase)

    Sub writeXML(ByRef msg As msgBase, ByRef outputStream As XmlWriter)
    Sub writeErr(ByRef msg As msgBase, ByRef outputStream As XmlWriter)

End Interface

Public Interface msgInterfaceData

    ReadOnly Property msgType As String
    ReadOnly Property verb As eVerb

End Interface
