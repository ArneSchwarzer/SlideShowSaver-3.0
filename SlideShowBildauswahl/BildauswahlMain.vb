Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports System.Windows.Forms
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Xmp
Imports MetadataExtractor.Formats.Exif
Imports SlideShowLogging
Imports System.Threading

Public Class BildauswahlMain

    'Variablen, Konstanten und Enums
    Private Shared rnd As New Random()
    Private Shared aktuelleSettings As New SettingsBildauswahl

    'Preload Bilderlisten
    Private Shared vorbereiteteDateien As New List(Of String)
    Private Shared vorbereiteteVerzeichnisse As New Dictionary(Of String, List(Of String))
    Private Shared vorbereitungsThreadPictures As Thread = Nothing
    Private Shared vorbereitungsThreadVerzeichnisse As Thread = Nothing
    Private Shared prepareSettingsSnapshot As SettingsBildauswahl
    Public Shared hasFirstResultsPictues As Boolean = False
    Public Shared hasFirstResultsVerzeichnisse As Boolean = False
    Private Shared cancelThreads As Boolean = False

    Public Structure SettingsBildauswahl
        Public Verzeichnisse As List(Of String)
        Public WhiteListTags As List(Of String)
        Public BlackListTags As List(Of String)
        Public Altersfreigabe As String
        Public Bewertung As Integer
    End Structure

    'Events
    Public Shared Event ErsteBilderGefunden()
    Public Shared Event ErsteVerzeichnisseGefunden()

    Public Shared Function GetBildauswahlDefaultSettings() As Dictionary(Of String, String)
        ' Gibt die Defaultwerte von SlideShowBildauswahl als Dictionary zurück.

        Dim defaults As New Dictionary(Of String, String)

        defaults("Verzeichnisse") = "D:\Arbeits- und Sortierbereich;D:\Eigene Bilder;J:\Bilder" 'Liste der Verzeichnisse (durch Semikola getrennt) 
        defaults("WhiteListTags") = "" 'Liste der Tags in der White-List (durch Semikola getrennt)
        defaults("BlackListTags") = "Extern; 18+; Akt; Anna_Akt; Kiki_Akt; Daphne_Akt; Sirenen_Akt" 'Liste der Tags in der Black-List (durch Semikola getrennt)
        defaults("Altersfreigabe") = "Lingerie" 'Stufe der Altersfreigabe
        defaults("Bewertung") = "4" 'Minimale Bewertung

        Return defaults

    End Function

    Public Shared Sub CheckYourSettings()
        'Aktualisiert die Settings der Bildauswahl und schreibt sie in die SettingsInbox

        aktuelleSettings = ReadSettingsBildauswahlFromRegistryOrDefauls()

        If Not SettingsSindIdentisch(aktuelleSettings, prepareSettingsSnapshot) Then
            StoreSettings("Bildauswahl", aktuelleSettings)
            vorbereitungsThreadPictures = Nothing 'löscht ggf. alten Thread
            vorbereitungsThreadVerzeichnisse = Nothing
            vorbereitungsThreadPictures = New Thread(AddressOf StarteVorbereitungenPictures)
            vorbereitungsThreadVerzeichnisse = New Thread(AddressOf StarteVorbereitungenVerzeichnisse)
            vorbereitungsThreadPictures.IsBackground = True
            vorbereitungsThreadVerzeichnisse.IsBackground = True
            vorbereitungsThreadPictures.Start()
            vorbereitungsThreadVerzeichnisse.Start()
        End If

    End Sub

    Public Shared Function ReadSettingsBildauswahlFromRegistryOrDefauls() As SettingsBildauswahl
        ' Alle Werte der Bildauswahl-Settings aus der Registry auslesen und als Struktur SettingsBildauswahl zurückgeben

        Dim defaults As New Dictionary(Of String, String)
        Dim settings As New SettingsBildauswahl
        Dim tempRegVal As String

        defaults = GetBildauswahlDefaultSettings()

        'Verzeichnisse
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse", defaults)
        settings.Verzeichnisse = SplitSemicolonList(tempRegVal)

        'White-List
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags", defaults)
        settings.WhiteListTags = SplitSemicolonList(tempRegVal)

        'Black-List
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", defaults)
        settings.BlackListTags = SplitSemicolonList(tempRegVal)

        'Altersfreigabe
        settings.Altersfreigabe = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", defaults)

        'Bewertung
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Bewertung", defaults)
        settings.Bewertung = CInt(tempRegVal)

        Return settings

    End Function

    Public Shared Function CreateFileList(verzeichnisse As List(Of String), endungen As List(Of String)) As List(Of String)
        'Liest alle Dateien (komplette Dateipfade) mit den in "endungen" angegebenen Dateiendungen aus der Liste der in
        '"verzeichnisse" angegebenen Verzeichnisse (und deren Unterverzichnisse) und gibt sie als Liste zurück.
        '
        'ACHTUNG! Diese Version ist auf 'Blattverzeichnisse' hin optimiert. D.h. es wird immer nur das letzte Unterverzeichnis
        'eines Dateipades berücksichtigt. Dies bedingt eine Dateistruktur, in der keine Bilddateien in Verzeichnissen
        'gefunden werden, in denen auch noch weitere Unterordner vorhanden sind! Dies dient zur Optimierung des Suchvorgangs
        'auf die Verzeichnisstruktur des Authors dieser Software.
        '
        'Eine "klassische" Verzeichnissuche, die auch Dateien in solchen Ordnern findet in denen Dateien und Unterordner
        'gemeinsam liegen ist auskommentiert am Ende der Funktion zu finden.

        Dim listOfFiles As New List(Of String)

        For Each verzeichnis In verzeichnisse

            If Not System.IO.Directory.Exists(verzeichnis) Then
                ' Verzeichnis existiert nicht, logge und überspringe
                LogHandling.LogError("Verzeichnis nicht gefunden: " & verzeichnis)
                Continue For
            End If

            Dim unterverzeichnisse As String() = {}
            Try
                unterverzeichnisse = System.IO.Directory.GetDirectories(verzeichnis, "*", SearchOption.AllDirectories)
            Catch ex As Exception
                LogHandling.LogError("Fehler beim Durchsuchen von Unterverzeichnissen in '" & verzeichnis & "': " & ex.Message)
                Continue For
            End Try

            ' Füge das Hauptverzeichnis zur Liste hinzu
            Dim alleVerzeichnisse = New List(Of String) From {verzeichnis}
            alleVerzeichnisse.AddRange(unterverzeichnisse)

            For Each pfad In alleVerzeichnisse
                Dim subdirs() As String = {}
                Try
                    subdirs = System.IO.Directory.GetDirectories(pfad)
                Catch ex As Exception
                    LogHandling.LogError("Fehler beim Abrufen von Unterverzeichnissen in '" & pfad & "': " & ex.Message)
                    Continue For
                End Try

                ' Nur wenn keine Unterverzeichnisse existieren => Blattverzeichnis
                If subdirs.Length = 0 Then
                    Try
                        Dim files = System.IO.Directory.GetFiles(pfad, "*.*", SearchOption.TopDirectoryOnly).
                            Where(Function(f) endungen.Any(Function(ext) f.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).
                            ToList()
                        listOfFiles.AddRange(files)
                    Catch ex As Exception
                        LogHandling.LogError("Fehler beim Durchsuchen von Dateien in '" & pfad & "': " & ex.Message)
                    End Try
                End If
            Next
        Next

        Return listOfFiles

        ' === Klassische Suchfunktion ===

        'Dim listOfFiles As New List(Of String)
        'Dim basefolder As String
        'Dim endung As String
        'Dim gefundeneDateien As String()

        'For Each endung In endungen
        '    For Each basefolder In verzeichnisse
        '        Try
        '            gefundeneDateien = Directory.GetFiles(basefolder, endung, SearchOption.AllDirectories)
        '            listOfFiles.AddRange(gefundeneDateien)
        '        Catch ex As Exception
        '            LogHandling.LogError("SlideShowBildauswahl meldet ein Problem bei der Erstellung der Dateiliste: " & ex.ToString)
        '        End Try
        '    Next
        'Next

        'Return listOfFiles

    End Function

    Public Shared Function CheckIfLegalFile(bild As String) As Boolean
        ' Prüft, ob ein Bild den Kriterien gemäß den aktuellen Settings entspricht

        Dim checkWhitelist As Boolean = False
        Dim checkBlacklist As Boolean = True
        Dim checkBewertung As Boolean = False

        Dim ratingStr As String
        Dim rating As Integer

        Try
            Dim directories = ImageMetadataReader.ReadMetadata(bild)
            Dim xmpDir = directories.OfType(Of XmpDirectory)().FirstOrDefault()

            If xmpDir IsNot Nothing Then
                Dim xmp = xmpDir.XmpMeta
                If xmp IsNot Nothing Then

                    'Check #1: Bewertung
                    ratingStr = xmp.GetPropertyString("http://ns.adobe.com/xap/1.0/", "Rating")
                    If Not String.IsNullOrEmpty(ratingStr) Then
                        If Integer.TryParse(ratingStr, rating) Then
                            If rating >= aktuelleSettings.Bewertung Then
                                checkBewertung = True
                            End If
                        End If
                    End If

                    'Check #2: WhiteList
                    If aktuelleSettings.WhiteListTags.Count = 0 Then
                        checkWhitelist = True
                    Else
                        Dim keywords = xmp.GetPropertyString("http://purl.org/dc/elements/1.1/", "subject")
                        If Not String.IsNullOrEmpty(keywords) Then
                            For Each includeTag In aktuelleSettings.WhiteListTags
                                If keywords.IndexOf(includeTag, StringComparison.OrdinalIgnoreCase) >= 0 Then
                                    checkWhitelist = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    'Check #3: BlackList
                    Dim blacklisted As Boolean = False
                    Dim blackKeywords = xmp.GetPropertyString("http://purl.org/dc/elements/1.1/", "subject")
                    If Not String.IsNullOrEmpty(blackKeywords) Then
                        For Each excludeTag In aktuelleSettings.BlackListTags
                            If blackKeywords.IndexOf(excludeTag, StringComparison.OrdinalIgnoreCase) >= 0 Then
                                blacklisted = True
                                Exit For
                            End If
                        Next
                    End If
                    If blacklisted Then checkBlacklist = False

                End If
            End If

        Catch ex As Exception
            LogHandling.LogError("SlideShowBildauswahl.CheckIfLegalFile(" & bild & ") - Fehler beim Auslesen mit MetadataExtractor: " & ex.Message)
        End Try

        Return checkBewertung AndAlso checkWhitelist AndAlso checkBlacklist
    End Function

    Public Shared Function GetCurrentScreen() As Image
        ' Erstellt einen Screenshot

        Dim bounds As Rectangle = Screen.PrimaryScreen.Bounds ' Größe des primären Bildschirms ermitteln
        Dim screenshot As New Bitmap(bounds.Width, bounds.Height) ' Bitmap mit Bildschirmgröße erzeugen

        ' Inhalt des Bildschirms in die Bitmap kopieren
        Using g As Graphics = Graphics.FromImage(screenshot)
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size)
        End Using

        Return screenshot

    End Function

    Public Shared Sub PreparePictures()
        prepareSettingsSnapshot = aktuelleSettings
        hasFirstResultsPictues = False
        vorbereiteteDateien.Clear()

        Dim dateiTypen As New List(Of String) From {".bmp", ".jpg", ".jpeg", ".png"}
        Dim bilderListe = CreateFileList(prepareSettingsSnapshot.Verzeichnisse, dateiTypen)

        If cancelThreads Then Exit Sub

        For Each bild In bilderListe
            If cancelThreads Then Exit For

            If Path.GetExtension(bild).ToLower() Like "*.jp*g" Then
                If Not CheckIfLegalFile(bild) Then Continue For
            End If

            vorbereiteteDateien.Add(bild)

            If Not hasFirstResultsPictues AndAlso vorbereiteteDateien.Count >= 2 Then
                hasFirstResultsPictues = True
                RaiseEvent ErsteBilderGefunden()
            End If
        Next

        vorbereitungsThreadPictures = Nothing
    End Sub

    Public Shared Function GetPictures(n As Integer, Optional targetDir As String = "") As List(Of String)
        ' Die Pfade von n Bildern werden gemäß den Einstellungen zufällig geladen. Die Liste der Bilder ist unsortiert.
        ' Ein optionales targetDir beschränkt die Suche auf ebendieses.

        Dim ergebnisListe As New List(Of String)
        Dim quelle As List(Of String)

        If targetDir = "" Then

            quelle = vorbereiteteDateien

        Else
            Dim dateiTypen As New List(Of String) From {".bmp", ".jpg", ".jpeg", ".png"}
            quelle.Clear()
            quelle.Add(targetDir)
            Dim bilderListe = CreateFileList(quelle, dateiTypen)
            quelle.Clear()

            For Each bild In bilderListe
                If Path.GetExtension(bild).ToLower() Like "*.jp*g" Then
                    If Not CheckIfLegalFile(bild) Then Continue For
                End If
                quelle.Add(bild)
            Next

        End If

        Dim i As Integer = 0
        While i < n AndAlso quelle.Count > 0
            Dim bild = quelle(rnd.Next(0, quelle.Count))
            ergebnisListe.Add(bild)
            i += 1
        End While

        Return ergebnisListe

    End Function

    Public Shared Sub PreparePicturesByDirectory()
        vorbereiteteVerzeichnisse.Clear()
        hasFirstResultsVerzeichnisse = False

        Dim dateiTypen As New List(Of String) From {".bmp", ".jpg", ".jpeg", ".png"}
        For Each verzeichnis In aktuelleSettings.Verzeichnisse
            If cancelThreads Then Exit For
            If Not System.IO.Directory.Exists(verzeichnis) Then Continue For

            Dim unterverzeichnisse = System.IO.Directory.GetDirectories(verzeichnis, "*", SearchOption.AllDirectories)

            For Each unterverzeichnis In unterverzeichnisse
                If cancelThreads Then Exit For

                Dim bilder = System.IO.Directory.GetFiles(unterverzeichnis, "*.*", SearchOption.TopDirectoryOnly).
                Where(Function(f) dateiTypen.Any(Function(ext) f.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToList()

                Dim gefiltert = bilder.Where(Function(bild)
                                                 Dim ext = Path.GetExtension(bild).ToLower()
                                                 If ext = ".jpg" OrElse ext = ".jpeg" Then
                                                     Return CheckIfLegalFile(bild)
                                                 End If
                                                 Return True
                                             End Function).ToList()

                If gefiltert.Count > 0 Then
                    vorbereiteteVerzeichnisse(unterverzeichnis) = gefiltert

                    If Not hasFirstResultsVerzeichnisse Then
                        hasFirstResultsVerzeichnisse = True
                        RaiseEvent ErsteVerzeichnisseGefunden()
                    End If
                End If

            Next
        Next

        vorbereitungsThreadVerzeichnisse = Nothing
    End Sub

    Public Shared Function GetPicturesByDirectory(Optional targetDir As String = "") As List(Of String)
        ' Die Pfade aller Bilder eines zufällig bestimmten Verzeichnisses werden gemäß den Einstellungen erstellt
        ' Die Liste der Bilder ist alphabetisch sortiert.
        ' Ein optionales targetDir gibt die Pfade aller gemäß Einstellungen legitimen Bilder ebendieses Verzeichnisses aus.

        If targetDir = "" Then
            Dim zufallsKey = vorbereiteteVerzeichnisse.Keys(rnd.Next(0, vorbereiteteVerzeichnisse.Count))
            Return vorbereiteteVerzeichnisse(zufallsKey)
        ElseIf vorbereiteteVerzeichnisse.ContainsKey(targetDir) Then
            Return vorbereiteteVerzeichnisse(targetDir)
        Else
            Return New List(Of String)()
        End If

    End Function

    Public Shared Function GetPictureByName(bild As String, Optional correctOrientation As Boolean = True) As Image
        'Gibt ein Bild gemäß des angegebenen Parameters "pfad" zurück. Eine Legitimations-Prüfung findet NICHT statt.
        'Falls nicht durch den Bool "correctOrientation" unterdrückt, wird das Bild gemäß seiner EXIF-Daten gedreht.
        Dim exifFormate As New List(Of String) From {".jpg", ".jpeg"}
        Dim extension As String

        extension = Path.GetExtension(bild).ToLower()
        If correctOrientation AndAlso exifFormate.Contains(extension) Then
            Return (CorrectPictureOrientation(New Bitmap(bild), bild))
        Else
            Return (New Bitmap(bild))
        End If

    End Function

    Public Shared Function CorrectPictureOrientation(picture As Image, pfad As String) As Image
        Dim orientation As Integer = 1 ' Default = "Normal"

        Try
            Dim directories = ImageMetadataReader.ReadMetadata(pfad)
            Dim exifDir = directories.OfType(Of ExifIfd0Directory)().FirstOrDefault()

            If exifDir IsNot Nothing AndAlso exifDir.ContainsTag(ExifDirectoryBase.TagOrientation) Then
                orientation = exifDir.GetInt32(ExifDirectoryBase.TagOrientation)
            End If
        Catch ex As Exception
            LogHandling.LogError("SlideShowBildauswahl.CorrectPictureOrientation(" & pfad & ") - Fehler beim Auslesen der Ausrichtung: " & ex.Message)
        End Try

        ' Orientierung anwenden
        Select Case orientation
            Case 1 : Return picture ' Kein Drehbedarf
            Case 2 : picture.RotateFlip(RotateFlipType.RotateNoneFlipX)
            Case 3 : picture.RotateFlip(RotateFlipType.Rotate180FlipNone)
            Case 4 : picture.RotateFlip(RotateFlipType.Rotate180FlipX)
            Case 5 : picture.RotateFlip(RotateFlipType.Rotate90FlipX)
            Case 6 : picture.RotateFlip(RotateFlipType.Rotate90FlipNone)
            Case 7 : picture.RotateFlip(RotateFlipType.Rotate270FlipX)
            Case 8 : picture.RotateFlip(RotateFlipType.Rotate270FlipNone)
        End Select

        Return picture
    End Function


    ''' <summary>
    ''' Dreht ein Bild um den angegebenen Winkel angle.
    ''' </summary>
    ''' <param name="img">Das zu drehende Originalbild.</param>
    ''' <param name="angle">Der Rotationswinkel in Grad (im Uhrzeigersinn, negative Werte gegen den Uhrzeigersinn).</param>
    ''' <returns>Ein neues Image-Objekt, das um angle Grad rotiert wurde. Transparente Ränder werden gesetzt.</returns>
    ''' 
    Public Shared Function RotateImage(img As Image, angle As Single) As Image
        Dim originalWidth As Integer
        Dim originalHeight As Integer
        Dim newWidth As Integer
        Dim newHeight As Integer
        Dim cos As Double
        Dim sin As Double
        Dim angleRad As Double
        Dim cx As Single
        Dim cy As Single
        Dim rotatedBmp As Bitmap

        ' Normalisiere Winkel auf [0, 360)
        angle = angle Mod 360
        If angle < 0 Then angle += 360

        ' Umwandlung in Radiant
        angleRad = angle * Math.PI / 180.0

        ' Ursprüngliche Maße
        originalWidth = img.Width
        originalHeight = img.Height

        ' Berechne die Größe des neuen Bildes nach Rotation
        cos = Math.Abs(Math.Cos(angleRad))
        sin = Math.Abs(Math.Sin(angleRad))

        newWidth = CInt(Math.Round(originalWidth * cos + originalHeight * sin))
        newHeight = CInt(Math.Round(originalWidth * sin + originalHeight * cos))

        ' Neues Bitmap mit transparentem Hintergrund
        rotatedBmp = New Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb)
        rotatedBmp.SetResolution(img.HorizontalResolution, img.VerticalResolution)

        ' Rotationszentrum berechnen
        cx = newWidth / 2
        cy = newHeight / 2

        Using g As Graphics = Graphics.FromImage(rotatedBmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.Clear(Color.Transparent)

            ' Transformation durchführen
            g.TranslateTransform(cx, cy)
            g.RotateTransform(angle)
            g.TranslateTransform(-originalWidth / 2.0F, -originalHeight / 2.0F)

            ' Originalbild zeichnen
            g.DrawImage(img, New PointF(0, 0))
        End Using

        Return rotatedBmp
    End Function

    Public Shared Function GetZufaelligesUnterverzeichnis() As String
        ' Zufällig gewähltes Unterverzeichnis aus der Liste der Verzeichnisse auswählen

        Dim alleVerzeichnisse As New List(Of String)

        ' Alle Unterverzeichnisse rekursiv sammeln
        For Each hauptVerzeichnis In aktuelleSettings.Verzeichnisse
            If System.IO.Directory.Exists(hauptVerzeichnis) Then
                Try
                    alleVerzeichnisse.AddRange(System.IO.Directory.GetDirectories(hauptVerzeichnis, "*", SearchOption.AllDirectories))
                Catch ex As Exception
                    ' Bei Zugriff verweigert o. Ä. einfach ignorieren
                End Try
            End If
        Next

        ' Wenn keine gefunden wurden, Rückgabe leer
        If alleVerzeichnisse.Count = 0 Then Return String.Empty

        ' Zufällig eines auswählen
        Return alleVerzeichnisse(rnd.Next(alleVerzeichnisse.Count))

    End Function

    Public Shared Function SettingsSindIdentisch(a As SettingsBildauswahl, b As SettingsBildauswahl) As Boolean
        Return a.Bewertung = b.Bewertung AndAlso
           a.Altersfreigabe = b.Altersfreigabe AndAlso
           a.BlackListTags.SequenceEqual(b.BlackListTags) AndAlso
           a.WhiteListTags.SequenceEqual(b.WhiteListTags) AndAlso
           a.Verzeichnisse.SequenceEqual(b.Verzeichnisse)
    End Function

    Private Shared Sub StarteVorbereitungenPictures()

        PreparePictures()

    End Sub

    Private Shared Sub StarteVorbereitungenVerzeichnisse()

        PreparePicturesByDirectory()

    End Sub

    Public Sub StoppeAlleThreads()
        cancelThreads = True
        If vorbereitungsThreadPictures IsNot Nothing AndAlso vorbereitungsThreadPictures.IsAlive Then
            vorbereitungsThreadPictures.Join(500)
        End If

        If vorbereitungsThreadVerzeichnisse IsNot Nothing AndAlso vorbereitungsThreadVerzeichnisse.IsAlive Then
            vorbereitungsThreadVerzeichnisse.Join(500)
        End If

        vorbereitungsThreadPictures = Nothing
        vorbereitungsThreadVerzeichnisse = Nothing

    End Sub
End Class
