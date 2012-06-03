@ECHO OFF
ECHO.

setlocal

REM need backup folder
REM optional: amount of backups to preserve
SET backupFolder=%1
SET backupsToKeep=%2

IF [%backupFolder%] EQU [] GOTO :ERROR
IF [%backupsToKeep%] EQU [] SET backupsToKeep = 10

REM Iterar por los nombres de archivo de la carpeta destino
REM Contar los primeros N y eliminar los posteriores uno a uno,
REM ordenandolos por fecha de creación más reciente primero

@ECHO ON
SET /a counter=1
FOR /F %%F IN ('DIR "%backupFolder%" /B /A-D /O-D /TC') DO (
	REM this does not work, the value of the variables are freezed in a for loop
	ECHO %%F
	ECHO %counter%
	REM IF %counter% GTR %backupsToKeep% (DEL %backupFolder%\%%F)
	SET /a counter="%counter%+1"
	ECHO %counter%
)

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
ECHO     backupsToKeep   (Opcional) Cantidad de backups a preservar.
ECHO                     Default: 10.
ECHO.
ECHO Ejemplos:
ECHO     deleteOldBackup.cmd C:\CommonJobsDEV\Backups 5
ECHO     deleteOldBackup.cmd C:\CommonJobs\Backups
GOTO END

:END
endlocal
ECHO.