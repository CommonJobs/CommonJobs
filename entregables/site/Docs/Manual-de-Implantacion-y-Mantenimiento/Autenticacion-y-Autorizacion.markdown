# Autenticación y Autorización

Actualmente el sistema verifica los siguientes roles:

* **Users**: Usuarios del sistema, con acceso a creación y edición de empleados y postulantes asi como también consultas.
* **Migrators**: Usuarios con acceso a la UI de migración de datos entre versiones.
* **Applicant Managers**: Usuarios con acceso a la administración de los postulantes a empleos.
* **Employee Managers**: Usuarios con acceso a la administración de los empleados cargados en el sistema.
* **Job Search Managers**: Usuarios con acceso a la administración de búsquedas de empleos.
* **Menu Managers**: Usuarios con permiso de administración de MyMenu.
* **_Usuarios del dominio_**: Los usuarios del dominio, mas allá de que pertenezcan a un grupo o no, tendrán acceso a determinado contenido, por ejemplo los archivos adjuntos si es que conocen su URL.


## Active Directory

Los roles del sistema se corresponderán con grupos de Active Directory del dominio de la empresa, según el entorno:

### Diferenciación de entornos

Para el entorno de producción se utilizará el prefijo `CommonJobs_` para identificar los grupos de Active Directory. De forma similar, se identificará a los grupos de Active Directory de desarrollo y pruebas con el prefijo `CommonJobsDEV_`.

Esto permitirá mantener ambos ambientes en un mismo árbol de Active Directory, si es que esto fuera necesario, sin que uno impacte al otro.

### Grupos

Como extra, se ha creado en el entorno de desarrollo un grupo `CommonJobsDEV` que incluye al grupo de desarrollo de la aplicación.

A continuación se enumeran los grupos del sistema correspondientes a los roles descriptos anteriormente, obviando el prefijo del entorno como ha sido enumerado más arriba.

* Grupo `Users`, es sugerido que se le asigne a los encargados de RRHH y tal vez a una persona con rol técnico dentro de la empresa.
   * Asignado al grupo `CommonJobsDEV`, para que los desarrolladores puedan acceder al entorno de desarrollo.
   * Asignado al grupo `Users`, para que quienes utilizan el sistema en producción puedan hacer pruebas en el entorno de desarrollo.
* Grupo `ApplicantManagers`, es sugerido que se le asigne a responsables del área de RRHH del seguimiento de postulantes y su entrevistado para la posible incorporación a la empresa.
* Grupo `EmployeeManagers`, es sugerido que se le asigne a responsables del área de RRHH de mantener la información de los empleados actualizada.
* Grupo `JobSearchManagers`, es sugerido que se le asigne a responsables del área de RRHH de generar búsquedas de empleo y su seguimiento.
* Grupo `MenuManagers`, es sugerido que se le asigne a algún empleado administrativo que sea responsable del manejo de los pedidos de almuerzos y el seguimiento de los mismos.
* Grupo `Migrators`, es sugerido que se le asigne a quien realiza el despliegue de la aplicación y tal vez a un usuario del sistema o un desarrollador.
   * Ha sido asignado al grupo `CommonJobsDEV`, para que los desarrolladores puedan acceder a las migraciones del entorno de desarrollo.
   * Ha sido asignado al grupo `Migrators`, para que quienes realizan las migraciones en el entorno de producción puedan hacer pruebas en el entorno de desarrollo.
