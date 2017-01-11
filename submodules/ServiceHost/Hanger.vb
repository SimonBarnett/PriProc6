Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting

Imports Priproc6.Interface.Message
Imports PriPROC6.svcMessage
Imports Priproc6.Interface.Subsciber
Imports Priproc6.Interface.Service

Public Class Hanger : Implements svc_hanger : Implements IDisposable

    <Runtime.InteropServices.DllImport("kernel32.dll")>
    Public Shared Function GetConsoleWindow() As IntPtr
    End Function

    Private _closing As Boolean = False
    Private _container As CompositionContainer

    <ImportMany()>
    Private Property _Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))

    <ImportMany()>
    Private Property _Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData))

    <ImportMany()>
    Private Property _Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))

#Region "Construct / Dispose"

    Public Sub New(
        ByRef HostAssembly As Reflection.Assembly,
        Optional ByVal ModulesFolder As IO.DirectoryInfo = Nothing
    ) _

        'An aggregate catalog that combines multiple catalogs
        Dim catalog = New AggregateCatalog()

        'Adds all the parts found in the same assembly as the Program class
        catalog.Catalogs.Add(New AssemblyCatalog(HostAssembly))

        If Not ModulesFolder Is Nothing Then
            catalog.Catalogs.Add(New DirectoryCatalog(ModulesFolder.FullName))
        End If

        'Create the CompositionContainer with the parts in the catalog
        _container = New CompositionContainer(catalog)

        'Fill the imports of this object

        Dim StartExecption As Exception = Nothing
        Try
            _container.ComposeParts(Me)

            StartExecption = svc_start()
            If Not IsNothing(StartExecption) Then
                Throw StartExecption
            End If

        Catch ex As Exception
            Console.WriteLine(ex.ToString)

        End Try

    End Sub

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region

#End Region

#Region "Public Properties"

    Private _logq As New Queue(Of Byte())
    Public Property logq As Queue(Of Byte()) Implements svc_hanger.logq
        Get
            Return _logq
        End Get
        Set(value As Queue(Of Byte()))
            _logq = value
        End Set
    End Property

    Public _msgFactory As msgFactory = Nothing
    Public ReadOnly Property msgFactory As msgFactory Implements svc_hanger.msgFactory
        Get
            If IsNothing(_msgFactory) Then
                _msgFactory = New msgFactory(_Messages)
            End If
            Return _msgFactory
        End Get
    End Property

    Public ReadOnly Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops)) Implements svc_hanger.Modules
        Get
            Return _Modules
        End Get
    End Property

    Public ReadOnly Property Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops)) Implements svc_hanger.Subscribers
        Get
            Return _Subscribers
        End Get
    End Property

#End Region

#Region "Start / Stop Modules"

    Public ReadOnly Property Count() As Integer Implements svc_hanger.Count
        Get
            Dim i As Integer = 0
            For Each svr As Lazy(Of svcDef, svcDefprops) In Modules
                With svr.Value
                    If .svc_state = eServiceState.started Then
                        i += 1
                    End If
                End With
            Next
            Return i
        End Get
    End Property

    Public Function svc_start() As Exception Implements svc_hanger.svc_start
        Try

            trdLog = New System.Threading.Thread(AddressOf NotifySubscribers)
            With trdLog
                .Name = String.Format(
                "{0}_LogQ",
                Environment.MachineName
            )
                .IsBackground = True
                .Start()

            End With

            For Each subscr As Lazy(Of SubscribeDef, SubscribeDefprops) In _Subscribers
                With subscr.Value
                    .setParent(Me, subscr.Metadata) 'msgFactory, logq
                    If .Console Then
                        If GetConsoleWindow() <> IntPtr.Zero Then
                            logq.Enqueue(msgFactory.EncodeRequest("log", .svc_start()))

                        End If
                    Else
                        If .Start Then
                            logq.Enqueue(msgFactory.EncodeRequest("log", .svc_start()))

                        End If
                    End If

                    Threading.Thread.Sleep(1)

                End With
            Next

            For Each svr As Lazy(Of svcDef, svcDefprops) In _Modules
                With svr.Value
                    .setParent(Me, svr.Metadata)
                    If .Start Then logq.Enqueue(msgFactory.EncodeRequest("log", .svc_start()))
                    Threading.Thread.Sleep(1)

                End With
            Next

            Return Nothing

        Catch ex As Exception
            Return ex

        End Try

    End Function

    Public Function svc_stop() As Exception Implements svc_hanger.svc_stop
        Try
            For Each svr As Lazy(Of svcDef, svcDefprops) In Modules
                With svr.Value
                    If .svc_state = eServiceState.started Then
                        logq.Enqueue(msgFactory.EncodeRequest("log", .svc_stop()))
                    End If
                End With
            Next

            Do Until Count = 0 And _logq.Count = 0
                Threading.Thread.Sleep(1000)
            Loop

            Dim scount As Integer
            Do
                scount = 0
                For Each subscr As Lazy(Of SubscribeDef, SubscribeDefprops) In _Subscribers
                    With subscr.Value
                        Select Case .svc_state
                            Case eServiceState.started
                                logq.Enqueue(msgFactory.EncodeRequest("log", .svc_stop()))
                                scount += 1

                            Case eServiceState.stopping
                                scount += 1

                        End Select
                    End With
                Next
            Loop Until scount = 0

            Do While logq.Count > 0
                Threading.Thread.Sleep(1000)
            Loop
            _closing = True

            Return Nothing

        Catch ex As Exception
            Return ex

        End Try

    End Function

#End Region

#Region "Logging"

    Private trdLog As System.Threading.Thread
    Private Sub NotifySubscribers()
        Do
            Do While _logq.Count > 0
                Dim msg As Byte() = _logq.Dequeue
                Dim scount As Integer = 0
                For Each subscr As Lazy(Of SubscribeDef, SubscribeDefprops) In _Subscribers
                    With subscr.Value
                        If .svc_state = eServiceState.started Then
                            scount += 1
                            .NewMessage(msg)
                        End If
                    End With
                Next
                If scount = 0 Then
                    TryCast(msgFactory.Decode(msg).thisObject, oMsgLog).toConsole()
                End If
            Loop

            Threading.Thread.Sleep(100)

        Loop While Not _closing

    End Sub

#End Region

End Class
