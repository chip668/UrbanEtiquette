Projektbeschreibung

Dieses Projekt namens "Anzeige" ist eine Anwendung, die für die Anzeige von Verkehrsordnungswidrigkeiten und Parkverstößen entwickelt wurde. Die Anwendung ist in C# geschrieben und nutzt das .NET 5.0-Framework. Ihre Hauptfunktion besteht darin, Verstöße und Ordnungswidrigkeiten im Straßenverkehr zu dokumentieren.
Verzeichnisstruktur

    /bin/Debug: Enthält die ausführbare Anwendung und alle zugehörigen Dateien für die Debug-Konfiguration.
    /obj/Debug: Enthält temporäre Build-Dateien für die Debug-Konfiguration.

Wichtige Dateien und Verzeichnisse

    /net5.0-windows: Hier sind die wichtigsten Dateien für die Anwendung abgelegt.
        Anzeige.exe: Ausführbare Datei der Anwendung.
        Anzeige.dll: Hauptbibliotheksdatei der Anwendung.
        Anzeige.pdb: Programmdebugdaten-Datei.
        Data.txt: Datei mit relevanten Daten für die Anwendung.
        Bibliotheken: Enthält externe Bibliotheken, darunter:
            BitMiracle.LibTiff.NET.dll: TIFF-Dateien lesen und schreiben.
            BouncyCastle.Crypto.dll: Kryptografische Funktionen.
            ExifLib.dll: Lesen von Exif-Daten aus Bildern.
            FFmpeg.AutoGen.dll: Verwenden von FFmpeg-Funktionen.
            IronOcr.dll: Optische Zeichenerkennung (OCR).
            Newtonsoft.Json.dll: Verarbeitung von JSON-Daten.
            SixLabors.Fonts.dll: Arbeit mit Schriftarten.
            SixLabors.ImageSharp.dll: Verarbeitung von Bildern.
            SixLabors.ImageSharp.Drawing.dll: Zeichnen von Bildern.

Bilder

    /net5.0-windows: Enthält Bilder von verschiedenen Automarken, vermutlich für die Dokumentation von Verstößen.
        Alfa Romeo.jpg, Audi.jpg, BMW.jpg, ...: Bilder von verschiedenen Automarken.

Verkehrsschilder

    /net5.0-windows: Bilddateien von Verkehrsschildern.
        Anzeige_einer_Verkehrsordnungswidrigkeit_-_Parkverstoss.pdf: PDF-Datei mit Bildern von Verkehrsschildern im Zusammenhang mit Parkverstößen.

Textdateien

    /net5.0-windows: Verschiedene Textdateien mit relevanten Informationen.
        Data.txt, Data2.txt: Dateien mit Daten für die Anwendung.
        ort.txt: Datei mit Ortinformationen.
        mail.txt: Template für E-Mail-Benachrichtigungen.

Lizenz

Dieses Projekt unterliegt den Lizenzbedingungen MIT License.

Funktionalität der Anzeige-Anwendung:

Die "Anzeige"-Anwendung dient der Erfassung, Verarbeitung und Anzeige von Verkehrsordnungswidrigkeiten und Parkverstößen. Die Funktionalität lässt sich in mehrere Hauptaspekte unterteilen:

    Erfassung von Verstößen:
        Die Anwendung ermöglicht die Erfassung von Verkehrsordnungswidrigkeiten durch die Eingabe relevanter Informationen, wie Ort, Zeitpunkt und Art des Verstoßes.
        Bilder von Verkehrsschildern und Automarken können mit den erfassten Verstößen verknüpft werden.

    Dokumentation mit Bildern:
        Die Anwendung unterstützt die Verwendung von Bildern, darunter Bilder von Verkehrsschildern und Bildern von Fahrzeugen verschiedener Automarken.
        Es können TIFF-Dateien, Bilder im JPEG-Format und PDF-Dateien mit Verkehrsschildern verarbeitet werden.

    Verarbeitung von Exif-Daten:
        Die Anwendung liest Exif-Daten aus Bildern, um zusätzliche Informationen über den Aufnahmezeitpunkt und -ort zu extrahieren.

    Optische Zeichenerkennung (OCR):
        Die Anwendung nutzt die OCR-Funktionalität, um Textinformationen aus Bildern zu extrahieren. Dies könnte insbesondere für die Kennzeichenerkennung von Fahrzeugen relevant sein.

    Verwaltung von Daten:
        Die Anwendung speichert relevante Daten in Textdateien, darunter Data.txt, Data2.txt, ort.txt und mail.txt.
        Es gibt auch eine Verwaltung von Ortinformationen.

    Bibliotheken und Abhängigkeiten:
        Die Anwendung verwendet verschiedene Bibliotheken, um Funktionen wie das Lesen/Schreiben von TIFF-Dateien, kryptografische Operationen, JSON-Verarbeitung, OCR und Bildverarbeitung zu unterstützen.

    E-Mail-Benachrichtigungen:
        Die mail.txt enthält ein Template für E-Mail-Benachrichtigungen, was darauf hindeutet, dass die Anwendung auch die Möglichkeit bietet, Benachrichtigungen per E-Mail zu versenden.

