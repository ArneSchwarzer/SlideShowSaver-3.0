Imports System.IO
Imports System.Reflection

Namespace SlideShowLoader
    Public MustInherit Class BaseLoader(Of T)
        Protected ReadOnly basePath As String
        Protected ReadOnly extension As String

        Protected Sub New(loaderDirectory As String, fileExtension As String)
            basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, loaderDirectory)
            extension = fileExtension
        End Sub

        Protected Function LadeDummyInstanz(Of TExpected)(filePath As String) As TExpected
            Dim asm As Assembly = Assembly.LoadFrom(filePath)

            For Each typ In asm.GetTypes()
                If GetType(TExpected).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                    Dim instance As Object = Activator.CreateInstance(typ)
                    Return CType(instance, TExpected)
                End If
            Next
            Return Nothing
        End Function

        Protected Function GetAllPluginFiles() As IEnumerable(Of String)
            If Not Directory.Exists(basePath) Then Return Enumerable.Empty(Of String)()
            Return Directory.GetFiles(basePath, "*" & extension, SearchOption.TopDirectoryOnly)
        End Function
    End Class
End Namespace
