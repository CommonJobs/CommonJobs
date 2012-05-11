@echo off

SET ravenDbUrl=%1
SET uploadsPath=%2
SET destinationPath=%3
SET prefix=CJBKP
SET /a counter=0

echo ravenDbUrl = %ravenDbUrl%
echo uploadsPath = %uploadsPath%
echo destinationPath = %destinationPath%
echo prefix = %prefix%

:TRY_CREATE_FOLDER
SET /a counter=%counter%+1
SET bkpname=%prefix%%counter%
SET bkpfolder=%destinationPath%\%bkpname%
SET bkpfile=%bkpfolder%.zip
IF EXIST %bkpfolder% GOTO TRY_CREATE_FOLDER
IF EXIST %bkpfile% GOTO TRY_CREATE_FOLDER
SET bkpfolder_db=%bkpfolder%\DB
SET bkpfolder_upl=%bkpfolder%\UPLOADS
MKDIR %bkpfolder_db%
MKDIR %bkpfolder_upl%

Raven.Backup.exe --url=%ravenDbUrl% --dest=%bkpfolder_db%
xcopy /e /f /y %uploadsPath% %bkpfolder_upl%
7za.exe a -tzip %bkpfile% %bkpfolder_db% %bkpfolder_upl%

RMDIR /S /Q %bkpfolder%
