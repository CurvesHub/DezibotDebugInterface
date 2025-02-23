# Dezibot Debug Interface

Das Dezibot Debug Interface bietet eine Möglichkeit, Daten von einem Dezibot an einen Backend-Server zu senden
und in einer Weboberfläche anzuzeigen. Dieses Repository enthält den Code für den Backend- und Frontend-Server. Der Code 
für den Dezibot kann [hier](https://github.com/CurvesHub/dezibot) gefunden werden.


## Inhaltsverzeichnis

1. [Überblick](#überblick)
2. [Erste Schritte](#erste-schritte)
    - [Achtung](#achtung)
3. [Dezibot Log-Klasse](#dezibot-log-klasse)
    - [Methoden](#methoden)
    - [Beispiel](#beispiel)
4. [Backend-API](#backend-api)
    - [Endpunkte](#endpunkte)
    - [Sitzungshandhabung](#sitzungshandhabung)
        - [Mehrere Clients](#mehrere-clients)
        - [Sitzungen löschen](#sitzungen-löschen)
        - [Beispielszenarien](#beispielszenarien)
    - [Beispiel Request für `PUT /api/dezibot/update`](#beispiel-request-für-put-apidezibotupdate)
        - [Statusdaten](#statusdaten)
        - [Protokolldaten](#protokolldaten)
5. [Lizenz](#lizenz)


## Überblick

Die Dezibot Debug-Schnittstelle besteht aus drei Hauptkomponenten:

1. **Dezibot**: Der Dezibot ist ein kleiner Roboter mit vielen Sensoren, der programmiert werden kann, um verschiedene Aufgaben auszuführen. Der Dezibot-Code ist dafür verantwortlich, Daten an den Backend-Server zu senden.
2. **Backend-Server**: Der Backend-Server empfängt Daten vom Dezibot und speichert sie in einer Datenbank. Er stellt auch eine API für das Frontend bereit, um die Daten abzurufen.
3. **Frontend-Server**: Der Frontend-Server zeigt die Daten des Dezibots in einer Weboberfläche an. Er ermöglicht es Benutzern, die Daten in Echtzeit zu sehen und zu analysieren.

Das Hauptziel der Dezibot Debug-Schnittstelle ist es, eine Möglichkeit zu bieten, Daten vom Dezibot zu protokollieren und in einer benutzerfreundlichen Oberfläche anzuzeigen. Dies ermöglicht es Benutzern, das Verhalten des Dezibots zu überwachen und Probleme zu debuggen, die während des Betriebs auftreten können. Die Schnittstelle unterstützt mehrere Dezibots und Benutzersitzungen, sodass jeder Benutzer die Daten in Echtzeit sehen kann, ohne andere Benutzer zu stören.


## Erste Schritte

Alle Dienste sind dockerisiert und können mit docker-compose gestartet werden. Befolgen Sie die folgenden Anweisungen, um die Dienste zu starten. Dies ist eine Schritt-für-Schritt-Anleitung zur Verwendung des Dezibot Debug Interface mit dem Beispielcode in der Datei `log_demo_simple.ino`.

1. Klonen Sie das Repository
2. Konfigurieren Sie die Datei `docker-compose.yml` mit den gewünschten Umgebungsvariablen
    - `NUXT_PUBLIC_SERVER_URL`: Dies ist die URL des Host-Rechners, auf dem der Backend-Server läuft
        - Für einen Client (Browser), der auf demselben Rechner wie das Backend/Docker läuft, können Sie `http://localhost:5160` verwenden
        - Wenn Clients (Browser) auf anderen Rechnern laufen, verwenden Sie die IP-Adresse des Host-Rechners, auf dem der Backend-Server läuft
3. Bereiten Sie das Dezibot-Codebeispiel vor
    - Starten Sie einen mobilen Hotspot oder verwenden Sie ein aktives WLAN-Netzwerk
    - Verbinden Sie den Host-Rechner (auf dem Docker läuft) mit dem WLAN-Netzwerk
    - Geben Sie die WLAN-SSID und das Passwort in der Datei `log_demo_simple.ino` ein
    - Geben Sie die IP-Adresse des Host-Rechners ein, auf dem der Backend-Server läuft
    - Laden Sie den Code auf einen Dezibot
4. Führen Sie `docker-compose up` im Stammverzeichnis des Projekts aus
    - Das Frontend ist verfügbar unter `http://localhost:3000`
    - Das Backend ist verfügbar unter `http://localhost:5160`
5. Öffnen Sie das Frontend in einem Browser
6. Starten Sie eine neue Sitzung oder treten Sie einer bestehenden bei
    - Wählen Sie eine Sitzung aus der Dropdown-Liste aus
    - Klicken Sie entweder auf die Schaltfläche `Ansehen` oder `Fortsetzen`, um der Sitzung beizutreten
        - `Ansehen` zeigt nur die aktuelle Sitzung an, ohne Updates zu erhalten
        - `Fortsetzen` zeigt die aktuelle Sitzung an und erhält Updates, wenn ein Dezibot Daten sendet
7. Das Frontend zeigt die Daten der Sitzung mit den dazugehörigen Dezibots an

### Achtung

Wenn Sie ein Hauptprogramm für den Dezibot schreiben, stellen Sie sicher, dass Sie `Log::begin(ssid, password, url)`
aufrufen, bevor Sie `dezibot.begin()` aufrufen. Andernfalls werden die anfänglichen Setup-Protokolle und Debug-Daten
nicht an den Backend-Server gesendet.

Wenn `Log::begin(ssid, password, url)` **nicht** aufgerufen wird, werden alle aufgerufenen Methoden der `Log`-Klasse
sofort beendet, ohne etwas zu tun.

## Dezibot Log-Klasse

Die `Log`-Klasse bietet Methoden zum Protokollieren von Ereignissen und Statusänderungen auf einem Webserver über WLAN.
Sie unterstützt verschiedene Protokollierungsstufen und kann Daten periodisch oder auf Abruf senden.

### Methoden

1. **Initialisierung**
    - **`Log::begin(const char* wifiSSID, const char* wifiPassword, String url)`**
        - Verbindet sich mit dem angegebenen WLAN-Netzwerk und richtet die URL des Protokollierungsservers ein.
        - **Parameter:**
            - `wifiSSID`: Die SSID des WLAN-Netzwerks.
            - `wifiPassword`: Das Passwort für das WLAN-Netzwerk.
            - `url`: Die URL des Webservers zum Protokollieren von Daten.

2. **Protokollierung von Eigenschaftsänderungen**
    - **`Log::propertyChanged(String className, String propertyName, String newValue)`**
        - Protokolliert ein Ereignis einer Eigenschaftsänderung für eine gegebene Klasse und Eigenschaft.
        - **Parameter:**
            - `className`: Der Name der Klasse, in der die Eigenschaft geändert wurde.
            - `propertyName`: Der Name der geänderten Eigenschaft.
            - `newValue`: Der neue Wert der Eigenschaft.

3. **Protokollierung von Nachrichten**
    - **`Log::d(DezibotLogLevel level, String className, String message, String data = "")`**
        - Protokolliert eine Nachricht mit einer angegebenen Protokollierungsstufe und zugehörigen Daten.
        - **Parameter:**
            - `level`: Die Protokollierungsstufe der Nachricht (`DEBUGLOG`, `INFOLOG`, `WARNLOG`, `ERRORLOG`).
            - `className`: Der Name der Klasse, die die Nachricht generiert.
            - `message`: Eine kurze Nachricht, die das Ereignis beschreibt.
            - `data`: Zusätzliche Daten zum Protokollieren (optional).

4. **Senden von Statusdaten**
    - **`Log::update()`**
        - Sendet die aktuellen Statusdaten an den Server.

### Example

```cpp
#include <Dezibot.h>
#include <../src/log/Log.h>

const char* ssid = "hotspot";
const char* password = "password";
String ipAdress = "xxx.xxx.xxx.xxx";

Dezibot dezibot = Dezibot();

void setup() {
    Serial.begin(115200);
    Log::begin(ssid, password, "http://" + ipAdress + ":5160/api/dezibot/update");
    dezibot.begin();
}

void loop() {
    Log::d(DEBUGLOG, MAIN_PROGRAM, "Debug log from main");
    Log::d(INFOLOG, MAIN_PROGRAM, "Info log from main");
    Log::d(WARNLOG, MAIN_PROGRAM, "Warnings log from main");
    Log::d(ERRORLOG, MAIN_PROGRAM, "Error log from main");

    Log::propertyChanged(MAIN_PROGRAM, "someProperty", "someValue");

    Log::update();
    delay(3000);
}
```

Dieses Beispiel zeigt, wie die `Log`-Klasse initialisiert, Nachrichten protokolliert, Eigenschaftsänderungen 
protokolliert und Statusdaten an den Server gesendet werden. Die Methode `Log::update()` sendet alle 3 Sekunden
die aktuellen Statusdaten an den Server. Die Methoden `Log::d()` senden die Protokolldaten jedoch sofort.

## Backend API

Die Backend-API bietet Endpunkte zum Verwalten von Sitzungen und Abrufen von Daten von Dezibots. Sie enthält auch
eine WebSocket-Verbindung, um Echtzeit-Updates an das Frontend zu senden. Die API ist mit Scalar UI unter Verwendung
der OpenAPI-Spezifikation dokumentiert. Sie kann unter `http://localhost:5160/api` aufgerufen werden.

### Endpunkte

Das Backend bietet die folgenden Endpunkte:

- `/api` - Eine Benutzeroberfläche zum Anzeigen der OpenAPI-Dokumentation der API und zum Testen der Endpunkte

- `GET /api/session/available` - Gibt eine Liste aller verfügbaren Sitzungidentifikatoren zurück
- `GET /api/session` - Gibt eine Liste aller Sitzungen mit ihren Dezibots zurück
- `GET /api/session/{sessionId}` - Gibt die Sitzung mit der angegebenen ID zurück
- `GET /api/session/{sessionId}/dezibot/{ip}` - Gibt den Dezibot mit der angegebenen IP-Adresse aus der Sitzung mit der angegebenen ID zurück

- `POST /api/session` - Erstellt eine neue Sitzung

- `DELETE /api/session` - Löscht alle nicht verwendeten Sitzungen
- `DELETE /api/session/{sessionId}` - Löscht die Sitzung mit der angegebenen ID, wenn sie nicht verwendet wird
- `DELETE /api/session/{sessionId}/dezibot/{ip}` - Löscht den Dezibot mit der angegebenen IP-Adresse aus der Sitzung mit der angegebenen ID

- `WS /api/dezibot-hub` - Signal R WebSocket für das Frontend zum Empfangen von Daten
    - Bietet die folgenden Methoden/Ereignisse:
        - `JoinSession`
            - Wird von einem Client aufgerufen, um einer Sitzung mit der angegebenen ID beizutreten
            - Der Client kann einen `continueSession`-Parameter angeben, um zu entscheiden, ob er Updates für die Sitzung erhalten möchte
        - `DezibotUpdated`
            - Wird vom Backend aufgerufen, wenn ein Dezibot neue Daten sendet oder ein Client einer Sitzung beitritt
            - Sendet den neuesten Dezibot an den/die Client(s)
        - `OnDisconnected`
            - Wird vom Backend aufgerufen, wenn ein Client die Verbindung trennt
            - Entfernt den Client aus den Sitzungen, in denen er sich befand

- `PUT /api/dezibot/update` - Empfängt Statusdaten oder Protokolle von Dezibots und speichert sie je nach verfügbarer Sitzung

### Sitzungshandhabung

Der Mechanismus zur Sitzungshandhabung stellt sicher, dass mehrere Clients verschiedene Dezibot-Sitzungen anzeigen
können, ohne sich gegenseitig zu stören. Jede Sitzung hat eine eindeutige Kennung, die zur Unterscheidung zwischen
verschiedenen Sitzungen verwendet wird. Wenn ein Client einer Sitzung beitritt, erhält er nur Updates für 
Sitzung. Der Mechanismus zur Sitzungshandhabung stellt auch sicher, dass Clients aus Sitzungen entfernt werden, wenn
sie die Verbindung trennen, um zu verhindern, dass veraltete Verbindungen das System beeinträchtigen.

#### Mehrere Clients

- Jeder Client kann einer Sitzung mit einem `continueSession`-Parameter beitreten, der auf `true` oder `false` gesetzt ist, um anzugeben, ob er Updates erhalten möchte.
- Solange mindestens ein Client in einer Sitzung ist, der Updates erhalten möchte, werden Dezibot-Updates gespeichert. Andernfalls werden Updates verworfen.
- Wenn mehrere Clients in einer Sitzung sind, erhalten nur die Clients, die sich für Updates entschieden haben, diese.

#### Sitzungen löschen

Eine Sitzung kann nur gelöscht werden, wenn keine Clients sie derzeit verwenden. Dies stellt sicher, dass eine Sitzung
nicht versehentlich gelöscht wird, während sie von einem anderen Client angezeigt wird. Sobald alle Clients die Sitzung
verlassen haben, wird sie zur Löschung freigegeben. Das Frontend zeigt eine Fehlermeldung an, wenn ein Client versucht,
eine Sitzung zu löschen, die verwendet wird.

#### Beispielszenarien

1. **Einzelner Client, der eine Sitzung ansieht:**
    - Client 1 tritt einer Sitzung mit `continueSession` auf `false` bei. Er kann die Sitzungsdaten anzeigen, erhält jedoch keine Updates.

2. **Mehrere Clients mit unterschiedlichen Präferenzen:**
    - Client 1 tritt einer Sitzung mit `continueSession` auf `false` bei. Er kann die Sitzungsdaten anzeigen, erhält jedoch keine Updates.
    - Client 2 tritt derselben Sitzung mit `continueSession` auf `true` bei. Die Sitzung wird mit neuen Dezibot-Daten aktualisiert, da Client 2 Updates erhalten möchte. Client 1 bemerkt keine Änderungen.
    - Wenn Client 1 später mit `continueSession` auf `true` beitritt, beginnt er ebenfalls, Updates zu erhalten.

3. **Client trennt die Verbindung:**
    - Wenn ein Client die Verbindung trennt, wird er aus der Sitzung entfernt. Wenn er der einzige Client war, der Updates erhalten hat, werden die Updates für diese Sitzung gestoppt.
    - Ein Trennungsereignis wird an das Backend gesendet, wenn entweder die Seite aktualisiert, geschlossen oder der Benutzer die Seite zurück zum Selektor verlässt.

Dieser Mechanismus stellt sicher, dass jeder Client Dezibot-Sitzungen gemäß seinen Präferenzen anzeigen und damit interagieren kann, ohne andere Clients zu beeinträchtigen.

### Beispiel Request für `PUT /api/dezibot/update`

Diese Beispieldaten können an den Backend-Server gesendet werden, um die Statusdaten oder Protokolldaten eines Dezibots
zu aktualisieren. Anstatt den Beispielcode direkt auf dem Dezibot auszuführen, können die Daten mit Scalar UI
oder einem Tool wie Postman an den Backend-Server gesendet werden.

#### Statusdaten

```json
{
  "Ip": "111.222.333.444",
  "Data": {
    "DISPLAY": {
      "isFlipped": "true",
      "currentLine": "12",
      "lastText": "Hello World"
    }
  }
}
```

#### Protokolldaten

```json
{
  "Ip": "111.222.333.444",
  "logLevel": "INFO",
  "className": "DISPLAY",
  "message": "My first message",
  "data": "Some data"
}
```

## Lizenz

Dieses Projekt ist unter der GPL-3.0-Lizenz lizenziert - beachten Sie die [LICENSE](LICENSE)-Datei für Details.
