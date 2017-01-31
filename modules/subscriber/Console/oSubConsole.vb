Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

Public Class oSubConsole : Inherits oSubscriber

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oSubConsole)
            MyBase.Update(Service)
        End With
    End Sub

#End Region

#Region "Tree"

    <Browsable(False)>
    Public Overrides ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}\{1}", Host, Name)
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Name, IconList("console"), IconList("console"))
            Else
                If IsTimedOut Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

        End With
    End Sub

#End Region

#Region "Control panel"

    Public Overrides Function useCpl(ByRef pnlName As String, ParamArray args() As String) As Object
        pnlName = "subconsole"
        Return Me
    End Function

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ParamArray args() As String)
        e.Cancel = True
    End Sub

#End Region

End Class