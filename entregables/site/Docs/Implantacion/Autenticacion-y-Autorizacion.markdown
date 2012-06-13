# Autenticación y Autorización

El sistema verifica los siguientes roles

* **Users**: Usuarios del sistema, con acceso a creción y edición de empleados y aplicantes asi como también consultas.
* **Migrators**: Usuarios con acceso a la UI de migración de datos entre versiones.
* **_Usuarios del dominio_**: Los usuarios del dominio, mas allá de que pertenezcan a un grupo o no, tendran acceso a determinado contenido, por ejemplo los archivos adjuntos si es que conocen su URL.

## Active Directory

Los roles del sistema se corresponderán con grupos de Active Directory del dominio CS según el entorno:

* Entorno `DEV`: Se utilizará el prefijo `CommonJobsDEV_`, por ejemplo `CommonJobsDEV_Migrators`.
* Entorno `PROD`: Se utilizará el prefijo `CommonJobs_`, por ejemplo `CommonJobs_Migrators`.

Los grupos de `PROD` serán administrados por el cliente como mejor lo considere. En el caso de `DEV` será conveniente agregar a cada uno de los roles el grupo `CS\CommonJobsDEV` y también los roles de `PROD` correspondientes.

