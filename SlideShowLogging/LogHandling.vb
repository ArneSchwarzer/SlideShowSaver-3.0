' LogHandling.vb
' Modul zur zentralen Protokollierung für SlideShowSaver 3.0

Imports System.IO
Imports Microsoft.Win32
Imports System.Threading

Public Enum LogLevel
    DebugLevel = 0
    Info = 1
    Warn = 2
    ErrorLevel = 3
End Enum

Public Class LogHandling

    ' --- Konstanten ---
    Private Shared ReadOnly logDirectory As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SlideShowSaver 3.0", "Logs")
    Private Const maxLogFiles As Integer = 10
    Private Const registryPath As String = "Software\SlideShowSaver 3.0"
    Private Const registryKey As String = "LogLevel"

    ' --- Felder ---
    Private Shared logLock As New Object()
    Private Shared minimumLevel As LogLevel = LogLevel.Info

    ' --- Initialisierung ---
    Shared Sub New()
        Directory.CreateDirectory(logDirectory)
        LoadLogLevelFromRegistry()
        RotateOldLogs()
    End Sub

    ' --- Öffentliche Methoden ---
    Public Shared Sub LogDebug(message As String)
        WriteLog(LogLevel.DebugLevel, message)
    End Sub

    Public Shared Sub LogInfo(message As String)
        WriteLog(LogLevel.Info, message)
    End Sub

    Public Shared Sub LogWarn(message As String)
        WriteLog(LogLevel.Warn, message)
    End Sub

    Public Shared Sub LogError(message As String)
        WriteLog(LogLevel.ErrorLevel, message)
    End Sub

    Public Shared Sub SetMinimumLogLevel(level As LogLevel)
        minimumLevel = level
    End Sub

    ' --- Private Methoden ---
    Private Shared Sub WriteLog(level As LogLevel, message As String)
        If level < minimumLevel Then Exit Sub

        Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim logFile As String = Path.Combine(logDirectory, $"Log_{DateTime.Now:yyyy-MM-dd}.txt")
        Dim finalMessage As String = $"[{timestamp}] [{level}] {message}"

        SyncLock logLock
            Try
                File.AppendAllText(logFile, finalMessage & Environment.NewLine)
                Debug.WriteLine(finalMessage)
            Catch ex As Exception
                ' Logging-Fehler ignorieren
            End Try
        End SyncLock
    End Sub

    Private Shared Sub LoadLogLevelFromRegistry()
        Try
            Using key = Registry.CurrentUser.OpenSubKey(registryPath)
                If key IsNot Nothing Then
                    Dim value = key.GetValue(registryKey, LogLevel.Info.ToString())
                    [Enum].TryParse(value.ToString(), True, minimumLevel)
                End If
            End Using
        Catch ex As Exception
            minimumLevel = LogLevel.Info
        End Try
    End Sub

    Private Shared Sub RotateOldLogs()
        Try
            Dim files = Directory.GetFiles(logDirectory, "Log_*.txt").OrderByDescending(Function(f) File.GetCreationTime(f)).ToList()
            If files.Count > maxLogFiles Then
                For i = maxLogFiles To files.Count - 1
                    File.Delete(files(i))
                Next
            End If
        Catch ex As Exception
            ' Fehler bei Rotation ignorieren
        End Try
    End Sub
End Class