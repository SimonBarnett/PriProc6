Imports System.IO
Imports System.Threading
Imports PriPROC6.svcMessage

Public Enum eqType
    env
    named
End Enum

Public Class LoadQ : Inherits Queue(Of oMsgLoading)

    Private LoadThread As Thread
    Private _Parent As Loader

    Public Event ReleaseUser(ByVal user As PriorityUser)

#Region "Public Properties"

    Private _thisUser As PriorityUser
    Public Property thisUser() As PriorityUser
        Get
            Return _thisUser
        End Get
        Set(ByVal value As PriorityUser)
            _thisUser = value
        End Set
    End Property

    Private _qName As String
    Public Property qName() As String
        Get
            Return _qName
        End Get
        Set(ByVal value As String)
            _qName = value
        End Set
    End Property

    'Private _qType As eqType
    'Public Property qType() As eqType
    '    Get
    '        Return _qType
    '    End Get
    '    Set(ByVal value As eqType)
    '        _qType = value
    '    End Set
    'End Property

    'Private _Procedures As New List(Of String)
    'Public Property Procedures() As List(Of String)
    '    Get
    '        Return _Procedures
    '    End Get
    '    Set(ByVal value As List(Of String))
    '        _Procedures = value
    '    End Set
    'End Property

    Private _Dir As DirectoryInfo
    Public ReadOnly Property Dir() As DirectoryInfo
        Get
            Return _Dir
        End Get
    End Property

    Private _Running As Boolean = False
    Public ReadOnly Property Running() As Boolean
        Get
            Return _Running
        End Get
    End Property

    Private _terminate As Boolean = False
    Public Property Terminate() As Boolean
        Get
            Return _terminate
        End Get
        Set(ByVal value As Boolean)
            _terminate = value
        End Set
    End Property

    'Private ReadOnly Property BubbleFolderFiles() As FileSystemInfo()
    '    Get
    '        Dim files As IO.FileSystemInfo() = Dir.GetFiles
    '        If files.Count > 0 Then
    '            Array.Sort(files, New CompareFilesByDateCreated)
    '        End If
    '        Return files
    '    End Get
    'End Property

#End Region

#Region "initialisation and finalisation"

    'Public Sub New(ByRef Parent As Loader, ByVal Dir As DirectoryInfo, ByVal qType As eqType)

    '    _Parent = Parent
    '    _Dir = Dir
    '    _qType = qType
    '    _qName = Dir.Name

    '    If Not _Dir.Exists Then
    '        _Dir.Create()
    '    Else
    '        For Each f As FileInfo In BubbleFolderFiles
    '            Enqueue(New loadRequest(f))
    '        Next
    '    End If

    'End Sub

