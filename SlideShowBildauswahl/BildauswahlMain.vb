Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports System.Windows.Forms
Imports TagLib
Imports SlideShowLogging
Imports TagLib.Image


Public Class BildauswahlMain

    'Variablen, Konstanten und Enums
    Private Shared rnd As New Random()
    Private Shared aktuelleSettings As New BildauswahlSettings

    Public Structure BildauswahlSettings
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
        'Aktualisiert die Bildauswahlsettings

        aktuelleSettings = ReadBildauswahlSettingsFromRegistryOrDefauls()

    End Sub

    'Public Shared Sub WriteBildauswahlSettingsToRegistry(settings As Dictionary(Of String, String))
    '    'Registry-Werte schreiben.

    '    Dim translatedDictionary As New Dictionary(Of String, String)

    '    'Da das "settings" als ausgelesenes UC zurückkommt, müssen wir es erst übersetzen
    '    translatedDictionary.Add("Verzeichnisse", settings("lstVerzeichnisse"))
    '    translatedDictionary.Add("WhiteListTags", settings("lstWhiteList"))
    '    translatedDictionary.Add("BlackListTags", settings("lstBlackList"))
    '    translatedDictionary.Add("Bewertung", settings("sbcBewertung"))

    '    If settings("rdo18") = True Then
    '        translatedDictionary.Add("Altersfreigabe", "18+")
    '    End If
    '    If settings("rdoAkt") = True Then
    '        translatedDictionary.Add("Altersfreigabe", "Akt")
    '    End If
    '    If settings("rdoLingerie") = True Then
    '        translatedDictionary.Add("Altersfreigabe", "Lingerie")
    '    End If
    '    If settings("rdoJugendfrei") = True Then
    '        translatedDictionary.Add("Altersfreigabe", "Jugendfrei")
    '    End If

    '    'Und nun ab damit in die Registry. Und ja, eine Schleife die über alle Keys des Dictionaries geht
    '    'wäre hier sicherlich eleganter. Aber so finde ich es halt besser nachzuvollziehen.

    '    'Verzeichnisse
    '    If translatedDictionary.ContainsKey("Verzeichnisse") Then
    '        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse", translatedDictionary("Verzeichnisse"))
    '    End If

    '    'WhiteListTags
    '    If translatedDictionary.ContainsKey("WhiteListTags") Then
    '        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags", translatedDictionary("WhiteListTags"))
    '    End If

    '    'BlackListTags
    '    If translatedDictionary.ContainsKey("BlackListTags") Then
    '        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", translatedDictionary("BlackListTags"))
    '    End If

    '    'Altersfreigabe (für RadioButtons, eigentliche Prüfung der Altersfreigabe erfolgt über BlackListTags)
    '    If translatedDictionary.ContainsKey("Altersfreigabe") Then
    '        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", translatedDictionary("Altersfreigabe"))
    '    End If

    '    'Bewertung
    '    If translatedDictionary.ContainsKey("Bewertung") Then
    '        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Bewertung", translatedDictionary("Bewertung"))
    '    End If

    'End Sub

    Public Shared Function ReadBildauswahlSettingsFromRegistryOrDefauls() As BildauswahlSettings
        ' Alle Werte der Bildauswahl-Settings aus der Registry auslesen und als Struktur BildauswahlSettings zurückgeben

        Dim defaults As New Dictionary(Of String, String)
        Dim settings As New BildauswahlSettings
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

    Public Shared Function GetPictures(n As Integer, Optional targetDir As String = "") As List(Of String)
        ' Die Pfade von n Bildern werden gemäß den Einstellungen zufällig geladen. Die Liste der Bilder ist unsortiert.
        ' Ein optionales targetDir beschränkt die Suche auf ebendieses.

        Dim suchVerzeichnisse As New List(Of String)
        Dim dateiTypen As New List(Of String) From {".bmp", ".jpg", ".jpeg", ".png"}
        Dim exifFormate As New List(Of String) From {".jpg", ".jpeg"}
        Dim bilderListe As List(Of String)
        Dim bild As String
        Dim ergebnisListe As New List(Of String)
        Dim extension As String
        Dim i As Integer = 0

        ' Zielverzeichnis erstellen
        If targetDir Is "" Then
            suchVerzeichnisse = aktuelleSettings.Verzeichnisse
        Else
            suchVerzeichnisse.Add(targetDir)
        End If

        ' Bilderliste initialisieren
        bilderListe = CreateFileList(suchVerzeichnisse, dateiTypen)

        While i < n
            ' Zufälliges Bild auswählen
            bild = bilderListe(rnd.Next(0, bilderListe.Count))
            extension = Path.GetExtension(bild).ToLower()

            ' Falls EXIF-relevant → CheckIfLegalFile prüfen, sonst direkt akzeptieren
            If exifFormate.Contains(extension) Then
                If Not CheckIfLegalFile(bild) Then Continue While
            End If

            ergebnisListe.Add(bild)

            i += 1
        End While

        Return ergebnisListe

    End Function

    Public Shared Function GetPicturesByDirectory(Optional targetDir As String = "") As List(Of String)
        ' Die Pfade aller Bilder eines zufällig bestimmten Verzeichnisses werden gemäß den Einstellungen erstellt
        ' Die Liste der Bilder ist alphabetisch sortiert.
        ' Ein optionales targetDir gibt die Pfade aller gemäß Einstellungen legitimen Bilder ebendieses Verzeichnisses aus.

        Dim dateiTypen As New List(Of String) From {"*.bmp", "*.jpg", "*.jpeg", "*.png"}
        Dim exifFormate As New List(Of String) From {".jpg", ".jpeg"}
        Dim listOfFiles As New List(Of String)
        Dim gefundeneDateien As String()
        Dim ergebnisListe As New List(Of String)
        Dim extension As String

        ' Zielverzeichnis initialisieren
        If targetDir = "" Then
            targetDir = GetZufaelligesUnterverzeichnis()
        End If

        ' Zielverzeichnis komplett auslesen
        For Each endung In dateiTypen
            Try
                gefundeneDateien = Directory.GetFiles(targetDir, endung, SearchOption.AllDirectories)
                listOfFiles.AddRange(gefundeneDateien)
            Catch ex As Exception
                LogHandling.LogError("SlideShowBildauswahl meldet ein Problem bei der Erstellung der Dateiliste in der Funktion GetPicturesByDirectory(): " & ex.ToString)
            End Try
        Next

        listOfFiles.Sort()

        ' Legitime Bilder in die Ergebnisliste schreiben
        For Each bild In listOfFiles
            extension = Path.GetExtension(bild).ToLower()

            If Not exifFormate.Contains(extension) OrElse CheckIfLegalFile(bild) Then
                ergebnisListe.Add(bild)
            End If
        Next

        Return ergebnisListe

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
End Class
