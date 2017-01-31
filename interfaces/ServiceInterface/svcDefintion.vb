Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports PriPROC6.Interface.Message
Imports PriPROC6.regConfig

Public Interface svcDefintion : Inherits Cpl.cplInterface

    'Property Name As String
    ReadOnly Property Tag As String
    ReadOnly Property ModuleVersion As Version

    Property defaultStart As Boolean
    ReadOnly Property Start As Boolean

    Property thisConfig() As svcConfig

    Sub writeXML(ByRef outputStream As XmlWriter)

    Function svc_start(Optional ByRef Log As objBase = Nothing) As objBase
    Function svc_stop(Optional ByRef Log As objBase = Nothing) As objBase
    Property svc_state() As eServiceState

    ReadOnly Property msgFactory As msgFactory

#Region "Control Panel"

    Function Icon() As Dictionary(Of String, Icon)
    Sub DrawTree(ByRef Parent As TreeNode, ByRef MEF As Object, ByVal p As oServiceBase, ByRef IconList As Dictionary(Of String, Integer))
    Sub ContextMenu(ByRef sender As Object, ByRef e As System.ComponentModel.CancelEventArgs, ByRef p As oServiceBase, ParamArray args() As String)
    ReadOnly Property TreeTag(p As oServiceBase) As String

#End Region

End Interface
