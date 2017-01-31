Imports System.Xml
Imports PriPROC6.Interface.Message
Imports PriPROC6.svcMessage

Public Class oSubConsole : Inherits oSubscriber

#Region "Constructor"

    Sub New(ByRef node As XmlNode)
        MyBase.New(node)

    End Sub

    Public Overrides Sub Update(ByRef Service As oServiceBase)
        With TryCast(Service, oSubConsole)
            MyBase.Update(Service)
        End With
    End Sub

#End Region

End Class