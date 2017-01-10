Imports System.ComponentModel.Composition

Public Class msgFactory : Implements IDisposable

    Private _Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData))
    Public Sub New(ByRef Messages As IEnumerable(Of Lazy(Of msgInterface, msgInterfaceData)))
        _Messages = Messages
        For Each msg As Lazy(Of msgInterface, msgInterfaceData) In _Messages
            With msg.Value
                .msgType = msg.Metadata.msgType
            End With
        Next
    End Sub

#Region "Message Factory"

    Public Function EncodeRequest(msgtype As String, ob As objBase) As Byte()
        For Each msg As Lazy(Of msgInterface, msgInterfaceData) In _Messages
            With msg.Metadata
                If ( ' the verb of this message equals the requested verb
                    .verb = eVerb.Request
                ) Then
                    If String.Compare(
                        .msgType,
                        msgtype,
                        True
                    ) = 0 Then

                        Return msg.Value.toByte(ob)

                    End If
                End If
            End With
        Next

        Throw New Exception("Unsupported Message type.")

    End Function

    Public Function EncodeResponse(msgtype As String, Optional Er As Integer = 200, Optional ErMsg As String = "", Optional ob As objBase = Nothing) As Byte()
        For Each msg As Lazy(Of msgInterface, msgInterfaceData) In _Messages
            With msg.Metadata
                If ( ' the verb of this message equals the requested verb
                    .verb = eVerb.Response
                ) Then
                    If String.Compare(
                        .msgType,
                        msgtype,
                        True
                    ) = 0 Then

                        With msg.Value
                            .ErrCde = Er
                            .errMsg = ErMsg
                        End With
                        Return msg.Value.toByte(ob)

                    End If
                End If
            End With
        Next

        Throw New Exception("Unsupported Message type.")

    End Function

    Public Function Decode(data As Byte()) As msgBase
        Dim m As New msgBase(data)
        For Each msg As Lazy(Of msgInterface, msgInterfaceData) In _Messages
            With msg.Metadata
                If _
                    m.Verb = .verb _
                    And String.Compare(m.msgType, .msgType, True) = 0 _
                Then
                    msg.Value.readXML(m)
                    Return m
                End If
            End With
        Next
        Throw New Exception("Unsupported Message type.")

    End Function

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

End Class
