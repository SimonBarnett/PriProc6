Imports System.Xml
Imports System.ComponentModel

Public Enum eSvcType
    Service
    Subscriber
End Enum

Public Class CmdEventArgs : Inherits EventArgs

    Sub New(Server As String, Port As Integer, Optional refresh As Boolean = False)
        _Server = Server
        _port = Port
        _refresh = refresh
    End Sub

    Private _refresh As Boolean
    Public Property Refresh As Boolean
        Get
            Return _refresh
        End Get
        Set(value As Boolean)
            _refresh = value
        End Set
    End Property

    Private _Server As String
    Public Property Server As String
        Get
            Return _Server
        End Get
        Set(value As String)
            _Server = value
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

    Private _message As Object
    Public Property Message As Object
        Get
            Return _message
        End Get
        Set(value As Object)
            _message = value
        End Set
    End Property

    Private _ErrCode As Integer
    Public Property errCode As Integer
        Get
            Return _ErrCode
        End Get
        Set(value As Integer)
            _ErrCode = value
        End Set
    End Property

End Class

Public MustInherit Class oServiceBase : Inherits oDictionary

    Public Event onSendCmd(Sender As Object, e As CmdEventArgs)
    Protected Sub SendCmd(Sender As Object, e As CmdEventArgs)
        RaiseEvent onSendCmd(Sender, e)
    End Sub

#Region "Consructor"

    Public Sub New(ByRef node As XmlNode)

        _LastSeen = Now
        _Host = node.Attributes("host").Value
        _Name = node.Attributes("name").Value
        _Version = node.Attributes("version").Value
        _BroacastXML = node.ParentNode

        Select Case node.Attributes("svctype").Value.ToLower
            Case "service"
                _ServiceType = eSvcType.Service
            Case "subscriber"
                _ServiceType = eSvcType.Subscriber
        End Select

    End Sub

    Public Sub New(Host As String, Name As String, ServiceType As eSvcType)

        _LastSeen = Now
        _Host = Host
        _Name = Name
        _ServiceType = ServiceType

    End Sub

    Public Overridable Sub Update(ByRef Service As oServiceBase)
        With Service
            _Host = .Host
            _Name = .Name
            _Version = .ModuleVersion
            _ServiceType = .ServiceType
            _LastSeen = Now
            _BroacastXML = .BroacastXML
        End With
    End Sub

#End Region

#Region "Properties"

    Private _mmc As Boolean = False
    <Browsable(False)>
    Public Property mmc As Boolean
        Get
            Return _mmc
        End Get
        Set(value As Boolean)
            _mmc = value
        End Set
    End Property

    Private _Parent As oServiceBase
    <Browsable(False)>
    Public Property Parent As oServiceBase
        Get
            Return _Parent
        End Get
        Set(value As oServiceBase)
            _Parent = value
        End Set
    End Property

    Private _BroacastXML As XmlNode
    <Browsable(False)>
    Public Property BroacastXML As XmlNode
        Get
            Return _BroacastXML
        End Get
        Set(value As XmlNode)
            _BroacastXML = value
        End Set
    End Property

    Private _Host As String
    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The Netbios name of the machine on which this service resides.")>
    Public Property Host As String
        Get
            Return _Host
        End Get
        Set(value As String)
            _Host = value
        End Set
    End Property

    Private _Version As String
    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The current version number of the module.")>
    Public Property ModuleVersion As String
        Get
            Return _Version
        End Get
        Set(value As String)
            _Version = value
        End Set
    End Property

    Private _Name As String
    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The name of the service.")>
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Private _ServiceType As eSvcType
    <Browsable(False)>
    Public Property ServiceType As eSvcType
        Get
            Return _ServiceType
        End Get
        Set(value As eSvcType)
            _ServiceType = value
        End Set
    End Property

    Private _LastSeen As DateTime
    <Browsable(False)>
    Public Property LastSeen As DateTime
        Get
            Return _LastSeen
        End Get
        Set(value As DateTime)
            _LastSeen = value
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property IsTimedOut As Boolean
        Get
            Return DateDiff(DateInterval.Minute, LastSeen, Now) > 1
        End Get
    End Property

#End Region

End Class