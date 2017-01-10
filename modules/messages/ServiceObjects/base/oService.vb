Imports System.Xml
Imports System.Windows.Forms
Imports System.ComponentModel
Imports PriPROC6.Interface.Message

Public MustInherit Class oService : Inherits oServiceBase

#Region "Constructor"

    Public Sub New(ByRef node As XmlNode)
        MyBase.New(node)
        _port = node.Attributes("port").Value

    End Sub

    Public Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)
        MyBase.New(Host, Name, ServiceType)
        _port = port

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oService)
            MyBase.Update(Service)
            _port = .Port
        End With
    End Sub

#End Region

#Region "Properties"

    Private _port As Integer
    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The port on which this service is running.")>
    Public Property Port As Integer
        Get
            Return _port
        End Get
        Set(value As Integer)
            If mmc Then

                Dim e As CmdEventArgs
                If IsNothing(TryCast(Parent, oService)) Then
                    e = New CmdEventArgs(Me.Host, Me.Port)
                Else
                    e = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                End If

                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("port", value)
                End With
                MyBase.SendCmd(Me, e)

            End If
            _port = value

        End Set
    End Property

#End Region

End Class