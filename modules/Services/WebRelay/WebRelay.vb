﻿Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Xml
Imports System.IO
Imports System.Drawing

Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports Microsoft.Web.Administration

Imports PriPROC6.Interface.Service
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Imports PriPROC6.Interface.Cpl
Imports PriPROC6.Services.Loader

<Export(GetType(svcDef))>
<ExportMetadata("Name", "webrelay")>
<ExportMetadata("udp", False)>
<ExportMetadata("defaultPort", 8092)>
<ExportMetadata("defaultStart", False)>
Public Class webRelay : Inherits svcbase
    Implements svcDef

    Private LocalWebs As oWebRelay
    Public Property HttpContext As Object

#Region "Start / Stop"

    Public Overrides Sub svcStart(ByRef log As oMsgLog)

        LocalWebs = New oWebRelay(Environment.MachineName, Name, eSvcType.Service, Me.Port)
        Using sm As New ServerManager
            log.LogData.Append("Scanning for websites...").AppendLine()
            UpdateSites(sm, log)
        End Using

    End Sub

    Public Overrides Sub svcStop(ByRef log As oMsgLog)
        With log.LogData

        End With
        svc_state = eServiceState.stopped

    End Sub

#End Region

    Public Function appSettings(Site As Microsoft.Web.Administration.Site, virtual As String) As ConfigurationElementCollection
        For Each app As Microsoft.Web.Administration.Application In Site.Applications
            If String.Compare(virtual, app.Path, True) = 0 Then
                Return app.GetWebConfiguration.GetSection("appSettings").GetCollection
            End If
        Next
        Return Nothing
    End Function

    Public Function cnSettings(Site As Microsoft.Web.Administration.Site, virtual As String) As ConfigurationElementCollection
        For Each app As Microsoft.Web.Administration.Application In Site.Applications
            If String.Compare(virtual, app.Path, True) = 0 Then
                Return app.GetWebConfiguration.GetSection("connectionStrings").GetCollection
            End If
        Next
        Return Nothing
    End Function

#Region "Discovery Messages"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)
        Using sm As New ServerManager
            UpdateSites(sm)

            With outputStream
                For Each w As PriWeb In LocalWebs.values
                    .WriteStartElement("relay")
                    .WriteAttributeString("endpoint", w.Endpoint)
                    .WriteElementString("sitename", w.SiteName)
                    .WriteElementString("hostname", w.Hostname)
                    .WriteElementString("port", w.Port.ToString)
                    .WriteElementString("physicalpath", w.Path)
                    .WriteElementString("virtual", w.virtual)

                    .WriteStartElement("appSettings")

                    For Each att As ConfigurationElement In appSettings(sm.Sites(w.SiteName), w.virtual)
                        .WriteElementString(att.Attributes("key").Value, att.Attributes("value").Value)
                    Next

                    .WriteEndElement()

                    .WriteStartElement("endpoints")
                    Dim ep As New EndPoints(
                        New DirectoryCatalog(
                            Path.Combine(
                                w.Path,
                                "bin"
                            )
                        )
                    )

                    Dim fi As FileInfo

                    For Each Feed As String In ep.siteFeeds
                        .WriteStartElement("endpoint")
                        .WriteAttributeString("name", Feed)
                        .WriteAttributeString("type", "feed")
                        .WriteEndElement()

                        fi = New FileInfo(Path.Combine(w.Path, String.Format("{0}.ashx", Feed)))
                        If Not fi.Exists() Then
                            File.WriteAllBytes(fi.FullName, My.Resources.Handler)
                        End If

                    Next

                    For Each handler As String In ep.siteHandlers
                        .WriteStartElement("endpoint")
                        .WriteAttributeString("name", handler)
                        .WriteAttributeString("type", "handler")
                        .WriteEndElement()

                        fi = New FileInfo(Path.Combine(w.Path, String.Format("{0}.ashx", handler)))
                        If Not fi.Exists() Then
                            File.WriteAllBytes(fi.FullName, My.Resources.Handler)
                        End If

                    Next

                    .WriteEndElement()

                    .WriteEndElement()

                Next

            End With
        End Using

    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oWebRelay(Service)
        For Each relay As XmlNode In Service.SelectNodes("//service/relay")
            Dim web As New PriWeb(
                relay.SelectSingleNode("sitename").InnerText,
                relay.SelectSingleNode("hostname").InnerText,
                relay.SelectSingleNode("port").InnerText,
                relay.SelectSingleNode("physicalpath").InnerText,
                relay.SelectSingleNode("virtual").InnerText
            )

            For Each sett As XmlNode In relay.SelectSingleNode("appSettings").ChildNodes
                web.Settings.Add(sett.Name, sett.InnerText)
            Next

            For Each ep As XmlNode In relay.SelectNodes("endpoints/endpoint")
                Select Case ep.Attributes("type").Value.ToLower
                    Case "feed"
                        web.siteFeeds.Add(New EndPoint(web, ep.Attributes("name").Value))

                    Case Else
                        web.siteHandlers.Add(New EndPoint(web, ep.Attributes("name").Value))

                End Select
            Next

            ret.Add(
                web.Endpoint,
                web
            )

        Next
        Return ret

    End Function

