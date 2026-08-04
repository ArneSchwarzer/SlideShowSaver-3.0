Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports SlideShowLogging
Imports SlideShowTools
Imports SlideShowTools.ListHandling
Imports SlideShowTools.MetaDataHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class BildauswahlMain

#Region "Variablen, Strukturen und Enumerationen"

    'Variablen, Konstanten und Enums
    Private Shared ReadOnly rnd As New Random()
    Private Shared ReadOnly zufallLock As New Object()
    Private Shared ReadOnly bestandLock As New Object()
    Private Shared ReadOnly vorbereitungsLock As New Object()

    Private Shared aktuelleSettings As New SettingsBildauswahl
    Private Shared vorbereitungsSettings As New SettingsBildauswahl

    'Progressiv befüllte Bildbestände
    Private Shared vorbereiteteDateien As New List(Of String)
    Private Shared vorbereiteteBildPfade As New HashSet(Of String)(
    StringComparer.OrdinalIgnoreCase)

    Private Shared vorbereiteteVerzeichnisse As New Dictionary(Of String, List(Of String))(
    StringComparer.OrdinalIgnoreCase)

    'Zuletzt vollständig ermittelte Verzeichnisstruktur
    Private Shared letzteBekannteVerzeichnisse As New List(Of String)

    'Progressiver Vorbereitungslauf
    Private Shared vorbereitungsTask As Task
    Private Shared vorbereitungsCancellation As CancellationTokenSource
    Private Shared vorbereitungsGeneration As Integer

    'Der persönliche Build verwendet die optimierte Blattverzeichnissuche.
    Private Shared ReadOnly verzeichnisSuchmodus As FileHandling.VerzeichnisSuchmodus =
    FileHandling.VerzeichnisSuchmodus.NurBlattverzeichnisse

    Public Structure SettingsBildauswahl
        Public Verzeichnisse As List(Of String)
        Public WhiteListTags As List(Of String)
        Public BlackListTags As List(Of String)
        Public Altersfreigabe As String
        Public Bewertung As Integer
    End Structure

    Private Enum AltersfreigabeStufe
        Jugendfrei = 0
        Lingerie = 1
        Akt = 2
        Volljaehrig = 3
    End Enum

    Private Shared ReadOnly altersfreigabeTags As New HashSet(Of String)(
    StringComparer.OrdinalIgnoreCase) From {
        "18+",
        "Akt",
        "Lingerie"
    }

#End Region

