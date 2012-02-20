
# Selección Base de Datos

## Base de datos de documentos

Creemos que la utilización de una [base de datos de documentos] se adapta a las necesidades de negocio ya que la mayoría de nuestros objetos de negocio representarán estructuras complejas como _Postulantes_ o _Curriculumns_.

De esta manera los datos almacenados serán mucho mas parecidos a nuestros objetos de negocio permitiéndonos ahorrar mucho trabajo en mapeo objeto/relacional.

[base de datos de documentos]: http://en.wikipedia.org/wiki/Document-oriented_database
## RavenDB

[RavenDB] es una base de datos de documentos programada en .NET con APIs .NET, Silverlight, Javascript y REST. A diferencia de otros sistemas de este tipo, RavenDB permite transacciones y su API para .NET es totalmente _.NET friendly_.

Ya que no tenemos experiencia previa _real_ en el uso de esta base de datos y que la misma está en constante desarrollo, antes de decidirnos a utilizarla, realizamos una prueba de concepto tratando de replicar posibles escenarios que se presentarán en el sistema. Los problemas surgidos se pudieron resolver de una forma elegante, y cuando fue necesario la comunidad de RavenDB mediante la [lista de correo](lista de correo RavenDB) fue de gran ayuda y el autor principal respondió rápidamente en [su GitHub](GitHubAyende).

Estamos convencidos de que la utilización de esta herramienta nos permitirá tener un código más limpio y que favorecerá las buenas prácticas y un buen diseño en nuestro sistema.

[RavenDB]: http://ravendb.net/
[lista de correo RavenDB]: http://groups.google.com/group/ravendb
[GitHubAyende]: https://github.com/ayende/ravendb