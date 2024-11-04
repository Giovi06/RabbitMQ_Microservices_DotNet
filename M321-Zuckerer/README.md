# M321-Zuckerer_L

Dies ist die kombinierte Musterlösung für mehrere Lern- und Arbeitsaufträge im [Modul 321](https://www.modulbaukasten.ch/module/321) an der [Berufsfachschule BBB](https://bbbaden.ch/) in Baden.

Die Musterlösung betrifft die folgenden Aufträge:

- LA_321_0113_ApiGatewayVorbereiten
- LA_321_0114_MQVorbereiten
- LA_321_0115_ArticleService
- LA_321_0116_AuthService
- LA_321_0117_CustomerService
- LA_321_0118_OrderService
- LA_321_0119_EventLogService
- LA_321_0120_ETL

In diesen Aufträgen wird das Projekt für den Kunden umgesetzt.


## ETL

Für den Auftrag LA_321_0120_ETL benötigen Sie einen erreichbaren MariaDB-Server, wie er im LA_321_4204 im Docker Image integriert ist.

Gehen Sie wie folgt vor.


### Schritt 1: Dump erstellen

⚠️ Sollten Sie keine Daten mit der "alten" PHP-Applikation (siehe LA_321_4204) erstellt haben, können Sie diesen Teil überspringen und bei Schritt 2 weitermachen.


- Starten Sie den Container zu LA_321_4204.
- Öffnen Sie http://localhost:41062/.
- Navigieren Sie zum phpMyAdmin.
- Wählen Sie Datenbank "M321".
- Klicken Sie auf die Registerkarte "Exportieren".
- Klicken Sie den Button "Exportieren".
- Speichern Sie die SQL-Datei auf Ihrem Computer (Datei `M321.sql`).


### Schritt 2: Erreichbaren DB-Server aufsetzen

Mit folgendem Befehl erstellen Sie einen Datenbankserver in einem Docker-Container, der nach der Verwendung gerade wieder von Ihrem System entfernt wird (Option `--rm`):

```
docker run --rm mariadb:10 -p "127.0.0.1:3306:3306" -e MYSQL_ROOT_PASSWORD=mysecret
```


### Schritt 3: Mit SQL-Client verbinden

Verwenden Sie einen SQL-Client, wie [HeidiSQL](https://www.heidisql.com/) oder [SQLectron](https://sqlectron.github.io/), um sich mit der Datenbank im Container zu verbinden:

| Einstellung | Wert            |
| ----------- | --------------- |
| Hostname    | localhost       |
| Port        | 3306            |
| Typ         | MySQL / MariaDB |
| Username    | root            |
| Passwort    | mysecret        |


### Schritt 4: SQL dump einlesen

Öffnen Sie in Ihrem SQL client ein neues Fenster, in dem Sie die SQL-Befehle aus dem SQL-Dump (siehe Schritt 1) ausführen (Datei `M321.sql`).


### Schritt 5: ETL konfigurieren

Konfigurieren Sie in `appsettings.json` die Connection Strings zu allen Datenbanken. Verwenden Sie absolute Pfade für die SQLite-Datenbanken.


### Schritt 6: ETL laufen lassen

Führen Sie jetzt das Programm für den Datentransfer aus.

<div style="border: 0.25rem solid red;border-radius:0.5rem;margin:1rem;padding:2rem;">

⚠️ **ACHTUNG!**

Die SQLite-Datenbanken müssen bereits existieren und die korrekte Struktur aufweisen.

Dazu können Sie die Solution in Visual Studio ausführen und in der Paketmanager-Console die folgenden Befehle ausführen:

```powershell
Update-Database -Project ArticleService
Update-Database -Project ClientService
Update-Database -Project OrderService
Update-Database -Project EventLogService
```
</div>

<div style="border: 0.25rem solid red;border-radius:0.5rem;margin:1rem;padding:2rem;">

⚠️ **ACHTUNG!**

Das Programm der Musterlösung löscht alle bestehenden Daten in den SQLite-DBs!
</div>


### Schritt 7: Bezugspunkt sichern

Gemäss Event-Sourcing-Pattern werden alle Operationen im Gesamtsystem im Eventlog mitgeschrieben. Damit daraus auch wirklich der Zustand wiederhergestellt werden kann, muss der jetzige Datenbestand als Bezugspunkt festgehalten werden. Sie müssen also ein Backup der Datenbanken erstellen. Das Vorgehen zum Backup unterscheidet sich je nach Datenbanksoftware: Bei SQLite reicht eine Kopie der Datenbankdatei, bei MySQL/MariaDB würden Sie einen Dump erstellen.


### Schritt 8: Docker container beenden

Stoppen Sie den Docker container aus Schritt 2 mit <kbd>Ctrl</kbd> + <kbd>C</kbd>.

Es kann sein, dass Sie noch Images und Volumes entfernen müssen, falls Sie das wollen.
