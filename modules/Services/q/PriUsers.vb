Imports PriPROC6.regConfig
Imports PriPROC6.svcMessage

Public Class PriorityUsers : Inherits Queue(Of PriorityUser)

    Private _parent As Loader
    Public Sub New(ByRef parent As Loader, Optional ByRef log As oMsgLog = Nothing)
        _parent = parent
        With _parent.thisConfig

            Dim err As New Exception
            Dim dic As Dictionary(Of String, String) = .regDictionary(True, "users")
            Dim InvalidUser As New List(Of String)

            For Each k As String In dic.Keys
                If VerifyUser(k, log) Then
                    Me.Enqueue(New PriorityUser(k, dic(k)))
                Else
                    If Not IsNothing(log) Then
                        With log
                            .EntryType = LogEntryType.Warning
                            .LogData.AppendFormat("User [{0}] does not exist in Priority.", k).AppendLine()
                        End With
                    End If
                    If IsNothing(err) Then
                        InvalidUser.Add(k)
                    End If
                End If
            Next

            ' Remove invalid users from registry settings
            For Each iu As String In InvalidUser
                log.LogData.AppendFormat("Removing invalid user [{0}] from Queue.", iu).AppendLine()
                dic.Remove(iu)
            Next

            ' Add tabula user if no other users
            If dic.Count = 0 Then
                log.LogData.AppendFormat("No users found. Adding [{0}].", "tabula").AppendLine()
                dic.Add("tabula", "Tabula!")
            End If

            ' Save the users to the registry
            .regDictionary(True, "users") = dic

        End With

    End Sub

    Public Function VerifyUser(ByVal UserName As String, ByRef log As oMsgLog) As Boolean

        Dim ret As Boolean = False
        Try
            Using cmd As New prisql(log, _parent.thisConfig)
                Select Case cmd.ExecuteScalar(
                    String.Format(
                        "USE [system] " _
                        & "select COUNT(USERLOGIN) " _
                        & "from USERS " _
                        & "where USERLOGIN = '{0}'",
                        UserName
                    )
                )
                    Case 0
                        ret = False

                    Case Else
                        ret = True

                End Select

            End Using

        Catch EX As Exception
            ret = False
            log.setException(EX)

        End Try

        Return ret

    End Function

    Public Sub AddUser(ByVal User As PriorityUser, ByRef log As oMsgLog)
        With _parent.thisConfig
            Dim dic As Dictionary(Of String, String) = .regDictionary(True, "users")
            For Each u As String In dic.Keys
                If String.Compare(u, User.Username, True) = 0 Then
                    Throw New Exception(String.Format("Priority username [{0}] is already in use.", User.Username))
                End If
            Next
            Dim Err As New Exception
            If VerifyUser(User.Username, log) Then
                dic.Add(User.Username, User.Password)
                .regDictionary(True, "users") = dic
                Me.Enqueue(User)
            Else
                Throw New Exception(String.Format("Username [{0}] does not exist in Priority.", User.Username))
            End If
        End With
    End Sub

    Public Sub DeleteUser(ByVal UserName As String, ByRef log As oMsgLog)
        With _parent.thisConfig
            Dim dic As Dictionary(Of String, String) = .regDictionary(True, "users")
            Dim f As Boolean = False
            For Each u As String In dic.Keys
                If String.Compare(u, UserName, True) = 0 Then
                    f = True
                    Exit For
                End If
            Next
            If Not f Then
                Throw New Exception(String.Format("User [{0}] not found.", UserName))
            End If

            dic.Remove(UserName)
            .regDictionary(True, "users") = dic

        End With

        'With Service
        '    For Each q As LoadQ In _parent.Queues.Values
        '        If Not IsNothing(q.thisUser) Then
        '            If String.Compare(q.thisUser.Username, User.Username, True) = 0 Then
        '                q.thisUser.Remove = True
        '                Exit Sub
        '            End If
        '        End If
        '    Next

        '    Dim u As PriorityUser
        '    Do
        '        u = .users.Dequeue()
        '        If String.Compare(u.Username, User.Username, True) = 0 Then
        '            u = Nothing
        '        End If
        '        If Not IsNothing(u) Then
        '            .users.Enqueue(u)
        '        End If
        '    Loop Until IsNothing(u)
        'End With

    End Sub

    Public Sub chPass(ByVal User As PriorityUser, ByRef log As oMsgLog)
        With _parent.thisConfig
            Dim dic As Dictionary(Of String, String) = .regDictionary(True, "users")
            Dim f As Boolean = False
            For Each u As String In dic.Keys
                If String.Compare(u, User.Username, True) = 0 Then
                    f = True
                    dic(u) = User.Password
                    Exit For
                End If
            Next
            If Not f Then
                Throw New Exception("User [{0}] not found.")
            End If
            .regDictionary(True, "users") = dic

        End With

        'With Service
        '    For Each q As LoadQ In .Queues.Values
        '        If Not IsNothing(q.thisUser) Then
        '            If String.Compare(q.thisUser.Username, User.Username, True) = 0 Then
        '                q.thisUser.Password = User.Password
        '                Exit Sub
        '            End If
        '        End If
        '    Next

        '    Dim u As PriorityUser
        '    Do
        '        u = .users.Dequeue()
        '        If String.Compare(u.Username, User.Username, True) = 0 Then
        '            u.Password = User.Password
        '            Exit Do
        '        End If
        '    Loop
        'End With

    End Sub

End Class