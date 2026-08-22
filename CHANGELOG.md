# WWE 2K26 MyGM Companion V10.6

## Launcher- und Update-Hotfix – 11.0.1

- Launcher beim Start visuell neu gestaltet: Arena-inspirierter rotschwarzer Hintergrund, Lichtakzente, klarere Panels und interaktive Schaltflächen.
- Eigenständiges, originales `MYGM`-Emblem im Launcher; keine offiziellen WWE- oder 2K-Logos werden kopiert.
- Echtes Mehrgrößen-Windows-Icon für Companion, Setup, Updater, Uninstaller und Desktop-Verknüpfung.
- Windows-Deinstallationseintrag verwendet ebenfalls das neue Programmsymbol.
- Der Menüpunkt `UPDATES` zeigt jetzt sichtbar `PRÜFE`, `AKTUELL`, `OFFLINE` oder einen roten Hinweis `UPDATE X VERFÜGBAR`.
- Beim Start wird einmalig nur die kleine `latest release`-Metadatenantwort von GitHub abgefragt; es erfolgt kein Download und keine dauerhafte Hintergrundprüfung.
- Ein verfügbares Update wird durch roten Button, weißen Rand und Versionsnummer deutlich hervorgehoben.

## Automatischer Showplaner – 11.0.0

- Neuer Button `AUTO-PLAN ERSTELLEN` erzeugt aus bestätigtem Roster und offenen Zielen eine vollständige Card.
- Bewertet Geschlecht, Face/Heel, Klassenkompatibilität, Popularität, Ausdauer, Manager und Rivalitätsstufe.
- Bevorzugt Fighter/Bruiser, Cruiser/Giant und flexibel einsetzbare Spezialisten.
- Schließt Superstars mit kritischer Ausdauer sowie ungültige Geschlechterpaarungen aus.
- Plant bis zu vier konfliktfreie Matches, ohne einen Superstar doppelt einzusetzen.
- Ordnet stärkstes Match als Main Event und zweistärkstes Match als Opener ein; schwächere Matches liegen in der Midcard.
- Wählt abhängig von Ausdauer, Rivalität und Budget zwischen Normal, Tables, Extreme Rules, TLC und Hell in a Cell.
- Offene Matchtyp-Ziele werden nach Möglichkeit genau einmal und budgetbewusst eingeplant.
- Wählt bis zu drei ungebuchte Superstars für Eigenwerbung, Call-Out oder geforderten Rollenwechsel.
- Zeigt erwartete Sterne-Spanne, geschätzte Matchkosten, Manager, Begründungen und Zielkonflikte.
- Warnt sichtbar bei unerfüllbaren Siegen, Ruhekonflikten, Eingriffen, Rollenwechseln und unvollständigem Roster.
- Aktualisiertes Rosterformat: `Name | Geschlecht | Rolle | Klasse | POP | Ausdauer | Manager | Rivalität`.
- Automatisierter Smoke-Test prüft vier Matches, drei Promos, keine Doppelbuchung, Geschlechtertrennung und Matchtyp-Ziel.

## Ziele und Versprechen – 10.9.0

- Neuer Bereich `ZIELE & VERSPRECHEN` für Commissioner-Ziele und Superstar-Versprechen.
- Zielseite wird ausschließlich auf Knopfdruck vom validierten WWE2K26-Fenster erfasst.
- Separater OCR-Prozess mit denselben harten Timeouts und Abbruchregeln wie die Roster-Erfassung.
- Regelbasierte Erkennung für Eingriffe, geforderte Siege, Matchtypen, Rollenwechsel, Ruhe, Promos und Buchungsforderungen.
- Deutsche und englische Zieltexte sowie explizite, aktuelle und relative Wochen-Deadlines werden berücksichtigt.
- Zweigeteilte Prüfoberfläche mit unverändertem OCR-Originaltext und strukturierten, editierbaren Zielzeilen.
- Prioritäten `Hoch` und `Mittel` bereiten die spätere Abwägung zwischen Zielerfüllung und Showqualität vor.
- Offene Ziele mit hoher Priorität werden automatisch in das Commissioner-Zielfeld der Show-Planung übernommen.
- Bestätigte Ziele bleiben updatefest in `data/objectives.json` gespeichert.

## Roster-Grundlage – 10.8.0

