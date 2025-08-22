Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowShader.LUT.ShaderMain
Imports SlideShowTools.ListHandling
Imports SlideShowInterfaces.InfoHandling
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowLoader

Public Class ucOptionsShader

    'Variablendeklaration
    Private aktuelleSettings As ShaderSettings_LUT

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Aktuelle Settings einlesen
        CheckYourMail()

        'Steuerelemente setzten
        IniOrReinitialize()

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

        'Combobox "LUTs"
        clbLUTs.DisplayMember = "DisplayName"
        clbLUTs.Items.Clear()
        For Each LUT In LUTList
            clbLUTs.Items.Add(LUT)
        Next

        markierteLUTs = JoinSemicolonList(aktuelleSettings.LUTs)
        SetCheckedItemsByName(Of LutInfo)(
            clbLUTs,
            markierteLUTs,
            Function(m) m.DisplayName
            )

        clbLUTs.Sorted = True

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Lädt und setzt die Default-Werte für die Dialogbox

        Dim defaults As Dictionary(Of String, String)

        'Defaultwerte einlesen
        defaults = ShaderMain.GetShaderDefaultSettings()

        'AktuelleSettings aktualisieren
        aktuelleSettings.intensitaet = CInt(defaults("Intensität"))
        aktuelleSettings.LUTs = SplitSemicolonList(defaults("LUTs"))

        'Steuerelemente re-initialisieren
        IniOrReinitialize()

    End Sub

    Private Sub trkIntensitaet_ValueChanged(sender As Object, e As EventArgs) Handles trkIntensitaet.ValueChanged
        'Behandelt die Trackbar "Intensität"

        lblIntensitaet.Text = trkIntensitaet.Value.ToString & " %"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_LUT_FULLPATH & "Intensität", trkIntensitaet.Value)

    End Sub

    Private Sub clbLUTs_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbLUTs.ItemCheck
        'DirectCommit für chlbModule sobald ein Eintrag gechecked/ungeschecked wird.

        ' BeginInvoke sorgt dafür, dass der Code erst ausgeführt wird,
        ' nachdem der Checked-Zustand aktualisiert wurde
        Dim clb As CheckedListBox = DirectCast(sender, CheckedListBox)

        BeginInvoke(New MethodInvoker(Sub()
                                          CheckedListBoxHandling.SaveListBoxToRegistry(clb, SLIDESHOWSHADER_LUT_FULLPATH & "LUTs")
                                      End Sub))

    End Sub

End Class
