Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message

Public Class oSubEventLog : Inherits oSubscriber

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oSubEventLog)
            MyBase.Update(Service)
            Me.MinimumRetentionDays = .MinimumRetentionDays
            Me.MaximumKilobytes = .MaximumKilobytes
        End With
    End Sub

#End Region

#Region "properties"

    Private _MinimumRetentionDays As Integer
    <CategoryAttribute("Windows Event Log"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The minimum number of days to retain Windows logs.")>
    Public Property MinimumRetentionDays As Integer
        Get
            Return _MinimumRetentionDays
        End Get
        Set(value As Integer)
            _MinimumRetentionDays = value
        End Set
    End Property

    Private _MaximumKilobytes As Integer
    <CategoryAttribute("Windows Event Log"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The maximium size in kilobytes for the Windows log.")>
    Public Property MaximumKilobytes As Integer
        Get
            Return _MaximumKilobytes
        End Get
        Set(value As Integer)
            _MaximumKilobytes = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oDiscovery).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("MaximumKilobytes", value)
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
                this = .Nodes.Add(TreeTag, Name, IconList("EventLog"), IconList("EventLog"))
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
        pnlName = "eventlog"
        Return Me
    End Function

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ParamArray args() As String)
        Select Case UBound(args)
            Case 1
                With TryCast(sender, ContextMenuStrip).Items
                    .Clear()
                    .Add("Stop subscriber", Nothing, AddressOf hStopClick)
                End With

            Case Else
                e.Cancel = True

        End Select

    End Sub

#Region "Context Menu Handlers"

    Private Sub hStopClick(Sender As Object, e As EventArgs)
        If MsgBox(String.Format("Stop subscriber {0} on {1}?", Me.Name, Parent.Host), vbYesNo) = vbYes Then
            Dim s As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oDiscovery).Port, True)
            s.Message = New oMsgCmd
            With TryCast(s.Message, oMsgCmd).Args
                .Add("service", Me.Name)
                .Add("state", "stop")
            End With
            LastSeen = #1/1/1#
            MyBase.SendCmd(Me, s)
        End If
    End Sub

#End Region

#End Region

End Class