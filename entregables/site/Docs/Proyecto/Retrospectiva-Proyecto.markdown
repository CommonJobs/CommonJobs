# Retrospectiva de Proyecto 

*Este documento incluirá información acerca de los inconvenientes que hemos detectado y sobre las acciones correctivas que hemos tomado. También incluirá el resumen de la última entrevista con el director técnico sobre cuestiones relativas a la terminación del proyecto.*

## Utilidad de las funcionalidades implementadas

A pesar de seguir las recomendaciones del Director Funcional y acordar al comienzo del proyecto un conjunto de funcionalidades mínimas, el plan actual que él fue contemplando conforme pasara el tiempo fue alejándose del plan original. Intentando corregir esta situación que impedía planificar a largo plazo, se decidió enfocar las decisiones a corto plazo, con el nuevo sub-objetivo de hacer útil la plataforma, esperando que su uso generara necesidades y eventualmente se descubriera la funcionalidad real necesaria.

Si bien esto ocurrió según lo deseado, se descubrió que el principal obstáculo a superar en la implantación del producto como plataforma no está relacionado a la funcionalidad que este pueda aportar, sino a la forma en la que define los procesos. Actualmente, el área de RRHH es altamente eficiente porque carece de procesos rigurosos a seguir. Si bien es deseable mejorar esto (desde un punto de vista organizacional), un sistema que impusiera procesos no existentes no sólo entorpecería la acción del cliente sino que eventualmente caería en el desuso por no adaptarse a sus necesidades.

Muchas de las recomendaciones de la cátedra y del director técnico recaían en procesos y validaciones de datos, pero creemos con seguridad que estos serían impedimentos para el cumplimiento de los objetivos en lugar de ayuda a definir procesos. Nuestro plan es ofrecer una herramienta que pueda adaptarse a las actividades actuales pero que sea adaptable al momento de definir procesos más estructurados.

Las primeras acciones correctivas fueron revisar completamente el Product Backlog y repriorizar las tareas. Esto probó ser útil en cuanto a la orientación del producto final al comienzo, pero por lo mencionado anteriormente, se volvió desafiante el continuar haciendo una planificación a largo plazo y entonces, aprovechando las ventajas de nuestro proceso ágil, decidimos enfocarnos en las tareas necesarias a un plazo más corto (uno o dos sprints).

### Resumen

   * Que?
      * Descubrimos que estábamos realizando tareas que no se iban a utilizar.
      * Se realizó una planificación con muchas tareas que no eran realmente útiles (pero nos sirvió para tener una estructura)
      * Luego se empezó a planificar a muy corto plazo y se implementaba funcionalidad que parecía de suma urgencia y al final del sprint ya no eran tan importantes.
      * Luego se descubrió que el principal problema con la implantación es que el área de RRHH es eficiente sin tener un proceso estructurado definido.
   * Cuando lo detectamos?
      * Esto empezó a suceder aproximadamente en el sprint 9. 
   * Que decidimos hacer?
      * Revisar el backlog completo
      * Actualizar el backlog como tarea en los sprints (requerimientos y análisis)
      * Incentivar el uso real cuando antes.
      * No forzar restricciones innecesarias típicas de otros sistemas para mejor adaptación a los procesos que RRHH realmente requiere.

## Preparación para el fin de proyecto

Dado que consideramos los principios principales del proyecto fueron cumplidos, y que nos estamos aproximando a los tiempos de proyecto pactados, procedimos a evaluar los distintos requerimientos necesarios para poder considerar el proyecto terminado. Tras considerarlo con el director técnico y con la cátedra, identificamos elementos que podrían mejorarse en el ámbito del proyecto:

   * Según la Cátedra: 
      * Faltan procesos sobre los datos y generación de información más útil. 
   * Según el Director Técnico: (ver Notas reunión director técnico)
      * Aspectos de terminación que el director funcional no considera importantes, pero hacen a la calidad del producto (ej. Crear un empleado y no guardarlo genera un empleado vacío).
      * Funcionalidad que el director funcional no considera importante, pero puede ser importante para la cátedra y para la empresa (ej. Flujo de postulantes)
	  
Por tanto, decidimos:

  * Prefijar tareas técnicas o terminaciones de UI que tengan un espacio importante en los sprints.
  * Buscar interesado que esté involucrado en cuanto a las funciones que son útiles para la empresa y no para el director funcional.

### Notas reunión director técnico:

#### Observaciones de cátedra:

Ampliar el valor agregado que tendrá el software con respecto a los otros productos de la competencia.

*Nota del equipo:* consideramos que la calidad de un producto y el valor agregado del mismo no se relaciona directamente con la cantidad de funcionalidad que tenga, sino con la forma en la que se adapte a los objetivos a cumplir y a las restricciones actuales.

#### Evaluación de Propuesta:

* Revisar las características deseadas y las mínimas, cree que algunas de las deseadas deberían incluirlas en las mínimas, por ej. Integración con JIRA.
* Por otro lado, ¿Cuantos finales le quedan a Matías José? Creo que al menos extendería la fecha límite del proyecto a Junio/Julio. +/- 3 meses. 
* La cátedra buscaba agregar condimentos interesantes al proyecto para que no sea un sistema de información simple. Se busca "valor agregado" en mejorar el proceso del cliente y ofrecer un producto más completo, no el mínimo necesario.
 
