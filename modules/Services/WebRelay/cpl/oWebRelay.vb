Imports System.Xml
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class oWebRelay : Inherits oService

#Region "Constructor"

    Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)
        MyBase.New(Host, Name, ServiceType, port)

    End Sub

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        MyBase.Update(Service)
        With TryCast(Service, oWebRelay)
            For Each web As PriWeb In Service.values
                If Not Keys.Contains(web.Endpoint) Then
                    Me.Add(web.Endpoint, web)
                Else
                    With TryCast(Me(web.Endpoint), PriWeb)
                        .Settings = web.Settings
                        .siteFeeds = web.siteFeeds
                        .siteHandlers = web.siteHandlers
                    End With
                End If
            Next
            Dim delEP As New List(Of String)
            For Each web As PriWeb In Me.values
                If Not .Keys.Contains(web.Endpoint) Then
                    delEP.Add(web.Endpoint)
                End If
            Next
            For Each del As String In delEP
                Me.Remove(del)
            Next
        End With
    End Sub

#End Region

#Region "Properties"

#End Region

#Region "Context Menu Handlers"

    Public Sub hStopClick(Sender As Object, e As EventArgs)
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

    Public Sub hRestartClick(Sender As Object, e As EventArgs)
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

    Overloads Sub SendCmd(Sender As Object, e As CmdEventArgs)
        MyBase.SendCmd(Sender, e)
    End Sub

End Class
