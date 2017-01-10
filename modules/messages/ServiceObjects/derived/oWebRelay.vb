Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message

Public Class oWebRelay : Inherits oService

#Region "Constructor"

    Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)
        MyBase.New(Host, Name, ServiceType, port)

    End Sub

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

#End Region

#Region "Properties"

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
                this = .Nodes.Add(TreeTag, Name, IconList("webrelay"), IconList("webrelay"))
            Else
                If DateDiff(DateInterval.Minute, LastSeen, Now) > 2 Then
                    .Nodes.Remove(this)
                    Exit Sub
                End If
            End If

            For Each host As Object In Me.values
                With TryCast(host, PriWeb)
                    .DrawTree(Me, this, IconList)
                End With
            Next

        End With
    End Sub

#End Region

#Region "Control Panel"

    Public Overrides Function useCpl(ByRef pnlName As String, ParamArray args() As String) As Object
        Select Case UBound(args)
            Case 1
                pnlName = "webrelay"
                Return Me

            Case 2
                pnlName = "endpoint"
                For Each web As PriWeb In values
                    If String.Compare(web.Endpoint, args(2), True) = 0 Then
                        Return web
                    End If
                Next
                Return Nothing

            Case 4
                Select Case args(3).ToLower
                    Case "feeds"
                        pnlName = "feed"
                        For Each web As PriWeb In values
                            If String.Compare(web.Endpoint, args(2), True) = 0 Then
                                For Each f As EndPoint In web.siteFeeds
                                    If String.Compare(f.Name, args(4), True) = 0 Then
                                        Return f
                                    End If
                                Next
                            End If
                        Next
                        Return Nothing

                    Case "handlers"
                        pnlName = "handler"
                        For Each web As PriWeb In values
                            If String.Compare(web.Endpoint, args(2), True) = 0 Then
                                For Each f As EndPoint In web.siteHandlers
                                    If String.Compare(f.Name, args(4), True) = 0 Then
                                        Return f
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

    Overloads Sub SendCmd(Sender As Object, e As CmdEventArgs)
        MyBase.SendCmd(Sender, e)
    End Sub

End Class