*Nota del equipo:* Según lo hablado con la Cátedra, la falta de finales podría extender la fecha de finalización del proyecto pero no requiere de desarrollo activo, sólo se suspendería hasta que esté todo listo para presentarlo. La funcionalidad a ser agregada está actualmente en discusión. (Ver arriba en sección de "Utilidad de las funciones implementadas")

* Al haber planteado un proyecto de desarrollo se esperará más en funcionalidad (producto), que en soporte y capacitación (servicio). Creo que está reducido en funcionalidad y podría ser criticada la dimensión del sistema.

*Nota del equipo:* Esta consideración es comprensible, pero creemos poder justificar estas razones. Ver notas arriba sobre valor agregado. Además, consideramos que deberíamos resaltar que una de las fortalezas indicadas en nuestra presentación de proyecto fueron la inclusión de A. Moschini y J. Raimondi como miembros de la empresa para poder detectar este tipo de situaciones, lo que efectivamente ocurrió.

* Es muy probable que la cátedra revise el resultado del proyecto contrastándolo con la propuesta:
	* JIRA no es efectiva para carga, actualización y búsqueda de candidatos. (*Nota del equipo:* Consideramos estos objetivos cumplidos.)
	* Se buscaba que la selección de candidatos no dependa de memoria y arte de empleados de RRHH.  (*Nota del equipo:* Consideramos estos objetivos cumplidos.)
	* No hay integración con bases externas como LinkedIn, ZonaJobs, etc. (*Nota del equipo:* Consideramos estos objetivos cumplido, con notas a realizar. Esto dejó de ser una necesidad de RRHH y por tanto dejó de ser un problema a solucionar.)
	* Cree que es importante pensar en circuitos, para que haya un flujo natural de información entre pantallas.
	* Lo de almuerzo podría verse como que no es para RRHH. Quizá se pueda plantear por el lado de Beneficios? No estoy seguro que encaje muy bien con el resto del producto. Tengan en cuenta que de acuerdo a la cátedra: "Cada cambio en lo comprometido en la propuesta original o cada acuerdo que se alcance en el transcurso del trabajo será documentado y firmado por las partes."
 
*Nota del equipo:*	Consideramos que la funcionalidad de almuerzo no encaja con el plan de producto original (de acuerdo con las observaciones), pero encaja con el producto por las siguientes razones. Uno: Es una problemática actual del área de RRHH a solucionar (objetivo explícito a cumplir). Dos: Es una oportunidad de convertir al sistema en plataforma para la empresa y no un sistema localizado (objetivo explícito a cumplir). Tres: Estaban previstos que ocurrieran cambios y por eso se decidió este ciclo de vida de proyecto, el cual probó ser útil en repetidas ocasiones (ver arriba). Con el desarrollo ágil estamos dispuestos a adaptarnos a los cambios en lugar de ignorar este tipo de problemáticas a solucionar. Consideramos este cambio una buena oportunidad para el producto y para el cliente. No consideramos que se requieran cambios en la propuesta original.

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

*Nota del equipo:* No todas estas sugerencias concuerdan con el objetivo ni con la problemática a solucionar. Todas se consideraron y algunas de estas sugerencias serán incluídas porque realmente agregan utilidad sin impactar el real valor agregado del producto.

#### Problemas encontrados / oportunidades de mejora:

- BUG: Agregar nuevo postulante, no guardo pero queda grabado aunque no modifico ningún dato (dejo todo en blanco).
- IMPROVEMENT: Calendario debería tener una forma fácil de navegar entre años, porque se usa para fechas no muy recientes también, como el caso de experiencia previa.
- IMPROVEMENT: Creo que en experiencia previa debería estar separado el nombre del puesto del nombre de la empresa.
- IMPROVEMENT: no alerta que hay cambios sin grabar cuando se hace clic en un link que se abre en la misma página (ejemplo: clic en link de acceso junto a la estrella)
- BUG: Agrego Notas en un postulante sin llenar datos y grabo. Se graban aunque no tengan más datos que la fecha actual que se sugiere.
- BUG: Agrego Link de acceso, edito el texto y hago clic en el link y sale mensaje:
Sorry, an error occurred while processing your request. Si el link no existe hasta que se graba, todavía no debería verse como un link.
- BUG: URL de búsqueda es localhost en lugar de URL pública. -- ???
- IMPROVEMENT: Publicación de búsquedas en redes sociales. Que el que siga el link caiga en página para registrar sus datos como postulante, quede automáticamente asociado a esa búsqueda y se envíe notificación por mail a RRHH. 

*Nota del equipo:* No todas estas sugerencias concuerdan con el objetivo ni con la problemática a solucionar. Todas se consideraron y algunas de estas sugerencias serán incluídas porque realmente agregan utilidad sin impactar el real valor agregado del producto.

#### Cuestiones Técnicas:

Consideren a documentar:

- Cómo les está resultando RavenDB? valió la pena? En algún sentido entorpece el desarrollo o creen que avanzarían más rápido con SQL?
- Los nombres que se muestran en pantalla están asociados en el mismo modelo, puede ser?
- Pudieron identificar stories prioritarios en epics que no lo son? Eso de expandir la mayor cantidad de epics. Lo intentaron? Valió la pena?
- Documenten Deployment y requerimientos de hardware y software como parte de un Plan de Implantación. También soporte y migración de datos.
- Están usando RabbitMQ para algo?
- Log de errores?
- Certificado de seguridad?