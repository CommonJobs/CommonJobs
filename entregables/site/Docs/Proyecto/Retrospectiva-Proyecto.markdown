# Retrospectiva de Proyecto 

## El problema

Alrededor del `sprint 9` (cerca de la mitad del proyecto) detectamos que el sistema no se estaba utilizando y que muchas de las funciones que estábamos implementando no iban a ser utilizadas, al menos no en el corto plazo. En un esquema ágil como el que intentamos seguir, esto es un problema grave, ya que es importante que en cada sprint agreguemos valor al sistema. De ese periodo anterior se rescata la construcción de la estructura, el aprendizaje, y por supuesto algo de funcionalidad.

El área de RRHH también estaba (y está) organizándose y modificando sus métodos internos. Ahora mismo, su principal necesidad está relacionada con la administración del legajo e información de los empleados. 

La propuesta de proyecto inicial estaba enfocada en la selección de postulantes, actualmente el area de RRHH maneja eso con una relativa informalidad, sin seguir procesos rigurosos, pero con gran flexibilidad y eficiencia. Si bien desde el punto de vista organizacional, es importante generar un proceso, no podemos forzar la definición del mismo, ni imponerlo nosotros ya que en el sistema cae en desuso.


## Las acciones

Intentando adaptarnos a esta situación, se decidió enfocar las decisiones al corto plazo, con el nuevo sub-objetivo de hacer útil la plataforma, esperando que su uso generara necesidades y eventualmente se descubriera la funcionalidad real necesaria, sin chocar con los procesos informales realizados.

Las primeras acciones correctivas fueron revisar completamente el Product Backlog y repriorizar las tareas. El mismo dinamismo que decidimos aceptar, nos impidió planificar a largo plazo ya que las prioridades cambiaban de sprint a sprint: la línea de fin de proyecto se perdió en un Backlog poco definido.

Decidimos dedicar tiempo de los sprints a analizar y planificar las siguientes funciones a implementar en el corto plazo (~2 sprints).

Esto nos alejó de la funcionalidad original propuesta, pero permitió que el sistema comience a ser útil y sea utilizado.


## Como seguir

Alcanzamos el `sprint 13` (~2/3 partes del proyecto) y vimos la necesidad de prepararnos para finalizar el proyecto. 

Decidimos que debemos volver a definir una línea de fin de proyecto sólida, de manera de evitar sorpresas. Para ello evaluamos los distintos requerimientos necesarios para poder considerar el proyecto terminado y lo consideramos con el director técnico y con la cátedra.

Muchas de las recomendaciones de la cátedra y del director técnico recaían en procesos y validaciones de datos. Pero creemos, con seguridad, que estos serían impedimentos para el cumplimiento de los objetivos en lugar de ayuda a definir procesos. Nuestro plan es ofrecer una herramienta que pueda adaptarse a las actividades actuales, pero que sea adaptable al momento de definir procesos más estructurados.

**`//TODO: Consultar con la cátedra `**  Resolver como adaptarnos al cambio de rumbo que tomó el sistema respecto a la propuesta original.

Debemos resolver varios aspectos de terminación que el director funcional no considera importantes, pero hacen a la calidad del producto (ej. Crear un empleado y no guardarlo genera un empleado vacío). Para esto, prefijaremos tareas técnicas o terminaciones de UI con un espacio importante en los sprints.

Creemos que puede haber funcionalidad que el director funcional no considera importante para el día a día pero que puede ser valiosa para la cátedra y útil para la empresa (ej. Flujo de postulantes). Con asesoramiento de la cátedra y del director funcional seleccionaremos esa funcionalidad y la definiremos con ayuda de algún interesado que esté involucrado en cuanto a las funciones que son útiles para la empresa.

### Notas reunión director técnico:

**`//TODO: Cambiar redacción como minuta o extraer solo las conclusiones`**

#### Sobre las observaciones de cátedra:

Ampliar el valor agregado que tendrá el software con respecto a los otros productos de la competencia.

*Nota del equipo:* consideramos que la calidad de un producto y el valor agregado del mismo no se relaciona directamente con la cantidad de funcionalidad que tenga, sino con la forma en la que se adapte a los objetivos a cumplir y a las restricciones actuales.

#### Evaluación de Propuesta:

* Revisar las características deseadas y las mínimas, cree que algunas de las deseadas deberían incluirlas en las mínimas, por ej. Integración con JIRA.
* Por otro lado, ¿Cuantos finales le quedan a Matías José? Creo que al menos extendería la fecha límite del proyecto a Junio/Julio. +/- 3 meses. 
* La cátedra buscaba agregar condimentos interesantes al proyecto para que no sea un sistema de información simple. Se busca "valor agregado" en mejorar el proceso del cliente y ofrecer un producto más completo, no el mínimo necesario.
 
*Nota del equipo:* Según lo hablado con la Cátedra, la falta de finales podría extender la fecha de finalización del proyecto pero no requiere de desarrollo activo, sólo se suspendería hasta que esté todo listo para presentarlo. La funcionalidad a ser agregada está actualmente en discusión. (Ver arriba en sección de "Utilidad de las funciones implementadas")

* Al haber planteado un proyecto de desarrollo se esperará más en funcionalidad (producto), que en soporte y capacitación (servicio). Creo que está reducido en funcionalidad y podría ser criticada la dimensión del sistema.

