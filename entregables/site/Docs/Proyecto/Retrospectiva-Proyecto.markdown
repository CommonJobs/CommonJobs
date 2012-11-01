# Retrospectiva de Proyecto 

*Este documento incluirá información acerca de los inconvenientes que estuvimos  detectando recientemente y de lo que estamos realizando para corregirlos para prepararnos para terminar el proyecto. También incluiría el resumen de la última entrevista con el director técnico y otros detalles relacionados.*

* Descubrimos que estábamos realizando cosas que no se iban a utilizar.
   * Que?
      * Se realizó una planificación con muchas tareas que no eran realmente útiles (nos sirvió para tener una estructura)
      * Luego se empezó a planificar a muy corto plazo y se hacían las cosas parecían de suma urgencia y al final del sprint ya no eran tan importantes.
      * Luego se descubrió que el principal problema con la implantación es que el área de RRHH es eficiente sin tener un proceso claramente definido.
   * Cuando lo detectamos?
      * Esto empezó a suceder aproximadamente en el sprint 9. 
   * Que decidimos hacer?
      * Revisar el backlog completo
      * Actualizar el backlog como tarea en los sprints (requerimientos y análisis)
      * Incentivar el uso real cuando antes.
      * No forzar restricciones innecesarias típicas de otros sistemas para mejor adaptación a los procesos que RRHH realmente requiere.

* Cuando pensamos en el fin de proyecto vimos que había cosas sin implementar.

   * Cátedra: 
      * Faltan procesos sobre los datos y generación de información más útil. 
   * Director Técnico: (ver Notas reunión director técnico)
      * Aspectos de terminación que el director funcional no considera importantes, pero hacen a la calidad del producto (ej. Crear un empleado y no guardarlo queda un empleado vacío).
      * Funcionalidad que el director funcional no considera importante, pero pueden ser importantes para la cátedra y (más importante todavía) para la empresa (ej. Flujo de postulantes)
   * Que decidimos: 
      * Prefijar tareas técnicas o terminaciones de UI que tengan un espacio importante en los sprints.
      * Buscar otro director funcional “complementario” para las funciones que son útiles para la empresa y no para el director funcional actual.
      * Para promover el uso real en la empresa y generar más interés en el proyecto se implementará una función que ayudará al área de RRHH, ya que depende de ella, pero no es directamente una función de RRHH: administración de menús diarios. Esto también ayudará a definir el sistema como plataforma dentro de la empresa  que era uno de los objetivos.




## Notas reunión director técnico:

### Observaciones de cátedra:


* Ampliar el valor agregado que tendrá el software con respecto a los otros productos de la competencia. - que un producto se adapte mejor no siempre quiere decir que "haga más", me parece que el que este producto sea mejor para MakingSense que otros no tiene que ver con la complejidad de sus procesos o la falta del mismo.

## Evaluación de Propuesta:

* Revisar las características deseadas y las mínimas, cree que algunas de las deseadas deberían incluirlas en las mínimas, por ej. Integración con JIRA.
* Por otro lado, ¿Cuantos finales le quedan a Matías José? Creo que al menos extendería la fecha límite del proyecto a Junio/Julio. +/- 3 meses. 
 
	*Según lo que hablamos con Ana, esto podría extender la fecha de finalización del proyecto pero no requiere de desarrollo activo, sólo se suspendería hasta que esté todo listo para presentarlo.*

### Observaciones director técnico: 

* Cree que la cátedra buscaba agregar condimentos interesantes al proyecto para que no sea un sistema de información simple. Se busca "valor agregado" en mejorar el proceso del cliente y ofrecer un producto más completo, no el mínimo necesario.
 
	*Creemos que el que sea un mejor producto no pasa por la cantidad de funcionalidad o la complejidad que tenga.*

