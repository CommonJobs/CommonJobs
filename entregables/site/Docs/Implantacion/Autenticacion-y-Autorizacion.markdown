# Autenticación y Autorización

Actualmente el sistema verifica los siguientes roles (_Este documento será actualizado cuando el sistema identifique otros roles_):

* **Users**: Usuarios del sistema, con acceso a creción y edición de empleados y aplicantes asi como también consultas.
* **Migrators**: Usuarios con acceso a la UI de migración de datos entre versiones.
* **_Usuarios del dominio_**: Los usuarios del dominio, mas allá de que pertenezcan a un grupo o no, tendran acceso a determinado contenido, por ejemplo los archivos adjuntos si es que conocen su URL.



## Active Directory

Los roles del sistema se corresponderán con grupos de Active Directory del dominio de la empresa, según el entorno:

* Entorno `PROD`: Se utilizará el prefijo `CommonJobs_`. Estos grupos serán administrados por el cliente como mejor lo considere.
  * Grupo `CommonJobs_Migrators`, sugerimos que se le asigne a quien realiza el despliegue de la aplicación y tal vez a un usuario del sistema o un desarrollador.
  * Grupo `CommonJobs_Users`, sugerimos que se le asigne a los encargados de RRHH y tal vez a una persona con rol técnico dentro de la empresa.
* Entorno `DEV`: Se utilizará el prefijo `CommonJobsDEV_`.
  * Grupo `CommonJobsDEV_Migrators` 
     * Asignado al grupo `CommonJobsDEV`, para que los desarrolladores puedan acceder a las migraciones del entorno `DEV`.
     * Asignado al grupo `CommonJobs_Migrators`, para que quienes realizan las migraciones en `PROD` puedan hacer pruebas en `DEV`.
  * `CommonJobsDEV_Users`
     * Asignado al grupo `CommonJobsDEV`, para que los desarrolladores puedan acceder al entorno `DEV`.
     * Asignado al grupo `CommonJobs_Users`, para que quienes utilizan el sistema en `PROD` puedan hacer pruebas en `DEV`.

