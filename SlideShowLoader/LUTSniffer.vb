Imports System.Globalization
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text
Imports SlideShowInterfaces.InfoHandling

Public NotInheritable Class LUTSniffer
    Private Sub New()
    End Sub

    Public Shared Function SniffFile(fp As String) As LutInfo
        Dim info As New LutInfo With {
            .FullPath = fp,
            .DisplayName = Path.GetFileName(fp),
            .Type = LutType.Unknown,
            .SizeN = 0,
            .ErrorMessage = ""
        }

        If Not File.Exists(fp) Then
            info.ErrorMessage = "Datei nicht gefunden."
            Return info
        End If

        ' --- 1) Bildversuch zuerst: PNG-Hald (auch wenn Endung .lut ist)
        Try
            Using img As Image = Image.FromFile(fp)
                If img.RawFormat.Guid = ImageFormat.Png.Guid AndAlso img.Width = img.Height Then
                    Dim side As Integer = img.Width
                    Dim L As Integer = DetectHaldLevel(side)
                    If L > 1 Then
                        info.Type = LutType.HaldPng
                        info.SizeN = L
                        Return info
                    End If
                End If
            End Using
        Catch
            ' kein Bild / kein PNG → weiter mit Text
        End Try

        ' --- 2) Textversuch: .cube (IRIDAS)
        Try
            Using sr As New StreamReader(fp, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True)
                Dim n As Integer = 0
                While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()
                    If line Is Nothing Then Exit While
                    line = line.Trim()
                    If line.Length = 0 OrElse line.StartsWith("#") Then Continue While

                    If line.StartsWith("LUT_3D_SIZE", StringComparison.OrdinalIgnoreCase) Then
                        Dim parts = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
                        If parts.Length >= 2 AndAlso Integer.TryParse(parts(1), NumberStyles.Integer, CultureInfo.InvariantCulture, n) Then
                            If n > 1 Then
                                info.Type = LutType.Cube3D
                                info.SizeN = n
                                Return info
                            End If
                        End If
                    End If
                End While
            End Using
        Catch ex As Exception
            info.ErrorMessage = ex.Message
        End Try

        If info.Type = LutType.Unknown Then
            info.ErrorMessage = "Unbekanntes/inkompatibles LUT-Format."
        End If
        Return info
    End Function

    ''' <summary>
    ''' Ermittelt das Hald-Level L aus der Seitenlänge einer quadratischen PNG.
    ''' Akzeptiert **hochskalierte** Halds: side = (L*L) * scale.
    ''' Bevorzugt gängige L-Werte; fällt sonst auf generisches Matching zurück.
    ''' </summary>
    Private Shared Function DetectHaldLevel(side As Integer) As Integer
        If side <= 0 Then Return 0

        ' 1) Gängige Hald-Level zuerst (liefert bei 1728 → L=8, weil 1728 % (8*8)=0)
        Dim typical As Integer() = {64, 33, 32, 17, 16, 9, 8, 6, 4, 3, 2}
        For Each L In typical
            Dim block = L * L
            If side >= block AndAlso side Mod block = 0 Then Return L
        Next

        ' 2) Generischer Fallback: größtes L (<=64), das side % (L*L) == 0 erfüllt
        Dim maxL As Integer = Math.Min(64, CInt(Math.Floor(Math.Sqrt(side))))
        For L As Integer = maxL To 2 Step -1
            Dim block = L * L
            If side Mod block = 0 Then Return L
        Next

        Return 0
    End Function

    Public Shared Function ScanFolder(root As String) As List(Of LutInfo)
        Dim res As New List(Of LutInfo)()
        If Not Directory.Exists(root) Then Return res
        For Each pat In New String() {"*.cube", "*.png", "*.lut"}
            For Each fp In Directory.EnumerateFiles(root, pat, SearchOption.TopDirectoryOnly)
                Dim li = SniffFile(fp)
                If li.Type <> LutType.Unknown Then res.Add(li)
            Next
        Next
        ' deduplizieren + sortieren
        Dim unique = res.GroupBy(Function(x) x.FullPath, StringComparer.OrdinalIgnoreCase).Select(Function(g) g.First()).ToList()
        unique.Sort(Function(a, b) StringComparer.CurrentCultureIgnoreCase.Compare(a.DisplayName, b.DisplayName))
        Return unique
    End Function
End Class
