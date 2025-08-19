' RegistryHandling.vb
' Zentrale Registry-Zugriffe für SlideShowSaver 3.0

Imports Microsoft.Win32
Imports SlideShowLogging

Public Class RegistryHandling

    ' --- Konstanten für Registry-Pfade ---
    Public Const SLIDESHOWMAIN_PATH As String = ""
    Public Const SLIDESHOWBILDAUSWAHL_PATH As String = "Bildauswahl\"
    Public Const SLIDESHOWMODULBASE_PATH As String = "Module\"
    Public Const SLIDESHOWTRANSITION_PATH As String = "Transition\"
    Public Const SLIDESHOWSHADER_PATH As String = "Shader\"

    ' --- Basis-Registry-Zweig ---
    Private Shared ReadOnly baseKey As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\SlideShowSaver 3.0")

    ' --- Hilfsfunktion zur Trennung von Pfad und Schlüsselname ---
    Private Shared Sub SplitKeyPath(fullKeyPath As String, ByRef subPath As String, ByRef valueName As String)
        fullKeyPath = fullKeyPath.Replace("//", "\").Replace("\\", "\").TrimStart("\"c).TrimEnd("\"c)

        Dim lastBackslash As Integer = fullKeyPath.LastIndexOf("\"c)
        If lastBackslash = -1 Then
            subPath = ""
            valueName = fullKeyPath
        Else
            subPath = fullKeyPath.Substring(0, lastBackslash)
            valueName = fullKeyPath.Substring(lastBackslash + 1)
        End If
    End Sub

    ' --- Werte schreiben ---
    Public Shared Sub WriteToRegistry(fullKeyPath As String, value As String)
        Try
            Dim subPath As String = ""
            Dim valueName As String = ""
            SplitKeyPath(fullKeyPath, subPath, valueName)

            Using key = baseKey.CreateSubKey(subPath)
                If key IsNot Nothing Then
                    key.SetValue(valueName, value)
                End If
            End Using


        Catch ex As Exception
            LogHandling.LogError("Fehler beim Schreiben in die Registry: " & ex.Message)
        End Try
    End Sub

    ' --- Werte lesen ---
    Public Shared Function ReadFromRegistry(fullKeyPath As String) As String
        Try
            Dim subPath As String = ""
            Dim valueName As String = ""
            SplitKeyPath(fullKeyPath, subPath, valueName)

            Using key As RegistryKey = baseKey.OpenSubKey(subPath)
                If key IsNot Nothing Then
                    Dim regValue = key.GetValue(valueName)
                    If regValue IsNot Nothing Then
                        Return regValue.ToString()
                    End If
                End If
            End Using
        Catch ex As Exception
            LogHandling.LogError("Fehler beim Lesen aus der Registry: " & ex.Message)
        End Try
        Return Nothing
    End Function

    ' --- Lesen mit Fallback auf Default-Werte ---
    Public Shared Function ReadFromRegOrDefaults(fullKeyPath As String, defaults As Dictionary(Of String, String)) As String
        Dim subPath As String = ""
        Dim valueName As String = ""
        SplitKeyPath(fullKeyPath, subPath, valueName)

        Dim value As String = ReadFromRegistry(fullKeyPath)

        If String.IsNullOrEmpty(value) AndAlso defaults.ContainsKey(valueName) Then
            Return defaults(valueName)
        Else
            Return value
        End If
    End Function

End Class
