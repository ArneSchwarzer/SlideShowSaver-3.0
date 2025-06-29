# SlideShowLoader

Dieses Projekt stellt Loader-Klassen zur Verfügung, die Module, Transitions und Shader für den Bildschirmschoner **SlideShowSaver 3.0** dynamisch auslesen und bei Bedarf instanziieren.

## 🔍 Projektübersicht

SlideShowLoader dient der dynamischen Verwaltung und Initialisierung von externen Komponenten. Dabei wird streng zwischen zwei Arten von Zugriff unterschieden:

- **Lade...Liste()** → Lädt nur Metadaten (z. B. Name, Beschreibung, Version) über eine temporäre Dummy-Instanz
- **Lade...NachName(name)** → Lädt eine vollständige Instanz anhand des jeweiligen Namens

## 📁 Verzeichnisstruktur

Erwartet werden die folgenden Ordner im gleichen Verzeichnis wie das Hauptprogramm (z. B. `SlideShowSaver.exe`):

```
/Module      → *.sssm  = SlideShowSaver Modul
/Transitions → *.ssst  = SlideShowSaver Transition
/Shader      → *.ssss  = SlideShowSaver Shader
```

Beispiel-Dateien:
- `Matrix.sssm` → Modul
- `WipeLeft.ssst` → Transition
- `Grayscale.ssss` → Shader

> ⚠️ Wichtig: Die Dateinamen selbst sind frei wählbar, **nur die Endung muss stimmen**!

## 📦 Loader-Klassen im Projekt

| Kategorie       | Klassen                                          | Beschreibung                           |
|-----------------|--------------------------------------------------|----------------------------------------|
| **Basis**       | `BaseLoader`                                     | Gemeinsame Hilfsmethoden               |
| **Module**      | `ModulListLoader`, `ModulByNameLoader`           | Laden von Metadaten oder Einzelmodulen |
| **Transitions** | `TransitionListLoader`, `TransitionByNameLoader` | Analog für Übergänge                   |
| **Shader**      | `ShaderListLoader`, `ShaderByNameLoader`         | Analog für Shader                      |

## 🧠 Ressourcen-Effizienz durch Dummy-Instanzen

Für die Metadatenermittlung werden DLLs nicht vollständig geladen:
- Es wird nur eine temporäre Instanz erzeugt
- Diese Instanz liefert per Interface z. B. Name, Beschreibung und Version
- Danach wird die Instanz **explizit wieder verworfen**

Das spart RAM und verhindert unnötige Initialisierungscode-Ausführung.

## 🧪 Beispielnutzung

### Modulnamen abrufen (für UI-Dropdowns):

```vb
Dim modulInfos As List(Of SlideShowModulInfo) = ModulListLoader.LadeModulInfoListe()
For Each info In modulInfos
    clbModule.Items.Add(info) ' info.ToString liefert den Namen
Next
```

### Konkretes Modul instanziieren:

```vb
Dim modulInstance As ISlideShowModul = ModulByNameLoader.LadeModulNachName("Matrix")
```

### Analog für Transitions und Shader:

```vb
Dim transition = TransitionByNameLoader.LadeTransitionNachName("WipeLeft")
Dim shader = ShaderByNameLoader.LadeShaderNachName("Grayscale")
```

## 🔐 Abhängigkeiten

Dieses Projekt referenziert:

- `SlideShowInterfaces.dll` → Enthält `ISlideShowModul`, `ISlideShowTransition`, `ISlideShowShader` und zugehörige Info-Strukturen

---

## 📌 Wichtig für Entwickler

- Vermeide harte Kopplungen: Das Hauptprogramm sollte nur über die Interfaces kommunizieren.
- Passe das Projekt ggf. an, wenn sich die Interfaces ändern.
- Berücksichtige bei Updates die Kompatibilität der Versionen (`ModulVersion`, `ShaderVersion`, etc.)

---

© SlideShowSaver 3.0 — Modular Screensaver System
