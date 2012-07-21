# Historial de cambios de las entidades

Con el objetivo de no perder información valiosa y llevar un registro de los cambios de las entidades utilizaremos una característica provista como [paquete opcional](http://ravendb.net/docs/server/bundles) del servidor de base de datos RavenDB.

[Versioning Bundle](http://ravendb.net/docs/server/bundles/versioning) almacenará nuevos documentos (`revisions`) para cada versión de los `Applicants` y `Employees` de manera transparente sin interferir en el desarrollo normal del sistema. Aún no aprovecharemos los datos almacenados, pero estarán disponibles para cuando querramos sacar provecho de ellos.

También nos permitirá acceder a esas versiones mediante _Raven Studio_ en caso de eliminarse la entidad o modificarse algún dato importante accidentalmente.

![ ](Images/versioning.jpg)

## Metadata

Cada vez que una entidad es almacenada en Raven DB se registra la fecha del servidor en el campo `Last-Modified` de su _metadata_, _CommonJobs_ además intentará determinar quien es el usuario actual y almacenará su `user name` en el campo `Last-Modified-By`.