- Neuer Bereich `ROSTER & BRAND` für Brand, Woche und bestätigte Superstar-Daten.
- Gezielter Screenshot ausschließlich vom validierten WWE2K26-Spielprozess.
- OCR nur nach Klick auf `WWE-SEITE ERFASSEN`; keine Schleife und keine Hintergrundüberwachung.
- Windows-OCR läuft in `MyGM.OcrWorker.exe` als separater Prozess ohne Konsolenfenster.
- Harter 15-Sekunden-Timeout; bei Timeout oder Abbruch wird der OCR-Prozess beendet.
- Erkannter Text wird niemals ungeprüft übernommen, sondern zunächst in einem editierbaren Vorschaubereich angezeigt.
- Bestätigte Rosterzeilen werden lokal in `data/roster.json` gespeichert und bleiben bei Updates erhalten.

## GitHub-Updater – 10.7.0

- Neuer `UPDATES`-Bereich direkt im Launcher.
- Versionsprüfung über die öffentlichen GitHub Releases von `DeanRotations/WWE-2K26-MyGM-Companion`.
- Updatepaket und separate SHA-256-Datei werden vor der Installation zwingend geprüft.
- Updater läuft aus einem temporären Pfad, damit er sich selbst sicher aktualisieren kann.
- Companion wird kontrolliert geschlossen, Programmdateien werden ersetzt und die neue Version anschließend gestartet.
- `data`, `config`, `cache` und `logs` werden bei Updates nicht überschrieben.
- Netzwerkzugriff findet ausschließlich nach Klick auf `UPDATES` statt; keine Hintergrundprüfung.

## Feature-Stufe 1 – 10.6.2

- Selbsterkennung behoben: Companion-, Setup- und eigener Prozess werden grundsätzlich ignoriert.
- Ein Fenster gilt nur noch als Spiel, wenn Fenstertitel und Prozessname eindeutig zu WWE 2K26 passen.
- Diagnose zeigt jetzt Prozessname und PID zur nachvollziehbaren Prüfung.
- Erfolgreicher Capture speichert einen lokalen Diagnose-Nachweis mit Zeit, Fenstermodus und Bildhash.
- Dashboard freigeschaltet: Diagnosezustand, aktuelle Show, Budget und Commissioner-Ziel.
- Manuelle Offline-Show-Planung freigeschaltet: Show, Budget, Commissioner-Ziel, Roster-Notizen und Match-Card.
- Pläne werden lokal im Ordner `data` gespeichert; keine OCR und keine Hintergrundüberwachung.

## Hotfix 10.6.1

- Freeze beim Öffnen der Installationspfad-Auswahl behoben: Der Windows-Ordnerdialog läuft nun garantiert im erforderlichen STA-Modell.
- Ordnerdialog erhält immer einen existierenden Startpfad und ist korrekt dem Setup-Fenster zugeordnet.
- Threadingfehler bei der Desktop-Verknüpfungsoption während der Installation behoben.

## Stabiler Neuaufbau

- Vollständiger Neuaufbau ohne Code oder Hintergrundlogik aus V10.5.
- Diagnosemodus ist beim ersten Start aktiv; OCR und automatische Erkennung sind deaktiviert.
- WWE-Fenstersuche erfolgt ausschließlich auf Benutzeranforderung.
- Screenshot-Capture erfasst gezielt nur die Grenzen des gefundenen WWE-Fensters.
- Capture läuft außerhalb des UI-Threads, besitzt einen harten 8-Sekunden-Timeout und kann abgebrochen werden.
- Jeder Capture erhält einen SHA-256-Kurzhash als Basis für spätere ereignis-/hash-gesteuerte Erkennung.
- Keine Dauer-OCR, kein globaler Input-Hook, keine Injection und kein RAM-Zugriff.
- OCR-Worker als separater, unsichtbarer Prozess vorbereitet; in der Diagnosephase bewusst deaktiviert.
- Overlay als eigenes, nicht aktivierbares Click-through-Fenster umgesetzt und standardmäßig ausgeschaltet.
- INSERT schaltet das Overlay über Windows `RegisterHotKey` um; es wird kein globaler Input-Hook installiert.
- Responsive WPF-Oberfläche mit getrennten Worker-Aufgaben, Cancellation und Fehlerprotokollierung.
- Lokale Ordnerstruktur: `assets`, `config`, `data`, `cache`, `logs`.
- Echter Setup-Assistent mit frei wählbarem Installationspfad und optionaler Desktop-Verknüpfung.
- Windows-Deinstallationseintrag und `uninstall.exe`; Benutzerdaten im Ordner `data` bleiben erhalten.
- `Updater.exe` vorbereitet, aber ohne konfigurierte signierte Quelle ohne Netzwerkaktivität.

## Bewusst noch deaktiviert

- OCR-Auswertung und automatische Show-Erkennung werden erst nach bestandener Diagnosephase aktiviert.
- Roster-, Rivalitäts-, Versprechen- und Show-Planungsmodule sind als UI-Ziele vorgesehen, aber nicht mit instabiler Live-Erkennung verbunden.

