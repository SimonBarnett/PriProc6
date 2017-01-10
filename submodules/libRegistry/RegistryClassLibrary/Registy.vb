Imports Microsoft.Win32

Public Class svcConfig

#Region "Private Variables"

    Private thisRegistryKey As RegistryKey
    Private EncryptKey As Encryption.Data
    Private EncryptionMethod As New Encryption.Symmetric(Encryption.Symmetric.Provider.TripleDES)

#End Region

#Region "Public properties"

    Public Property regValue(ByVal isEncrypt As Boolean, ByVal ParamArray Args() As String)
        Get
            If Args.Length = 0 Then Return String.Empty
            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args) - 1
                If Not sk(thisReg).Contains(Args(i)) Then
                    Return String.Empty
                Else
                    thisReg = thisReg.OpenSubKey(Args(i), True)
                End If
            Next

            Dim strVal As String = thisReg.GetValue(Args(UBound(Args)), String.Empty).ToString
            Select Case isEncrypt
                Case True
                    If strVal.Length > 0 Then
                        Dim EncData As New Encryption.Data
                        EncData.Base64 = strVal
                        Return EncryptionMethod.Decrypt( _
                            EncData, _
                            EncryptKey _
                        ).ToString
                    Else
                        Return String.Empty
                    End If
                Case Else
                    Return strVal
            End Select
        End Get

        Set(ByVal value)
            If Args.Length = 0 Then
                Throw New Exception("No registry path specified.")
            End If
            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args) - 1
                If Not sk(thisReg).Contains(Args(i)) Then
                    thisReg.CreateSubKey(Args(i))
                End If
                thisReg = thisReg.OpenSubKey(Args(i), True)
            Next
            Select Case isEncrypt
                Case True
                    Dim encryptedData As Encryption.Data = _
                        EncryptionMethod.Encrypt( _
                            New Encryption.Data( _
                                value.ToString _
                            ), _
                            EncryptKey _
                    )
                    thisReg.SetValue(Args(UBound(Args)), encryptedData.ToBase64)
                Case Else
                    thisReg.SetValue(Args(UBound(Args)), value)
            End Select
        End Set
    End Property

    Public Property regDictionary(ByVal isEncrypt As Boolean, ByVal ParamArray Args() As String) As Dictionary(Of String, String)
        Get
            Dim ret As New Dictionary(Of String, String)
            If Args.Length = 0 Then Return ret

            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args)
                If Not sk(thisReg).Contains(Args(i)) Then
                    Return ret
                Else
                    thisReg = thisReg.OpenSubKey(Args(i), True)
                End If
            Next

            For Each valueName As String In thisReg.GetValueNames()
                Dim strval = thisReg.GetValue(valueName).ToString()
                Select Case isEncrypt
                    Case True
                        If strval.Length > 0 Then
                            Dim EncData As New Encryption.Data
                            EncData.Base64 = strval
                            ret.Add( _
                                valueName, _
                                EncryptionMethod.Decrypt( _
                                    EncData, _
                                    EncryptKey _
                                ).ToString _
                            )
                        Else
                            ret.Add( _
                                valueName, _
                                String.Empty _
                            )
                        End If
                    Case Else
                        ret.Add( _
                            valueName, _
                            strval _
                        )
                End Select
            Next
            Return ret
        End Get

        Set(ByVal value As Dictionary(Of String, String))
            If Args.Length = 0 Then
                Throw New Exception("No registry path specified.")
            End If

            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args)
                If Not sk(thisReg).Contains(Args(i)) Then
                    thisReg.CreateSubKey(Args(i))
                End If
                thisReg = thisReg.OpenSubKey(Args(i), True)
            Next

            ' Delete and edit
            For Each valueName As String In thisReg.GetValueNames()
                If value.Keys.Contains(valueName) Then
                    Select Case isEncrypt
                        Case True
                            Dim encryptedData As Encryption.Data = _
                                EncryptionMethod.Encrypt( _
                                    New Encryption.Data( _
                                        value(valueName).ToString _
                                    ), _
                                    EncryptKey _
                            )
                            thisReg.SetValue(valueName, encryptedData.ToBase64)
                        Case Else
                            thisReg.SetValue(valueName, value(valueName).ToString)
                    End Select
                Else
                    thisReg.DeleteValue(valueName)
                End If
            Next

            ' Create
            For Each key As String In value.Keys
                If Not thisReg.GetValueNames().Contains(key) Then
                    Select Case isEncrypt
                        Case True
                            Dim encryptedData As Encryption.Data = _
                                EncryptionMethod.Encrypt( _
                                    New Encryption.Data( _
                                        value(key).ToString _
                                    ), _
                                    EncryptKey _
                            )
                            thisReg.SetValue(key, encryptedData.ToBase64)
                        Case Else
                            thisReg.SetValue(key, value(key).ToString)
                    End Select
                End If
            Next

        End Set

    End Property

    Public Property regList(ByVal ParamArray Args() As String) As List(Of String)
        Get
            Dim ret As New List(Of String)
            If Args.Length = 0 Then Return ret

            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args)
                If Not sk(thisReg).Contains(Args(i)) Then
                    Return ret
                Else
                    thisReg = thisReg.OpenSubKey(Args(i), True)
                End If
            Next

            For Each valueName As String In thisReg.GetValueNames()
                Dim strval = thisReg.GetValue(valueName).ToString()
                ret.Add( _
                    valueName _
                )
            Next
            Return ret
        End Get

        Set(ByVal value As List(Of String))
            If Args.Length = 0 Then
                Throw New Exception("No registry path specified.")
            End If

            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args)
                If Not sk(thisReg).Contains(Args(i)) Then
                    thisReg.CreateSubKey(Args(i))
                End If
                thisReg = thisReg.OpenSubKey(Args(i), True)
            Next

            ' Delete and edit
            For Each valueName As String In thisReg.GetValueNames()
                If Not value.Contains(valueName) Then
                    thisReg.DeleteValue(valueName)
                End If
            Next

            ' Create
            For Each key As String In value
                If Not thisReg.GetValueNames().Contains(key) Then
                    thisReg.SetValue(key, "")
                End If
            Next

        End Set

    End Property

    Public Property SubKeys(ByVal ParamArray Args() As String) As List(Of String)
        Get
            Dim ret As New List(Of String)
            If Args.Length = 0 Then Return ret

            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args)
                If Not sk(thisReg).Contains(Args(i)) Then
                    Return ret
                Else
                    thisReg = thisReg.OpenSubKey(Args(i), True)
                End If
            Next
            Return sk(thisReg)
        End Get
        Set(ByVal value As List(Of String))
            Dim thisReg As RegistryKey = thisRegistryKey
            For i As Integer = 0 To UBound(Args)
                If Not sk(thisReg).Contains(Args(i)) Then
                    Exit Property
                Else
                    thisReg = thisReg.OpenSubKey(Args(i), True)
                End If
            Next
            For Each Str As String In sk(thisReg)
                If Not value.Contains(Str) Then
                    thisReg.DeleteSubKey(Str)
                End If
            Next
            For Each Str As String In value
                If Not sk(thisReg).Contains(Str) Then
                    thisReg.CreateSubKey(Str)
                End If
            Next
        End Set
    End Property

#End Region

#Region "Initialisation and finalisation"

    Public Sub New(ByVal strNameSpace As String, ByVal strAppName As String)
        EncryptKey = New Encryption.Data(Environment.MachineName)
        Using LM_Software As RegistryKey = Registry.LocalMachine.OpenSubKey("Software", True)
            If Not sk(LM_Software).Contains(strNameSpace) Then
                LM_Software.CreateSubKey(strNameSpace)
            End If
            Using LM_Software_Namespace As RegistryKey = Registry.LocalMachine.OpenSubKey("Software", True).OpenSubKey(strNameSpace, True)
                If Not sk(LM_Software_Namespace).Contains(strAppName) Then
                    LM_Software_Namespace.CreateSubKey(strAppName)
                End If
                thisRegistryKey = Registry.LocalMachine.OpenSubKey("Software", True).OpenSubKey(strNameSpace, True).OpenSubKey(strAppName, True)
            End Using
        End Using
    End Sub

#End Region

#Region "Private Methods"

    Public Function sk(ByRef thisKey As RegistryKey) As List(Of String)
        Dim ret As New List(Of String)
        For Each skey As String In thisKey.GetSubKeyNames
            ret.Add(skey)
        Next
        Return ret
    End Function

#End Region

End Class
