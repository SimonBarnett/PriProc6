Imports System.Drawing
Imports System.Xml
Imports PriPROC6.Interface.Message
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Base
Imports PriPROC6.regConfig
Imports PriPROC6.svcMessage

Public Interface svcDef : Inherits svcDefintion
    Sub setParent(ByRef ServiceHost As Object, ByVal Props As svcDefprops)

    Property defaultPort As Integer
    ReadOnly Property Port As Integer
    Property udp As Boolean

    Function svc_info() As svcbase
    Function tcpByte(ByVal e As Byte(), RemoteEndpoint As String) As Byte()

    Sub udpByte(ByVal e As Byte(), RemoteEndpoint As String)
    Sub Config(ByRef Svc As List(Of Object), ByRef Log As oMsgLog)

End Interface

Public Interface svcDefprops : Inherits svcProps
    ReadOnly Property udp As Boolean
    ReadOnly Property defaultPort As Integer

End Interface