Imports System.Xml
Imports PriPROC6.regConfig

Public Enum eServType
    Subscriber
    Service
End Enum

Public Enum eServiceState
    stopped
    starting
    started
    stopping
    notresponding
End Enum

Public MustInherit Class WritableXML

    Public MustOverride Property Name As String
    Public MustOverride ReadOnly Property Tag As String
    Public MustOverride ReadOnly Property SvcType As eservType
    Public MustOverride Function svc_start(Optional ByRef Log As objBase = Nothing) As objBase
    Public MustOverride Function svc_stop(Optional ByRef Log As objBase = Nothing) As objBase
    Public MustOverride Property svc_state As eServiceState
    Public MustOverride Sub toXML(ByRef outputStream As XmlWriter)
    Public MustOverride Property thisConfig() As svcConfig
    Public MustOverride Property myProperties(log As Object, ParamArray Name() As String) As String
    Public MustOverride Function readXML(ByRef Service As XmlNode) As oServiceBase

End Class
