Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message

Public Class oSubBroadcast : Inherits oSubscriber

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oSubBroadcast)
            MyBase.Update(Service)
            _broadcastport = .broadcastport
        End With
    End Sub

#End Region

#Region "Properties"

    Private _broadcastport As Integer
    <CategoryAttribute("UDP Broadcast"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The port on which Broadcast messages are sent to the MMC.")>
    Public Property broadcastport As Integer
        Get
            Return _broadcastport
        End Get
        Set(value As Integer)
            _broadcastport = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("broadcastport", _broadcastport)
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

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
                this = .Nodes.Add(TreeTag, Name, IconList("broadcast"), IconList("broadcast"))
            Else
                If IsTimedOut Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If


        End With
    End Sub

#End Region

#Region "Control Panel"

    Public Overrides Function useCpl(ByRef pnlName As String, ParamArray args() As String) As Object
        pnlName = "subudp"
        Return Me
    End Function

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ParamArray args() As String)
        e.Cancel = True
    End Sub

#End Region

End Class