Die Anwendung zielt darauf ab, den Prozess der Verkehrsordnungswidrigkeitserfassung zu automatisieren und die relevanten Daten effizient zu dokumentieren. Eine genaue Funktionsweise kann durch die Analyse des Quellcodes und der Anwendungslogik ermittelt werden.


Anzeige-Anwendung

Die Anzeige-Anwendung bietet eine Plattform zur Verwaltung von Anzeigen und zugehörigen Informationen. Die Anwendung wurde in C# entwickelt und enthält mehrere Klassen mit spezifischen Funktionen.
Klassenübersicht:
1. Assistent

    Dateien:
        assistent.cs
        assistent.Designer.cs

    Funktionalität:
    Die Klasse Assistent enthält die Implementierung für einen schrittweisen Assistenten. Weitere Informationen finden Sie in der Dokumentation.

2. ConfigEditorForm

    Dateien:
        ConfigEditorForm.cs
        ConfigEditorForm.Designer.cs

    Funktionalität:
    Diese Klasse ist der Konfigurationseditor. Momentan wird sie nicht verwendet und sollte entfernt werden.

3. Form1

    Dateien:
        Form1.cs
        Form1.Designer.cs

    Funktionalität:
    Form1 repräsentiert das Hauptformular der Anwendung. Weitere Informationen zur Funktionalität finden Sie in der Dokumentation.

4. Form2

    Dateien:
        Form2.cs
        Form2.Designer.cs

    Funktionalität:
    Diese Klasse ist obsolet und sollte entfernt werden.

5. LocationInfo

    Dateien:
        LocationInfo.cs

    Funktionalität:
    Die Klasse LocationInfo dient dazu, Adressinformationen aus den Metadaten eines Bildes zu extrahieren.

6. Lupe

    Dateien:
        lupe.cs
        lupe.Designer.cs

    Funktionalität:
    Diese Klasse ist obsolet und sollte entfernt werden.

7. Ort

    Dateien:
        Ort.cs

    Funktionalität:
    Die Klasse Ort bestimmt anhand von Daten aus data1.txt und der Google-Adresse den Datensatz für Stadt, Ordnungsamt und Mailadresse.

8. PdfHelper

    Dateien:
        PdfHelper.cs

    Funktionalität:
    Die Klasse PdfHelper ermöglicht die Ausgabe der Anzeige in eine PDF-Datei.

9. PhotoMetadataExtractor

    Dateien:
        PhotoMetadataExtractor.cs

    Funktionalität:
    Diese Klasse extrahiert Metadaten aus einer Bilddatei.

10. Program

    Dateien:
        Program.cs

    Funktionalität:
    Die Klasse Program enthält die Hauptlogik der Anwendung.

11. TimePicker

    Dateien:
        TimePicker.cs
        TimePicker.Designer.cs

    Funktionalität:
    TimePicker ist ein UserControl zur reinen Zeiterfassung.

Build und Ausführung:

Die Anwendung wurde in C# entwickelt. Um die Anwendung zu kompilieren und auszuführen, können Sie eine geeignete Entwicklungsumgebung wie Visual Studio verwenden.