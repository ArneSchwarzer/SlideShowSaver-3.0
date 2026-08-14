Imports System.Windows.Forms
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowShader.LUT.ShaderMain
Imports SlideShowTools
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ToolTipHandling

Public Class ucOptionsShader

    'Variablendeklaration
    Private aktuelleSettings As ShaderSettings_LUT

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load

        CheckYourMail()

        Try

            IniOrReinitialize()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of ShaderSettings_LUT)(nameShader)

    End Sub

    Private Sub IniOrReinitialize()
        'Initialisiert oder Re-Initialisiert die Steuerelemente
        Dim LUTList As List(Of LutInfo)
        Dim markierteLUTs As String
        Dim LUT As LutInfo

        LUTList = SlideShowLoader.LUTListLoader.LUTListLoader()

        'Trackbar "Raster"
        trkIntensitaet.Value = aktuelleSettings.intensitaet
        lblIntensitaet.Text = aktuelleSettings.intensitaet.ToString & " %"

        'CheckedListBox "LUTs"
        clbLUTs.DisplayMember = "LUTName"
        clbLUTs.Items.Clear()
        For Each LUT In LUTList
            clbLUTs.Items.Add(LUT)
        Next

        EnableToolTipsForCLB(clbLUTs)

        markierteLUTs = JoinSemicolonList(aktuelleSettings.LUTs)
        SetCheckedItemsByName(Of LutInfo)(
            clbLUTs,
            markierteLUTs,
            Function(m) m.LUTName
            )

        clbLUTs.Sorted = True

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.intensitaet = CInt(defaults("Intensität"))
        aktuelleSettings.LUTs = SplitSemicolonList(defaults("LUTs"))

        WriteToRegistry(SLIDESHOWSHADER_LUT_FULLPATH & "Intensität", defaults("Intensität"))
        WriteToRegistry(SLIDESHOWSHADER_LUT_FULLPATH & "LUTs", defaults("LUTs"))

        wirdInitialisiert = True

        Try

            IniOrReinitialize()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub trkIntensitaet_ValueChanged(sender As Object, e As EventArgs) Handles trkIntensitaet.ValueChanged

        lblIntensitaet.Text = trkIntensitaet.Value.ToString() & " %"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.intensitaet = trkIntensitaet.Value

        WriteToRegistry(SLIDESHOWSHADER_LUT_FULLPATH & "Intensität", aktuelleSettings.intensitaet)

    End Sub

    Private Sub clbLUTs_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbLUTs.ItemCheck
        'Übernimmt Änderungen der aktivierten LUTs per Direct Commit.

        Dim clb As CheckedListBox

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        clb = DirectCast(sender, CheckedListBox)

        BeginInvoke(
        New MethodInvoker(
            Sub()

                Dim lutString As String

                If wirdInitialisiert OrElse wurdeBereinigt Then
                    Exit Sub
                End If

                lutString =
                    GetCheckedItemsAsString(
                        Of LutInfo)(
                            clb,
                            Function(lut)
                                Return lut.LUTName
                            End Function)

                aktuelleSettings.LUTs = SplitSemicolonList(lutString)

                WriteToRegistry(SLIDESHOWSHADER_LUT_FULLPATH & "LUTs", lutString)

            End Sub))

    End Sub

    Private Sub ucOptionsShader_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed

        wurdeBereinigt = True

    End Sub

End Class
