Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message

Imports PriPROC6.svcMessage

Public MustInherit Class oSubscriber : Inherits oServiceBase

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

        Dim filter As XmlNode = node.SelectSingleNode("filter")
        EntryType = filter.Attributes("EntryType").Value
        Verbosity = filter.Attributes("Verbosity").Value
        Source = filter.Attributes("Source").Value

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oSubscriber)
            MyBase.Update(Service)
            EntryType = .EntryType
            Verbosity = .Verbosity
            Source = .Source
        End With
    End Sub

    'Err = 1
    'Information = 4
    'FailureAudit = 16
    'SuccessAudit = 8
    'Warning = 2
    Private _EntryType As Integer
    <Browsable(False)>
    Public Property EntryType As Integer
        Get
            Return _EntryType
        End Get
        Set(value As Integer)
            _EntryType = value
            Dim i As Integer = value
            If i >= 16 Then
                _FailureAudit = True
                i -= 16
            Else
                _FailureAudit = False
            End If

            If i >= 8 Then
                _SuccessAudit = True
                i -= 8
            Else
                _SuccessAudit = False
            End If

            If i >= 4 Then
                _Information = True
                i -= 4
            Else
                _Information = False
            End If

            If i >= 2 Then
                i -= 2
                _Warning = True
            Else
                _Warning = False
            End If

            If i >= 1 Then
                i -= 1
                _Err = True
            Else
                _Err = False
            End If

        End Set
    End Property

    'Normal = 1
    'Verbose = 10
    'VeryVerbose = 50
    'Arcane = 99
    Private _Verbosity As Integer
    <Browsable(False)>
    Public Property Verbosity As Integer
        Get
            Return _Verbosity
        End Get
        Set(value As Integer)
            _Verbosity = value
            Select Case value
                Case 1
                    _MessageVerbosity = "Normal"
                Case 10
                    _MessageVerbosity = "Verbose"
                Case 50
                    _MessageVerbosity = "VeryVerbose"
                Case Else
                    _MessageVerbosity = "Arcane"

            End Select
        End Set
    End Property


    'APPLICATION = 1
    'SYSTEM = 2
    'WEB = 4
    Private _Source As Integer
    <Browsable(False)>
    Public Property Source As Integer
        Get
            Return _Source
        End Get
        Set(value As Integer)
            _Source = value
            Dim i As Integer = value

            If i >= 4 Then
                _WEB = True
                i -= 4
            Else
                _WEB = False
            End If

            If i >= 2 Then
                i -= 2
                _SYSTEM = True
            Else
                _SYSTEM = False
            End If

            If i >= 1 Then
                i -= 1
                _APPLICATION = True
            Else
                _APPLICATION = False
            End If
        End Set
    End Property

#Region "verbosity"

    Private _MessageVerbosity As String
    <TypeConverter(GetType(Prop_Verbosity)),
    CategoryAttribute("Filter"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Lowest message verbosity to relay.")>
    Public Property MessageVerbosity As String
        Get
            Return _MessageVerbosity
        End Get
        Set(value As String)
            _MessageVerbosity = value
            Select Case value
                Case "Normal"
                    _Verbosity = 1
                Case "Verbose"
                    _Verbosity = 10
                Case "VeryVerbose"
                    _Verbosity = 50
                Case Else
                    _Verbosity = 99
            End Select

            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("Verbosity", _Verbosity)
                End With
                MyBase.SendCmd(Me, e)

            End If
        End Set
    End Property

#End Region

#Region "Entry Types"

    Private _FailureAudit As Boolean
    <CategoryAttribute("Entry Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [FailureAudit] messages.")>
    Public Property FailureAudit As Boolean
        Get
            Return _FailureAudit
        End Get
        Set(value As Boolean)
            _FailureAudit = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("EntryType", CalculateEntryType())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _SuccessAudit As Boolean
    <CategoryAttribute("Entry Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [SuccessAudit] messages.")>
    Public Property SuccessAudit As Boolean
        Get
            Return _SuccessAudit
        End Get
        Set(value As Boolean)
            _SuccessAudit = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("EntryType", CalculateEntryType())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _Information As Boolean
    <CategoryAttribute("Entry Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [Information] messages.")>
    Public Property Information As Boolean
        Get
            Return _Information
        End Get
        Set(value As Boolean)
            _Information = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("EntryType", CalculateEntryType())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _Warning As Boolean
    <CategoryAttribute("Entry Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [Warning] messages.")>
    Public Property Warning As Boolean
        Get
            Return _Warning
        End Get
        Set(value As Boolean)
            _Warning = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("EntryType", CalculateEntryType())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _Err As Boolean
    <CategoryAttribute("Entry Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [Error] messages.")>
    Public Property Err As Boolean
        Get
            Return _Err
        End Get
        Set(value As Boolean)
            _Err = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("EntryType", CalculateEntryType())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private Function CalculateEntryType() As Integer
        Dim ret As Integer = 0
        If _FailureAudit Then ret += 16
        If _SuccessAudit Then ret += 8
        If _Information Then ret += 4
        If _Warning Then ret += 2
        If _Err Then ret += 1
        _EntryType = ret
        Return ret
    End Function

#End Region

#Region "Source"

    Private _WEB As Boolean
    <CategoryAttribute("Source Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [WEB] messages.")>
    Public Property WEB As Boolean
        Get
            Return _WEB
        End Get
        Set(value As Boolean)
            _WEB = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("Source", CalculateLogSource())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _SYSTEM As Boolean
    <CategoryAttribute("Source Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [SYSTEM] messages.")>
    Public Property SYSTEM As Boolean
        Get
            Return _SYSTEM
        End Get
        Set(value As Boolean)
            _SYSTEM = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("Source", CalculateLogSource())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _APPLICATION As Boolean
    <CategoryAttribute("Source Types"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("Capture [APPLICATION] messages.")>
    Public Property APPLICATION As Boolean
        Get
            Return _APPLICATION
        End Get
        Set(value As Boolean)
            _APPLICATION = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Parent.Host, TryCast(Parent, oService).Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("Source", CalculateLogSource())
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private Function CalculateLogSource() As Integer
        Dim ret As Integer = 0
        If _WEB Then ret += 4
        If _SYSTEM Then ret += 2
        If _APPLICATION Then ret += 1
        _EntryType = ret
        Return ret
    End Function

#End Region

End Class