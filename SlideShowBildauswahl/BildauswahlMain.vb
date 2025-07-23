Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports System.Windows.Forms
Imports TagLib
Imports SlideShowLogging
Imports TagLib.Image
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
    Private Shared hasFirstResultsPictues As Boolean = False
    Private Shared hasFirstResultsVerzeichnisse As Boolean = False

    Public Structure SettingsBildauswahl
        Public Verzeichnisse As List(Of String)
        Public WhiteListTags As List(Of String)
        Public BlackListTags As List(Of String)
        Public Altersfreigabe As String
        Public Bewertung As Integer
    End Structure

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

            If Not Directory.Exists(verzeichnis) Then
                ' Verzeichnis existiert nicht, logge und überspringe
                LogHandling.LogError("Verzeichnis nicht gefunden: " & verzeichnis)
                Continue For
            End If

            Dim unterverzeichnisse As String() = {}
            Try
                unterverzeichnisse = Directory.GetDirectories(verzeichnis, "*", SearchOption.AllDirectories)
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
                    subdirs = Directory.GetDirectories(pfad)
                Catch ex As Exception
                    LogHandling.LogError("Fehler beim Abrufen von Unterverzeichnissen in '" & pfad & "': " & ex.Message)
                    Continue For
                End Try

                ' Nur wenn keine Unterverzeichnisse existieren => Blattverzeichnis
                If subdirs.Length = 0 Then
                    Try
                        Dim files = Directory.GetFiles(pfad, "*.*", SearchOption.TopDirectoryOnly).
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
        'Prüft, ob ein Bild den Kriterien gemäß den aktuellen Settings entspricht 

        Dim tagLibFile As TagLib.Jpeg.File
        Dim checkWhitelist As Boolean = False
        Dim checkBlacklist As Boolean = True 'Wird während des Test ggf. auf 'False' gesetzt
        Dim checkBewertung As Boolean = False

        Try
            tagLibFile = TagLib.File.Create(bild)

            'Check #1: Bewertung
            If tagLibFile.ImageTag.Rating >= aktuelleSettings.Bewertung Then checkBewertung = True

            'Check #2: White-List
            If aktuelleSettings.WhiteListTags.Count = 0 Then
                checkWhitelist = True
            Else
                For Each includeTag As String In aktuelleSettings.WhiteListTags
                    If tagLibFile.ImageTag.Keywords.Contains(includeTag) Then
                        checkWhitelist = True
                    End If
                Next
            End If

            'Check #3: Black-List (enthält Prüfung der Altersfreigabe-Stufe)
            For Each excludeTag As String In aktuelleSettings.BlackListTags
                If tagLibFile.ImageTag.Keywords.Contains(excludeTag) Then
                    checkBlacklist = False
                End If
            Next

            'Aufräumen
            tagLibFile.Dispose()

        Catch ex As Exception
            LogHandling.LogError("SlideShowBildauswahl.CheckIfLegalFile(" & bild & ") - Problem mit tagLibFile: " & ex.Message)
        End Try

        'Checks überstanden?
        If checkBewertung And checkWhitelist And checkBlacklist Then
            Return True
        Else
            Return False
        End If


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

        For Each bild In bilderListe
            If Path.GetExtension(bild).ToLower() Like "*.jp*g" Then
                If Not CheckIfLegalFile(bild) Then Continue For
            End If
            vorbereiteteDateien.Add(bild)
            If Not hasFirstResultsPictues AndAlso vorbereiteteDateien.Count >= 2 Then
                hasFirstResultsPictues = True
            End If
        Next
    End Sub

    Public Shared Function GetPictures(n As Integer, Optional targetDir As String = "") As List(Of String)
        ' Die Pfade von n Bildern werden gemäß den Einstellungen zufällig geladen. Die Liste der Bilder ist unsortiert.
        ' Ein optionales targetDir beschränkt die Suche auf ebendieses.

        Dim ergebnisListe As New List(Of String)
        Dim quelle As List(Of String)
        Dim maxWaitTime As Integer = 2000 ' max. 2 Sekunden warten
        Dim waited As Integer = 0

        While Not hasFirstResultsPictues AndAlso waited < maxWaitTime
            Thread.Sleep(50)
            waited += 50
        End While

        If vorbereiteteDateien.Count = 0 Then
            If vorbereitungsThreadPictures Is Nothing OrElse Not vorbereitungsThreadPictures.IsAlive Then
                vorbereitungsThreadPictures = New Thread(AddressOf StarteVorbereitungenPictures)
                vorbereitungsThreadPictures.IsBackground = True
                vorbereitungsThreadPictures.Start()
            End If
        End If


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
            If Not Directory.Exists(verzeichnis) Then Continue For
            Dim unterverzeichnisse = Directory.GetDirectories(verzeichnis, "*", SearchOption.AllDirectories)

            For Each unterverzeichnis In unterverzeichnisse
                Dim bilder = Directory.GetFiles(unterverzeichnis, "*.*", SearchOption.TopDirectoryOnly).
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
                    If Not hasFirstResultsVerzeichnisse AndAlso vorbereiteteVerzeichnisse.Count > 0 Then
                        hasFirstResultsVerzeichnisse = True
                    End If
                End If
            Next
        Next

    End Sub

    Public Shared Function GetPicturesByDirectory(Optional targetDir As String = "") As List(Of String)
        ' Die Pfade aller Bilder eines zufällig bestimmten Verzeichnisses werden gemäß den Einstellungen erstellt
        ' Die Liste der Bilder ist alphabetisch sortiert.
        ' Ein optionales targetDir gibt die Pfade aller gemäß Einstellungen legitimen Bilder ebendieses Verzeichnisses aus.

        If vorbereiteteDateien.Count = 0 Then
            If vorbereitungsThreadVerzeichnisse Is Nothing OrElse Not vorbereitungsThreadVerzeichnisse.IsAlive Then
                vorbereitungsThreadVerzeichnisse = New Thread(AddressOf StarteVorbereitungenVerzeichnisse)
                vorbereitungsThreadVerzeichnisse.IsBackground = True
                vorbereitungsThreadVerzeichnisse.Start()
            End If
        End If

        Dim maxWaitTime As Integer = 2000 ' max. 2 Sekunden warten
        Dim waited As Integer = 0

        While Not hasFirstResultsVerzeichnisse AndAlso waited < maxWaitTime
            Thread.Sleep(50)
            waited += 50
        End While

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
            Return (CorrectPictureOrientation(bild))
        Else
            Return (New Bitmap(bild))
        End If

    End Function

    Public Shared Function CorrectPictureOrientation(pfad As String) As Image
        ' Dreht und Spiegelt ein Bild gemäß seiner EXIF Daten

        Dim img As Image = Image.FromFile(pfad)
        Dim exifOrientation As Integer = 1 ' Default: Normal
        Dim tagLibFile As TagLib.Jpeg.File

        ' Ausrichtung aus EXIF auslesen
        Try
            tagLibFile = TagLib.File.Create(pfad)
            exifOrientation = tagLibFile.ImageTag.Orientation
        Catch ex As Exception
            LogHandling.LogWarn("Die EXIF von " & pfad & " hat keine Information 'Orientation'. Das Bild wird belassen wie ist.")
        End Try

        ' Bild drehen
        Select Case exifOrientation

            ' Case 1: keine Aktion
            Case 2 : img.RotateFlip(RotateFlipType.RotateNoneFlipX)
            Case 3 : img.RotateFlip(RotateFlipType.Rotate180FlipNone)
            Case 4 : img.RotateFlip(RotateFlipType.Rotate180FlipX)
            Case 5 : img.RotateFlip(RotateFlipType.Rotate90FlipX)
            Case 6 : img.RotateFlip(RotateFlipType.Rotate90FlipNone)
            Case 7 : img.RotateFlip(RotateFlipType.Rotate270FlipX)
            Case 8 : img.RotateFlip(RotateFlipType.Rotate270FlipNone)

        End Select

        Return img

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
            If Directory.Exists(hauptVerzeichnis) Then
                Try
                    alleVerzeichnisse.AddRange(Directory.GetDirectories(hauptVerzeichnis, "*", SearchOption.AllDirectories))
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
End Class
