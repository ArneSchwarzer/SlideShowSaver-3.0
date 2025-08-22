Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports SlideShowInterfaces.InfoHandling


Public Enum CubeTableOrder
    RGB   ' R läuft am schnellsten (innere Schleife)
    BGR   ' B läuft am schnellsten (innere Schleife) – „Blue-fastest“
End Enum

' Ergebnisobjekt: 2D-Atlas + effektives N
Public Structure LutAtlas
        Public Atlas As BitmapSource      ' WriteableBitmap (Frozen), BGRA32, Größe: (N*N) x N
        Public N As Integer
    End Structure

    Friend Structure Vec3F
        Public R As Single
        Public G As Single
        Public B As Single
        Public Sub New(r As Single, g As Single, b As Single)
            Me.R = r : Me.G = g : Me.B = b
        End Sub
    End Structure

    Public Module LUTHandling

        ' *** Variablen am Anfang ***
        Private ReadOnly Inv As CultureInfo = CultureInfo.InvariantCulture

        ' simpler Cache: Key = fullPath|type|srcN|targetN
        Private ReadOnly _cache As New Dictionary(Of String, LutAtlas)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _cacheOrder As New Queue(Of String)()
        Private Const MAX_CACHE As Integer = 6
        Private ReadOnly _sync As New Object()

    ''' <summary>
    ''' Baut aus einer LUT-Datei (.cube oder Hald-PNG) einen 2D-Atlas (N*N x N, BGRA32).
    ''' Optionales Resample auf targetN (Standard 33). Ergebnisse werden gecached.
    ''' NOTE: WPF-Effects erwarten BGRA8. Wir schreiben B,G,R,A (Bytes 0..3).
    ''' Der Shader swizzelt ggf. (return lut.bgr), siehe z.B. LUTShader.fxh.
    ''' </summary>
    Public Function BuildAtlas(lut As LutInfo, Optional targetN As Integer = 33, Optional useCache As Boolean = True) As LutAtlas
            If lut.Type = LutType.Unknown OrElse String.IsNullOrWhiteSpace(lut.FullPath) Then
                Throw New InvalidOperationException("Ungültige LUT.")
            End If

            ' 1) Quelle in 3D-Array lesen
            Dim srcN As Integer
            Dim src As Vec3F(,,) = LoadLutToGrid(lut, srcN) ' wirft bei Fehlern

            ' 2) Ziel-N bestimmen / ggf. resamplen
            Dim N As Integer = If(targetN > 1, targetN, srcN)
            Dim cacheKey As String = $"{lut.FullPath}|{lut.Type}|src{srcN}|dst{N}"

            If useCache Then
                SyncLock _sync
                    If _cache.TryGetValue(cacheKey, Nothing) Then
                        Return _cache(cacheKey)
                    End If
                End SyncLock
            End If

            Dim data As Vec3F(,,) = If(N = srcN, src, ResampleLut(src, srcN, N))

            ' 3) 2D-Atlas erzeugen
            Dim atlas As BitmapSource = CreateAtlasBitmap(data, N)

            Dim res As New LutAtlas With {.Atlas = atlas, .N = N}

            If useCache Then
                SyncLock _sync
                    _cache(cacheKey) = res
                    _cacheOrder.Enqueue(cacheKey)
                    While _cacheOrder.Count > MAX_CACHE
                        Dim oldKey = _cacheOrder.Dequeue()
                        _cache.Remove(oldKey)
                    End While
                End SyncLock
            End If

            Return res
        End Function

        ' ---------- Dispatcher: Datei -> 3D-Gitter ----------
        Private Function LoadLutToGrid(lut As LutInfo, ByRef N As Integer) As Vec3F(,,)
            Select Case lut.Type
            Case LutType.Cube3D
                ' Standard: RGB wie Photoshop-Default
                Return LoadCube3D(lut.FullPath, N, CubeTableOrder.RGB)
            Case LutType.HaldPng : Return LoadHaldPng(lut.FullPath, N)
                Case Else : Throw New NotSupportedException("Nicht unterstützter LUT-Typ.")
            End Select
        End Function

    ' ---------- .cube laden (IRIDAS/Adobe) ----------
    ' Erwartete Datenreihenfolge in .cube: Blue läuft am schnellsten, dann Green, dann Red.
    Private Function LoadCube3D(path As String, ByRef N As Integer,
                            Optional order As CubeTableOrder = CubeTableOrder.RGB) As Vec3F(,,)
        Dim list As New List(Of Vec3F)(32768)
        N = 0

        Using sr As New StreamReader(path, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks:=True)
                While Not sr.EndOfStream
                    Dim line = sr.ReadLine()
                    If line Is Nothing Then Exit While
                    line = line.Trim()
                    If line.Length = 0 OrElse line.StartsWith("#") Then Continue While

                    If line.StartsWith("LUT_3D_SIZE", StringComparison.OrdinalIgnoreCase) Then
                        Dim p = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
                        If p.Length >= 2 Then Integer.TryParse(p(1), NumberStyles.Integer, Inv, N)
                        Continue While
                    End If
                    ' DOMAIN_MIN/MAX ignorieren wir hier; Normierung ggfs. im Shader
                    If Char.IsDigit(line(0)) OrElse line(0) = "-"c OrElse line(0) = "."c Then
                        Dim parts = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
                        If parts.Length >= 3 Then
                            Dim r = Single.Parse(parts(0), Inv)
                            Dim g = Single.Parse(parts(1), Inv)
                            Dim b = Single.Parse(parts(2), Inv)
                            list.Add(New Vec3F(r, g, b))
                        End If
                    End If
                End While
            End Using

        If N <= 1 Then Throw New InvalidOperationException("Ungültiges/fehlendes LUT_3D_SIZE.")
        If list.Count < N * N * N Then Throw New InvalidOperationException("Zu wenige Einträge in .cube.")

        Dim grid(N - 1, N - 1, N - 1) As Vec3F
        Dim idx As Integer = 0

        Select Case order
            Case CubeTableOrder.RGB
                ' R innere Schleife (schnellste Achse), dann G, dann B
                For b As Integer = 0 To N - 1
                    For g As Integer = 0 To N - 1
                        For r As Integer = 0 To N - 1
                            grid(r, g, b) = list(idx) : idx += 1
                        Next
                    Next
                Next

            Case CubeTableOrder.BGR
                ' B innere Schleife (Blue-fastest), dann G, dann R
                For r As Integer = 0 To N - 1
                    For g As Integer = 0 To N - 1
                        For b As Integer = 0 To N - 1
                            grid(r, g, b) = list(idx) : idx += 1
                        Next
                    Next
                Next
        End Select

        Return grid

    End Function

    ' ---------- Hald-PNG laden ----------
    ' side = (L*L) * scale; wir lesen zentrisch aus jedem Block.
    Private Function LoadHaldPng(path As String, ByRef L As Integer) As Vec3F(,,)
            Using srcBmp As New Bitmap(path)
                If srcBmp.Width <> srcBmp.Height Then Throw New InvalidOperationException("PNG ist nicht quadratisch.")
                Dim side As Integer = srcBmp.Width
                L = DetectHaldLevel(side)
                If L <= 1 Then Throw New InvalidOperationException("Kein Hald-CLUT erkannt.")

                Dim block As Integer = L * L
                Dim scale As Integer = Math.Max(1, side \ block)

                Dim bmp As Bitmap = srcBmp
            If srcBmp.PixelFormat <> Imaging.PixelFormat.Format32bppArgb Then
                bmp = New Bitmap(srcBmp.Width, srcBmp.Height, Imaging.PixelFormat.Format32bppArgb)
                Using g = Graphics.FromImage(bmp)
                    g.DrawImage(srcBmp, 0, 0, srcBmp.Width, srcBmp.Height)
                End Using
            End If

            Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                                      ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
            Try
                    Dim stride As Integer = data.Stride
                    Dim basePtr As IntPtr = data.Scan0
                    Dim lut(L - 1, L - 1, L - 1) As Vec3F

                    For r As Integer = 0 To L - 1
                        For g As Integer = 0 To L - 1
                            For b As Integer = 0 To L - 1
                                Dim tileX As Integer = b Mod L
                                Dim tileY As Integer = b \ L
                                Dim x As Integer = tileX * L * scale + r * scale + scale \ 2
                                Dim y As Integer = tileY * L * scale + g * scale + scale \ 2
                                If x >= bmp.Width Then x = bmp.Width - 1
                                If y >= bmp.Height Then y = bmp.Height - 1

                                Dim p As IntPtr = basePtr + y * stride + x * 4
                                Dim B0 As Byte = Marshal.ReadByte(p, 0)
                                Dim G0 As Byte = Marshal.ReadByte(p, 1)
                                Dim R0 As Byte = Marshal.ReadByte(p, 2)
                                lut(r, g, b) = New Vec3F(R0 / 255.0F, G0 / 255.0F, B0 / 255.0F)
                            Next
                        Next
                    Next
                    Return lut
                Finally
                    bmp.UnlockBits(data)
                    If Not Object.ReferenceEquals(bmp, srcBmp) Then bmp.Dispose()
                End Try
            End Using
        End Function

        ' ---------- 3D-Resampling (tri-linear) ----------
        Private Function ResampleLut(src As Vec3F(,,), srcN As Integer, dstN As Integer) As Vec3F(,,)
            Dim dst(dstN - 1, dstN - 1, dstN - 1) As Vec3F
            For r As Integer = 0 To dstN - 1
                Dim fr As Single = r * (srcN - 1.0F) / (dstN - 1.0F)
                Dim r0 As Integer = CInt(Math.Floor(fr))
                Dim r1 As Integer = Math.Min(r0 + 1, srcN - 1)
                Dim rf As Single = fr - r0
                For g As Integer = 0 To dstN - 1
                    Dim fg As Single = g * (srcN - 1.0F) / (dstN - 1.0F)
                    Dim g0 As Integer = CInt(Math.Floor(fg))
                    Dim g1 As Integer = Math.Min(g0 + 1, srcN - 1)
                    Dim gf As Single = fg - g0
                    For b As Integer = 0 To dstN - 1
                        Dim fb As Single = b * (srcN - 1.0F) / (dstN - 1.0F)
                        Dim b0 As Integer = CInt(Math.Floor(fb))
                        Dim b1 As Integer = Math.Min(b0 + 1, srcN - 1)
                        Dim bf As Single = fb - b0

                        Dim c000 = src(r0, g0, b0)
                        Dim c100 = src(r1, g0, b0)
                        Dim c010 = src(r0, g1, b0)
                        Dim c110 = src(r1, g1, b0)
                        Dim c001 = src(r0, g0, b1)
                        Dim c101 = src(r1, g0, b1)
                        Dim c011 = src(r0, g1, b1)
                        Dim c111 = src(r1, g1, b1)

                        Dim c00 = Lerp(c000, c100, rf)
                        Dim c01 = Lerp(c001, c101, rf)
                        Dim c10 = Lerp(c010, c110, rf)
                        Dim c11 = Lerp(c011, c111, rf)
                        Dim c0 = Lerp(c00, c10, gf)
                        Dim c1 = Lerp(c01, c11, gf)
                        dst(r, g, b) = Lerp(c0, c1, bf)
                    Next
                Next
            Next
            Return dst
        End Function

        Private Function Lerp(a As Vec3F, b As Vec3F, t As Single) As Vec3F
            Return New Vec3F(a.R + (b.R - a.R) * t,
                             a.G + (b.G - a.G) * t,
                             a.B + (b.B - a.B) * t)
        End Function

        ' ---------- 2D-Atlas (N*N x N, BGRA32) ----------
        ' Layout muss zum Shader passen: u = (r + b*N + 0.5) / (N*N), v = (g + 0.5)/N
        Private Function CreateAtlasBitmap(data As Vec3F(,,), N As Integer) As BitmapSource
            Dim width As Integer = N * N
            Dim height As Integer = N
            Dim wb As New WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, Nothing)
            Dim stride As Integer = width * 4
            Dim buf(stride * height - 1) As Byte

            For r As Integer = 0 To N - 1
                For g As Integer = 0 To N - 1
                    For b As Integer = 0 To N - 1
                        Dim v As Vec3F = data(r, g, b)
                        Dim x As Integer = r + b * N
                        Dim y As Integer = g
                        Dim ofs As Integer = y * stride + x * 4
                        ' BGRA
                        buf(ofs + 0) = CByte(Math.Max(0, Math.Min(255, CInt(Math.Round(v.B * 255.0F)))))
                        buf(ofs + 1) = CByte(Math.Max(0, Math.Min(255, CInt(Math.Round(v.G * 255.0F)))))
                        buf(ofs + 2) = CByte(Math.Max(0, Math.Min(255, CInt(Math.Round(v.R * 255.0F)))))
                        buf(ofs + 3) = 255
                    Next
                Next
            Next

            wb.WritePixels(New System.Windows.Int32Rect(0, 0, width, height), buf, stride, 0)
            wb.Freeze()
            Return wb
        End Function

        ' ---------- Hald-Level erkennen (tolerant für hochskalierte PNGs) ----------
        Private Function DetectHaldLevel(side As Integer) As Integer
            If side <= 0 Then Return 0
            Dim typical As Integer() = {64, 33, 32, 17, 16, 9, 8, 6, 4, 3, 2}
            For Each L In typical
                Dim block = L * L
                If side >= block AndAlso side Mod block = 0 Then Return L
            Next
            Dim maxL As Integer = Math.Min(64, CInt(Math.Floor(Math.Sqrt(side))))
            For L As Integer = maxL To 2 Step -1
                Dim block = L * L
                If side Mod block = 0 Then Return L
            Next
            Return 0
        End Function

    End Module

