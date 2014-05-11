# Clases de dominio

Si bien el diseño de la aplicación en cuanto a su representación del dominio ha resultado en clases anémicas (es decir, clases que no contienen mucho comportamiento, sino que se comportan más cercanas a DTOs), este no es faltante. A continuación se detallarán las clases de dominio, su relación entre ellas y los conceptos que ellas representan en el dominio particular de esta aplicación.

## Diagrama de clases

A continuación puede verse un diagrama de clases completo, que será seccionado en partes para su mejor lectura y comprensión. El diagrama completo en su versión digital servirá como mapa conceptual de todas las entidades que forman parte del dominio del sistema.

![Diagrama completo de clases de dominio](Images/FullDomain.png)

Podemos ver en el diagrama que dos secciones particulares destacan por encontrarse separadas del resto. Una de ellas es _MyMenu_, vacaciones y ofertas de trabajo. Estas dos destacan de las demás por tener una cierta cantidad de clases que son únicas para el desarrollo de esa sección, mientras que las demás se encuentran íntimamente ligadas con varias partes del sistema, haciendo a veces difícil su distinción.

Puede observarse también que muchas clases no tienen relaciones directas indicadas en el diagrama con clases que deberían tener. Esto es porque las relaciones se han construido alrededor de la interacción con la persistencia y no con el diseño del dominio como parte central del sistema. Incluirlas en el diagrama incluiría relaciones que expondrían más clases que poco tienen que ver con el dominio de la aplicación. Por eso se ha elegido excluirlas del diagrama.

## Secciones funcionales y sus clases de dominio

A continuación se expondrá la explicación por secciones funcionales de la aplicación y se verán enumeradas las clases relevantes.

### Búsquedas de trabajo

![Diagrama de dominio: Búsquedas de trabajo](Images/DomainSection-JobSearch.png)

Clases involucradas:

- `TechnicalSkill`: Habilidad técnica, conocimiento específico sobre alguna técnica o herramienta relacionada al oficio técnico de desarrollo de software. Se incluyen su nombre, su nivel, su ponderación (importancia) y atributos que la hacen fácilmente buscable en otras secciones del sistema.
- `TechnicalSkillLevel`: Nivel de habilidad técnica. Medición subjetiva de la eficiencia de utilización de dicha habilidad en el oficio. Puede ser desconocido, básico, intermedio o avanzado.
- `JobSearch`: Búsqueda de trabajo. Representación de una vacante y oferta abierta al mercado. Incluye un título, notas a mostrar públicamente, notas internas del departamento de RRHH, habilidades técnicas relevantes a la búsqueda, su estado de publicación y un código amigable para compartir la búsqueda en redes sociales.
- `Postulation`: Postulación. Ofrecimiento de un postulante a ser considerado para esa búsqueda de trabajo. Incluye datos básicos del postulante (nombre, apellido, email, teléfono, perfil de LinkedIn), el Curriculum Vitae del postulante un comentario personal, y un listado de habilidades técnicas que el postulante considera posee que son útiles para el puesto.
- `TemporalFileReference`: Referencia temporal a archivo. Utilizado internamente por el sistema para guardar el Curriculum Vitae del postulante y mantener el nombre original del archivo evitando colisiones con otros archivos que posiblemente tengan el mismo nombre.

La creación de una búsqueda de trabajo generará una instancia de `JobSearch`,  que incluirá una serie de `TechnicalSkills` con distintos niveles de ponderación según su nivel de importancia para esa búsqueda particular. Tras completar un texto de formato arbitrario que se puede mostrar públicamente con la búsqueda y opcionalmente algunas notas internas que el departamento de RRHH deba tener en mente respecto de la búsqueda, se puede decidir el código de compartimiento de la búsqueda (para permitir URLs amigables en las redes sociales) y una vez establecido el estado de la búsqueda como pública, dicha búsqueda puede ser accedida públicamente desde el sitio público desarrollado para este propósito.

