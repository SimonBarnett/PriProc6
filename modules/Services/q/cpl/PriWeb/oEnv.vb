Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms

Public Class oEnv

    Public Sub New(ByRef Parent As oPriWeb, Name As String)
        _Parent = Parent
        _Name = Name

    End Sub

#Region "Properties"

    Private _Parent As oPriWeb
    <Browsable(False)>
    Public Property Parent As oPriWeb
        Get
            Return _Parent
        End Get
        Set(value As oPriWeb)
            _Parent = value
        End Set
    End Property

    Private _Name As String
    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The name of the company in Priority.")>
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The path to save data send to the company.")>
    Public ReadOnly Property qFolder As DirectoryInfo
        Get
            Return New DirectoryInfo(Path.Combine(_Parent.qfolder.FullName, Name))
        End Get
    End Property

    <CategoryAttribute("Priority Settings"),
    Browsable(True),
    [ReadOnly](True),
    BindableAttribute(False),
    DefaultValueAttribute(""),
    DesignOnly(False),
    DescriptionAttribute("The URL for oData transactions.")>
    Public ReadOnly Property oDataPath As String
        Get
            Return String.Format(
                "{0}/odata/Priority/{1}/{2}",
                _Parent.Hostname,
                _Parent.tabini,
                _Name
            )
        End Get
    End Property

#End Region

#Region "Control Panel"

    <Browsable(False)>
    ReadOnly Property TreeTag(parent As TreeNode) As String
        Get
            Return String.Format("{0}", parent.Name)
        End Get
    End Property

    Public Sub DrawTree(ByRef env As oEnv, ByRef Parent As TreeNode, ByRef IconList As Dictionary(Of String, Integer))
        With Parent

            Dim del As New List(Of String)
            For Each di As DirectoryInfo In New DirectoryInfo(env.qFolder.FullName).GetDirectories
                Dim diNode As TreeNode
                del.Add(String.Format("{0}\{1}", TreeTag(Parent), di.Name))
                If IsNothing(.Nodes(String.Format("{0}\{1}", TreeTag(Parent), di.Name))) Then
                    diNode = .Nodes.Add(String.Format("{0}\{1}", TreeTag(Parent), di.Name), di.Name, IconList("procedure"), IconList("procedure"))
                Else
                    diNode = .Nodes(String.Format("{0}\{1}", TreeTag(Parent), di.Name))
                End If

            Next
            For Each tv As TreeNode In Parent.Nodes
                If Not tv Is Nothing Then
                    If Not del.Contains(tv.Name) Then
                        tv.Remove()
                    End If
                End If
            Next

        End With
    End Sub

#End Region
End Class
