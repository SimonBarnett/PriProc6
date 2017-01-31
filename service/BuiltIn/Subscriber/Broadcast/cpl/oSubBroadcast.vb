Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class oSubBroadcast : Inherits oSubscriber

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oSubBroadcast)
            MyBase.Update(Service)
            _broadcastport = .broadcastport
        End With
    End Sub

#End Region

#Region "Properties"

    Private _broadcastport As Integer
    <CategoryAttribute("UDP Broadcast"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The port on which Broadcast messages are sent to the MMC.")>
    Public Property broadcastport As Integer
        Get
            Return _broadcastport
        End Get
        Set(value As Integer)
            _broadcastport = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("broadcastport", _broadcastport)
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

#End Region

End Class