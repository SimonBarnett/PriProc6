Imports Priproc6.svcMessages
Imports Priproc6.PriSock
Imports Priproc6.regConfig

Public Enum eServiceState
    stopped
    starting
    started
    stopping
    notresponding
End Enum

Public Interface svcDef

    Property ServiceName As String
    Property udp As Boolean
    Property tcp As Boolean
    Property defaultPort As Integer
    ReadOnly Property BroadcastPort As Integer

    Sub setParent(ByRef p As svc_hanger)
    Sub svc_info(ByRef wr As System.Xml.XmlWriter)
    ReadOnly Property Start As Boolean
    Function svc_start() As Byte()
    Function svc_stop() As Byte()
    Property svc_state() As eServiceState
    ReadOnly Property Parent As svc_hanger
    ReadOnly Property Cpl As System.Windows.Forms.UserControl
    ReadOnly Property msgFactory As msgFactory
    Sub newMessage(ByVal sender As Object, ByVal e As byteMsg)
    Property thisConfig() As svcConfig

End Interface

Public Interface svcDefprops

    ReadOnly Property serviceName As String
    ReadOnly Property udp As Boolean
    ReadOnly Property tcp As Boolean
    ReadOnly Property defaultPort As Integer

End Interface