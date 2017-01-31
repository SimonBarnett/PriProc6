Imports System.ComponentModel

Public Class objBase : Implements IDisposable

    Sub New()
        _source = Environment.MachineName
        _TimeStamp = Now.ToString("dd/MM/yyyy HH:mm:ss")
    End Sub

    Private _verb As eVerb
    <Browsable(False),
    [ReadOnly](True)>
    Public Property Verb As eVerb
        Get
            Return _verb
        End Get
        Set(value As eVerb)
            _verb = value
        End Set
    End Property

    Private _msgType As String
    <Browsable(False)>
    Public Property msgType As String
        Get
            Return _msgType
        End Get
        Set(value As String)
            _msgType = value
        End Set
    End Property

    Private _source As String
    <CategoryAttribute("Source"),
    DescriptionAttribute("The originating server."),
    [ReadOnly](True)>
    Public Property Source As String
        Get
            Return _source
        End Get
        Set(value As String)
            _source = value
        End Set
    End Property

    Private _TimeStamp As String
    <CategoryAttribute("Source"),
    DescriptionAttribute("Message receipt timestamp."),
    [ReadOnly](True)>
    Public Property TimeStamp As String
        Get
            Return _TimeStamp
        End Get
        Set(value As String)
            _TimeStamp = value
        End Set
    End Property

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
