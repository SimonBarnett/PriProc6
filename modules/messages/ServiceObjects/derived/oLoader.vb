Imports System.Xml
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message
Imports System.IO

Public Class oLoader : Inherits oService

#Region "Constructor"

    Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)
        MyBase.New(Host, Name, ServiceType, port)

    End Sub

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oLoader)
            MyBase.Update(Service)
            _PriorityShare = .PriorityShare
            _PriorityPath = .PriorityPath
            _PriorityDB = .PriorityDB
            For Each k As String In .Keys
                If Not Me.Keys.Contains(k) Then
                    Me.Add(k, .Item(k))
                End If

            Next
            For Each u As String In .Users
                If Not Me.Users.Contains(u) Then
                    Me.Users.Add(u)
                End If
            Next
            Dim remove As New List(Of String)
            For Each u As String In Me.Users
                If Not .Users.Contains(u) Then
                    remove.Add(u)
                End If
            Next
            For Each u As String In remove
                Me.Users.Remove(u)
            Next
        End With
    End Sub

    Overloads Sub SendCmd(Sender As Object, e As CmdEventArgs)
        MyBase.SendCmd(Sender, e)
    End Sub

#End Region

#Region "Properties"

    Private _PriorityShare As String
    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The UNC pathname to the Priority Share.")>
    Public Property PriorityShare As String
        Get
            Return _PriorityShare
        End Get
        Set(value As String)
            _PriorityShare = value
        End Set
    End Property

    Private _PriorityPath As String
    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The physical pathname to the installation of Priority.")>
    Public Property PriorityPath As String
        Get
            Return _PriorityPath
        End Get
        Set(value As String)
            _PriorityPath = value
        End Set
    End Property

    Private _PriorityDB As String
    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The Database used by Priority.")>
    Public Property PriorityDB As String
        Get
            Return _PriorityDB
        End Get
        Set(value As String)
            _PriorityDB = value
        End Set
    End Property

    Private _Users As New List(Of String)
    <Browsable(False)>
    Public Property Users As List(Of String)
        Get
            Return _Users
        End Get
        Set(value As List(Of String))
            _Users = value
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
                this = .Nodes.Add(TreeTag, Name, IconList("loader"), IconList("loader"))
            Else
                If IsTimedOut Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            With this
                Dim usersNode As TreeNode = .Nodes(String.Format("{0}\users", TreeTag))
                If IsNothing(usersNode) Then
                    usersNode = .Nodes.Add(String.Format("{0}\users", TreeTag), "Users", IconList("user"), IconList("user"))
                End If

                Dim envNode As TreeNode = .Nodes(String.Format("{0}\env", TreeTag))
                If IsNothing(envNode) Then
                    envNode = .Nodes.Add(String.Format("{0}\env", TreeTag), "Company", IconList("environment"), IconList("environment"))
                End If

                For Each Env As Object In Me.values
                    With TryCast(Env, PriEnv)
                        .DrawTree(Me, envNode, IconList)
                    End With
                Next
            End With
        End With

    End Sub

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ParamArray args() As String)
        Select Case UBound(args)
            Case 1
                With TryCast(sender, ContextMenuStrip).Items
                    .Clear()
                    .Add("Stop service", Nothing, AddressOf hStopClick)
                    .Add("Restart service", Nothing, AddressOf hRestartClick)
                End With

            Case Else
                e.Cancel = True

        End Select

    End Sub

#Region "Context Menu Handlers"

    Private Sub hStopClick(Sender As Object, e As EventArgs)
        If MsgBox(String.Format("Stop service {0} on {1}?", Me.Name, Parent.Host), vbYesNo) = vbYes Then
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

    Private Sub hRestartClick(Sender As Object, e As EventArgs)
        If MsgBox(String.Format("Restart service {0} on {1}?", Me.Name, Parent.Host), vbYesNo) = vbYes Then
            Dim s As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oDiscovery).Port, True)
            s.Message = New oMsgCmd
            With TryCast(s.Message, oMsgCmd).Args
                .Add("service", Me.Name)
                .Add("state", "restart")
            End With
            LastSeen = #1/1/1#
            MyBase.SendCmd(Me, s)
        End If
    End Sub

#End Region

#End Region

#Region "Control Panel"

    Public Overrides Function useCpl(ByRef pnlName As String, ParamArray args() As String) As Object
        Select Case UBound(args)
            Case 1
                pnlName = "loader"
                Return Me
            Case 2
                Select Case args(2)
                    Case "users"
                        pnlName = "User"
                        Return Me

                    Case "env"
                        Return Nothing

                    Case Else
                        Return Nothing

                End Select

            Case 4
                Select Case args(2)
                    Case "env"
                        pnlName = "Bubble"
                        Return New DirectoryInfo(Path.Combine(Me.PriorityShare, "system\queue", args(3), args(4)))

                    Case Else
                        Return Nothing

                End Select
            Case Else
                Return Nothing

        End Select

    End Function

#End Region

End Class