#Region "Defaults und effektive Blacklist"

    Public Shared Function GetBildauswahlDefaultSettings() As Dictionary(Of String, String)
        ' Gibt die Defaultwerte von SlideShowBildauswahl als Dictionary zurück.

        Dim defaults As New Dictionary(Of String, String)

        defaults("Verzeichnisse") = "D:\Arbeits- und Sortierbereich;D:\Eigene Bilder;J:\Bilder" 'Liste der Verzeichnisse (durch Semikola getrennt) 
        defaults("WhiteListTags") = "" 'Liste der Tags in der White-List (durch Semikola getrennt)
        defaults("BlackListTags") = "Extern; Anna_Akt; Kiki_Akt; Daphne_Akt; Sirenen_Akt" 'Liste der Tags in der Black-List (durch Semikola getrennt)
        defaults("Altersfreigabe") = "Lingerie" 'Stufe der Altersfreigabe
        defaults("Bewertung") = "4" 'Minimale Bewertung

        Return defaults

    End Function

    Public Shared Function IstAltersfreigabeTag(tag As String) As Boolean
        'Prüft, ob ein Tag intern durch die Altersfreigabe verwaltet wird.

        If String.IsNullOrWhiteSpace(tag) Then
            Return False
        End If

        Return altersfreigabeTags.Contains(tag.Trim())

    End Function

    Public Shared Function BereinigeBenutzerBlacklist(tags As IEnumerable(Of String)) As List(Of String)
        'Entfernt die intern verwalteten Altersfreigabe-Tags aus einer Benutzer-Blacklist.

        Dim ergebnis As New List(Of String)
        Dim bekannteTags As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim tag As String
        Dim bereinigtesTag As String

        If tags Is Nothing Then
            Return ergebnis
        End If

        For Each tag In tags

            If String.IsNullOrWhiteSpace(tag) Then
                Continue For
            End If

            bereinigtesTag = tag.Trim()

            If IstAltersfreigabeTag(bereinigtesTag) Then
                Continue For
            End If

            If bekannteTags.Add(bereinigtesTag) Then
                ergebnis.Add(bereinigtesTag)
            End If

        Next

        Return ergebnis

    End Function

    Private Shared Function ErstelleEffektiveBlacklist(settings As SettingsBildauswahl) As HashSet(Of String)
        'Kombiniert Benutzer-Blacklist und unsichtbare Altersfreigabe-Blacklist.

        Dim ergebnis As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim benutzerTags As List(Of String)
        Dim tag As String

        benutzerTags = BereinigeBenutzerBlacklist(settings.BlackListTags)

        For Each tag In benutzerTags
            ergebnis.Add(tag)
        Next

        Select Case settings.Altersfreigabe

            Case "18+"
            'Keine zusätzlichen Sperrtags

            Case "Akt"
                ergebnis.Add("18+")

            Case "Lingerie"
                ergebnis.Add("18+")
                ergebnis.Add("Akt")

            Case "Jugendfrei"
                ergebnis.Add("18+")
                ergebnis.Add("Akt")
                ergebnis.Add("Lingerie")

            Case Else

                'Unbekannte oder beschädigte Einstellung:
                'Sicherer Fallback auf Jugendfrei.
                ergebnis.Add("18+")
                ergebnis.Add("Akt")
                ergebnis.Add("Lingerie")

                LogHandling.LogWarn(
                "Unbekannte Altersfreigabe """ &
                settings.Altersfreigabe &
                """. Es wird sicherheitshalber Jugendfrei verwendet.")

        End Select

        Return ergebnis

    End Function

#End Region

#Region "Settingsübernahme und Vorbereitungslauf"

    Public Shared Sub CheckYourSettings()
        'Liest die aktuellen Bildauswahlsettings ein und startet bei einer Änderung
        'einen neuen progressiven Vorbereitungslauf.

        Dim neueSettings As SettingsBildauswahl
        Dim settingsSnapshot As SettingsBildauswahl
        Dim bekannteVerzeichnisse As List(Of String)
        Dim alteCancellation As CancellationTokenSource
        Dim alterTask As Task
        Dim neueCancellation As CancellationTokenSource
        Dim neueGeneration As Integer
        Dim neuerTask As Task

        neueSettings = ReadSettingsBildauswahlFromRegistryOrDefauls()

        'Die Settings werden unabhängig von einem nötigen Neuaufbau für das
        'Options-UserControl bereitgestellt.
        StoreSettings("Bildauswahl", neueSettings)

        SyncLock vorbereitungsLock

            If SettingsSindIdentisch(
            neueSettings,
            vorbereitungsSettings) AndAlso
           vorbereitungsTask IsNot Nothing Then

                aktuelleSettings = KopiereSettings(neueSettings)

                Exit Sub

            End If

            alteCancellation = vorbereitungsCancellation
            alterTask = vorbereitungsTask

            'Eine neue Generation macht Veröffentlichungen eines alten Tasks
            'ab diesem Moment ungültig.
            neueGeneration = Interlocked.Increment(vorbereitungsGeneration)

            aktuelleSettings = KopiereSettings(neueSettings)
            vorbereitungsSettings = KopiereSettings(neueSettings)
            settingsSnapshot = KopiereSettings(neueSettings)

            bekannteVerzeichnisse = letzteBekannteVerzeichnisse.ToList()

            neueCancellation = New CancellationTokenSource()
            vorbereitungsCancellation = neueCancellation

            'Die aktiven Bestände werden sofort geleert. Ein bereits im Modul
            'geladenes Bild bleibt davon unberührt.
            SyncLock bestandLock

                vorbereiteteDateien.Clear()
                vorbereiteteBildPfade.Clear()
                vorbereiteteVerzeichnisse.Clear()

            End SyncLock

            neuerTask =
            Task.Run(
                Sub()
                    FuehreVorbereitungDurch(
                        settingsSnapshot,
                        bekannteVerzeichnisse,
                        neueGeneration,
                        neueCancellation.Token)
                End Sub,
                neueCancellation.Token)

            vorbereitungsTask = neuerTask

        End SyncLock

        'Der alte Lauf wird außerhalb des Locks abgebrochen.
        If alteCancellation IsNot Nothing Then

            Try
                alteCancellation.Cancel()
            Catch ex As ObjectDisposedException
                'Die CancellationTokenSource war bereits beendet.
            End Try

        End If

        EntsorgeCancellationNachTaskende(alterTask, alteCancellation)

    End Sub

    Private Shared Sub FuehreVorbereitungDurch(
    settingsSnapshot As SettingsBildauswahl,
    bekannteVerzeichnisse As List(Of String),
    generation As Integer,
    cancellationToken As CancellationToken
)
        'Ermittelt die aktuelle Verzeichnisstruktur, priorisiert bereits bekannte
        'Verzeichnisse und prüft jedes Verzeichnis genau einmal.

        Dim alleVerzeichnisse As List(Of String)
        Dim bekannteVerzeichnisMenge As HashSet(Of String)
        Dim bekannteAktuelleVerzeichnisse As List(Of String)
        Dim neueVerzeichnisse As List(Of String)
        Dim sortiertePruefreihenfolge As New List(Of String)
        Dim verzeichnis As String

        If cancellationToken.IsCancellationRequested Then
            Exit Sub
        End If

        alleVerzeichnisse =
        FileHandling.ErmittleVerzeichnisse(
            settingsSnapshot.Verzeichnisse,
            verzeichnisSuchmodus,
            cancellationToken)

        If cancellationToken.IsCancellationRequested OrElse Not IstAktuelleVorbereitung(generation) Then

            Exit Sub

        End If

        bekannteVerzeichnisMenge =
        New HashSet(Of String)(
            If(
                bekannteVerzeichnisse,
                New List(Of String)),
            StringComparer.OrdinalIgnoreCase)

        bekannteAktuelleVerzeichnisse =
        alleVerzeichnisse.
        Where(
            Function(pfad)
                Return bekannteVerzeichnisMenge.Contains(pfad)
            End Function).
        ToList()

        neueVerzeichnisse =
        alleVerzeichnisse.
        Where(
            Function(pfad)
                Return Not bekannteVerzeichnisMenge.Contains(pfad)
            End Function).
        ToList()

        'Beide Gruppen werden unabhängig gemischt. Bereits bekannte Verzeichnisse
        'bleiben als Gruppe vorn, beginnen aber nicht immer in derselben Reihenfolge.
        MischeListe(bekannteAktuelleVerzeichnisse)
        MischeListe(neueVerzeichnisse)

        sortiertePruefreihenfolge.AddRange(bekannteAktuelleVerzeichnisse)
        sortiertePruefreihenfolge.AddRange(neueVerzeichnisse)

        'Die vollständig ermittelte aktuelle Verzeichnisstruktur wird für einen
        'späteren Settingswechsel vorgemerkt.
        SyncLock vorbereitungsLock

            If IstAktuelleVorbereitung(generation) Then

                letzteBekannteVerzeichnisse =
                alleVerzeichnisse.ToList()

            End If

        End SyncLock

        For Each verzeichnis In sortiertePruefreihenfolge

            If cancellationToken.IsCancellationRequested OrElse Not IstAktuelleVorbereitung(generation) Then

                Exit For

            End If

            VerarbeiteVerzeichnis(verzeichnis, settingsSnapshot, generation, cancellationToken)

        Next

    End Sub

#End Region

#Region "Settings lesen und Bildfilterung"

    Public Shared Function ReadSettingsBildauswahlFromRegistryOrDefauls() As SettingsBildauswahl
        'Liest alle Bildauswahl-Settings aus der Registry oder verwendet die Defaults.
        'Historisch gespeicherte Altersfreigabe-Tags werden aus der Benutzer-Blacklist entfernt.

        Dim defaults As New Dictionary(Of String, String)
        Dim settings As New SettingsBildauswahl
        Dim gespeicherteBlacklist As List(Of String)
        Dim bereinigteBlacklist As List(Of String)
        Dim tempRegVal As String

        defaults = GetBildauswahlDefaultSettings()

        'Verzeichnisse
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse", defaults)

        settings.Verzeichnisse = SplitSemicolonList(tempRegVal)

        'White-List
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags", defaults)

        settings.WhiteListTags = SplitSemicolonList(tempRegVal)

        'Benutzer-Blacklist
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", defaults)

        gespeicherteBlacklist = SplitSemicolonList(tempRegVal)
        bereinigteBlacklist = BereinigeBenutzerBlacklist(gespeicherteBlacklist)

        settings.BlackListTags = bereinigteBlacklist

        'Altersfreigabe
        settings.Altersfreigabe = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", defaults)

        'Bewertung
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Bewertung", defaults)

        If Not Integer.TryParse(tempRegVal, settings.Bewertung) Then
            settings.Bewertung = CInt(defaults("Bewertung"))
        End If

        'Alte Registryeinträge einmalig auf das neue Modell migrieren.
        If gespeicherteBlacklist.Count <> bereinigteBlacklist.Count Then

            WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", String.Join(";", bereinigteBlacklist))

            LogHandling.LogInfo("Historische Altersfreigabe-Tags wurden aus der Benutzer-Blacklist entfernt.")

        End If

        Return settings

    End Function

    Public Shared Function CheckIfLegalFile(bild As String) As Boolean
        'Prüft ein Bild anhand eines stabilen Snapshots der aktuellen Settings.

        Dim settingsSnapshot As SettingsBildauswahl

        SyncLock vorbereitungsLock

            settingsSnapshot = KopiereSettings(aktuelleSettings)

        End SyncLock

        Return CheckIfLegalFile(bild, settingsSnapshot)

    End Function

    Private Shared Function CheckIfLegalFile(bild As String, settingsSnapshot As SettingsBildauswahl) As Boolean
        'Prüft, ob ein Bild den übergebenen Auswahlkriterien entspricht.

        Dim metadaten As Metadata
        Dim keywords As HashSet(Of String)
        Dim whitelist As HashSet(Of String)
        Dim effektiveBlacklist As HashSet(Of String)
        Dim whitelistIstErfuellt As Boolean
        Dim blacklistIstErfuellt As Boolean
        Dim bewertungIstErfuellt As Boolean
        Dim tag As String

        metadaten = ExtractMetadataFromImage(bild)

        keywords = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        If metadaten.Keywords IsNot Nothing Then

            For Each tag In metadaten.Keywords

                If Not String.IsNullOrWhiteSpace(tag) Then

                    keywords.Add(tag.Trim())

                End If

            Next

        End If

        whitelist =
        New HashSet(Of String)(
            If(
                settingsSnapshot.WhiteListTags,
                New List(Of String)),
            StringComparer.OrdinalIgnoreCase)

        effektiveBlacklist = ErstelleEffektiveBlacklist(settingsSnapshot)

        bewertungIstErfuellt = (metadaten.Rating >= settingsSnapshot.Bewertung)

        If whitelist.Count = 0 Then

            whitelistIstErfuellt = True

        Else

            whitelistIstErfuellt =
            whitelist.Any(
                Function(whitelistTag)
                    Return keywords.Contains(
                        whitelistTag)
                End Function)

        End If

        blacklistIstErfuellt =
        Not effektiveBlacklist.Any(
            Function(blacklistTag)
                Return keywords.Contains(
                    blacklistTag)
            End Function)

        Return bewertungIstErfuellt AndAlso whitelistIstErfuellt AndAlso blacklistIstErfuellt

    End Function

    Private Shared Function KopiereSettings(quelle As SettingsBildauswahl) As SettingsBildauswahl
        'Erstellt eine unabhängige Kopie der Bildauswahlsettings.

        Dim ergebnis As New SettingsBildauswahl

        ergebnis.Bewertung = quelle.Bewertung
        ergebnis.Altersfreigabe = quelle.Altersfreigabe

        ergebnis.Verzeichnisse =
        If(
            quelle.Verzeichnisse,
            New List(Of String)).
        ToList()

        ergebnis.WhiteListTags =
        If(
            quelle.WhiteListTags,
            New List(Of String)).
        ToList()

        ergebnis.BlackListTags =
        If(
            quelle.BlackListTags,
            New List(Of String)).
        ToList()

        Return ergebnis

    End Function

#End Region

#Region "Verzeichnisverarbeitung und Veröffentlichung"

    Private Shared Sub VerarbeiteVerzeichnis(
    verzeichnis As String,
    settingsSnapshot As SettingsBildauswahl,
    generation As Integer,
    cancellationToken As CancellationToken
)
        'Prüft die Bilder eines Verzeichnisses und veröffentlicht jedes gültige
        'Bild unmittelbar in beiden aktiven Beständen.

        Dim dateiTypen As New List(Of String) From {".bmp", ".jpg", ".jpeg", ".png"}

        Dim bilder As List(Of String)
        Dim bild As String
        Dim bildIstZulaessig As Boolean

        bilder =
        FileHandling.ErmittleDateienImVerzeichnis(
            verzeichnis,
            dateiTypen,
            cancellationToken)

        For Each bild In bilder

            If cancellationToken.IsCancellationRequested OrElse Not IstAktuelleVorbereitung(generation) Then

                Exit For

            End If

            If FileHandling.IstJpegDatei(bild) Then

                bildIstZulaessig =
                CheckIfLegalFile(
                    bild,
                    settingsSnapshot)

            Else

                'BMP und PNG werden weiterhin ohne JPEG-Metadatenprüfung zugelassen.
                bildIstZulaessig = True

            End If

            If Not bildIstZulaessig Then
                Continue For
            End If

            VeroeffentlicheBild(verzeichnis, bild, generation, cancellationToken)

        Next

    End Sub

    Private Shared Sub VeroeffentlicheBild(
    verzeichnis As String,
    bild As String,
    generation As Integer,
    cancellationToken As CancellationToken
)
        'Veröffentlicht ein gültiges Bild threadsicher in beiden Beständen.

        Dim verzeichnisBilder As List(Of String)

        If cancellationToken.IsCancellationRequested OrElse Not IstAktuelleVorbereitung(generation) Then

            Exit Sub

        End If

        SyncLock bestandLock

            'Nach dem Warten auf den Lock muss der Lauf erneut validiert werden.
            If cancellationToken.IsCancellationRequested OrElse Not IstAktuelleVorbereitung(generation) Then

                Exit Sub

            End If

            If Not vorbereiteteBildPfade.Add(bild) Then
                Exit Sub
            End If

            vorbereiteteDateien.Add(bild)

            If Not vorbereiteteVerzeichnisse.TryGetValue(verzeichnis, verzeichnisBilder) Then

                verzeichnisBilder = New List(Of String)

                vorbereiteteVerzeichnisse.Add(verzeichnis, verzeichnisBilder)

            End If

            verzeichnisBilder.Add(bild)

        End SyncLock

    End Sub

#End Region

#Region "Öffentliche Auswahl-API"

    Public Shared Function TryGetRandomPicture(ByRef bildPfad As String) As Boolean
        'Liefert ein zufälliges Bild aus dem aktuell verfügbaren Bestand.

        Dim index As Integer

        bildPfad = Nothing

        SyncLock bestandLock

            If vorbereiteteDateien.Count = 0 Then
                Return False
            End If

            index = GetRandomNumber(vorbereiteteDateien.Count)

            bildPfad = vorbereiteteDateien(index)

        End SyncLock

        Return True

    End Function

    Public Shared Function TryGetRandomDirectoryPictures(ByRef bildPfade As List(Of String)) As Boolean
        'Liefert eine Momentaufnahme der derzeit bekannten Bilder eines zufälligen
        'Verzeichnisses. Die Bilder werden alphabetisch sortiert zurückgegeben.

        Dim verzeichnisPfade As List(Of String)
        Dim verzeichnisPfad As String
        Dim index As Integer

        bildPfade = New List(Of String)

        SyncLock bestandLock

            If vorbereiteteVerzeichnisse.Count = 0 Then
                Return False
            End If

            verzeichnisPfade = vorbereiteteVerzeichnisse.Keys.ToList()

            index = GetRandomNumber(verzeichnisPfade.Count)

            verzeichnisPfad = verzeichnisPfade(index)

            bildPfade = vorbereiteteVerzeichnisse(verzeichnisPfad).ToList()

        End SyncLock

        bildPfade.Sort(StringComparer.OrdinalIgnoreCase)

        Return bildPfade.Count > 0

    End Function

    Public Shared Function TryGetDirectoryPictures(verzeichnisPfad As String,
                                                   ByRef bildPfade As List(Of String)) As Boolean
        'Liefert eine Momentaufnahme der derzeit bekannten Bilder des angegebenen
        'Verzeichnisses.

        Dim gespeicherteBilder As List(Of String)

        bildPfade = New List(Of String)

        If String.IsNullOrWhiteSpace(verzeichnisPfad) Then
            Return False
        End If

        SyncLock bestandLock

            If Not vorbereiteteVerzeichnisse.TryGetValue(verzeichnisPfad, gespeicherteBilder) Then

                Return False

            End If

            bildPfade = gespeicherteBilder.ToList()

        End SyncLock

        bildPfade.Sort(StringComparer.OrdinalIgnoreCase)

        Return bildPfade.Count > 0

    End Function

    Public Shared Function GetPreparedPictureCount() As Integer
        'Liefert die aktuelle Anzahl vorbereiteter Einzelbilder.

        SyncLock bestandLock
            Return vorbereiteteDateien.Count
        End SyncLock

    End Function

    Public Shared Function GetPreparedDirectoryCount() As Integer
        'Liefert die aktuelle Anzahl verwendbarer Bildverzeichnisse.

        SyncLock bestandLock
            Return vorbereiteteVerzeichnisse.Count
        End SyncLock

    End Function

#End Region

#Region "Zufall, Abbruch und Vergleichshilfen"

    Private Shared Function GetRandomNumber(maximumExclusive As Integer) As Integer
        'Liefert threadsicher eine Zufallszahl zwischen 0 und maximumExclusive - 1.

        If maximumExclusive <= 0 Then
            Return 0
        End If

        SyncLock zufallLock

            Return rnd.Next(maximumExclusive)

        End SyncLock

    End Function

    Private Shared Sub MischeListe(Of T)(liste As IList(Of T))
        'Mischt eine Liste mit dem Fisher-Yates-Verfahren.

        Dim i As Integer
        Dim tauschIndex As Integer
        Dim tempItem As T

        If liste Is Nothing OrElse liste.Count < 2 Then
            Exit Sub
        End If

        For i = liste.Count - 1 To 1 Step -1

            tauschIndex = GetRandomNumber(i + 1)

            tempItem = liste(i)
            liste(i) = liste(tauschIndex)
            liste(tauschIndex) = tempItem

        Next

    End Sub

    Private Shared Function IstAktuelleVorbereitung(generation As Integer) As Boolean
        'Prüft, ob ein Hintergrundlauf noch zur aktuellen Vorbereitung gehört.

        Return generation = Volatile.Read(vorbereitungsGeneration)

    End Function

    Private Shared Sub EntsorgeCancellationNachTaskende(task As Task, cancellation As CancellationTokenSource)
        'Entsorgt eine abgelöste CancellationTokenSource nach dem Ende ihres Tasks.

        If cancellation Is Nothing Then
            Exit Sub
        End If

        If task Is Nothing OrElse task.IsCompleted Then

            cancellation.Dispose()

            Exit Sub

        End If

        task.ContinueWith(
        Sub(abgeschlossenerTask)

            cancellation.Dispose()

        End Sub,
        CancellationToken.None,
        TaskContinuationOptions.ExecuteSynchronously,
        TaskScheduler.Default)

    End Sub

    Public Shared Sub StoppeVorbereitung()
        'Beendet den aktuell laufenden Vorbereitungstask kontrolliert.

        Dim cancellation As CancellationTokenSource
        Dim task As Task

        SyncLock vorbereitungsLock

            Interlocked.Increment(vorbereitungsGeneration)

            cancellation = vorbereitungsCancellation
            task = vorbereitungsTask

            vorbereitungsCancellation = Nothing
            vorbereitungsTask = Nothing

        End SyncLock

        If cancellation IsNot Nothing Then

            Try
                cancellation.Cancel()
            Catch ex As ObjectDisposedException
                'Bereits beendet.
            End Try

        End If

        If task IsNot Nothing AndAlso
       Not task.IsCompleted Then

            Try

                task.Wait(500)

            Catch ex As AggregateException

                'Ein abgebrochener Task ist beim Beenden erwartbar.

            Catch ex As OperationCanceledException

                'Der Task wurde wie angefordert abgebrochen.

            End Try

        End If

        If cancellation IsNot Nothing Then
            cancellation.Dispose()
        End If

    End Sub

    Public Shared Function SettingsSindIdentisch(a As SettingsBildauswahl, b As SettingsBildauswahl) As Boolean
        'Vergleicht Bildauswahlsettings unabhängig von Reihenfolge und Großschreibung.

        If a.Bewertung <> b.Bewertung Then
            Return False
        End If

        If Not String.Equals(a.Altersfreigabe, b.Altersfreigabe, StringComparison.OrdinalIgnoreCase) Then

            Return False

        End If

        If Not ListenSindIdentisch(a.BlackListTags, b.BlackListTags) Then

            Return False

        End If

        If Not ListenSindIdentisch(a.WhiteListTags, b.WhiteListTags) Then

            Return False

        End If

        If Not ListenSindIdentisch(a.Verzeichnisse, b.Verzeichnisse) Then

            Return False

        End If

        Return True

    End Function

    Private Shared Function ListenSindIdentisch(a As IEnumerable(Of String), b As IEnumerable(Of String)) As Boolean
        'Vergleicht zwei Stringlisten als case-insensitive Mengen.

        Dim mengeA As HashSet(Of String)
        Dim mengeB As HashSet(Of String)

        mengeA = ErstelleStringMenge(a)
        mengeB = ErstelleStringMenge(b)

        Return mengeA.SetEquals(mengeB)

    End Function

    Private Shared Function ErstelleStringMenge(werte As IEnumerable(Of String)) As HashSet(Of String)
        'Erstellt eine bereinigte case-insensitive Stringmenge.

        Dim ergebnis As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim wert As String

        If werte Is Nothing Then
            Return ergebnis
        End If

        For Each wert In werte

            If String.IsNullOrWhiteSpace(wert) Then
                Continue For
            End If

            ergebnis.Add(wert.Trim())

        Next

        Return ergebnis

    End Function

#End Region

#Region "Altersfreigabe"

    Private Shared Function ErmittleAltersfreigabeStufe(altersfreigabe As String) As AltersfreigabeStufe
        'Ermittelt die interne Hierarchiestufe einer Altersfreigabe.

        Select Case altersfreigabe

            Case "18+"

                Return AltersfreigabeStufe.Volljaehrig

            Case "Akt"

                Return AltersfreigabeStufe.Akt

            Case "Lingerie"

                Return AltersfreigabeStufe.Lingerie

            Case "Jugendfrei"

                Return AltersfreigabeStufe.Jugendfrei

            Case Else

                Return AltersfreigabeStufe.Jugendfrei

        End Select

    End Function

    Private Shared Function ErmittleMindestfreigabeFuerTag(tag As String, ByRef mindestfreigabe As AltersfreigabeStufe) As Boolean
        'Ermittelt die Mindestfreigabe eines bekannten Altersfreigabe-Tags.
        'Bei gewöhnlichen Benutzertags wird False zurückgegeben.

        If String.IsNullOrWhiteSpace(tag) Then
            Return False
        End If

        Select Case tag.Trim().ToLowerInvariant()

            Case "18+"

                mindestfreigabe = AltersfreigabeStufe.Volljaehrig

                Return True

            Case "akt"

                mindestfreigabe = AltersfreigabeStufe.Akt

                Return True

            Case "lingerie"

                mindestfreigabe = AltersfreigabeStufe.Lingerie

                Return True

            Case Else

                Return False

        End Select

    End Function

    Private Shared Function KonvertiereAltersfreigabeStufe(stufe As AltersfreigabeStufe) As String
        'Konvertiert eine interne Altersfreigabestufe in ihren gespeicherten Wert.

        Select Case stufe

            Case AltersfreigabeStufe.Volljaehrig

                Return "18+"

            Case AltersfreigabeStufe.Akt

                Return "Akt"

            Case AltersfreigabeStufe.Lingerie

                Return "Lingerie"

            Case Else

                Return "Jugendfrei"

        End Select

    End Function

    Public Shared Function IstWhitelistTagMitAltersfreigabeKompatibel(tag As String, altersfreigabe As String,
                                                                      ByRef erforderlicheAltersfreigabe As String) As Boolean
        'Prüft, ob ein Whitelist-Tag mit der aktuellen Altersfreigabe vereinbar ist.
        'Gewöhnliche Benutzertags gelten grundsätzlich als kompatibel.

        Dim aktuelleStufe As AltersfreigabeStufe
        Dim erforderlicheStufe As AltersfreigabeStufe

        erforderlicheAltersfreigabe = Nothing

        If Not ErmittleMindestfreigabeFuerTag(tag, erforderlicheStufe) Then

            Return True

        End If

        aktuelleStufe = ErmittleAltersfreigabeStufe(altersfreigabe)

        erforderlicheAltersfreigabe = KonvertiereAltersfreigabeStufe(erforderlicheStufe)

        Return aktuelleStufe >= erforderlicheStufe

    End Function

#End Region

End Class