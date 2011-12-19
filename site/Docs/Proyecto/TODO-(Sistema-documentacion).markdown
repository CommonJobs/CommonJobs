#NuGet

* Reemplazar todas las referencias a NuGet por CommonJobs, 
* Revisar la licencia

# Jerarquía

Hay que modificar el sistema para permitir de forma opcional 3 niveles, quedaría algo así:

* __Scrum__ _(Carpeta)_
  * Product Backlog _(archivo .markdown)_
  * __Sprint 1__ _(Carpeta)_
     * Planificación Sprint 1 _(archivo .markdown)_
	 * Revisión del Sprint 1 _(archivo .markdown)_
	 * Mejoras y Oportunidades _(archivo .markdown)_
	 * Resumen de horas _(archivo .markdown)_
	 * Elección de sistema de base de datos _(archivo .markdown)_
	 * Sistema de documentación elegido _(archivo .markdown)_
* __Entregas__ _(Carpeta)_
  * __2011.12.16 Presentación del proyecto__ _(Carpeta)_
	 * Presentación del proyecto _(archivo .markdown)_
	 * Otros archivos entregados _(archivo .markdown con link al .pdf entregado)_
  * __2011.12.16 Propuesta de Trabajo__ _(Carpeta)_
     * Propuesta Trabajo _(archivo .markdown)_
	 * Otros archivos entregados _(archivo .markdown con link al .pdf entregado)_
  * __2012.03.17 Entrega 1__ _(Carpeta)_
     * Product Backlog _(archivo .markdown)_
     * Planificación Sprint 1 _(archivo .markdown)_
	 * Revisión del Sprint 1 _(archivo .markdown)_
	 * Mejoras y Oportunidades _(archivo .markdown)_
	 * Resumen de horas _(archivo .markdown)_
	 * Elección de sistema de base de datos _(archivo .markdown)_
	 * Sistema de documentación elegido _(archivo .markdown)_

Aunque creo que lo mejos es dejar 2 niveles y que para las carpetas del 3er nivel se genere automáticamente algo como lo que hice en entregas:

![images](images/IndicesEntregas.png)
