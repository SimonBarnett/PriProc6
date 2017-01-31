Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Priproc6.Interface.Base
Imports Priproc6.svcMessage
Imports Priproc6.Interface.Message

Public Class oDiscovery : Inherits oService

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oDiscovery)
            MyBase.Update(Service)
            _BroadcastDelay = .BroadcastDelay
            _Dormant = .Dormant
        End With
    End Sub

#End Region

#Region "Properties"

    Private _Dormant As New Dictionary(Of String, eSvcType)
    <Browsable(False)>
    Public Property Dormant As Dictionary(Of String, eSvcType)
        Get
            Return _Dormant
        End Get
        Set(value As Dictionary(Of String, eSvcType))
            _Dormant = value
        End Set
    End Property

    Private _BroadcastDelay As Integer
    <CategoryAttribute("Settings"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The delay in seconds between discovery broadcasts.")>
    Public Property BroadcastDelay As Integer
        Get
            Return _BroadcastDelay
        End Get
        Set(value As Integer)
            _BroadcastDelay = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Host, Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("BroadcastDelay", _BroadcastDelay)
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
            Return String.Format("{0}", Host)
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property SubscribersTag As String
        Get
            Return String.Format("{0}\subscribers", TreeTag)
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Host, IconList("discovery"), IconList("discovery"))
            Else
                If IsTimedOut Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            For Each childService As Object In Me.values
                With TryCast(childService, oServiceBase)
                    If .ServiceType = eSvcType.Subscriber Then
                        Dim subscr As TreeNode = this.Nodes(SubscribersTag)
                        If IsNothing(subscr) Then
                            subscr = this.Nodes.Add(SubscribersTag, "Subscibers", IconList("subscribers"), IconList("subscribers"))
                        End If
                        .DrawTree(subscr, IconList)
                    Else
                        .DrawTree(this, IconList)
                    End If

                End With
            Next

        End With
    End Sub

#End Region

#Region "Control Panel"

    Public Overrides Function useCpl(ByRef pnlName As String, ParamArray args() As String) As Object
        Select Case UBound(args)
            Case 0
                pnlName = "discovery"
                Return Me

            Case Else
                Return Nothing

        End Select

    End Function

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ParamArray args() As String)
        Select Case UBound(args)
            Case 0
                Dim f As Boolean = False
                With TryCast(sender, ContextMenuStrip).Items
                    .Clear()
                    For Each d As String In Dormant.Keys
                        If Dormant(d) = eSvcType.Service Then
                            .Add(String.Format("Start {0}", d), Nothing, AddressOf hStartClick)
                            .Item(.Count - 1).Tag = d
                            f = True
                        End If
                    Next
                End With
                If Not f Then e.Cancel = True

            Case Else
                Select Case args(UBound(args)).ToLower
                    Case "subscribers"
                        Dim f As Boolean = False
                        With TryCast(sender, ContextMenuStrip).Items
                            .Clear()
                            For Each d As String In Dormant.Keys
                                If Dormant(d) = eSvcType.Subscriber Then
                                    .Add(String.Format("Start {0}", d), Nothing, AddressOf hStartClick)
                                    .Item(.Count - 1).Tag = d
                                    f = True
                                End If
                            Next
                        End With
                        If Not f Then e.Cancel = True

                    Case Else
                        e.Cancel = True

                End Select

        End Select

    End Sub

#Region "Context Menu Handlers"

    Private Sub hStartClick(Sender As Object, e As EventArgs)
        Dim s As CmdEventArgs = New CmdEventArgs(Host, Port, True)
        s.Message = New oMsgCmd
        With TryCast(s.Message, oMsgCmd).Args
            .Add("service", TryCast(Sender, ToolStripItem).Tag)
            .Add("state", "start")
        End With
        MyBase.SendCmd(Me, s)
    End Sub

#End Region

#End Region

End Class