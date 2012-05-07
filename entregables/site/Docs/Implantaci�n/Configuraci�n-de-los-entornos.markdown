# Configuración de los entornos

## Notas

Por cuestiones de seguridad, se han reemplazado algunos identificadores por las siguientes cadenas:

* `LAN-SERVER-NAME`
* `LAN-SERVER-IP`
* `DNS-SERVER-NAME`

También se han ocultado en las capturas de pantalla.

Los valores correspondientes y las capturas originales se envirán por email a los interesados.

Por otro lado, en algunas capturas de pantalla se puede ver un usuario `CommonJobs` ese usuario a sido creado de forma temporal y será eliminado, no tiene utilidad en la configuración del sistema.

## Resumen 

Para ambos entornos DEV y PROD, el cliente pone a disposición el servidor `LAN-SERVER-NAME` con la dirección IP interna `LAN-SERVER-IP` y accesible externamente mediante el siguiente nombre: `DNS-SERVER-NAME`.

Ambos servidores web, DEV y PROD, serán accesibles externamente mediante las siguientes URLs http://DNS-SERVER-NAME:8888 y http://DNS-SERVER-NAME respectivamente.

Los servicios de base de datos correrán en el mismo servidor. El servicio correspondiente a DEV será accesible externamente a través del puerto `8080` por los desarrolladores del sistema autenticados mediante _Windows Authentication_. La base de datos PROD solo será accesible localmente por el servidor web de PROD.


## Configuración de los usuarios y grupos

Para configurar la seguridad del sistema se crearán los grupos `LAN-SERVER-NAME\CommonJobsDEV` y `LAN-SERVER-NAME\CommonJobsPROD` que representarán los usuarios admitidos para acceder al sistema de archivos y a la base de datos de cada entorno. Los usuarios `LAN-SERVER-NAME\CommonJobsDEVUsr` y `LAN-SERVER-NAME\CommonJobsPRODUsr` corresponden a los usuarios de los Application Pool que ejecutarán la aplicación .NET en cada uno de los entornos, en las siguientes secciones se verán más detalles relacionados.

* Grupo `LAN-SERVER-NAME\CommonJobsDEV`
   * Usuario `LAN-SERVER-NAME\CommonJobsDEVUsr`
   * Usuarios de dominio correspondientes a los desarrolladores del sistema.
* Grupo `LAN-SERVER-NAME\CommonJobsPROD`
   * Usuario `LAN-SERVER-NAME\CommonJobsPRODUsr`
   * Usuarios de dominio correspondientes a los administradores del sistema.
* Grupo `IIS_IUSRS`
   * Usuario `LAN-SERVER-NAME\CommonJobsDEVUsr`
   * Usuario `LAN-SERVER-NAME\CommonJobsPRODUsr`

![ ](Images/UsersGroups.jpg)

## Configuración del Servidor Web

### Application Pools

* `CommonJobs DEV AppPool`
   * Identity `LAN-SERVER-NAME\CommonJobsDEVUsr`
* `CommonJobs PROD AppPool`
   * Identity `LAN-SERVER-NAME\CommonJobsPRODUsr`

![ ](Images/IIS_AppPools.jpg)

### Sitios

* `CommonJobs DEV` (http://DNS-SERVER-NAME:8888)
   * Bindings: `http:*:8888:`
   * Application Pool: `CommonJobs DEV AppPool`
   * Physical Path: `C:\CommonJobsDEV\DNS-SERVER-NAME_8888`
   * Physical Path Credentials: `LAN-SERVER-NAME\CommonJobsDEVUsr`
   * Permisos completos en el sistema de archivos y _File Sharing_ habilitado para `LAN-SERVER-NAME\CommonJobsDEV` 

![ ](Images/IIS_DEV_Site.jpg)

![ ](Images/FileSystem_DEV.jpg)

![ ](Images/Sharing_DEV_folder.jpg)

* `Documentation` (http://DNS-SERVER-NAME:8888/Documentation)
   * Subsitio de `CommonJobs DEV` con similar configuración
   * Physical Path: `DNS-SERVER-NAME_8888_documentation`

![ ](Images/IIS_DEV_Documentation_Site.jpg)

* `CommonJobs PROD`
   * Bindings: `http:*:80:`
   * Application Pool: `CommonJobs PROD AppPool`
   * Physical Path: `C:\Sites\CommonJobs\DNS-SERVER-NAME`
   * Physical Path Credentials: `LAN-SERVER-NAME\CommonJobsPRODUsr`
   * Permisos completos en el sistema de archivos para `LAN-SERVER-NAME\CommonJobsPROD`

![ ](Images/IIS_PROD_Site.jpg)

![ ](Images/FileSystem_PROD.jpg)

## Configuración del Servidor de Base de Datos

### Servicios Windows de RavenDB

![ ](Images/Services.jpg)

* `RavenDB CommonJobs DEV`
   * Physical Path: `C:\CommonJobsDEV\RavenDB`
   * Escuchando puerto: `8080`
   * Bloqueado acceso anonimo
   * Acceso permitido para el grupo `LAN-SERVER-NAME\CommonJobsDEV`

![ ](Images/RavenConfig_DEV.jpg)

* `RavenDB CommonJobs PROD`
   * Physical Path: `C:\Sites\CommonJobs\DNS-SERVER-NAME`
   * Escuchando puerto: `8090`
   * Bloqueado acceso anonimo
   * Acceso permitido para el grupo `LAN-SERVER-NAME\CommonJobsPROD`

![ ](Images/RavenConfig_PROD.jpg)

## Firewall

Mediante Windows Firewall vamos a limitar el acceso a los diferentes servicios.

* Bloqueado todo acceso no local al puerto `8090` (servicio de base de datos de PROD)
* Permitido acceso externo al puerto `80` (sitio web de PROD)
* Permitido acceso externo al puerto `8888` (sitio web de DEV)
* Permitido acceso externo al puerto `8080` (servicio de base de datos de DEV)

![ ](Images/Firewall.jpg)

## Otros detalles

### Archivos adjuntos

Los archivos adjuntos se almacenarán en el sistema de archivos:

* `C:\CommonJobsDEV\Uploads` (DEV)
   * Permisos completos en el sistema de archivos y _File Sharing_ habilitado para `LAN-SERVER-NAME\CommonJobsDEV` 
* `C:\Sites\CommonJobs\Uploads` (PROD)
   * Permisos completos en el sistema de archivos para `LAN-SERVER-NAME\CommonJobsPROD` 

### Migraciones

Las migraciones permitirán modificar los documentos de la base de datos para adaptarse a la versión de la aplicación. Deberán ejecutarse manualmente y el acceso será restringido por dirección IP configurable en el web.config de la aplicación.

* Migraciones de DEV
  * URL: http://DNS-SERVER-NAME:8888/migrations / http://localhost:8888/migrations
  * Accesible localmente desde `LAN-SERVER-NAME` o desde las direcciones IPs de los desarrolladores
* Migraciones de PROD
  * URL: http://localhost/migrations
  * Solo accesible localmente desde `LAN-SERVER-NAME`

![ ](Images/Migrations.jpg)

### Archivos web.config

![ ](Images/WebConfig_DEV.jpg)

![ ](Images/WebConfig_PROD.jpg)
