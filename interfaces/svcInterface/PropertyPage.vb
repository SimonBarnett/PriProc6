Imports System.ComponentModel

Public MustInherit Class PropertyPage

    Private _ServerName As String
    Private _ServicePort As Integer
    Private _ServiceTag As String
    Private _Version As String

    Public Sub New(ByVal ServerName As String, ByVal ServicePort As Integer, ByVal ServiceTag As String, ByVal Version As String)
        _ServerName = ServerName
        _ServicePort = ServicePort
        _ServiceTag = ServiceTag
        _Version = Version
    End Sub

    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Netbios Name of server")>
    Public Property ServerName() As String
        Get
            Return _ServerName
        End Get
        Set(ByVal Value As String)
            _ServerName = Value
        End Set
    End Property

    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The port number for this service")>
    Public Property ServicePort() As String
        Get
            Return _ServicePort
        End Get
        Set(ByVal Value As String)
            _ServicePort = Value
        End Set
    End Property

    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Service Type")>
    Public Property ServiceTag() As String
        Get
            Return _ServiceTag
        End Get
        Set(ByVal Value As String)
            _ServiceTag = Value
        End Set
    End Property

    <CategoryAttribute("Service"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The version number of the service handler")>
    Public Property Version() As String
        Get
            Return _Version
        End Get
        Set(ByVal Value As String)
            _Version = Value
        End Set
    End Property

End Class
