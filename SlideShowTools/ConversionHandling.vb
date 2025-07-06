Imports System.Reflection
Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowLogging

Public Class ConversionHandling

    ''' <summary>
    ''' Wandelt eine beliebige Struktur in ein Dictionary(Of String, String) um.
    ''' </summary>
    Public Shared Function StrukturZuDictionary(Of T)(struktur As T) As Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)
        Dim felder As FieldInfo() = GetType(T).GetFields(BindingFlags.Instance Or BindingFlags.Public)

        For Each feld In felder
            Dim wert = feld.GetValue(struktur)
            If wert IsNot Nothing Then
                If feld.FieldType = GetType(List(Of String)) Then
                    Dim liste As List(Of String) = DirectCast(wert, List(Of String))
                    dict(feld.Name) = String.Join(";", liste)
                Else
                    dict(feld.Name) = wert.ToString()
                End If
            End If
        Next

        Return dict
    End Function


    ''' <summary>
    ''' Wandelt ein Dictionary(Of String, String) zurück in eine Struktur des Typs T.
    ''' </summary>
    Public Shared Function DictionaryZuStruktur(Of T As Structure)(dict As Dictionary(Of String, String)) As T
        Dim boxed As Object = New T()
        Dim felder As FieldInfo() = GetType(T).GetFields(BindingFlags.Instance Or BindingFlags.Public)

        For Each feld In felder
            If dict.ContainsKey(feld.Name) Then
                Try
                    Dim typ = feld.FieldType
                    Dim quellwert = dict(feld.Name)
                    LogHandling.LogDebug("Konvertiere: Feld=" & feld.Name & ", Typ=" & typ.Name & ", Wert='" & quellwert & "'")

                    If typ = GetType(List(Of String)) Then
                        Dim list As New List(Of String)
                        If Not String.IsNullOrEmpty(quellwert) Then
                            list = quellwert.Split(";"c).ToList()
                        End If
                        feld.SetValue(boxed, list)

                    Else
                        Dim konvertiert = Convert.ChangeType(quellwert, typ)
                        feld.SetValue(boxed, konvertiert)
                    End If

                Catch ex As Exception
                    LogHandling.LogError("DictionaryZuStruktur - Konvertierungsfehler bei Feld '" & feld.Name & "': " & ex.Message)
                End Try
            End If
        Next

        Return CType(boxed, T)
    End Function





    ''' <summary>
    ''' Liest die Werte von Standard-UI-Controls aus und schreibt sie in ein Dictionary.
    ''' </summary>
    Public Shared Function UserControlZuDictionary(uc As UserControl) As Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)

        For Each ctrl As Control In uc.Controls
            If TypeOf ctrl Is TextBox Then
                dict(ctrl.Name) = DirectCast(ctrl, TextBox).Text

            ElseIf TypeOf ctrl Is ComboBox Then
                Dim cb = DirectCast(ctrl, ComboBox)
                dict(ctrl.Name) = If(cb.SelectedItem IsNot Nothing, cb.SelectedItem.ToString(), "")

            ElseIf TypeOf ctrl Is CheckBox Then
                dict(ctrl.Name) = DirectCast(ctrl, CheckBox).Checked.ToString()

            ElseIf TypeOf ctrl Is RadioButton Then
                dict(ctrl.Name) = DirectCast(ctrl, RadioButton).Checked.ToString()

            ElseIf TypeOf ctrl Is TrackBar Then
                dict(ctrl.Name) = DirectCast(ctrl, TrackBar).Value.ToString()

            ElseIf TypeOf ctrl Is ListBox Then
                Dim lb = DirectCast(ctrl, ListBox)
                Dim itemsList As New List(Of String)
                For Each item In lb.Items
                    itemsList.Add(item.ToString())
                Next
                dict(ctrl.Name) = String.Join(";", itemsList)

            ElseIf TypeOf ctrl Is CheckedListBox Then
                Dim clb = DirectCast(ctrl, CheckedListBox)
                Dim checkedList As New List(Of String)
                For i As Integer = 0 To clb.CheckedItems.Count - 1
                    checkedList.Add(clb.CheckedItems(i).ToString())
                Next
                dict(ctrl.Name) = String.Join(";", checkedList)

            ElseIf TypeOf ctrl Is PictureBox Then
                Dim pb = DirectCast(ctrl, PictureBox)
                dict(ctrl.Name) = pb.BackColor.ToArgb().ToString()

            End If
        Next

        Return dict
    End Function


    ''' <summary>
    ''' Überträgt ein Dictionary zurück auf die Controls in einem UserControl.
    ''' </summary>
    Public Shared Sub DictionaryZuUserControl(container As Control, dict As Dictionary(Of String, String))
        For Each ctrl As Control In container.Controls
            Dim key = ctrl.Name
            If TypeOf ctrl Is Panel OrElse TypeOf ctrl Is GroupBox Then
                ' Rekursiver Aufruf
                DictionaryZuUserControl(ctrl, dict)
            ElseIf dict.ContainsKey(key) Then
                Select Case True
                    Case TypeOf ctrl Is TextBox
                        DirectCast(ctrl, TextBox).Text = dict(key)

                    Case TypeOf ctrl Is ComboBox
                        DirectCast(ctrl, ComboBox).SelectedItem = dict(key)

                    Case TypeOf ctrl Is CheckBox
                        DirectCast(ctrl, CheckBox).Checked = Boolean.Parse(dict(key))

                    Case TypeOf ctrl Is RadioButton
                        DirectCast(ctrl, RadioButton).Checked = Boolean.Parse(dict(key))

                    Case TypeOf ctrl Is TrackBar
                        DirectCast(ctrl, TrackBar).Value = Integer.Parse(dict(key))

                    Case TypeOf ctrl Is ListBox
                        Dim lb = DirectCast(ctrl, ListBox)
                        lb.Items.Clear()
                        For Each item In dict(key).Split(";"c)
                            lb.Items.Add(item)
                        Next

                    Case TypeOf ctrl Is CheckedListBox
                        Dim clb = DirectCast(ctrl, CheckedListBox)
                        For i = 0 To clb.Items.Count - 1
                            clb.SetItemChecked(i, dict(key).Split(";"c).Contains(clb.Items(i).ToString()))
                        Next
                End Select
            End If
        Next
    End Sub

End Class