#End Region

#Region "Message Handlers"

    Public Overrides Function tcpMsg(ByRef msg As msgBase, ByRef changeLog As oMsgLog) As Byte()
        With msg
            Select Case .msgType
                Case "cmd"
                    With TryCast(msg.thisObject, oMsgCmd)

                        If Not .Args.Keys.Contains("endpoint") Then
                            Return msgFactory.EncodeResponse(
                                "generic",
                                400,
                                String.Format(
                                    "Missing Endpoint.",
                                    ""
                                )
                            )
                        End If

                        If Not LocalWebs.Keys.Contains(.Args("endpoint")) Then
                            Return msgFactory.EncodeResponse(
                                "generic",
                                400,
                                String.Format(
                                    "Invalid Endpoint {0}.",
                                    .Args("endpoint")
                                )
                            )
                        End If

                        Dim changes As Boolean = False
                        Using sm As New ServerManager

                            Dim targetWeb As PriWeb = TryCast(LocalWebs(.Args("endpoint")), PriWeb)
                            changeLog.LogData.AppendFormat("Updating Endpoint {0}.", targetWeb.Endpoint).AppendLine()

                            For Each k As String In .Args.Keys
                                Select Case k
                                    Case "endpoint"
                                        ' Ignore

                                    Case "db"
                                        Dim f As Boolean = False

                                        For Each att As ConfigurationElement In cnSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)

                                            If String.Compare(att.Attributes("name").Value, "priority", True) = 0 Then
                                                If Not String.Compare(att.Attributes("connectionString").Value, DSN(.Args(k))) = 0 Then
                                                    changes = True
                                                    changeLog.LogData.AppendFormat("Updating Priority database connection [{0}]", DSN(.Args(k))).AppendLine()
                                                    att.Attributes("connectionString").Value = DSN(.Args(k))
                                                End If
                                                f = True
                                                Exit For

                                            End If
                                        Next

                                        If Not f Then
                                            changes = True
                                            changeLog.LogData.AppendFormat("Adding Priority database connection [{0}]", DSN(.Args(k))).AppendLine()
                                            Dim addelement As ConfigurationElement = cnSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
                                            addelement("connectionString") = DSN(.Args(k))
                                            addelement("name") = "priority"
                                            addelement("providerName") = "System.Data.SqlClient"
                                            cnSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
                                        End If

                                    Case Else
                                        Dim f As Boolean = False

                                        For Each att As ConfigurationElement In appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)
                                            If String.Compare(att.Attributes("key").Value, k, True) = 0 Then
                                                If Not String.Compare(att.Attributes("value").Value, .Args(k)) = 0 Then
                                                    changes = True
                                                    changeLog.LogData.AppendFormat("Updating setting [{0}]={1}", k, .Args(k)).AppendLine()
                                                    att.Attributes("value").Value = .Args(k)
                                                End If
                                                f = True
                                                Exit For
                                            End If
                                        Next

                                        If Not f Then
                                            changes = True
                                            changeLog.LogData.AppendFormat("Adding Setting [{0}]={1}", k, .Args(k)).AppendLine()
                                            Dim addelement As ConfigurationElement = appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
                                            addelement("key") = k
                                            addelement("value") = .Args(k)
                                            appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
                                        End If

                                End Select
                            Next

                            If changes Then
                                Dim c As Integer = 0
                                Dim cmmit As Boolean = False

                                Do
                                    Try
                                        sm.CommitChanges()
                                        cmmit = True
                                        changeLog.EntryType = LogEntryType.SuccessAudit
                                        Return msgFactory.EncodeResponse("generic", 200)

                                    Catch ex As Exception
                                        c += 1
                                        If c > 9 Then
                                            changeLog.setException(ex)
                                            Return msgFactory.EncodeResponse("generic", 400, "The web.config is already open. Please try again.")

                                        Else
                                            Threading.Thread.Sleep(500)
                                        End If

                                    End Try

                                Loop Until c > 9 Or cmmit

                            End If

                        End Using

                    End With

                Case Else
                    Return msgFactory.EncodeResponse("generic", 400, "Unknown message type.")

            End Select

        End With

    End Function

