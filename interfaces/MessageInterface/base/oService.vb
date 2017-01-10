Imports System.Xml
Imports System.Windows.Forms

Public Enum eSvcType
    Service
    Subscriber
End Enum

Public MustInherit Class oService : Inherits Dictionary(Of String, Object)

    Public Sub New(ByRef node As XmlNode)

        _Host = node.Attributes("host").Value
        _Name = node.Attributes("name").Value
        Select Case node.Attributes("svctype").Value.ToLower
            Case "service"
                _ServiceType = eSvcType.Service
                _port = node.Attributes("port").Value
            Case "subscriber"
                _ServiceType = eSvcType.Subscriber
        End Select

    End Sub

    Public Sub New(Host As String, Name As String, ServiceType As eSvcType, port As Integer)

        _Host = Host
        _Name = Name
        _ServiceType = ServiceType
        _port = port

    End Sub

    Private _Version As String
    Public Property ModuleVersion As String
        Get
            Return _Version
        End Get
        Set(value As String)
            _Version = value
        End Set
    End Property

    Private _Host As String
    Public Property Host As String
        Get
            Return _Host
        End Get
        Set(value As String)
            _Host = value
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Private _ServiceType As eSvcType
    Public Property ServiceType As eSvcType
        Get
            Return _ServiceType
        End Get
        Set(value As eSvcType)
            _ServiceType = value
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

    Private _LastSeen As DateTime
    Public Property LastSeen As DateTime
        Get
            Return _LastSeen
        End Get
        Set(value As DateTime)
            _LastSeen = value
        End Set
    End Property

    Public MustOverride Sub DrawTree(ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))

    Public MustOverride ReadOnly Property TreeTag As String


End Class