Una vez que los postulantes proveen su información en este sitio público, se instanciarán objetos de la clase `Postulation` y `TemporalFileReference` para almancenar la información provista sobre ellos como candidatos a la búsqueda. También incluirá una serie de `TechnicalSkills` que internamente el sistema utilizará para sugerir qué candidatos son los mejores capacitados para las necesidades de esta búsqueda particular entre todo el banco de postulantes disponible.

### MyMenu

![Diagrama de dominio: MyMenu](Images/DomainSection-MyMenu.png)

Clases involucradas:

- `EmployeeMenu`: Menú de empleado. Elecciones de comidas que el empleado ha elegido desde las opciones disponibles a lo largo del tiempo que los distintos menús se encuentran disponibles. Incluye referencias al empleado, la oficina por defecto en la que el empleado almorzará, las opciones por defecto que el empleado ha seleccionado y las elecciones excepcionales que ha realizado para fechas particulares.
- `EmployeeMenuItem`: Item de menú de empleado. Cada una de las selecciones de comida que el empleado ha elegido. Tiene referencias al número de semana y número de día que ese menú representa, también como la opción seleccionada y el lugar en donde el empleado ha elegido tener su comida.
- `EmployeeMenuOverrideItem`: Item de excepción de menú de empleado. Cada una de las excepciones que el empleado ha registrado a sus reglas registradas como `EmployeeMenuItem`. Esta, a diferencia de la anterior, mantiene una referencia a una fecha en particular, como también la opción seleccionada, el lugar seleccionado y la opción de cancelarlo, que tomarán precedencia ante la opción por defecto del usuario. También incluye un campo para un comentario personal.
- `IWeekDayKeyed`: Indexado por día y semana. Interfaz utilizada para clases que deben contener una referencia al índice del día y de la semana del menú para ser indexada a través de estos atributos.
- `IWeekDayOptionKeyed`: Indexado por día, semana y opción. Interfaz utilizada para clases que deben contener una referencia al índice del día, de la semana y a la opción de menú seleccionada para ser indexada a través de esos atributos.
- `MenuOrder`: Orden de menú. Clase que representa un pedido de menú para todos los empleados de la empresa. Incluye la fecha del pedido, referencias al índice de la semana y día que ese pedido representa, descripción de los distintos lugares a los que se deben hacer los pedidos (en donde se consumirán), las distintas elecciones de los empleados, resumen de pedidos por opción por lugar, un detalle de elecciones por cada empleado y la indicación de si el pedido se ha efectuado.
- `MenuOrderDetailItem`: Item de detalle de la orden de menú. Cada uno de las elecciones efectuadas para un pedido particular. Incluye referencias al lugar en donde se ha elegido, la opción elegida, el nombre del empleado que realizó la elección, y un comentario personal provisto por ese empleado.
- `Menu`: Menú. Conjunto de opciones disponibles para los empleados a lo largo de un período de tiempo. Incluye un índice por semanas de opciones disponibles, las distintas opciones en cada semana y los lugares en los cuales se pueden pedir dichas opciones. También incluye las fechas de comienzo y fin del período en el cual este menú estará disponible, el momento en el día hasta el cual se pueden hacer cambios de elecciones y detalles de las comidas disponibles en cada opción.
- `MenuItem`: Item de menú. Cada una de las comidas disponibles por opción, por semana y día.
- `Option`: Opción. Cada una de las opciones disponibles en el menú.
- `Place`: Lugar. Cada uno de los lugares en los cuales se puede pedir una de las opciones de menú.
- `IKeyText`: Clave textual. Clase que el sistema utiliza para indexar objetos que dependen de una clave de texto.

