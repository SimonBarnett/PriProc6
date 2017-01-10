Imports System.ComponentModel.Composition
Imports Priproc6.service
Imports Priproc6.svcMessages

<Export(GetType(svc_hanger))>
Public Class Hanger : Implements svc_hanger

    Public ReadOnly Property BroadcastPort As Integer Implements svc_hanger.BroadcastPort
        Get
            Return 8090
        End Get
    End Property

#Region "Parent Reference"

    Private _host As CompositionHost
    Public Sub setParent(ByRef p As CompositionHost) Implements svc_hanger.setParent
        _host = p
        _msgFactory = New msgFactory(_host.Messages)
    End Sub

    Public _msgFactory As msgFactory
    Public ReadOnly Property msgFactory As msgFactory Implements svc_hanger.msgFactory
        Get
            Return _msgFactory
        End Get
    End Property

    Public ReadOnly Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops)) Implements svc_hanger.Modules
        Get
            Return _host.Modules
        End Get
    End Property

    Public ReadOnly Property LoadedModules As IEnumerable(Of Lazy(Of svcDef, svcDefprops)) Implements svc_hanger.LoadedModules
        Get
            Dim ret As New List(Of Lazy(Of svcDef, svcDefprops))
            For Each svr As Lazy(Of svcDef, svcDefprops) In _host.Modules
                Select Case svr.Value.svc_state
                    Case eServiceState.stopped, eServiceState.notresponding
                    Case Else
                        ret.Add(svr)
                End Select
            Next
            Return ret
        End Get
    End Property

#End Region

#Region "Start / Stop Modules"

    Public Function svc_start() As Exception Implements svc_hanger.svc_start
        Try
            For Each svr As Lazy(Of svcDef, svcDefprops) In _host.Modules
                With svr.Value

                    .ServiceName = svr.Metadata.serviceName
                    .defaultPort = svr.Metadata.defaultPort
                    .tcp = svr.Metadata.tcp
                    .udp = svr.Metadata.udp

                    .setParent(Me)
                    If .Start Then logq.Enqueue(.svc_start())

                End With
            Next
            Return Nothing

        Catch ex As Exception
            Return ex

        End Try

    End Function

    Public Function svc_stop() As Exception Implements svc_hanger.svc_stop
        Try
            For Each svr As Lazy(Of svcDef, svcDefprops) In _host.Modules
                With svr.Value
                    logq.Enqueue(.svc_stop())

                End With
            Next
            Return Nothing

        Catch ex As Exception
            Return ex

        End Try

    End Function

#End Region

#Region "Logging"

    Private _logq As New Queue(Of Byte())
    Public Property logq As Queue(Of Byte()) Implements svc_hanger.logq
        Get
            Return _logq
        End Get
        Set(value As Queue(Of Byte()))
            _logq = value
        End Set
    End Property

#End Region

End Class
