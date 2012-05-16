@ECHO OFF
ECHO.

REM Verificar minimamente los parametros
IF [%6] EQU [] GOTO :HELP
IF NOT [%7] EQU [] GOTO :HELP
IF %3%4%5%6 EQU XXXX GOTO :NOTHING

REM Parsear configuracion desde linea de comandos
SET destinationPath=%1
SET prefix=%2
SET ravenDbUrl=%3
SET uploadsPath=%4
SET sitePath=%5
SET ravenDBServerPath=%6

ECHO destinationPath = %destinationPath% 
ECHO prefix = %prefix%
ECHO ravenDbUrl = %ravenDbUrl%
ECHO uploadsPath = %uploadsPath%
ECHO sitePath = %sitePath%
ECHO ravenDBServerPath = %ravenDBServerPath%

REM Generar fecha para el nombre del archivo (http://www.tech-recipes.com/rx/956/windows-batch-file-bat-to-get-current-date-in-mmddyyyy-format/)
FOR /F "TOKENS=1* DELIMS= " %%A IN ('DATE/T') DO SET CDATE=%%B
FOR /F "TOKENS=1,2 eol=/ DELIMS=/ " %%A IN ('DATE/T') DO SET mm=%%B
FOR /F "TOKENS=1,2 DELIMS=/ eol=/" %%A IN ('echo %CDATE%') DO SET dd=%%B
FOR /F "TOKENS=2,3 DELIMS=/ " %%A IN ('echo %CDATE%') DO SET yyyy=%%B
SET date=%yyyy%%mm%%dd%

REM Generar nombre de archivo
SET /a counter=0
:GET_FILE_NAME
SET /a counter=%counter%+1
SET bkpname=%prefix%_%date%_%counter%
SET bkpfolder=%destinationPath%\%bkpname%
SET bkpfile=%bkpfolder%.zip
IF EXIST %bkpfolder% GOTO GET_FILE_NAME
IF EXIST %bkpfile% GOTO GET_FILE_NAME
MKDIR %bkpfolder%
ECHO bkpfolder = %bkpfolder%

REM Backup de base de datos a carpeta temporal
IF %ravenDbUrl% EQU X GOTO BACKUP_UPLOADS
SET bkpfolder_ravenDb=%bkpfolder%\DB
MKDIR %bkpfolder_ravenDb%
Raven.Backup.exe --url=%ravenDbUrl% --dest=%bkpfolder_ravenDb%

REM Backup de uploads a carpeta temporal
:BACKUP_UPLOADS
IF %uploadsPath% EQU X GOTO BACKUP_SITE
SET bkpfolder_uploads=%bkpfolder%\UPLOADS
MKDIR %bkpfolder_uploads%
XCOPY /e /f /y %uploadsPath% %bkpfolder_uploads%

REM Backup del sitio a carpeta temporal
:BACKUP_SITE
IF %sitePath% EQU X GOTO BACKUP_SERVER
SET bkpfolder_site=%bkpfolder%\SITE
MKDIR %bkpfolder_site%
XCOPY /e /f /y %sitePath% %bkpfolder_site%

REM Backup del servidor de RavenDB a carpeta temporal
:BACKUP_SERVER
IF %ravenDBServerPath% EQU X GOTO COMPRESS
SET bkpfolder_ravenDBServer=%bkpfolder%\RAVENDBSERVER
MKDIR %bkpfolder_ravenDBServer%
XCOPY /f /y %ravenDBServerPath% %bkpfolder_ravenDBServer%

REM Comprimir archivos y eliminar carpeta temporal
:COMPRESS
ECHO bkpfile = %bkpfile%
7za.exe a -tzip %bkpfile% %bkpfolder_ravenDb% %bkpfolder_uploads% %bkpfolder_site% %bkpfolder_ravenDBServer%
RMDIR /S /Q %bkpfolder%

GOTO END

REM Ayuda
:NOTHING
ECHO ERROR: Debe especificarse al menos uno de los siguiente parametros ravenDbUrl, uploadsPath, sitePath o ravenDBServerPath.
ECHO.
:HELP
ECHO Herramienta de Backup de CommonJobs
ECHO ===================================
ECHO.
ECHO Sintaxis: 
ECHO BACKUP.CMD destinationPath prefix ravenDbUrl uploadsPath sitePath ravenDBServerPath
ECHO.
ECHO (Se puede ingresar X en lugar de ravenDbUrl, uploadsPath, sitePath o ravenDBServerPath para omitir el backup correspondiente)
ECHO.
ECHO Ejemplo: 
ECHO BACKUP.CMD C:\CommonJobsDEV\Backups CommonJobsDEV http://localhost:8080 C:\CommonJobsDEV\Uploads C:\CommonJobsDEV\commonjobs.makingsense.com_8888 C:\CommonJobsDEV\RavenDB\Server
GOTO END

:END
ECHO.