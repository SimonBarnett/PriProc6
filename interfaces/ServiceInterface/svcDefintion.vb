Imports System.Drawing
Imports System.Xml
Imports PriPROC6.Interface.Message
Imports PriPROC6.regConfig

Public Interface svcDefintion

    Property Name As String
    ReadOnly Property Tag As String
    ReadOnly Property ModuleVersion As Version

    Property defaultStart As Boolean
    ReadOnly Property Start As Boolean

    Property thisConfig() As svcConfig

    Sub writeXML(ByRef outputStream As XmlWriter)
    Function Icon() As Dictionary(Of String, Icon)

    Function svc_start(Optional ByRef Log As objBase = Nothing) As objBase
    Function svc_stop(Optional ByRef Log As objBase = Nothing) As objBase
    Property svc_state() As eServiceState

    ReadOnly Property msgFactory As msgFactory

End Interface
