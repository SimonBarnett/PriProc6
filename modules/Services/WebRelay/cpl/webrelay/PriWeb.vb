Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports PriPROC6.Services.Loader
Imports Microsoft.Web.Administration

Public Class PriWeb

#Region "Constructor"

    Sub New(Sitename As String, Hostname As String, port As Integer, path As String, virtual As String)
        _Sitename = Sitename
        _Hostname = Hostname
        _port = port
        _Path = path
        _virtual = virtual
    End Sub

#End Region

#Region "Draw Tree"

    <Browsable(False)>
    ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}\{1}\{2}", _webrelay.Host, _webrelay.Name, Endpoint)
        End Get
    End Property

    Private _webrelay As oWebRelay
    Public Sub DrawTree(ByRef webrelay As oWebRelay, ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        _webrelay = webrelay
        With Parent

            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, _Sitename, IconList("host"), IconList("host"))
            End If

            Dim feeds As TreeNode = this.Nodes(String.Format("{0}\feeds", TreeTag))
            If IsNothing(feeds) Then
                feeds = this.Nodes.Add(String.Format("{0}\feeds", TreeTag), "Feeds", IconList("feeds"), IconList("feeds"))
            End If

            For Each f As EndPoint In siteFeeds
                Dim feed As TreeNode = feeds.Nodes(String.Format("{0}\feeds\{1}", TreeTag, f.Name))
                If IsNothing(feed) Then
                    feeds.Nodes.Add(String.Format("{0}\feeds\{1}", TreeTag, f.Name), String.Format("{0}.ashx", f.Name), IconList("feed"), IconList("feed"))
                End If
            Next

            Dim handlers As TreeNode = this.Nodes(String.Format("{0}\handlers", TreeTag))
            If IsNothing(handlers) Then
                handlers = this.Nodes.Add(String.Format("{0}\handlers", TreeTag), "Handlers", IconList("handlers"), IconList("handlers"))
            End If

            For Each h As EndPoint In siteHandlers
                Dim handler As TreeNode = handlers.Nodes(String.Format("{0}\handlers\{1}", TreeTag, h.Name))
                If IsNothing(handler) Then
                    handlers.Nodes.Add(String.Format("{0}\handlers\{1}", TreeTag, h.Name), String.Format("{0}.ashx", h.Name), IconList("feed"), IconList("feed"))
                End If
            Next

        End With
    End Sub

#End Region

#Region "Properties"

    <CategoryAttribute("Host"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The hostname/port of the website.")>
    Public ReadOnly Property Endpoint As String
        Get
            Dim p As String
            Select Case Port
                Case 80
                    p = String.Empty
                Case Else
                    p = String.Format(":{0}", Port.ToString)
            End Select

            Dim v As String
            Select Case _virtual.Length
                Case 1
                    v = String.Empty
                Case Else
                    v = _virtual
            End Select

            Return String.Format("{0}{1}{2}", Hostname, p, v)
        End Get
    End Property

    Private _Sitename As String
    <CategoryAttribute("Host"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The friendly name of the website.")>
    Public Property SiteName As String
        Get
            Return _Sitename
        End Get
        Set(value As String)
            _Sitename = value
        End Set
    End Property

    Private _Hostname As String
    <Browsable(False),
    DesignOnly(True)>
    Public Property Hostname As String
        Get
            If _Hostname.Length > 0 Then
                Return _Hostname
            Else
                Return String.Format("{0}", System.Environment.MachineName)
            End If
        End Get
        Set(value As String)
            _Hostname = value
        End Set
    End Property

    Private _port As Integer
    <Browsable(False),
    DesignOnly(True)>
    Public Property Port As Integer
        Get
            Return _port
        End Get
        Set(value As Integer)
            _port = value
        End Set
    End Property

    Private _Path As String
    <CategoryAttribute("Host"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The path to the website.")>
    Public Property Path As String
        Get
            Return _Path
        End Get
        Set(value As String)
            _Path = value
        End Set
    End Property

    Private _virtual As String
    <Browsable(False),
    DesignOnly(True)>
    Public Property virtual As String
        Get
            Return _virtual
        End Get
        Set(value As String)
            _virtual = value
        End Set
    End Property

    Private _Settings As New Dictionary(Of String, String)
    <Browsable(False),
    DesignOnly(True)>
    Public Property Settings As Dictionary(Of String, String)
        Get
            Return _Settings
        End Get
        Set(value As Dictionary(Of String, String))
            _Settings = value
        End Set
    End Property

    Private _siteFeeds As New List(Of EndPoint)
    <Browsable(False),
    DesignOnly(False)>
    Public ReadOnly Property siteFeeds As List(Of EndPoint)
        Get
            Return _siteFeeds
        End Get
    End Property

    Private _siteHandlers As New List(Of EndPoint)
    <Browsable(False),
    DesignOnly(False)>
    Public ReadOnly Property siteHandlers As List(Of EndPoint)
        Get
            Return _siteHandlers
        End Get
    End Property

    <Browsable(False)>
    Public Property Service() As String
        Get
            Return Settings("service")
        End Get
        Set(ByVal value As String)
            Settings("service") = value
        End Set
    End Property

    <Browsable(False)>
    Public Property Environment() As String
        Get
            Return Settings("environment")
        End Get
        Set(ByVal value As String)
            Settings("environment") = value
        End Set
    End Property

    '<TypeConverter(GetType(Prop_Env)),
    'CategoryAttribute("Settings"),
    'DescriptionAttribute("The server and default environment to post bubbles.")>
    'Public Property ServiceEnvironment() As String
    '    Get
    '        Return String.Format("{0}\{1}", Service, Environment)
    '    End Get
    '    Set(ByVal value As String)
    '        Service = value.Split("\")(0)
    '        Environment = value.Split("\")(1)
    '        If _webrelay.mmc Then
    '            Dim e As CmdEventArgs = New CmdEventArgs(_webrelay.Host, _webrelay.Port)
    '            e.Message = New oMsgCmd
    '            With TryCast(e.Message, oMsgCmd).Args
    '                .Add("endpoint", Me.Endpoint)
    '                .Add("service", Service)
    '                .Add("environment", Environment)
    '                For Each svc As oDiscovery In propSvcMap.svcMap.Values
    '                    If String.Compare(svc.Host, Service, True) = 0 Then
    '                        For Each i As oServiceBase In svc.values
    '                            If Not IsNothing(TryCast(i, oLoader)) Then
    '                                .Add("loadport", TryCast(i, oLoader).Port)

    '                            End If
    '                        Next
    '                    End If
    '                Next
    '            End With
    '            _webrelay.SendCmd(Me, e)
    '        End If
    '    End Set
    'End Property

#End Region

End Class