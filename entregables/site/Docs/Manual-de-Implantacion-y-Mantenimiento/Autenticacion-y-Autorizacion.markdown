# Autenticación y Autorización

Actualmente el sistema identifica los siguientes roles:

* **Users**: Usuarios del sistema, con acceso a creación y edición de empleados y postulantes así como también consultas.
* **Migrators**: Usuarios con acceso a la UI de migración de datos entre versiones.
* **Applicant Managers**: Usuarios con acceso a la administración de los postulantes a empleos.
* **Employee Managers**: Usuarios con acceso a la administración de los empleados cargados en el sistema.
* **Job Search Managers**: Usuarios con acceso a la administración de búsquedas de empleos.
* **Menu Managers**: Usuarios con permiso de administración de MyMenu.
* **_Usuarios del dominio_**: Los usuarios del dominio, mas allá de que pertenezcan a un grupo o no, tendrán acceso a determinado contenido, por ejemplo los archivos adjuntos si es que conocen su URL.

## Relación entre usuarios y empleados

Los usuarios, en principio, no son entidades del sistema. CommonJobs delega en Active Directory o _Google Authentication_ la autenticación.

En algunos casos, es útil identificar cual es el empleado (entidad de dominio) relacionado con el usuario que está actualmente utilizando el sistema, por ejemplo en el sub-sistema de almuerzos. Esto se realiza seteando los campos `Usuario de Dominio` y `Email Corporativo` en la página de edición de empleados.

![Asociación entre usuarios y empleados](images/empoloyee-user-association.png)

De esta manera la asociación funcionará tanto para Active Directory o Google Authentication.

## Autenticación por Active Directory

_**Nota**: Originalmente el sistema estaba preparado para autenticar usuarios utilizando Active Directory, ya que era un requerimiento de seguridad del cliente. Circunstancialmente, hubo problemas con el servicio de Active Directory de la empresa, por lo que se requirió urgentemente implementar un sistema alternativo de autenticación. Se desactivó Active Directory y se comenzó a utilizar autenticación con Google Authentication y, como backup, autenticación con usuario y password almacenados en el sistema. No se descarta volver a utilizar Active Directory en un futuro._

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

## Autenticación por Google Authentication y Local

Si bien originalmente la utilización de Active Directory era un requerimiento importante, debido a un problema en la infraestructura del cliente, tuvimos que implementar de urgencia otro tipo de autenticación. Dado que el cliente utiliza _Google Apps_ y cada usuario tiene un mail de Google con el dominio de la empresa, se decidió utilizar _Google Authentication_. Alternativamente si debiera acceder al sistema un usuario externo, permitimos la autenticación por usuario y password almacenados por el sistema.

![Pantalla de autenticación actual](images/commonjobs-authentication.png)

Si el email del usuario autenticado mediante _Google Authentication_ pertenece al dominio de la compañía, se considera al usuario un _Usuario del dominio_. Si además se corresponde con el `Email Corporativo` de un empleado, se considera que usuario y empleado se corresponden entre si.

_Google Authentication_ solo se encarga de la autenticación. Para asociar los roles a los usuarios se utilizarán documentos en nuestra base de datos, relacionados con los nombres de usuario a través del id del documento.

![Documentos de Usuarios](images/user-documents.png)

Como se puede observar en la imagen superior, tanto el usuario `admin` como `abanderas` tienen los roles `Users`, `MenuManagers`, `EmployeeManagers`, `Migrators` y `JobSearchManagers`.

Dado que el usuario `admin` tiene definidos los campos `HashedPassword` y `PasswordSalt`, este usuario podría autenticarse en el sistema directamente, sin utilizar _Google Authentication_.