Los administradores de MyMenu generarán primeramente una serie de `Menu`s, una serie de opciones que se podrá proveer como comida a lo largo del tiempo. Idealmente, sólo se encuentra activo un menú a la vez. Este menú dispone de varias opciones (`Option`, por ejemplo: opción calórica, opción vegetariana, etc.). El menú cicla una serie de comidas durante una determinada cantidad de semanas, tras el cual se repiten las opciones disponibles. Cada comida particular se identifica, dentro del menú, según el índice de la semana, el índice del día y la opción seleccionada. Como ejemplo, en un determinado menú, en la primer semana y el primer día, la opción calórica puede ser un puré de papas, mientras que la misma opción al día siguiente sea una porción de tarta. Si el menú se repite cada 3 semanas, el primer día de la cuarta semana tendrá las mismas comidas en las mismas opciones que el primer día de la primera semana.

Tras la creación del menú, los empleados procederán a hacer sus elecciones por defecto durante la duración del menú. Cada una de esas elecciones se convertirá en una instancia de `EmployeeMenu`, en donde sus `WeeklyChoices indican las elecciones de opciones en cada semana del menú. A la vez, cada una de sus `Overrides` (`EmployeeMenuOverrideItem`) son las excepciones a la regla de las elecciones. Por ejemplo, si una opción no se encuentra disponible un día particular (y para el cuál no es meritorio alterar el menú completo), o si un empleado estará ausente una fecha particular y prefiere cancelar su menú. Para estas situaciones la exepción se encuentra relacionada con una fecha particular e incluye también un comentario personal sobre la razón que haya impulsado el cambio, de ser necesario.

Cada empleado puede pedir su menú en una oficina particular, para las cuales se encuentran las opciones de lugares, según las distintas localizaciones que MakingSense disponible para que sus empleados almuercen. A la vez, las excepciones pueden indicar sólo cambio de lugares, dado que un empleado podría estar visitando otra de las oficinas y decidiría almorzar ahí.

### Empleados

![Diagrama de dominio: Empleados](Images/DomainSection-Employee.png)

Clases involucradas:

- `Person`: Persona. Clase base para entidades representado personas en el sistema, como empleados, y postulantes a empleos. Contiene información básica de contacto y de habilidades profesionales de esa persona.
- `MaritalStatus`: Estado civil.
- `ImageAttachment`: Imagen adjunta. Utilizada para avatares.
- `Attachment`: Adjunto. Utilizado para archivos generales formando parte del perfil de la persona.
- `SlotWithAttachment`: Espacio con adjunto. Representa un lugar en donde debería haber un adjunto ocupado por uno.
- `AttachmentSlot`: Espacio para adjuntos. Representa un lugar en donde debería haber un adjunto.
- `Employee`: Empleado. Clase con campos particulares para almacenar la información propia de la relación con la empresa y la situación de trabajo particular de este empleado.
- `Absence`: Ausencia. Representa una falta al horario normal de trabajo.
- `AttachmentReference`: Referencia a adjunto. Clase que permite obtener información del archivo adjunto en el sistema de archivos.
- `AttachmentSlotNecessity`: Necesidad de adjunto en espacios. Se refiere al nivel de la necesidad de que dicho espacio sea rellenado con un adjunto para considerar el perfil del empleado completo. Puede ser opcional, deseable y requerido.
- `Certification`: Certificación. Descripción de las certificaciones profesionales que el empleado posee.
- `IEventWithAttachment`: Evento con adjunto. Interfaz para clases que representan eventos que ocurren y que pueden tener un adjunto como parte de ellos.
- `NoteWithAttachment`: Nota con adjunto. Nota registrada para el perfil del empleado con un adjunto en ella.
- `AbsenceType`: Tipo de ausencia. Clasificación de la ausencia que cometió el empleado.
- `AbsenceReason`: Razón de ausencia. Explicación de las causas que generaron la ausencia del empleado.
- `SimpleNote`: Nota simple. Nota registrada para el perfil del empleado.
- `SalaryChange`: Cambio salarial. Evento que indica un cambio en la remuneración dada al empleado.
- `IEvent`: Evento. Interfaz para clases que representan cambios en el estado del perfil con un punto en el tiempo de registro del evento, opuesto al tiempo en el que se registra el evento en el sistema.

Al momento de creación de un empleado, una instancia de la clase `Employee` se persistirá, generalizada en la clase `Person` que también es utilizada para los postulantes a puestos de trabajo. Employee contiene la información necesaria para identificar el perfil del empleado y sus responsabilidades dentro de la empresa, a la vez de las capacidades que esta persona tiene.

El perfil de empleado permite registrar adjuntos como información extra o documentación del empleado, que pueden ser hechos en espacios o `Slot`s. Estos slots son espacios predefinidos de información que deben ser llenados para el perfil de un empleado, como pueden ser el Curriculum Vitae, el contrato laboral, o el documento de alta. Cada uno de estos espacios tiene distintos niveles de requerimientos, representados por `AttachmentSlotNecessity`. Algunos serán solo opcionales para complementar la información del empleado, mientras otros serán realmente requeridos para poder considerar que la información legal del empleado se encuentra completa.

El sistema almacena los cambios salariales que dicho empleado ha tenido a lo largo de su trabajo en la empresa, lo que permite calcular su remuneración inicial como su remuneración actual y ver los cambios intermedios. Estos cambios se clasifican como eventos (`IEvent`) que pueden ser registrados en una fecha en el sistema mientras el evento ocurrió en otra fecha fuera del sistema.

De forma complementaria, se permite el agregado de notas como eventos, que pueden utilizarse para mantener un registro de cambios o conversaciones con el empleado respecto a su relación con la empresa y su puesto laboral. Por ejemplo, reuniones de performance, cambios salariales acordados o cambios de puesto pueden indicarse aquí. Opcionalmente puede registrarse un adjunto con la nota que complemente la información escrita en ella, lo que determinará el uso de una clase `SimpleNote` o `NoteWithAttachment`.

### Postulantes

![Diagrama de dominio: Postulantes](Images/DomainSection-Applicant.png)

Clases involucradas:

- `MaritalStatus`: Estado civil.
- `EventType`: Tipo de evento. Clasificación del evento ocurrido, para búsquedas.
- `ApplicantEventType`: Tipo de evento de postulante. Clasificación del evento ocurrido en el contexto de postulantes, para búsquedas. 
- `Person`: Persona. Clase base para entidades representado personas en el sistema, como empleados, y postulantes a empleos. Contiene información básica de contacto y de habilidades profesionales de esa persona.
- `Attachment`: Adjunto. Utilizado para archivos generales formando parte del perfil de la persona.
- `SharedLink`: Link compartido. Utilizado para proveer acceso a usuarios ajenos al sistema a una entidad particular.
- `SharedLinkList`: Lista de links compartidos. Utilizado para manejar los accesos disponibles a una entidad particular para usuarios ajenos al sistema.
- `SlotWithAttachment`: Espacio con adjunto. Representa un lugar en donde debería haber un adjunto ocupado por uno.
- `ImageAttachment`: Imagen adjunta. Utilizada para avatares.
- `Applicant`: Postulante. Representa a un postulante a un puesto de trabajo, y esta clase posee información sobre su experiencia pasada y habilidades.
- `IShareableEntity`: Entidad compartible. Interfaz utilizada para clases que permiten ser compartidas para usuarios ajenos al sistema.
- `CompanyHistory`: Historia empresarial. Representa la experiencia pasada o actual de un postulante en otras empresas.
- `SimpleNote`: Nota simple. Nota registrada para el perfil del postulante.
- `AttachmentSlotNecessity`: Necesidad de adjunto en espacios. Se refiere al nivel de la necesidad de que dicho espacio sea rellenado con un adjunto para considerar el perfil del postulante completo. Puede ser opcional, deseable y requerido.
- `AttachmentReference`: Referencia a adjunto. Clase que permite obtener información del archivo adjunto en el sistema de archivos.
- `IEvent`: Evento. Interfaz para clases que representan cambios en el estado del perfil con un punto en el tiempo de registro del evento, opuesto al tiempo en el que se registra el evento en el sistema.
- `IEventWithAttachment`: Evento con adjunto. Interfaz para clases que representan eventos que ocurren y que pueden tener un adjunto como parte de ellos.
- `NoteWithAttachment`: Nota con adjunto. Nota registrada para el perfil del postulante con un adjunto en ella.
- `AttachmentSlot`: Espacio para adjuntos. Representa un lugar en donde debería haber un adjunto.

Al momento de creación de un postulante en el sistema, una instancia de la clase `Applicant` será creada, heredando muchas de las propiedades de `Person` y agregando varias otras específicas para identificar la aptitud del postulante para ser parte de la empresa. Estas incluyen habilidades técnicas, historia y experiencia en otras empresas (`CompanyHistory`) e información adicional en formato de adjuntos que pueden ser, al igual que los empleados, organizados en espacios o slots.

Se pueden registrar notas (`SimpleNote`) que registran parte de la comunicación o acuerdos con el postulante, como también agregales adjuntos que complementan esa información (`NoteWithAttachment`). Cada uno de esos cambios es registrado como un evento, que referneciará un `EventType`. Este tipo de evento puede marcar hitos en la fase de adquisición de postulantes, como entrevistas técnicas o de idioma extranjero, lo cual también facilitará las búsquedas pudiendo ser filtrados según aquellos que hayan llegado a estos hitos particulares.

Los postulantes son una entidad compartible (`IShareableEntity`), lo que significa que pueden tener un conjunto de links (`SharedLinkList`) que definirá para cada link (`SharedLink`): el código de compartimiento (url-friendly), un nombre de identificación del link y una fecha de expiración para ese link. Esto permite compartir el link para un postulante particular a usuarios que no posean acceso al sistema y darles un acceso temporal a él. Por ejemplo, para compartir con coordinadores de proyectos la información del postulante y sin ofrecer acceso directo a CommonJobs, un link compartido puede ser creado con la URL fácil de acceder "/JuanPerez" y dar un acceso límite hasta el día siguiente. Cualquier persona que utilice ese link puede acceder al perfil del empleado y modificarlo sin necesidad de identificarse en el sistema, por lo que los links expiran por defecto en la fecha de expiración indicada al momento de ser creados.

### Vacaciones

![Diagrama de dominio: Vacaciones](Images/DomainSection-Vacations.png)

Clases involucradas:

- `Vacation`: Vacación. Período de ausecia programada por el empleado que se descuenta de sus días libres por ley.
- `VacationReportData`: Datos de reporte de vacaciones.
- `VacationPeriodReport`: Reporte de vacaciones por período.
- `VacationsReport`: Reporte de vacaciones.
- `VacationsReportConfiguration`: Configuración de reporte de vacaciones.

Las vacaciones son registradas directamente contra el perfil de cada uno de los empleados, como instancias de `Vacation`, que permite identificar cuántos días fueron descontados de la cantidad de días disponibles para un período particular. Por lo general los períodos corresponderán a los años, siendo para cada año disponible una cantidad de días particular dependiendo de la antigüedad del empleado.

Los reportes de vacaciones comienzan desde la configuración que la generará (`VacationsReportConfiguration`). Estos indicarán cuál es el año actual (para poder cambiarlo y planificar a futuro o ver reportes de vacaciones en el pasado), y cuántos años de detalle se necesita, obteniendo el resto de los años sumarizados.

Esta información se verá en `VacationsReport`, una clase que sumarizará para cada empleado el estado de las vacaciones y los días libre de los que dispone esa persona. Incluye la cantidad total disponibles, utilizados y permitidos para este empleado, un reporte de los mismos por período y una especificación de cuántos días se han dado por adelantado.