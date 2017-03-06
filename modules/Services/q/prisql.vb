Imports System.Data.SqlClient
Imports System.Xml
Imports PriPROC6.regConfig
Imports PriPROC6.svcMessage

Public Class prisql : Implements IDisposable

    Private _db As String
    Private log As oMsgLog

    Sub New(ByRef Log As oMsgLog, db As String)
        _db = db
        Me.log = Log
    End Sub

#Region "DB Connection"

    ReadOnly Property ConnectionString() As String
        Get
            Return String.Format(
                "Server={0};Trusted_Connection=True;",
                _db
            )
        End Get
    End Property

    Private ReadOnly Property dbConnection As SqlConnection
        Get
            log.LogData.AppendFormat(
                "Opening datasource: {0}",
                _db
            ).AppendLine()
            Dim _connection As New SqlConnection()
            With _connection
                .ConnectionString = ConnectionString
                .Open()
            End With
            Return _connection
        End Get
    End Property

#End Region

#Region "Log SQL commands"

    Public Function LogStartSQL(Method As String, ByRef Statement As String) As Date
        log.LogData.AppendFormat("Executing {0}:", Method.ToUpper).AppendLine.Append(Statement).AppendLine().AppendLine()
        Return Now
    End Function

    Public Sub LogEndSQL(starttime As Date)
        log.LogData.AppendFormat(
            "Completed in {0} seconds.",
            (Now - starttime).ToString.Replace("00:", "")
        ).AppendLine()
    End Sub

#End Region

#Region "Execute SQL"

#Region "Stringbuilder support"

    Public Function ExecuteReader(ByRef sqlString As Text.StringBuilder) As SqlClient.SqlDataReader
        Return ExecuteReader(sqlString.ToString)
    End Function

    Public Function ExecuteScalar(ByRef sqlString As Text.StringBuilder) As String
        Return ExecuteScalar(sqlString.ToString)
    End Function

    Public Function ExecuteXmlReader(ByRef sqlString As Text.StringBuilder) As XmlReader
        Return ExecuteXmlReader(sqlString.ToString)
    End Function

    Public Function ExecuteNonQuery(ByRef sqlString As Text.StringBuilder) As Integer
        Return ExecuteNonQuery(sqlString.ToString)
    End Function

#End Region

    Public Function ExecuteReader(ByRef sqlString As String) As SqlClient.SqlDataReader
        Dim ret As SqlDataReader
        Dim command As New SqlCommand(sqlString, dbConnection)
        Dim start As Date = LogStartSQL("Reader", sqlString)
        Try
            ret = command.ExecuteReader
        Catch ex As Exception
            log.setException(ex)
            Throw ex
        End Try
        LogEndSQL(start)
        Return ret
    End Function

    Public Function ExecuteScalar(ByRef sqlString As String) As String
        Dim ret As String
        Dim command As New SqlCommand(sqlString, dbConnection)
        Dim start As Date = LogStartSQL("Scalar", sqlString)
        Try
            ret = command.ExecuteScalar.ToString
        Catch ex As Exception
            log.setException(ex)
            Throw ex
        End Try
        LogEndSQL(start)
        Return ret
    End Function

    Public Function ExecuteXmlReader(ByRef sqlString As String) As XmlReader
        Dim ret As XmlReader
        Dim command As New SqlCommand(sqlString, dbConnection)
        Dim start As Date = LogStartSQL("XmlReader", sqlString)
        Try
            ret = command.ExecuteXmlReader
        Catch ex As Exception
            log.setException(ex)
            Throw ex
        End Try
        LogEndSQL(start)
        Return ret
    End Function

    Public Function ExecuteNonQuery(ByRef sqlString As String) As Integer
        Dim ret As Integer
        Dim command As New SqlCommand(sqlString, dbConnection)
        Dim start As Date = LogStartSQL("NonQuery", sqlString)
        Try
            ret = command.ExecuteNonQuery
        Catch ex As Exception
            log.setException(ex)
            Throw ex
        End Try
        LogEndSQL(start)
        Return ret
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
