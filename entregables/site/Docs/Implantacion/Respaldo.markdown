# Respaldo y restauración

## Introducción

Para resguardar la información almacenada por el sistema, proveemos un script de backup que se encargará de copiar y comprimir los datos junto con otros archivos importantes para poner en funcionamiento rápidamente el sitio en caso de existir algún problema. 

La restauración deberá realizarse manualmente según instrucciones descritas más abajo. 

El backup podrá realizarse con el sistema funcionando, en cambio la restauración deberá realizarse con el servicio de base de datos detenido.

## Backup

El script de comandos de Windows `Backup.cmd` permite realizar un backup de todos los elementos necesarios para poner en funcionamiento el sistema:

* Datos almacenados en la base de datos RavenDB (no requiere detener el servicio)
* Archivos adjuntos a las entidades del sistema (fotos, curriculums, etc)
* Archivos del sitio en funcionamiento (binarios, vistas, archivos JavaScript, archivos CSS, etc)
* Archivos binarios del servidor de RavenDB en funcionamiento

No es necesario detener el funcionamiento del sistema para realizar el backup.

### Modo de uso

    BACKUP.CMD destinationPath prefix ravenDbUrl uploadsPath sitePath ravenDBServerPath

* `destinationPath`: carpeta destino del archivo de backup, por ejemplo `Y:\backups\commonjobs\prod\`
* `prefix`: prefijo del archivo de backup, por ejemplo `CommonJobsProd` producirá un archivo con un nombre similar a `CommonJobsProd_20120518_1.zip` 
* `ravenDbUrl`: URL del servicio de RavenDB correspondiente, por ejemplo `http://localhost:8090`
* `uploadsPath`: Ubicación en el sistema de archivos local de los attachments, por ejemplo `C:\Sites\CommonJobs\Uploads`
* `sitePath`: Ubicación en el sistema de archivos local de los archivos del sitio, por ejemplo `C:\Sites\CommonJobs\www.commonjobs.com`
* `ravenDBServerPath`: Ubicación en el sistema de archivos local de los archivos del servidor de RavenDB, por ejemplo `C:\Sites\CommonJobs\RavenDB\Server`

_En caso de desear omitir alguno de los parámetros, puede ingresarse una letra `X` en su lugar_

Ejemplo de un backup de datos exclusivamente, sin incluir archivos del sitio o de RavenDB:

    BACKUP.CMD Y:\backups\commonjobs\prod\ CommonJobsProd http://localhost:8090 C:\Sites\CommonJobs\Uploads X X

### Archivo resultante

![ ](Images/backup_file.jpg)

El resultado del backup consiste en un archivo ZIP con cuatro carpetas:

* _`DB`_: Backup de datos de RavenDB
* _`UPLOADS`_: Archivos adjuntos a las entidades del sistema (fotos, curriculums, etc)
* _`SITE`_: Archivos del sitio resguardado (binarios, vistas, archivos JavaScript, archivos CSS, etc)
* _`RAVENDBSERVER`_: Archivos binarios del servidor de RavenDB.

## Restauración

A diferencia del backup, la restauración de los datos deberá realizarse manualmente y con el servicio de RavenDB correspondiente detenido.

### Archivos del sitio y del servidor RavenDB

En general no debería ser necesario restaurar estos archivos, ya que no son datos propiamente dichos, sino archivos funcionales del sistema. En caso de requerir restaurarlos, bastaría copiar los contenidos de las carpetas _SITE_ y _RAVENDBSERVER_ a las carpetas del sitio de IIS y del servidor de RavenDB respectivamente.

### Restauración de los datos de la base de datos

Antes de restaurar los datos, debe verificarse RavenDB correspondiente esté detenido. También que la carpeta `Data` correspondiente al servidor no exista. En caso de existir renombrar o eliminar.

![ ](Images/data_folder.jpg)

Luego debe extraerse la carpeta `DB` del archivo `.zip` del backup a una ubicación accesible, por ejemplo `C:\TEMP\DB` e invocar al ejecutable del servidor de RavenDB (`Raven.Server.exe`) con los parámetros de restauración, siendo `dest` la carpeta Data del servidor y `src` la carpeta con los datos de backup descomprimidos. Por ejemplo: 

    Raven.Server.exe --restore --dest=C:\CommonJobsDEV\RavenDB\Server\Data --src=C:\TEMP\DB

Realizado esto, ya puede iniciarse el servidor de RavenDB.

 
### Restauración de los archivos adjuntos

Para restaurar los archivos adjuntos basta con copiar el contenido de la carpeta _UPLOADS_ del archivo `.zip` del backup a la carpeta de archivos adjuntos a las entidades del sistema correspondiente al entorno que se está restaurando.