#End Region

    '#Region "Thread"

    '    Public Sub Start(ByVal thisUser As PriorityUser)

    '        _Running = True
    '        _thisUser = thisUser

    '        LoadThread = New Thread(AddressOf thLoad)
    '        With LoadThread
    '            .Name = String.Format("q_{0}: {1}", Dir.Name, Peek.BubbleID)
    '            .Start(thisUser)
    '        End With

    '    End Sub

    '    Private Sub thLoad(ByVal thisUser As PriorityUser)

    '        With Peek()
    '            Using log As New oMsgLog(
    '                _Parent.ServiceType,
    '                EvtLogSource.APPLICATION,
    '                EvtLogVerbosity.Verbose,
    '                LogEntryType.Information
    '            )
    '                log.LogData.AppendFormat("Queue [{1}]: Loading Bubble #{0} ...", .Loading.BubbleID, Dir.Name).AppendLine()
    '                log.LogData.AppendFormat("Data: {0}", .ToString()).AppendLine()

    '                Try
    '                    Using ret As msgGenericResponse = SendLoading(thisUser)
    '                        Select Case ret.ErrCde
    '                            Case 200 ' Inserted data to load table ok
    '                                If .Loading.Procedure.Length > 0 Then
    '                                    ' Run the procedure
    '                                    Dim exCode As Integer = prpExitCode(thisUser, .Loading, log)
    '                                    ' Get result from the SQL service
    '                                    Using response As LoadResponse = SendPostLoad()
    '                                        Select Case exCode
    '                                            Case Ex_OK
    '                                                With response

    '                                                    ' Responses:
    '                                                    ' 200 - No uloaded, No Warnings
    '                                                    ' 300 - Records loaded but warnings issued
    '                                                    ' 400 - There are unloaded records
    '                                                    ' 401 - Licence Fail
    '                                                    ' 405 - No loaded records, no errors - Procedure Failed        
    '                                                    ' 500 - Failed to get Loading Results

    '                                                    ' Add errors to application log
    '                                                    Select Case .ErrCde
    '                                                        Case 200, 300
    '                                                            .errMsg = "+Ok."
    '                                                        Case 400, 401
    '                                                            log.SetOption(, EvtLogVerbosity.Normal, LogEntryType.Warning)
    '                                                        Case Else
    '                                                            log.SetOption(, EvtLogVerbosity.Normal, LogEntryType.Err)
    '                                                    End Select
    '                                                    log.LogData.AppendFormat("{0}.", response.errMsg).AppendLine()

    '                                                    ' Add errors to the bubble
    '                                                    AddWarning(response.errMsg)

    '                                                    ' Save the response
    '                                                    Select Case .ErrCde
    '                                                        Case 200, 300, 400, 405
    '                                                            ' Add warnings to log
    '                                                            With .Loading.LoadAttempts.Last
    '                                                                For Each w As Warning In .Value.Warnings
    '                                                                    log.LogData.AppendFormat("Line {0}: {1}.", w.Line, w.Mesage).AppendLine()
    '                                                                Next
    '                                                            End With

    '                                                        Case Else
    '                                                            RemoveLastAttempt()

    '                                                    End Select
    '                                                    .Save(Dir)

    '                                                    ' Move the message to log / badmail
    '                                                    Select Case .ErrCde
    '                                                        Case 200, 300
    '                                                            MoveBubble("log", .Loading, log)
    '                                                            Dequeue()
    '                                                        Case 400
    '                                                            MoveBubble("badmail", .Loading, log)
    '                                                            Dequeue()
    '                                                        Case 405 ' - No loaded records, no errors - Procedure Failed
    '                                                            If response.Loading.LoadAttempts.Count > 2 Then
    '                                                                log.LogData.AppendFormat("Procedure [{0}] failed after 3 attempts.", .Loading.Procedure).AppendLine()
    '                                                                MoveBubble("badmail", .Loading, log)
    '                                                                Dequeue()
    '                                                            End If
    '                                                        Case Else
    '                                                            ' retry
    '                                                    End Select

    '                                                End With

    '                                            Case Ex_WINACTIV_Fail ' Invalid Priority user?
    '                                                With log
    '                                                    .SetOption(, EvtLogVerbosity.Normal, LogEntryType.Err)
    '                                                    .LogData.AppendFormat("WinACTIV failed to start for Priority user {0}.", thisUser.Username).AppendLine()
    '                                                End With
    '                                                AddWarning("WinACTIV failed to start for Priority user {0}.", thisUser.Username)
    '                                                .Save(Dir)

    '                                            Case Else
    '                                                log.SetOption(EvtLogSource.SYSTEM, EvtLogVerbosity.Normal, LogEntryType.Err)
    '                                                Select Case exCode
    '                                                    Case Ex_Timeout
    '                                                        log.LogData.AppendFormat("Loading bubble#{0} timed out.", .Loading.BubbleID).AppendLine()
    '                                                        AddWarning("Bubble timed out after {0} seconds.", .Loading.LoadTimeOut)

    '                                                    Case Ex_WINRUN_Fail, Ex_MissingParameter
    '                                                        log.LogData.Append("PRP.EXE fail. See PRP output.").AppendLine()
    '                                                        AddWarning("PRP.EXE fail.")

    '                                                End Select

    '                                                .Save(Dir)
    '                                                MoveBubble("badmail", .Loading, log)
    '                                                Dequeue()

    '                                        End Select
    '                                    End Using
    '                                End If

    '                            Case Else ' sql Service returned an error
    '                                Select Case ret.ErrCde
    '                                    Case 400 ' Failed validation                                        
    '                                        With log
    '                                            .SetOption(EvtLogSource.APPLICATION, EvtLogVerbosity.Normal, LogEntryType.Err)
    '                                            .LogData.AppendFormat("{0}", ret.errMsg).AppendLine()
    '                                        End With
    '                                        AddWarning("Invalid bubble data. {0}", ret.errMsg)

    '                                        .Save(Dir)
    '                                        MoveBubble("badmail", .Loading, log)
    '                                        Dequeue()

    '                                    Case Else ' A different error occured
    '                                        RemoveLastAttempt()
    '                                        With log
    '                                            .SetOption(, EvtLogVerbosity.Normal, LogEntryType.Err)
    '                                            .LogData.AppendFormat(ret.errMsg)
    '                                        End With
    '                                        Select Case ret.ErrCde
    '                                            Case 500 ' Server error
    '                                            Case 0 ' Connection failed                                                
    '                                            Case 999 ' Unexpected Error
    '                                        End Select
    '                                End Select
    '                        End Select

    '                    End Using

    '                Catch ex As Exception
    '                    Using syslog As New oMsgLog(
    '                        _Parent.ServiceType,
    '                        EvtLogSource.SYSTEM,
    '                        EvtLogVerbosity.Normal,
    '                        LogEntryType.Err
    '                    )
    '                        _Parent.LogEvent(syslog)
    '                    End Using

    '                Finally
    '                    ' Release the thread
    '                    _Parent.LogEvent(log)
    '                    RaiseEvent ReleaseUser(thisUser)
    '                    _Running = False

    '                End Try

    '            End Using

    '        End With
    '    End Sub


    '#Region "Loading Requests"

    '    Private Function SendLoading(ByVal thisUser As PriorityUser) As msgGenericResponse

    '        ' Responses:
    '        ' 200 - ok
    '        ' 400 - Invalid Procedure / SQL
    '        ' 500 - database error   
    '        ' 0   - Connection Error
    '        ' 999 - Unexpected Error

    '        With Peek()
    '            ' Set to loading mode and specify user
    '            .LoadMode = eLoadMode.Load
    '            With .Loading
    '                .User = thisUser
    '                ' Start a new loading attempt
    '                NewAttempt(thisUser)
    '            End With

    '            ' Post the loading to the sql server
    '            Try
    '                Using cli As New iClient("localhost", eServicePorts.prisql)
    '                    Return New msgGenericResponse(
    '                        New svcMsgXML(
    '                            eProtocolType.tcp,
    '                            cli.Send(
    '                                .toByte
    '                            )
    '                        ).msgNode
    '                    )
    '                End Using

    '            Catch ConnErr As System.InvalidOperationException
    '                ' Connection Error
    '                Return New msgGenericResponse(
    '                    0,
    '                    New Exception(
    '                        String.Format(
    '                            "Could not connect to PriSQL service [{0}]. {1}",
    '                            "localhost",
    '                            ConnErr.Message
    '                        )
    '                    )
    '                )

    '            Catch ex As Exception
    '                ' Unexpected error
    '                Return New msgGenericResponse(
    '                    999,
    '                    New Exception(
    '                        String.Format(
    '                            "{0}",
    '                            ex.Message
    '                        )
    '                    )
    '                )

    '            End Try

    '        End With

    '    End Function

    '    Private Function SendPostLoad() As LoadResponse

    '        ' Responses:
    '        ' 200 - No uloaded, No Warnings
    '        ' 300 - Records loaded but warnings issued
    '        ' 400 - There are unloaded records
    '        ' 401 - Licence Fail
    '        ' 405 - No loaded records, no errors - Procedure Failed        
    '        ' 500 - Failed to get Loading Results
    '10:
    '        With Peek()
    '            .LoadMode = eLoadMode.ldErrs
    '            Try
    '                Using cli As New iClient("localhost", 8094)
    '                    Using ret As New LoadResponse(
    '                        New svcMsgXML(
    '                            eProtocolType.tcp,
    '                            cli.Send(
    '                                .toByte
    '                            )
    '                        ).msgNode
    '                    )
    '                        With ret
    '                            Select Case .ErrCde
    '                                Case 300 ' Database Error
    '                                    Throw New Exception("Database error.")

    '                                Case 500 ' Failed to get Loading Results
    '                                    With ret
    '                                        .ErrCde = 500
    '                                        .errMsg = "Failed to get Loading Results."
    '                                    End With

    '                                Case Else
    '                                    With .Loading.LoadAttempts.Last.Value
    '                                        If .Loaded = 0 And .Warnings.Count = 0 Then
    '                                            ' No load, no errors - Procedure Failed
    '                                            With ret
    '                                                .ErrCde = 405
    '                                                .errMsg = "Procedure Failed."
    '                                            End With

    '                                        ElseIf LicenceViolation(.Warnings) Then
    '                                            ' Priority Licence fail
    '                                            With ret
    '                                                .ErrCde = 401
    '                                                .errMsg = "Licence Violation."
    '                                            End With

    '                                        ElseIf .Unloaded > 0 Then
    '                                            ' There are unloaded records                                                                                        
    '                                            ret.ErrCde = 400
    '                                            ret.errMsg = "Not all lines were loaded."
    '                                            If .Warnings.Count > 0 Then
    '                                                For Each warn As Warning In .Warnings
    '                                                    ret.errMsg &= String.Format("{2}  {0}: {1}", warn.Line, warn.Mesage, vbCrLf)
    '                                                Next
    '                                            End If


    '                                        ElseIf .Warnings.Count > 0 Then
    '                                            ' Records loaded but warnings issued                                            
    '                                            With ret
    '                                                .ErrCde = 300
    '                                                .errMsg = "Loading issued warnings."
    '                                            End With
    '                                            For Each warn As Warning In .Warnings
    '                                                ret.errMsg &= String.Format("{2}  {0}: {1}", warn.Line, warn.Mesage, vbCrLf)
    '                                            Next

    '                                        Else
    '                                            ' No unloaded, no warnings
    '                                            ret.ErrCde = 200
    '                                        End If

    '                                    End With

    '                            End Select
    '                            Return ret
    '                        End With
    '                    End Using

    '                End Using

    '            Catch ' Any Errors                
    '                ' Wait 5 seconds and retry
    '                Thread.Sleep(5000)
    '                GoTo 10

    '            End Try

    '        End With

    '    End Function

    '#End Region

    '#Region "Licence messages"

    '    Private Function LicenceViolation(ByRef Warnings As List(Of Warning)) As Boolean
    '        Dim str As New System.Text.StringBuilder
    '        For Each w As Warning In Warnings
    '            str.Append(w.Mesage)
    '        Next
    '        If System.Text.RegularExpressions.Regex.Match(str.ToString, "licence is limited".Replace(" ", ".*"), Text.RegularExpressions.RegexOptions.IgnoreCase).Success Then
    '            Return True
    '        End If
    '        If System.Text.RegularExpressions.Regex.Match(str.ToString, "user name was assigned to different employees".Replace(" ", ".*"), Text.RegularExpressions.RegexOptions.IgnoreCase).Success Then
    '            Return True
    '        End If
    '        Return False
    '    End Function

    '#End Region

    '#End Region

    '#Region "Move bubble file"

    '    Private Sub MoveBubble(ByVal Folder As String, ByRef Loading As oMsgLoading, ByRef Log As oMsgLog)

    '        Dim fn As String = String.Format(
    '            "{0}.xml",
    '            Loading.BubbleID
    '        )

    '        Dim Destintion As String = IO.Path.Combine(
    '            BubbleDestination(Folder, Loading).FullName,
    '            fn
    '        )

    '        Log.LogData.AppendFormat(
    '            "Moving to file://{1}",
    '            Loading.BubbleID,
    '            Destintion
    '        ).AppendLine()

    '        Try
    '            While File.Exists(Destintion)
    '                File.Delete(Destintion)
    '            End While

    '            File.Move(
    '                IO.Path.Combine(
    '                    Dir.FullName,
    '                    fn
    '                ),
    '                Destintion
    '            )
    '            Log.LogData.Append("+OK").AppendLine()

    '        Catch ex As Exception
    '            If Not IsNothing(Log) Then
    '                Log.LogData.AppendFormat("Failed: {0}", ex.Message).AppendLine()
    '            End If
    '            Throw New Exception(String.Format("Could not move bubble to [{0}].", Destintion))
    '        End Try

    '    End Sub

    '    Public Function BubbleDestination(ByVal folder As String, ByRef loading As oMsgLoading) As DirectoryInfo

    '        Dim proc As String
    '        If loading.Procedure.Length > 0 Then
    '            proc = loading.Procedure
    '        ElseIf loading.Table.Length > 0 Then
    '            proc = loading.Table
    '        Else
    '            proc = "unknown"
    '        End If

    '        Dim BubbleDest As New DirectoryInfo(
    '            IO.Path.Combine(
    '                Dir.FullName,
    '                String.Format(
    '                    "{0}\{1}\{2}\{3}",
    '                    folder,
    '                    proc,
    '                    Now.ToString("yyyy-MM"),
    '                    Now.ToString("dd")
    '                )
    '            )
    '        )
    '        If Not BubbleDest.Exists Then
    '            BubbleDest.Create()
    '        End If

    '        Return BubbleDest

    '    End Function

    '#End Region

    '#Region "Run PRP"

    '    Private prpOut As List(Of String)

    '    Private Function prpExitCode(ByRef thisUser As PriorityUser, ByRef Loading As oMsgLoading, ByRef log As oMsgLog) As Integer

    '        Try
    '            Using p As New Process
    '                With p
    '                    With .StartInfo
    '                        .WorkingDirectory = IO.Path.Combine(_Parent.PriorityDir, "bin.95")
    '                        .FileName = IO.Path.Combine(_Parent.PriorityDir, "bin.95\prp.exe")

    '                        .Arguments = String.Format("-dir {0} -user {1} -pwd {2} -env {3} -proc {4} -wait {5}",
    '                           _Parent.PriorityDir,
    '                           thisUser.Username,
    '                           thisUser.Password,
    '                           Loading.Environment,
    '                           Loading.Procedure,
    '                           CInt(Loading.LoadTimeOut / 60).ToString
    '                        )
    '                        log.LogData.AppendFormat("{0} {1}", .FileName, .Arguments).AppendLine()
    '                        .UseShellExecute = False
    '                        .RedirectStandardOutput = True
    '                        .RedirectStandardError = True
    '                    End With

    '                    .EnableRaisingEvents = True
    '                    AddHandler .OutputDataReceived, AddressOf GotData
    '                    AddHandler .ErrorDataReceived, AddressOf GotData

    '                    prpOut = New List(Of String)
    '                    .Start()
    '                    .BeginOutputReadLine()

    '                    Do
    '                        .WaitForExit(1000)
    '                    Loop Until .HasExited
    '                    Thread.Sleep(1000)

    '                    For Each l As String In prpOut
    '                        log.LogData.Append(l).AppendLine()
    '                    Next

    '                    Return .ExitCode
    '                End With
    '            End Using

    '        Catch ex As Exception
    '            Return 999
    '        End Try

    '    End Function

    '    Private Sub GotData(ByVal sendingProcess As Object, ByVal outLine As DataReceivedEventArgs)
    '        If Not IsNothing(outLine.Data) Then
    '            prpOut.Add(outLine.Data)
    '        End If
    '    End Sub

    '#End Region

End Class
