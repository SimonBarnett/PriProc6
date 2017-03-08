Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
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

            Dim removefeed As New List(Of String)
            For Each f As EndPoint In siteFeeds
                Dim feed As TreeNode = feeds.Nodes(String.Format("{0}\feeds\{1}", TreeTag, f.Name))
                removefeed.Add(String.Format("{0}\feeds\{1}", TreeTag, f.Name))
                If IsNothing(feed) Then
                    feeds.Nodes.Add(String.Format("{0}\feeds\{1}", TreeTag, f.Name), String.Format("{0}.ashx", f.Name), IconList("feed"), IconList("feed"))
                End If
            Next
            For Each rf As TreeNode In feeds.Nodes
                If Not rf Is Nothing Then
                    If Not removefeed.Contains(rf.Name) Then
                        rf.Remove()
                    End If
                End If
            Next

            Dim handlers As TreeNode = this.Nodes(String.Format("{0}\handlers", TreeTag))
            If IsNothing(handlers) Then
                handlers = this.Nodes.Add(String.Format("{0}\handlers", TreeTag), "Handlers", IconList("handlers"), IconList("handlers"))
            End If

            Dim removehandlers As New List(Of String)
            For Each h As EndPoint In siteHandlers
                Dim handler As TreeNode = handlers.Nodes(String.Format("{0}\handlers\{1}", TreeTag, h.Name))
                removehandlers.Add(String.Format("{0}\handlers\{1}", TreeTag, h.Name))

                If IsNothing(handler) Then
                    handlers.Nodes.Add(String.Format("{0}\handlers\{1}", TreeTag, h.Name), String.Format("{0}.ashx", h.Name), IconList("feed"), IconList("feed"))
                End If
            Next
            For Each rh As TreeNode In handlers.Nodes
                If Not rh Is Nothing Then
                    If Not removehandlers.Contains(rh.Name) Then
                        rh.Remove()
                    End If
                End If

            Next

        End With
    End Sub

#End Region

