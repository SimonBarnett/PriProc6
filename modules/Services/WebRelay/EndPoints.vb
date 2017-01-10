Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports PriPROC6.Interface.Web

Public Class EndPoints : Implements IDisposable

#Region "Imported MEF Enumerables"

    <ImportMany()>
    Public Property handlers As IEnumerable(Of Lazy(Of xmlHandler, xmlHandlerProps))

    <ImportMany()>
    Public Property feeds As IEnumerable(Of Lazy(Of xmlFeed, xmlFeedProps))

#End Region

    Private _siteFeeds As New List(Of String)
    Public ReadOnly Property siteFeeds As List(Of String)
        Get
            Return _siteFeeds
        End Get
    End Property

    Private _siteHandlers As New List(Of String)
    Public ReadOnly Property siteHandlers As List(Of String)
        Get
            Return _siteHandlers
        End Get
    End Property

    Sub New(ByRef BinCatalog As Primitives.ComposablePartCatalog)

        Dim catalog = New AggregateCatalog()
        catalog.Catalogs.Add(BinCatalog)

        Dim container As New CompositionContainer(catalog)
        container.ComposeParts(Me)

        If Not IsNothing(feeds) Then
            For Each feed As Lazy(Of xmlFeed, xmlFeedProps) In feeds
                _siteFeeds.Add(feed.Metadata.EndPoint)
            Next
        End If

        If Not IsNothing(handlers) Then
            For Each hdlr As Lazy(Of xmlHandler, xmlHandlerProps) In handlers
                _siteHandlers.Add(hdlr.Metadata.EndPoint)
            Next
        End If

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

End Class
