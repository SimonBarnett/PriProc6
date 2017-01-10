Imports System.ComponentModel
Public Class oDictionary
    Inherits Dictionary(Of String, Object)

    <Browsable(False)>
    Public Overloads ReadOnly Property Keys As KeyCollection
        Get
            Return MyBase.Keys
        End Get
    End Property

    <Browsable(False)>
    Public Overloads ReadOnly Property values As ValueCollection
        Get
            Return MyBase.Values
        End Get
    End Property

    <Browsable(False)>
    Public Overloads ReadOnly Property Comparer As Generic.IEqualityComparer(Of String)
        Get
            Return MyBase.Comparer
        End Get
    End Property

    <Browsable(False)>
    Public Overloads ReadOnly Property Count As Integer
        Get
            Return MyBase.Count
        End Get
    End Property

End Class
