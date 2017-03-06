Imports System.Xml
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PriPROC6.Interface.Message
Imports System.IO

Public Class PriEnv

    Sub New(Name As String)
        _Name = Name
    End Sub

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    ReadOnly Property TreeTag As String
        Get
            Return String.Format("{0}\{1}\{2}\{3}", _loader.Host, _loader.Name, "env", Name)
        End Get
    End Property

    Private _loader As oLoader
    Public Sub DrawTree(ByRef loader As oLoader, ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        _loader = loader
        With Parent
            Dim this As TreeNode = .Nodes(TreeTag)
            If IsNothing(this) Then
                this = .Nodes.Add(TreeTag, Name, IconList("environment"), IconList("environment"))
            End If

            'Dim dir As New DirectoryInfo(
            '    Path.Combine(
            '        Path.Combine(_loader.PriorityShare, "system\queue"),
            '        Name
            '    )
            ')
            'If dir.Exists Then
            '    For Each d As DirectoryInfo In dir.GetDirectories
            '        Dim proc As TreeNode = this.Nodes(String.Format("{0}\{1}", TreeTag, d.Name))
            '        If IsNothing(proc) Then
            '            this.Nodes.Add(String.Format("{0}\{1}", TreeTag, d.Name), d.Name, IconList("procedure"), IconList("procedure"))
            '        End If
            '    Next
            'End If
        End With
    End Sub

End Class