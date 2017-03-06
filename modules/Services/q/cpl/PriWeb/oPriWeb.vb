Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms
Imports PriPROC6.svcMessage

Public Class oPriWeb

#Region "Constructor"

    Sub New(PriorityDB As String, Hostname As String, path As String, tabini As String)
        _PriorityDB = PriorityDB
        _Hostname = Hostname
        _tabini = tabini
        _Path = path
    End Sub

#End Region

#Region "Properties"

    Private _Environments As New Dictionary(Of String, oEnv)
    <Browsable(False)>
    Public Property Environments As Dictionary(Of String, oEnv)
        Get
            Return _Environments
        End Get
        Set(value As Dictionary(Of String, oEnv))
            _Environments = value
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

    Private _Hostname As String
    <CategoryAttribute("Host"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The URL of the Priority web.")>
    Public Property Hostname As String
        Get
            Return _Hostname
        End Get
        Set(value As String)
            _Hostname = value
        End Set
    End Property

    Private _Path As String
    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The path to Priority.")>
    Public Property Path As String
        Get
            Return _Path
        End Get
        Set(value As String)
            _Path = value
        End Set
    End Property

    Private _tabini As String
    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The ini file to use for this host.")>
    Public Property tabini As String
        Get
            Return _tabini
        End Get
        Set(value As String)
            _tabini = value
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property qfolder As DirectoryInfo
        Get
            Return New DirectoryInfo(
                IO.Path.Combine(
                    _Path,
                    "queue"
                )
            )
        End Get
    End Property

#End Region

#Region "Draw Tree"

    <Browsable(False)>
    ReadOnly Property TreeTag(parent As TreeNode) As String
        Get
            Return String.Format("{0}", parent.Name)
        End Get
    End Property

    Private _web As oPriWeb
    Public Sub DrawTree(ByRef web As oPriWeb, ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        _web = web
        With Parent

            For Each env As oEnv In web.Environments.Values
                Dim envNode As TreeNode
                If IsNothing(.Nodes(String.Format("{0}\{1}", TreeTag(Parent), env.Name))) Then
                    envNode = .Nodes.Add(String.Format("{0}\{1}", TreeTag(Parent), env.Name), env.Name, IconList("user"), IconList("user"))
                Else
                    envNode = .Nodes(String.Format("{0}\{1}", TreeTag(Parent), env.Name))
                End If
                env.DrawTree(env, envNode, IconList)
            Next

        End With
    End Sub

#End Region

End Class
