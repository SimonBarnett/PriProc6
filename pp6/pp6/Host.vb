Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports System.IO

Imports Priproc6.Interface.Hanger
Imports Priproc6.Interface.Message
Imports Priproc6.Interface.Service
Imports Priproc6.Interface.Subsciber

Public Class Host : Implements IDisposable

    Dim _container As CompositionContainer

    '<Import(GetType(svc_hanger))>
    Public Property Hanger As svc_hanger

    <ImportMany()>
    Public Property Modules As IEnumerable(Of Lazy(Of svcDef, svcDefprops))

    <ImportMany()>
    Public Property Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData))

    <ImportMany()>
    Public Property Subscribers As IEnumerable(Of Lazy(Of SubscribeDef, SubscribeDefprops))

#Region "Composition Constructor"

    Public Sub New()

        'An aggregate catalog that combines multiple catalogs
        Dim catalog = New AggregateCatalog()

        'Adds all the parts found in the same assembly as the Program class
        catalog.Catalogs.Add(New AssemblyCatalog(GetType(Host).Assembly))

        'IMPORTANT!
        'You will need to adjust this line to match your local path!
        With My.Settings
            Dim d As New DirectoryInfo(.ModuleDir)
            If d.Exists() Then
                catalog.Catalogs.Add(
                    New DirectoryCatalog(.ModuleDir)
                )
            End If
        End With

        'Create the CompositionContainer with the parts in the catalog
        _container = New CompositionContainer(catalog)

        'Fill the imports of this object

        Dim StartExecption As Exception = Nothing
        Try
            _container.ComposeParts(Me)

            Hanger = New Hanger()
            With Hanger
                .setParent(Modules, Messages, Subscribers)
                StartExecption = .svc_start()
                If Not IsNothing(StartExecption) Then
                    Throw StartExecption
                End If
            End With

        Catch ex As Exception
            Console.WriteLine(ex.ToString)

        End Try

    End Sub

#End Region

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

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

End Class
