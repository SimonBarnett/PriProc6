Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

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

#Region "Context Menu Handlers"

    Public Sub hStopClick(Sender As Object, e As EventArgs)
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

End Class