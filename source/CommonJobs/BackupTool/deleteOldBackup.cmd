@ECHO OFF
ECHO.

setlocal

REM need backup folder
REM optional: amount of backups to preserve
SET backupFolder=%1
SET keepRecentDays=%2

IF "%backupFolder%" EQU "" GOTO :ERROR
IF "%keepRecentDays%" EQU "" SET /a keepRecentDays = 10

REM Iterar por los nombres de archivo de la carpeta destino
REM Contar los primeros N y eliminar los posteriores uno a uno,
REM ordenandolos por fecha de creación más reciente primero

REM http://stackoverflow.com/q/51054/147507
FORFILES -p "%backupFolder%" -m *.* -d %keepRecentDays% -c "cmd /c del @path"

ECHO Finalizado.
GOTO END

:ERROR
ECHO ERROR: Debe especificarse el path a los archivos de backup.
ECHO.
:HELP
ECHO Herramienta de Backup de CommonJobs
ECHO ===================================
ECHO.
ECHO Eliminador de backups antiguos
ECHO ------------------------------
ECHO.
ECHO Este script elimina los N archivos mas antiguos especificados en la
ECHO carpeta de destino. Fue disenado para la automatizacion de backup de
ECHO CommonJobs, pero puede utilizarse para otros fines.
ECHO CUIDADO: Esta herramienta eliminara archivos.
ECHO.
ECHO Sintaxis: 
ECHO deleteOldBackup.cmd backupPath [backupsToKeep]
ECHO.
ECHO     backupPath      Path de la carpeta en donde se almacenan los backups
ECHO                     de CommonJobs.
ECHO     keepRecentDays  (Opcional) Limite de antiguedad (en dias) para
ECHO                     preservar backups. Default: 10.
ECHO.
ECHO Ejemplos:
ECHO     deleteOldBackup.cmd C:\CommonJobsDEV\Backups 5
ECHO     deleteOldBackup.cmd C:\CommonJobs\Backups
GOTO END

:END
endlocal
ECHO.