Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message

Public Class oDiscovery : Inherits oService

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oDiscovery)
            MyBase.Update(Service)
            _BroadcastDelay = .BroadcastDelay
            _Dormant = .Dormant
        End With
    End Sub

#End Region

#Region "Properties"

    Private _Dormant As New Dictionary(Of String, eSvcType)
    <Browsable(False)>
    Public Property Dormant As Dictionary(Of String, eSvcType)
        Get
            Return _Dormant
        End Get
        Set(value As Dictionary(Of String, eSvcType))
            _Dormant = value
        End Set
    End Property

    Private _BroadcastDelay As Integer
    <CategoryAttribute("Settings"),
    Browsable(True),
    [ReadOnly](False),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The delay in seconds between discovery broadcasts.")>
    Public Property BroadcastDelay As Integer
        Get
            Return _BroadcastDelay
        End Get
        Set(value As Integer)
            _BroadcastDelay = value
            If mmc Then
                Dim e As CmdEventArgs = New CmdEventArgs(Host, Port)
                e.Message = New oMsgCmd
                With TryCast(e.Message, oMsgCmd).Args
                    .Add("service", Me.Name)
                    .Add("BroadcastDelay", _BroadcastDelay)
                End With
                MyBase.SendCmd(Me, e)
            End If
        End Set
    End Property

    Private _mefdir As String
    <CategoryAttribute("Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The path to the discovery servers MEF objects.")>
    Public Property mefdir As String
        Get
            Return _mefdir
        End Get
        Set(value As String)
            _mefdir = value
        End Set
    End Property

#End Region

#Region "Context Menu Handlers"

    Public Sub hStartClick(Sender As Object, e As EventArgs)
        Dim s As CmdEventArgs = New CmdEventArgs(Host, Port, True)
        s.Message = New oMsgCmd
        With TryCast(s.Message, oMsgCmd).Args
            .Add("service", TryCast(Sender, ToolStripItem).Tag)
            .Add("state", "start")
        End With
        MyBase.SendCmd(Me, s)
    End Sub

    Public Sub hInstallClick(Sender As Object, e As EventArgs)

        Dim OpenFileDialog As New OpenFileDialog
        With OpenFileDialog
            .DefaultExt = "dll"
            .FileName = ""
            If .ShowDialog = DialogResult.OK Then
                Dim fn As New IO.FileInfo(.FileName)
                If fn.Exists And String.Compare(fn.Extension, ".dll", True) = 0 Then
                    Try
                        IO.File.Copy(.FileName, IO.Path.Combine(_mefdir, .SafeFileName))

                    Catch ex As Exception

                    End Try

                Else
                    MsgBox("Invalid File.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Error.")

                End If
            End If
        End With

        Dim s As CmdEventArgs = New CmdEventArgs(Host, Port, True)
        s.Message = New oMsgCmd
        With TryCast(s.Message, oMsgCmd).Args
            .Add("service", "discovery")
            .Add("install", "start")
        End With
        MyBase.SendCmd(Me, s)
    End Sub

#End Region

End Class