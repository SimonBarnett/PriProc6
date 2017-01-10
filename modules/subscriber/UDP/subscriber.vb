Imports System.ComponentModel.Composition
Imports PriPROC6.Interface.Subsciber
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage
Imports System.Xml
Imports PriPROC6.PriSock
Imports PriPROC6.regConfig
Imports System.Drawing

<Export(GetType(SubscribeDef))>
<ExportMetadata("Name", "broadcast")>
<ExportMetadata("defaultStart", True)>
<ExportMetadata("Console", False)>
<ExportMetadata("EntryType", 31)>
<ExportMetadata("Verbosity", 99)>
<ExportMetadata("Source", 7)>
Public Class Subscriber : Inherits SubscriberBase

    Public Overrides Sub svcStart(ByRef Log As objBase)
        With thisConfig
            If .regValue(False, "broadcastport").ToString.Length = 0 Then
                .regValue(False, "broadcastport") = "8100"
            End If
        End With
    End Sub

    Public Overrides Sub NewEntry(Entry As msgBase)
        Try
            With Entry
                Using uclient As New uClient(thisConfig.regValue(False, "broadcastport"), eBroadcastType.bcPrivate)
                    uclient.Send(Entry.toByte)
                End Using
            End With

        Catch ex As Exception

        End Try

    End Sub

#Region "Discovery Message"

    Public Overrides Sub writeXML(ByRef outputStream As XmlWriter)
        With outputStream
            .WriteElementString("broadcastport", thisConfig.regValue(False, "broadcastport"))
        End With
    End Sub

    Public Overrides Function readXML(ByRef Service As XmlNode) As oServiceBase
        Dim ret As New oSubBroadcast(Service)
        With ret
            ret.broadcastport = Service.SelectSingleNode("broadcastport").InnerText
        End With
        Return ret
    End Function

#End Region

#Region "control panel"

    Public Overrides ReadOnly Property thisIcon As Dictionary(Of String, Icon)
        Get
            Dim ret As New Dictionary(Of String, Icon)
            ret.Add(Me.Name, My.Resources.SubIRC)
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