*Nota del equipo:* Esta consideración es comprensible, pero creemos poder justificar estas razones. Ver notas arriba sobre valor agregado. Además, consideramos que deberíamos resaltar que una de las fortalezas indicadas en nuestra presentación de proyecto fue la inclusión de A. Moschini y J. Raimondi como miembros de la empresa para poder detectar este tipo de situaciones, lo que efectivamente ocurrió.

* Al contrastar el estado actual del proyecto con la propuesta original se detectó:
	* JIRA no es efectiva para carga, actualización y búsqueda de candidatos. (*Nota del equipo:* Consideramos estos objetivos cumplidos.)
	* Se buscaba que la selección de candidatos no dependa de memoria y arte de empleados de RRHH.  (*Nota del equipo:* Consideramos estos objetivos cumplidos.)
	* No hay integración con bases externas como LinkedIn, ZonaJobs, etc. (*Nota del equipo:* Consideramos estos objetivos cumplido, con notas a realizar. Esto dejó de ser una necesidad de RRHH y por tanto dejó de ser un problema a solucionar.)
	* Cree que es importante pensar en circuitos, para que haya un flujo natural de información entre pantallas.
	* Lo de almuerzo podría verse como que no es para RRHH. Quizá se pueda plantear por el lado de Beneficios? No estoy seguro que encaje muy bien con el resto del producto. Tengan en cuenta que de acuerdo a la cátedra: "Cada cambio en lo comprometido en la propuesta original o cada acuerdo que se alcance en el transcurso del trabajo será documentado y firmado por las partes."
 
*Nota del equipo:*	Consideramos que la funcionalidad de almuerzo no encaja con el plan de producto original (de acuerdo con las observaciones), pero encaja con el producto por las siguientes razones. Uno: Es una problemática actual del área de RRHH a solucionar (objetivo explícito a cumplir). Dos: Es una oportunidad de convertir al sistema en plataforma para la empresa y no un sistema localizado (objetivo explícito a cumplir). Tres: Estaban previstos que ocurrieran cambios y por eso se decidió este ciclo de vida de proyecto, el cual probó ser útil en repetidas ocasiones (ver arriba). Con el desarrollo ágil estamos dispuestos a adaptarnos a los cambios en lugar de ignorar este tipo de problemáticas a solucionar. Consideramos este cambio una buena oportunidad para el producto y para el cliente. No consideramos que se requieran cambios en la propuesta original.

* Es importante mejorar la estética y experiencia de usuario para causar una buena impresión a primera vista. 

* Antes de pedir fecha les recomiendo que asistan a ver otras presentaciones de proyecto, para estar más preparados. Pidan a otros compañeros documentación y memorias para contrastar con las propias.

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

*Nota del equipo:* No todas estas sugerencias concuerdan con el objetivo ni con la problemática a solucionar. Todas se consideraron y algunas de estas sugerencias serán incluidas porque realmente agregan utilidad sin impactar el real valor agregado del producto.

#### Problemas encontrados / oportunidades de mejora:

- BUG: Agregar nuevo postulante, no guardo pero queda grabado aunque no modifico ningún dato (dejo todo en blanco).
- IMPROVEMENT: Calendario debería tener una forma fácil de navegar entre años, porque se usa para fechas no muy recientes también, como el caso de experiencia previa.
- IMPROVEMENT: Creo que en experiencia previa debería estar separado el nombre del puesto del nombre de la empresa.
- IMPROVEMENT: no alerta que hay cambios sin grabar cuando se hace clic en un link que se abre en la misma página (ejemplo: clic en link de acceso junto a la estrella)
- BUG: Agrego Notas en un postulante sin llenar datos y grabo. Se graban aunque no tengan más datos que la fecha actual que se sugiere.
- BUG: Agrego Link de acceso, edito el texto y hago clic en el link y sale mensaje:
Sorry, an error occurred while processing your request. Si el link no existe hasta que se graba, todavía no debería verse como un link.
- BUG: URL de búsqueda es localhost en lugar de URL pública. (*Nota del equipo:* Eso solo ocurre en el sitio de demo en AppHarbor)
- IMPROVEMENT: Publicación de búsquedas en redes sociales. Que el que siga el link caiga en página para registrar sus datos como postulante, quede automáticamente asociado a esa búsqueda y se envíe notificación por mail a RRHH. 

*Nota del equipo:* No todas estas sugerencias concuerdan con el objetivo ni con la problemática a solucionar. Todas se consideraron y algunas de estas sugerencias serán incluídas porque realmente agregan utilidad sin impactar el real valor agregado del producto.

#### Cuestiones Técnicas *(no se llegó a hablar de todo)*:

Consideren a documentar:

- Cómo les está resultando RavenDB? valió la pena? En algún sentido entorpece el desarrollo o creen que avanzarían más rápido con SQL?
- Los nombres que se muestran en pantalla están asociados en el mismo modelo, puede ser?
- Pudieron identificar stories prioritarios en epics que no lo son? Eso de expandir la mayor cantidad de epics. Lo intentaron? Valió la pena?
- Documenten Deployment y requerimientos de hardware y software como parte de un Plan de Implantación. También soporte y migración de datos.
- Están usando RabbitMQ para algo?
- Log de errores?
- Certificado de seguridad?