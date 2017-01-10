Imports System.Xml
Imports PriPROC6.Interface.Message
Imports System.Windows.Forms

#Region "Services"

Public Class oDiscovery : Inherits oService

    Private _BroadcastDelay As Integer
    Public Property BroadcastDelay As Integer
        Get
            Return _BroadcastDelay
        End Get
        Set(value As Integer)
            _BroadcastDelay = value
        End Set
    End Property

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}", Host)
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Host, IconList("discovery"), IconList("discovery"))
            Else
                If DateDiff(DateInterval.Minute, LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            For Each childService As Object In Me.Values
                TryCast(childService, oService).DrawTree(this, IconList)
            Next

        End With
    End Sub

End Class

Public Class PriEnv

    Sub New(Name As String)
        _Name = Name
    End Sub

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}\{1}\{2}", _loader.Host, _loader.Name, Name)
        End Get
    End Property

    Private _loader As oLoader
    Public Sub DrawTree(ByRef loader As oLoader, ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        _loader = loader
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Name, IconList("environment"), IconList("environment"))
            End If

        End With
    End Sub

End Class

Public Class oWebRelay : Inherits oService

    Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)
        MyBase.New(Host, Name, ServiceType, port)

    End Sub

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}\{1}", Host, Name)
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Name, IconList("webrelay"), IconList("webrelay"))
            Else
                If DateDiff(DateInterval.Minute, LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            For Each host As Object In Me.Values
                With TryCast(host, PriWeb)
                    .DrawTree(Me, this, IconList)
                End With
            Next

        End With
    End Sub

End Class

Public Class PriWeb

    Sub New(Sitename As String, Hostname As String, port As Integer, path As String)
        _Sitename = Sitename
        _Hostname = Hostname
        _port = port
        _Path = path

    End Sub

    Public ReadOnly Property Endpoint As String
        Get
            Return String.Format("{0}:{1}", Hostname, Port.ToString)
        End Get
    End Property

    Private _Sitename As String
    Public Property SiteName As String
        Get
            Return _Sitename
        End Get
        Set(value As String)
            _Sitename = value
        End Set
    End Property

    Private _Hostname As String
    Public Property Hostname As String
        Get
            If _Hostname.Length > 0 Then
                Return _Hostname
            Else
                Return String.Format("{0}", Environment.MachineName)
            End If
        End Get
        Set(value As String)
            _Hostname = value
        End Set
    End Property

    Private _port As Integer
    Public Property Port As Integer
        Get
            Return _port
        End Get
        Set(value As Integer)
            _port = value
        End Set
    End Property

    Private _Path As String
    Public Property Path As String
        Get
            Return _Path
        End Get
        Set(value As String)
            _Path = value
        End Set
    End Property

    Private _Settings As New Dictionary(Of String, String)
    Public Property Settings As Dictionary(Of String, String)
        Get
            Return _Settings
        End Get
        Set(value As Dictionary(Of String, String))
            _Settings = value
        End Set
    End Property

    Private _siteFeeds As New List(Of String)
    Public ReadOnly Property siteFeeds As List(Of String)
        Get
            Return _siteFeeds
        End Get
    End Property

    Private _siteHandlers As New List(Of String)
    Public ReadOnly Property siteHandlers As List(Of String)
        Get
            Return _siteHandlers
        End Get
    End Property

    ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}\{1}\{2}", _webrelay.Host, _webrelay.Name, Hostname)
        End Get
    End Property

    Private _webrelay As oWebRelay
    Public Sub DrawTree(ByRef webrelay As oWebRelay, ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        _webrelay = webrelay
        With Parent

            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Hostname, IconList("host"), IconList("host"))
            End If

            Dim feeds As TreeNode = this.Nodes(String.Format("{0}/feeds", TreeTag))
            If IsNothing(feeds) Then
                feeds = this.Nodes.Add(String.Format("{0}/feeds", TreeTag), "Feeds", IconList("feeds"), IconList("feeds"))
            End If

            For Each f As String In siteFeeds
                Dim feed As TreeNode = feeds.Nodes(String.Format("{0}/feeds/{1}", TreeTag, f))
                If IsNothing(feed) Then
                    feeds.Nodes.Add(String.Format("{0}/feeds/{1}", TreeTag, f), String.Format("{0}.ashx", f), IconList("feed"), IconList("feed"))
                End If
            Next

            Dim handlers As TreeNode = this.Nodes(String.Format("{0}/handlers", TreeTag))
            If IsNothing(handlers) Then
                handlers = this.Nodes.Add(String.Format("{0}/handlers", TreeTag), "Handlers", IconList("handlers"), IconList("handlers"))
            End If

            For Each h As String In siteHandlers
                Dim handler As TreeNode = handlers.Nodes(String.Format("{0}/handlers/{1}", TreeTag, h))
                If IsNothing(handler) Then
                    handlers.Nodes.Add(String.Format("{0}/handlers/{1}", TreeTag, h), String.Format("{0}.ashx", h), IconList("feed"), IconList("feed"))
                End If
            Next

        End With
    End Sub

End Class

Public Class oLoader : Inherits oService

    Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)
        MyBase.New(Host, Name, ServiceType, port)

    End Sub

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Private _PriorityShare As String
    Public Property PriorityShare As String
        Get
            Return _PriorityShare
        End Get
        Set(value As String)
            _PriorityShare = value
        End Set
    End Property

    Private _PriorityPath As String
    Public Property PriorityPath As String
        Get
            Return _PriorityPath
        End Get
        Set(value As String)
            _PriorityPath = value
        End Set
    End Property

    Private _PriorityDB As String
    Public Property PriorityDB As String
        Get
            Return _PriorityDB
        End Get
        Set(value As String)
            _PriorityDB = value
        End Set
    End Property

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
                If DateDiff(DateInterval.Minute, LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            For Each Env As Object In Me.Values
                With TryCast(Env, PriEnv)
                    .DrawTree(Me, this, IconList)
                End With
            Next

        End With
    End Sub

End Class

#End Region

#Region "Subscribers"

Public MustInherit Class oSubscriber : Inherits oService

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

        Dim filter As XmlNode = node.SelectSingleNode("filter")
        _EntryType = filter.Attributes("EntryType").Value
        _Verbosity = filter.Attributes("Verbosity").Value
        _Source = filter.Attributes("Source").Value

    End Sub

    Private _EntryType As Integer
    Public Property EntryType As Integer
        Get
            Return _EntryType
        End Get
        Set(value As Integer)
            _EntryType = value
        End Set
    End Property

    Private _Verbosity As Integer
    Public Property Verbosity As Integer
        Get
            Return _Verbosity
        End Get
        Set(value As Integer)
            _Verbosity = value
        End Set
    End Property

    Private _Source As Integer
    Public Property Source As Integer
        Get
            Return _Source
        End Get
        Set(value As Integer)
            _Source = value
        End Set
    End Property
End Class

Public Class oSubConsole : Inherits oSubscriber

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

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
                If DateDiff(DateInterval.Minute, LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

        End With
    End Sub

End Class

Public Class oSubBroadcast : Inherits oSubscriber

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Private _broadcastport As Integer
    Public Property broadcastport As Integer
        Get
            Return _broadcastport
        End Get
        Set(value As Integer)
            _broadcastport = value
        End Set
    End Property

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
                If DateDiff(DateInterval.Minute, LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If


        End With
    End Sub

End Class

#End Region