#Region "Properties"

    Public Sub SetoDataServers(oDataServers As List(Of XmlNode), SvcMap As Dictionary(Of String, oDiscovery))
        share.oDataServers = oDataServers
        share.SvcMap = SvcMap

    End Sub

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
    Public Property siteFeeds As List(Of EndPoint)
        Get
            Return _siteFeeds
        End Get
        Set(value As List(Of EndPoint))
            _siteFeeds = value
        End Set
    End Property

    Private _siteHandlers As New List(Of EndPoint)
    <Browsable(False),
    DesignOnly(False)>
    Public Property siteHandlers As List(Of EndPoint)
        Get
            Return _siteHandlers
        End Get
        Set(value As List(Of EndPoint))
            _siteHandlers = value
        End Set
    End Property

    <CategoryAttribute("Settings"),
    DescriptionAttribute("The database connection string."),
    Browsable(True),
    [ReadOnly](True)>
    Public Property Database() As String
        Get
            Try
                Return Settings("db")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)
            Settings("db") = value
        End Set
    End Property

    <CategoryAttribute("Settings"),
    DescriptionAttribute("The location to store bubbles."),
    Browsable(True),
    [ReadOnly](True)>
    Public Property SavePath() As String
        Get
            Try
                Return Settings("path")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)
            Settings("path") = value
        End Set
    End Property

    <CategoryAttribute("oData Server"),
    DescriptionAttribute("The tabula.ini file to use for oData calls."),
    Browsable(True),
    [ReadOnly](True)>
    Public Property TabulaINI() As String
        Get
            Try
                Return Settings("tabini")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)
            Settings("tabini") = value
        End Set
    End Property

    <TypeConverter(GetType(Prop_oDataServers)),
    CategoryAttribute("oData Server"),
    DescriptionAttribute("The odata server to post bubbles.")>
    Public Property Service() As String
        Get
            Try
                Return Settings("service")

            Catch ex As Exception
                Return String.Empty

            End Try

        End Get
        Set(ByVal value As String)
            If _webrelay.mmc Then

                Dim f As Boolean = False
                Dim e As CmdEventArgs = New CmdEventArgs(_webrelay.Host, _webrelay.Port)
                e.Message = New oMsgCmd

                With TryCast(e.Message, oMsgCmd).Args
                    .Add("endpoint", Me.Endpoint)
                    .Add("service", value)
                    For Each oDataServer As XmlNode In oDataServers
                        If String.Compare(value, oDataServer.Attributes("hostname").Value, True) = 0 Then
                            f = True
                            .Add("db", oDataServer.Attributes("database").Value)
                            .Add("path", oDataServer.Attributes("path").Value)
                            .Add("tabini", oDataServer.Attributes("tabini").Value)

                        End If
                    Next

                    If Not f Then
                        .Add("db", "")
                        .Add("path", "")
                        .Add("tabini", "")
                    End If

                    .Add("environment", "")
                    .Add("oDataUser", "")
                    .Add("oDataPassword", "")

                End With

                _webrelay.SendCmd(Me, e)

                If e.errCode = 200 Then
                    Settings("service") = value

                    f = False
                    For Each oDataServer As XmlNode In oDataServers
                        If String.Compare(oDataServer.Attributes("hostname").Value, Service, True) = 0 Then
                            f = True
                            Settings("db") = oDataServer.Attributes("database").Value
                            Settings("path") = oDataServer.Attributes("path").Value
                            Settings("tabini") = oDataServer.Attributes("tabini").Value

                        End If
                    Next

                    If Not f Then
                        Settings("db") = ""
                        Settings("path") = ""
                        Settings("tabini") = ""
                    End If

                    Settings("environment") = ""
                    Settings("oDataUser") = ""
                    Settings("oDataPassword") = ""

                End If

            End If
        End Set
    End Property

    <TypeConverter(GetType(Prop_Env)),
    CategoryAttribute("oData Server"),
    DescriptionAttribute("The Priority Company."),
    Browsable(True)>
    Public Property Environment() As String
        Get
            Try
                Return Settings("environment")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)

            If _webrelay.mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(_webrelay.Host, _webrelay.Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("endpoint", Me.Endpoint)
                    .Add("service", Service)
                    .Add("environment", value)
                End With

                _webrelay.SendCmd(Me, e)
                If e.errCode = 200 Then
                    Settings("environment") = value
                End If

            End If
        End Set
    End Property

    <TypeConverter(GetType(prop_User)),
        CategoryAttribute("oData User"),
        DescriptionAttribute("The Priority oData user for loadings."),
        Browsable(True)>
    Public Property Username() As String
        Get
            Try
                Return Settings("oDataUser")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)

            If _webrelay.mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(_webrelay.Host, _webrelay.Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("endpoint", Me.Endpoint)
                    .Add("service", Service)
                    .Add("oDataUser", value)
                End With

                _webrelay.SendCmd(Me, e)
                If e.errCode = 200 Then
                    Settings("oDataUser") = value
                End If

            End If
        End Set
    End Property

    <CategoryAttribute("oData User"),
    DescriptionAttribute("The Priority oData users password."),
    Browsable(True),
    PasswordPropertyText(True)>
    Public Property Password() As String
        Get
            Try
                Return Settings("oDataPassword")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)
            If String.Compare(value, InputBox("Please confirm password.", "Confirm")) = 0 Then

                If _webrelay.mmc Then
                    Dim e As CmdEventArgs = New CmdEventArgs(_webrelay.Host, _webrelay.Port)
                    e.Message = New oMsgCmd
                    With TryCast(e.Message, oMsgCmd).Args
                        .Add("endpoint", Me.Endpoint)
                        .Add("service", Service)
                        .Add("oDataPassword", value)
                    End With

                    _webrelay.SendCmd(Me, e)
                    If e.errCode = 200 Then
                        Settings("oDataPassword") = value
                    End If

                End If

            Else
                MsgBox("Passwords do not match.", vbCritical, "Error")

            End If
        End Set
    End Property

    <TypeConverter(GetType(prop_LogServer)),
    CategoryAttribute("Logging"),
    DescriptionAttribute("The server to send log info."),
    Browsable(True)>
    Public Property LogServer() As String
        Get
            Try
                Return Settings("LogServer")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)
            If _webrelay.mmc Then
                Dim lp As String = String.Empty
                Dim e As CmdEventArgs = New CmdEventArgs(_webrelay.Host, _webrelay.Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("endpoint", Me.Endpoint)
                    .Add("service", Service)
                    .Add("LogServer", value)
                    For Each d As oDiscovery In share.SvcMap.Values
                        If String.Compare(value, d.Host) = 0 Then
                            lp = d.Port
                            .Add("LogPort", lp)
                            Exit For
                        End If
                    Next
                End With

                _webrelay.SendCmd(Me, e)
                If e.errCode = 200 Then
                    Settings("LogServer") = value
                    Settings("LogPort") = lp
                End If
            End If

        End Set
    End Property

    <CategoryAttribute("Logging"),
    DescriptionAttribute("The port used to log errors."),
    Browsable(True),
    [ReadOnly](True)>
    Public Property LogPort() As String
        Get
            Try
                Return Settings("LogPort")
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
        Set(ByVal value As String)
            Settings("LogPort") = value
        End Set
    End Property

#End Region

End Class