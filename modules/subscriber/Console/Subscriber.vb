Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports System.Xml
Imports System.Drawing

<Export(GetType(SubscribeDef))>
<ExportMetadata("Name", "console")>
<ExportMetadata("defaultStart", True)>
<ExportMetadata("Console", True)>
<ExportMetadata("EntryType", 31)>
<ExportMetadata("Verbosity", 99)>
<ExportMetadata("Source", 7)>
Public Class Subscriber : Inherits SubscriberBase

    Public Overrides Sub NewEntry(Entry As msgBase)
        Try
            With Entry
                TryCast(.thisObject, oMsgLog).toConsole()
            End With

        Catch ex As Exception

        End Try

    End Sub

#Region "Discovery Message"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)

    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oSubConsole(Service)
        With ret

        End With
        Return ret
    End Function

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.subcmd)
            Return ret
        End Get
    End Property

    Public Overrides ReadOnly Property ModuleVersion As Version
        Get
            Return Reflection.Assembly.GetExecutingAssembly.GetName.Version
        End Get
    End Property

#End Region

End Class
