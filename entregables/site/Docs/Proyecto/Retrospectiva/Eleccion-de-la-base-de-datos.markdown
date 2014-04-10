# Elección de la base de datos (retrospectiva)

Casi dos años atrás escribimos cuando el proyecto comenzaba sobre nuestra [eleccion de la base datos, favoreciendo RavenDB](../2012-02-15-Detalles-Seleccion-Base-de-Datos). En el día de hoy, ya casi culminando el proyecto, presentaremos una reflexión sobre dicha elección.

## La perspectiva sobre RavenDB

Al momento de la investigación inicial, elegimos RavenDB como nuestro sistema de base de datos por las siguientes razones:

- Programado en .NET (simplificando interoperabilidad)
- Amplia variedad de APIs disponibles
- Comunidad activa (incluyendo al autor original)
- Curiosidad personal y oportunidad de aprendizaje

A la vez, reconocíamos las siguientes desventajas:

- Poca experiencia nuestra con bases de datos no relacionales
- Poca experiencia nuestra con RavenDB mismo

Desde este punto de vista, hacer un trabajo con RavenDB significaba aprender el paradigma de utilización de bases de datos no relaciones, y la forma correcta de utilizar RavenDB a la vez, con sus características particulares.

A nuestro favor, la comunidad era altamente activa y el mismo autor del producto, [Ayende Rahien](http://ayende.com/), no sólo es activo en la propia comunidad sino que procura que la gente que utilice esta base de datos lo haga de una forma acorde a las mejores prácticas de código genéricas. De hecho, en varias ocasiones, incluso no relacionadas con interacción en base de datos, nos hemos basado en sus recomendaciones de buenas prácticas, hasta el día de hoy.

## Bases de datos no relacionales

Nuestra experiencia previa se ha basado mayoritariamente en bases de datos relacionales, especialmente MySQL, MS SQL Server y PostgreSQL.

Esta experiencia significaba que para cada base de datos de este tipo, existía un conjunto de operaciones comunes que podrían realizarse con ellas (instrucciones SQL), e independientemente de cuál conociéramos de antemano, la mayor diferencia sería la administración pero no su uso.

En el caso de bases de datos relacionales, también poseen ciertas diferencias, pero la forma de interactuar con ellas varía consistentemente de una a otra, y más aún, la forma que los datos pueden adoptar es muy distinto según las capacidades del motor.

## Aprendiendo RavenDB

Nuestro comienzo con RavenDB fue sobre la interfaz de administración, que nos introdujo al concepto de documentos (análogo a "registros"), colecciones (análogo a "tablas"), índices (análogo a "vistas") y la estragia de map-reduce para el procesado de datos.

A la vez, varios ejemplos y tutoriales de internet nos ayudaron a comenzar nuestro camino:

- [Embedding RavenDB into a ASP.NET MVC 3 Application](http://msdn.microsoft.com/en-us/magazine/hh547101.aspx) - Justin Schwartzenberger (MSDN, Nov 2011)
- [RavenDB (I) - Empezamos...](http://geeks.ms/blogs/unai/archive/2011/12/01/ravendb-i-empezamos.aspx) - Unai Zorrilla Castro (MVP), 1 Dic 2011
- [RavenDB (II) - Los documents](http://geeks.ms/blogs/unai/archive/2011/12/05/ravendb-ii-los-documentos.aspx) - Unai Zorilla Castro (MVP), 5 Dic 2011
- [Orders Search in RavenDB](http://ayende.com/blog/152833/orders-search-in-ravendb) - Ayende Rahien, 9 Ene 2012
- [RavenDB Migrations: Rolling Updates](http://ayende.com/blog/66563/ravendb-migrations-rolling-updates) - Ayende Rahien, 26 Ago 2011
- [RavenDB: Includes](http://ayende.com/blog/4584/ravendb-includes) - Ayende Rahien, 12 Ago 2010
- [Today I played with RavenDB](http://openmymind.net/2011/10/17/Today-I-Played-With-RavenDB/) - Karl Seguin, 17 Oct 2011
- [When should you use RavenDB?](http://ayende.com/blog/136196/when-should-you-use-ravendb) - Ayende Rahien, 21 Nov 2011
- [When should you NOT use RavenDB?](http://ayende.com/blog/136197/when-should-you-not-use-ravendb) - Ayende Rahien, 22 Nov 2011
- [RavenDB documentation](http://ravendb.net/docs) - Hibernating Rhinos

Una última [prueba de concepto](https://github.com/andresmoschini/CommonJobs/tree/master/spikes/RavenPOC1) nos convenció de que era viable para nuestra solución.

## Mirando hacia atrás

En retrospectiva, creemos que RavenDB fue una buena elección.

Su característica de bases de datos no relacional nos permitió ser liberales con la estructura de los datos, manteniendo a la vez un significado semántico de los objetos que tratábamos, sin sacrificar la funcionalidad existente.

La construcción de índices con map-reduce nos permitió generar búsquedas en el sistema de una forma simple, que no deterioraban la performance ni generaban complejidad en el desarrollo.

La creación de índices automáticos basadado en los tipos de las colecciones nos permitió hacer búsquedas básicas sobre documentos sin necesidad de configuraciones previas.

La característica de versionamiento de documentos nos permitió adquirir la posibilidad de múltiples revisiones sin cambiar en absoluto la forma en la que el código trabaja. Fácilmente permitiría en el futuro agregar la capacidad de auditoría de cambios y regresión a versiones pasadas de los datos.

Su administrador gráficos nos permitió fácilmente trabajar con el proyecto en las primeras etapas y configurarlo correctamente para iniciar nuestro desarrollo de forma rápida.

Su característica de migraciones permitiría mejorar la forma en la que el sistema puede avanzar en etapas, estrategia que permitió agregar funcionalidad a un sistema que se encontraba funcionando en producción, siempre mantiendo los datos y las funcionalides ya existentes.

## Conclusión

Creemos que la experiencia de elección de RavenDB fue una muy enriquecedora, tanto para el proyecto por las capacidades del producto como para nosotros por los nuevos conocimientos adquiridos.

Consideramos que la investigación realizada fue hecha de la forma correcta, y si bien podría haber sido más extensiva y detallista, fue lo suficientemente profunda como para predecir cualquier riesgo que podríamos haber tenido.