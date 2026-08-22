# WWE 2K26 MyGM Companion

Stabiler Windows-Companion für WWE 2K26 MyGM mit gezielter Fensterdiagnose, lokalem Dashboard, manueller Show-Planung und opt-in GitHub-Updater.

## Stabilitätsprinzipien

- keine Injection und kein RAM-Zugriff
- kein globaler Input-Hook
- keine Dauer-OCR oder automatische Vollbildabfrage
- Capture nur gezielt vom validierten WWE2K26-Prozess
- UI, Capture und künftige OCR-Worker sind voneinander getrennt
- harte Timeouts und Abbruchmöglichkeiten
- Overlay ist standardmäßig aus und click-through

## Updates

Ab Version 10.7.0 prüft die App ausschließlich nach einem Klick auf **UPDATES** das neueste GitHub Release. Ein Release muss diese beiden Assets enthalten:

- `MyGMCompanion-update.zip`
- `MyGMCompanion-update.zip.sha256`

Vor der Installation wird der SHA-256-Wert geprüft. Die Ordner `data`, `config`, `cache` und `logs` werden nicht überschrieben.

## Build

Windows und .NET SDK 8 werden benötigt:

```powershell
./build.ps1
```

Tags im Format `v10.7.0` lösen den Release-Workflow aus.
