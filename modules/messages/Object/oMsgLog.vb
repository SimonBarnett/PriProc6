Imports Priproc6.Interface.Message

#Region "Enumerations"

Public Enum LogEntryType As Integer
    Err = 1
    Information = 4
    FailureAudit = 16
    SuccessAudit = 8
    Warning = 2
End Enum

Public Enum EvtLogVerbosity As Integer
    Normal = 1
    Verbose = 10
    VeryVerbose = 50
    Arcane = 99
End Enum

Public Enum EvtLogSource As Integer
    APPLICATION = 1
    SYSTEM = 2
    WEB = 4
End Enum

#End Region

Public Class oMsgLog : Inherits objBase

#Region "Constructors"

    Public Sub New()

    End Sub

    Public Sub New(ByRef msg As msgBase)
        With Me
            .Verb = msg.Verb
            .Source = msg.Source
            .msgType = msg.msgType
            .TimeStamp = msg.TimeStamp
        End With
    End Sub

    Public Sub New(
        ByVal svcType As String,
           Optional ByVal LogSource As EvtLogSource = EvtLogSource.APPLICATION,
           Optional ByVal Verbosity As EvtLogVerbosity = EvtLogVerbosity.VeryVerbose,
           Optional ByVal EntryType As LogEntryType = LogEntryType.Information
        )
        MyBase.New
        _svcType = svcType
        _LogSource = LogSource
        _Verbosity = Verbosity
        _EntryType = EntryType
        _EventOnly = 0

    End Sub

    Public Sub New(ByVal svcType As String, ex As Exception)
        MyBase.New
        _svcType = svcType
        _Verbosity = EvtLogVerbosity.Normal
        _EntryType = LogEntryType.FailureAudit
        _LogSource = EvtLogSource.SYSTEM
        _LogData.Append(ex.Message).AppendLine()
        _LogData.Append(ex.StackTrace).AppendLine()
    End Sub

    Public Sub setException(ex As Exception)
        _Verbosity = EvtLogVerbosity.Normal
        _EntryType = LogEntryType.FailureAudit
        _LogData.Append(ex.Message).AppendLine()
        _LogData.Append(ex.StackTrace).AppendLine()
    End Sub

    Sub toConsole()
        Try
            With Me
                Select Case .EntryType
                    Case LogEntryType.Err, LogEntryType.FailureAudit
                        Console.BackgroundColor = ConsoleColor.Red
                    Case LogEntryType.Warning
                        Console.BackgroundColor = ConsoleColor.Yellow
                    Case LogEntryType.SuccessAudit, LogEntryType.Information
                        Console.BackgroundColor = ConsoleColor.Green
                End Select

                Select Case .Verbosity
                    Case EvtLogVerbosity.Normal
                        Console.ForegroundColor = ConsoleColor.Black
                    Case EvtLogVerbosity.Verbose
                        Console.ForegroundColor = ConsoleColor.DarkBlue
                    Case EvtLogVerbosity.VeryVerbose
                        Console.ForegroundColor = ConsoleColor.DarkGray
                    Case EvtLogVerbosity.Arcane
                        Console.ForegroundColor = ConsoleColor.Gray

                End Select
            End With

            Console.WriteLine("{0}", Pad(
            String.Format(
                "{0}{1}{2}{3}",
                Pad(TimeStamp, 22),
                Pad(LogSource, 12),
                Pad(Source.ToUpper, 25),
                Pad(svcType.ToUpper, 16)
            ),
                Console.WindowWidth - 1
            )
        )

            Console.BackgroundColor = ConsoleColor.Black
            Console.ForegroundColor = ConsoleColor.Green

            Console.Write(
            "{0}",
            Me.LogData.AppendLine.ToString
        )

        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(ex.Message)

        End Try

    End Sub

#End Region

#Region "Padding"

    Private Function Pad(ByVal Str As String, ByVal Width As Integer) As String
        Return String.Format("{0}{1}", Str, New String(" ", Width)).Substring(0, Width)
    End Function

    Private Function Pad(ByVal logSource As EvtLogSource, ByVal width As Integer)
        Select Case logSource
            Case EvtLogSource.APPLICATION
                Return String.Format("{0}{1}", "APPLICATION", New String(" ", width)).Substring(0, width)

            Case EvtLogSource.SYSTEM
                Return String.Format("{0}{1}", "SYSTEM", New String(" ", width)).Substring(0, width)

            Case Else
                Return String.Format("{0}{1}", "WEB", New String(" ", width)).Substring(0, width)

        End Select
    End Function

#End Region

#Region "Message Properies"

    Private _svcType As String
    Public Property svcType() As String
        Get
            Return _svcType
        End Get
        Set(ByVal value As String)
            _svcType = value
        End Set
    End Property

    Private _LogSource As EvtLogSource
    Public Property LogSource() As EvtLogSource
        Get
            Return _LogSource
        End Get
        Set(ByVal value As EvtLogSource)
            _LogSource = value
        End Set
    End Property

    Private _Verbosity As EvtLogVerbosity
    Public Property Verbosity() As EvtLogVerbosity
        Get
            Return _Verbosity
        End Get
        Set(ByVal value As EvtLogVerbosity)
            _Verbosity = value
        End Set
    End Property

    Private _EntryType As LogEntryType
    Public Property EntryType() As LogEntryType
        Get
            Return _EntryType
        End Get
        Set(ByVal value As LogEntryType)
            _EntryType = value
        End Set
    End Property

    Private _LogData As New System.Text.StringBuilder
    Public Property LogData() As System.Text.StringBuilder
        Get
            Return _LogData
        End Get
        Set(ByVal value As System.Text.StringBuilder)
            _LogData = value
        End Set
    End Property

    Private _EventOnly As Integer = 0
    Public Property EventOnly() As Integer
        Get
            Return _EventOnly
        End Get
        Set(ByVal value As Integer)
            _EventOnly = value
        End Set
    End Property

#End Region

End Class
