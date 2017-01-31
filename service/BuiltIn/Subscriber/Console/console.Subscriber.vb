Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports System.Xml
Imports System.Drawing
Imports PriPROC6.Interface.Cpl
Imports System.ComponentModel
Imports System.Windows.Forms

<Export(GetType(SubscribeDef))>
<ExportMetadata("Name", "console")>
<ExportMetadata("defaultStart", True)>
<ExportMetadata("Console", True)>
<ExportMetadata("EntryType", 31)>
<ExportMetadata("Verbosity", 99)>
<ExportMetadata("Source", 7)>
Public Class consoleSubscriber : Inherits SubscriberBase

    Public Overrides Sub NewEntry(Entry As msgBase)
        Try
            With Entry
                TryCast(.thisObject, oMsgLog).toConsole()

            End With

        Catch ex As Exception

        End Try

    End Sub

#Region "Discovery Message"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)

    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oSubConsole(Service)
        With ret

        End With
        Return ret
    End Function

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property ModuleVersion As Version
        Get
            Return Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Return New cplPropertyPage(TryCast(o, oSubConsole))

    End Function

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ByRef p As oServiceBase, ParamArray args() As String)
        e.Cancel = True
    End Sub

#Region "Tree"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Name, My.Resources.subcmd)
            Return ret
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef MEF As Object, ByVal p As oServiceBase, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag(p))
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag(p), Name, IconList(Name), IconList(Name))
            Else
                If p.IsTimedOut Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

        End With
    End Sub

#End Region

#End Region

End Class