* Al haber planteado un proyecto de desarrollo van a esperar más en funcionalidad (producto), que en soporte y capacitación (servicio). Creo que está reducido en funcionalidad y podrían criticarles la dimensión del sistema.

	*Se entiende por qué lo dice, pero eso lo hace más proyecto que desarrollo, nos parece que es algo muy valorable y que, de hecho, deberíamos resaltar (al fin y al cabo, una de las fortalezas que presentamos era la inclusión de Andrés y JD. en la empresa, lo que nos dio otra visión de las necesidades lo cual  se reflejó perfectamente.*

* Es muy probable que la cátedra revise el resultado del proyecto contrastándolo con la propuesta:

	* JIRA no es efectiva para carga, actualización y búsqueda de candidatos.

		*Se cumple.*

	* No hay integración con bases externas como LinkedIn, ZonaJobs, etc.

 		*Puede que tengamos un problema en este ítem. Lo estamos analizando.*

	* Se buscaba que la selección de candidatos no dependa de memoria y arte de empleados de RRHH.

		*se cumple.*

	* Cree que es importante pensar en circuitos, para que haya un flujo natural de información entre pantallas.

		*Estamos de acuerdo.*

	* Lo de almuerzo podría verse como que no es para RRHH. Quizá se pueda plantear por el lado de Beneficios? No estoy seguro que encaje muy bien con el resto del producto. Tengan en cuenta que de acuerdo a la cátedra:
"Cada cambio en lo comprometido en la propuesta original o cada acuerdo que se alcance en el transcurso del trabajo será documentado y firmado por las partes."
 
		*Estamos de acuerdo. Vamos a consultarlo con la cátedra.*

* En algún sentido la cátedra podría tener una visión superficial del producto y creo importante mejorar estética y experiencia de usuario para causar una buena impresión a primera vista. Antes de pedir fecha les recomiendo que asistan a ver otras presentaciones de proyecto, para estar más preparados. Pidan a otros compañeros documentación y memorias para contrastar con las propias.

#### Dudas que le surgieron con el uso del sistema:

- Estado civil y motivos de ausencias deberían ser los establecidos por ANSES? 
- Cómo es el flujo de búsqueda de personal hoy día, que no se está usando JIRA?
- Cómo se indica si un postulante califica para una búsqueda abierta?
- Estadísticas?
- Usuario Entrevistador?
- Cómo indico que un candidato aplicó a una cierta búsqueda?
- Cómo veo qué tan reciente es la informacion que tengo de un candidato?
- Cómo indico que un candidato ha sido contratado?
- Cómo indico el empleado que entrevistó a un empleado?
- No debería tener una tipo de entrevista de idioma?
- No debería registrar de alguna forma el output de la entrevista para que pueda procesarse y decidir el candidato?
- Cómo se "cierra" una búsqueda laboral?
- Cómo mido/analizo qué tan efectiva fue una búsqueda? 
- Cómo indico la baja de un empleado?
- Cómo indico el tipo/modalidad de contratación? 
- Cómo indico períodos discontinuos de contratación? No debería soportar varias relaciones laborales en lugar de tener Altas y Bajas como adjuntos?
- Cómo indico promociones/cambios de puesto?
- Cómo indico más de una universidad o varios títulos?
- Cómo indico cursos de un empleado?
- Qué pasa con el personal sin estudios universitarios? no debería indicarse el nivel de estudios? 
- Pedido de vacaciones va a hacerse en CommonJobs? o seguirá a través de JIRA? si es así, sería una buena integración.

#### Issues:

- BUG: Agregar nuevo postulante, no guardo pero queda grabado aunque no modifico ningún dato (dejo todo en blanco).
- IMPROVEMENT: Calendario debería tener una forma fácil de navegar entre años, porque se usa para fechas no muy recientes también, como el caso de experiencia previa.
- IMPROVEMENT: Creo que en experiencia previa debería estar separado el nombre del puesto del nombre de la empresa.
- IMPROVEMENT: no alerta que hay cambios sin grabar cuando se hace clic en un link que se abre en la misma página (ejemplo: clic en link de acceso junto a la estrella)
- BUG: Agrego Notas en un postulante sin llenar datos y grabo. Se graban aunque no tengan más datos que la fecha actual que se sugiere.
- BUG: Agrego Link de acceso, edito el texto y hago clic en el link y sale mensaje:
Sorry, an error occurred while processing your request. Si el link no existe hasta que se graba, todavía no debería verse como un link.
- BUG: URL de búsqueda es localhost en lugar de URL pública. -- ???
- IMPROVEMENT: Publicación de búsquedas en redes sociales. Que el que siga el link caiga en página para registrar sus datos como postulante, quede automáticamente asociado a esa búsqueda y se envíe notificación por mail a RRHH. 

#### Cuestiones Técnicas:

- Cómo les está resultando RavenDB? valió la pena? En algún sentido entorpece el desarrollo o creen que avanzarían más rápido con SQL?
- Los nombres que se muestran en pantalla están asociados en el mismo modelo, puede ser?
- Pudieron identificar stories prioritarios en epics que no lo son? Eso de expandir la mayor cantidad de epics. Lo intentaron? Valió la pena?
- Documenten Deployment y requerimientos de hardware y software como parte de un Plan de Implantación. También soporte y migración de datos.
- Están usando RabbitMQ para algo?
- Log de errores?
- Certificado de seguridad?