#End Region

#Region "Private Methods"

    ReadOnly Property DSN(db As String)
        Get
            Return String.Format("Server={0};Database=system;Trusted_Connection=Yes;", db)
        End Get
    End Property

    Private Sub UpdateSites(sm As ServerManager, Optional ByRef log As oMsgLog = Nothing)
        For Each site As Site In sm.Sites
            With site
                For Each binding As Microsoft.Web.Administration.Binding In .Bindings
                    For Each app As Microsoft.Web.Administration.Application In .Applications

                        Dim pw As New PriWeb(
                            site.Name,
                            binding.Host,
                            binding.EndPoint.Port,
                            app.VirtualDirectories("/").PhysicalPath(),
                            app.Path()
                        )

                        If Not LocalWebs.Keys.Contains(pw.Endpoint) Then
                            If site.State = ObjectState.Started Then
                                If File.Exists(
                                Path.Combine(
                                    pw.Path,
                                    "bin\webinterface.dll"
                                )
                            ) Then
                                    LocalWebs.Add(
                                    pw.Endpoint,
                                    pw
                                )
                                    If Not IsNothing(log) Then
                                        log.LogData.AppendFormat("Found HTTP EndPoint {0}.", pw.Endpoint).AppendLine()

                                    End If

                                End If
                            End If

                        Else
                            If Not site.State = ObjectState.Started Then
                                LocalWebs.Remove(pw.Endpoint)

                            End If

                        End If

                    Next
                Next

            End With
        Next

    End Sub

#End Region

#Region "Config"

    Overrides Sub configMSG(ByRef Svc As List(Of Object), ByRef Log As oMsgLog)
        Log.svcType = Me.Name

        'For Each s As XmlNode In Svc
        '    Select Case s.Attributes("name").Value.ToLower
        '        Case "loader"
        '            Dim o As New oLoader(s)
        '            With o
        '                For Each priweb As XmlNode In s.SelectNodes("priweb")

        '                    If Not PriPROC6.oDataInstall.Contains(priweb.Attributes("hostname").Value) Then
        '                        oDataInstall.Add(priweb.Attributes("hostname").Value)
        '                    End If

        '                    '.Add(
        '                    '    priweb.Attributes("hostname").Value,
        '                    '    New oPriWeb(
        '                    '        priweb.Attributes("database").Value,
        '                    '        priweb.Attributes("hostname").Value,
        '                    '        priweb.Attributes("path").Value,
        '                    '        priweb.Attributes("tabini").Value
        '                    '    )
        '                    ')

        '                    'With TryCast(o(priweb.Attributes("hostname").Value), oPriWeb)
        '                    '    For Each env As XmlNode In priweb.SelectNodes("env")
        '                    '        .Environments.Add(
        '                    '            env.Attributes("name").Value,
        '                    '            New oEnv(
        '                    '                o(priweb.Attributes("hostname").Value),
        '                    '                env.Attributes("name").Value
        '                    '           )
        '                    '        )
        '                    '    Next

        '                    'End With

        '                Next
        '            End With

        '    End Select
        'Next

    End Sub

    Private Sub ConfigDiscovery(ByRef svc As oDiscovery, ByRef changeLog As oMsgLog)

        Dim changes As Boolean = False
        Using sm As New ServerManager
            Try
                For Each w As String In LocalWebs.Keys
                    Dim f As Boolean = False
                    Dim targetWeb As PriWeb = TryCast(LocalWebs(w), PriWeb)

                    For Each att As ConfigurationElement In appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)
                        If String.Compare(att.Attributes("key").Value, "service", True) = 0 Then
                            If String.Compare(att.Attributes("value").Value, svc.Host, True) = 0 Then
                                f = True
                                Exit For
                            Else
                                f = False
                                Exit For
                            End If
                        End If
                    Next

                    If f Then
                        f = False
                        For Each att As ConfigurationElement In appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)
                            If String.Compare(att.Attributes("key").Value, "logport", True) = 0 Then
                                f = True
                                If Not String.Compare(att.Attributes("value").Value, svc.Port) = 0 Then
                                    changes = True
                                    changeLog.LogData.AppendFormat("Updating setting [{0}]={1}", "logport", svc.Port).AppendLine()
                                    att.Attributes("value").Value = svc.Port
                                End If
                            End If
                        Next
                        If Not f Then
                            changes = True
                            changeLog.LogData.AppendFormat("Adding Setting [{0}]={1}", "logport", svc.Port).AppendLine()
                            Dim addelement As ConfigurationElement = appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
                            addelement("key") = "logport"
                            addelement("value") = svc.Port
                            appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
                        End If
                    End If
                Next

            Catch ex As Exception
                changeLog.setException(ex)

            Finally
                If changes Then
                    sm.CommitChanges()
                    changeLog.LogData.AppendFormat("Commiting changes.", "").AppendLine()
                    changeLog.EntryType = LogEntryType.SuccessAudit
                End If

            End Try
        End Using

    End Sub

    'Private Sub ConfigLoader(ByRef svc As oLoader, ByRef changeLog As oMsgLog)

    '    Dim reload As Boolean = False
    '    Dim isMissing_service As Boolean = True
    '    Dim isMissing_environment As Boolean = True
    '    Dim isMissing_loadingtimeout As Boolean = True
    '    Dim isMissing_loadport As Boolean = True

    '    Dim changes As Boolean = False
    '    Using sm As New ServerManager
    '        Try
    '            For Each w As String In LocalWebs.Keys

    '                Dim targetWeb As PriWeb = TryCast(LocalWebs(w), PriWeb)
    '                Dim f As Boolean = False

    '                For Each att As ConfigurationElement In appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)
    '                    Select Case att.Attributes("key").Value.ToString.ToLower
    '                        Case "loadport"
    '                            isMissing_loadport = False
    '                            If Not String.Compare(att.Attributes("value").Value, svc.Port) = 0 Then
    '                                changes = True
    '                                changeLog.LogData.AppendFormat(
    '                                    "Updating setting on {2} [{0}]={1}",
    '                                    att.Attributes("key").Value.ToString.ToLower,
    '                                    svc.Port,
    '                                    targetWeb.Endpoint
    '                                ).AppendLine()
    '                                att.Attributes("value").Value = svc.Port
    '                            End If

    '                        Case "service"
    '                            isMissing_service = False
    '                            If String.Compare(att.Attributes("value").Value, svc.Host, True) = 0 Then
    '                                For Each cn As ConfigurationElement In cnSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)
    '                                    If String.Compare(cn.Attributes("name").Value, "priority", True) = 0 Then
    '                                        f = True
    '                                        If Not String.Compare(cn.Attributes("connectionString").Value, DSN(svc.PriorityDB), True) = 0 Then
    '                                            changes = True
    '                                            changeLog.LogData.AppendFormat(
    '                                                "Updating {1} DSN=[{0}]",
    '                                                DSN(svc.PriorityDB),
    '                                                targetWeb.Endpoint
    '                                            ).AppendLine()
    '                                            cn.Attributes("connectionString").Value = DSN(svc.PriorityDB)
    '                                        End If
    '                                        Exit For
    '                                    End If
    '                                Next

    '                                If Not f Then
    '                                    changes = True
    '                                    changeLog.LogData.AppendFormat(
    '                                        "Adding {1} DSN=[{0}]",
    '                                        DSN(svc.PriorityDB),
    '                                        targetWeb.Endpoint
    '                                    ).AppendLine()

    '                                    Dim addelement As ConfigurationElement = cnSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual)
    '                                    addelement("name") = "priority"
    '                                    addelement("connectionString") = DSN(svc.PriorityDB)
    '                                    cnSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
    '                                End If
    '                            End If

    '                        Case "environment"
    '                            isMissing_environment = False

    '                        Case "loadingtimeout"
    '                            isMissing_loadingtimeout = False

    '                    End Select

    '                Next

    '                If isMissing_loadport Then
    '                    changes = True
    '                    changeLog.LogData.AppendFormat(
    '                        "Adding Setting on {2} [{0}]={1}",
    '                        "loadport",
    '                        svc.Port,
    '                        targetWeb.Endpoint
    '                    ).AppendLine()
    '                    Dim addelement As ConfigurationElement = appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
    '                    addelement("key") = "loadport"
    '                    addelement("value") = svc.Port
    '                    appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
    '                End If

    '                If isMissing_service Then
    '                    changes = True
    '                    reload = True
    '                    changeLog.LogData.AppendFormat(
    '                        "Adding Setting on {2} [{0}]={1}",
    '                        "service",
    '                        svc.Host,
    '                        targetWeb.Endpoint
    '                    ).AppendLine()
    '                    Dim addelement As ConfigurationElement = appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
    '                    addelement("key") = "service"
    '                    addelement("value") = svc.Host
    '                    appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
    '                End If

    '                If isMissing_environment Then
    '                    changes = True
    '                    reload = True
    '                    changeLog.LogData.AppendFormat(
    '                        "Adding Setting on {2} [{0}]={1}",
    '                        "environment",
    '                        TryCast(svc.First.Value, PriEnv).Name,
    '                        targetWeb.Endpoint
    '                    ).AppendLine()
    '                    Dim addelement As ConfigurationElement = appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
    '                    addelement("key") = "environment"
    '                    addelement("value") = TryCast(svc.First.Value, PriEnv).Name
    '                    appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
    '                End If

    '                If isMissing_loadingtimeout Then
    '                    changes = True
    '                    reload = True
    '                    changeLog.LogData.AppendFormat(
    '                        "Adding Setting on {2} [{0}]={1}",
    '                        "loadingtimeout",
    '                        "120",
    '                        targetWeb.Endpoint
    '                    ).AppendLine()
    '                    Dim addelement As ConfigurationElement = appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).CreateElement("add")
    '                    addelement("key") = "loadingtimeout"
    '                    addelement("value") = "120"
    '                    appSettings(sm.Sites(targetWeb.SiteName), targetWeb.virtual).Add(addelement)
    '                End If

    '            Next

    '        Catch ex As Exception
    '            changeLog.setException(ex)

    '        Finally
    '            If changes Then
    '                sm.CommitChanges()
    '                changeLog.LogData.AppendFormat("Commiting changes.", "").AppendLine()
    '                changeLog.EntryType = LogEntryType.SuccessAudit
    '            End If
    '            If reload Then ConfigLoader(svc, changeLog)

    '        End Try

    '    End Using

    'End Sub

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property ModuleVersion As Version
        Get
            Return Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property

    Public Overrides Function useCpl(ByRef o As Object, ParamArray args() As String) As Object
        Select Case UBound(args)
            Case 1
                Return New cplPropertyPage(TryCast(o, oWebRelay))

            Case 2
                For Each web As PriWeb In TryCast(o, oWebRelay).values
                    If String.Compare(web.Endpoint, args(2), True) = 0 Then
                        Return New cplPropertyPage(TryCast(web, PriWeb))
                    End If
                Next
                Return Nothing

            Case 4
                Select Case args(3).ToLower
                    Case "feeds"
                        For Each web As PriWeb In TryCast(o, oWebRelay).values
                            If String.Compare(web.Endpoint, args(2), True) = 0 Then
                                For Each f As EndPoint In web.siteFeeds
                                    If String.Compare(f.Name, args(4), True) = 0 Then
                                        Return New cplFeedControl(f)
                                    End If
                                Next
                            End If
                        Next
                        Return Nothing

                    Case "handlers"
                        For Each web As PriWeb In TryCast(o, oWebRelay).values
                            If String.Compare(web.Endpoint, args(2), True) = 0 Then
                                For Each f As EndPoint In web.siteHandlers
                                    If String.Compare(f.Name, args(4), True) = 0 Then
                                        Return New cplHanderControl(f)
                                    End If
                                Next
                            End If
                        Next
                        Return Nothing

                    Case Else
                        Return Nothing

                End Select

            Case Else
                Return Nothing

        End Select

    End Function

    Public Overrides Sub ContextMenu(ByRef sender As Object, ByRef e As CancelEventArgs, ByRef p As oServiceBase, ParamArray args() As String)

        Dim ThisRelay As oWebRelay = TryCast(p, oWebRelay)

        Select Case UBound(args)
            Case 1
                With TryCast(sender, ContextMenuStrip).Items
                    .Clear()
                    .Add("Stop service", Nothing, AddressOf ThisRelay.hStopClick)
                    .Add("Restart service", Nothing, AddressOf ThisRelay.hRestartClick)
                End With

            Case Else
                e.Cancel = True

        End Select

    End Sub

#Region "Tree"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.ppq)
            ret.Add("host", My.Resources.http_file_server)
            ret.Add("feeds", My.Resources.Feeds)
            ret.Add("handlers", My.Resources.handlers)
            ret.Add("feed", My.Resources.feed)
            Return ret
        End Get
    End Property

    Public Overrides Sub DrawTree(ByRef Parent As TreeNode, ByRef MEF As Object, ByVal p As oServiceBase, ByRef IconList As Dictionary(Of String, Integer))
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag(p))
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag(p), Name, IconList(Name), IconList(Name))
            Else
                If DateDiff(DateInterval.Minute, p.LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            Dim tags As New List(Of String)
            For Each host As Object In TryCast(p, oWebRelay).values
                With TryCast(host, PriWeb)
                    .DrawTree(p, this, IconList)
                    tags.Add(.TreeTag)
                End With
            Next

            For Each tv As TreeNode In this.Nodes
                If Not tags.Contains(tv.Name) Then
                    this.Nodes.Remove(tv)
                End If
                ' 
            Next

        End With

    End Sub

#End Region

#End Region

End Class
