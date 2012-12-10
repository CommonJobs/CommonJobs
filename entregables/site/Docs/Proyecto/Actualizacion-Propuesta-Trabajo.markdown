# Introducción 

## Propósito de este documento

Como observamos en el documento [Retrospectiva de proyecto](retrospectiva-proyecto#El_problema), la metodología ágil utilizada permitió al cliente ir descubriendo sus necesidades reales durante el desarrollo. 

Este documento actualiza la [Propuesta de trabajo](propuesta-trabajo) de los alumnos Andrés Moschini, Matías José y Juan Diego Raimondi, correspondiente al Proyecto Final de la Carrera Ingeniería en Informática de Universidad FASTA según lo consensuado entre el Director Funcional, el Director Técnico y el equipo siguiendo las recomendaciones de la Cátedra, como se explica en la [Retrospectiva de proyecto](Retrospectiva-Proyecto#C%c3%b3mo_seguir).

## Alcance de este documento

El presente documento está destinado a la Cátedra de Proyecto Final de la Facultad de Ingeniería de la Universidad FASTA, incluyendo al titular, asociados y auditores de la misma, al Director Técnico del proyecto Ing. Alejandro Fantini, al Director Funcional del proyecto Gabriel Buyatti, a los alumnos Andrés Moschini, Matías José y Juan Diego Raimondi, y al personal de _Making Sense Argentina_.  Describe los cambios en los requerimientos originales y establece las principales características que tendrá el sistema al momento de finalizar este proyecto final.

## Objetivo del proyecto

_**Cambios:** Se modificó el [Objetivo del proyecto](propuesta-trabajo#Objetivo_del_proyecto) para resaltar la administración de empleados además de las incorporaciones. Se incorporaron objetivos secundarios._

Brindar una herramienta para facilitar el trabajo del área de Recursos Humanos (RRHH) simplificando los procesos de carga, almacenamiento y lectura de la información manejada por dicha área e, indirectamente, optimizar sus resultados mejorando la calidad de las incorporaciones a la empresa y las búsquedas internas.

### Objetivos Secundarios

* Aprendizaje de tecnologías, metodologías y arquitecturas novedosas para los miembros el equipo, que estimamos nos serán útiles a futuro nuestra carrera profesional.
* Generación de una plataforma abierta que pueda ser útil para la empresa en futuros desarrollos internos.
* Generación de una plataforma que pueda implantarse fácilmente y comenzar a utilizarse como herramienta de la empresa sin interferir en los procesos actuales que esta tenga.

## Descripción del Proyecto

_**Cambios:** Se modificó la [Descripción del Proyecto](propuesta-trabajo#Descripci%c3%b3n_del_Proyecto) para resaltar el área de empleados además de la de postulantes. Se eliminaron menciones de funcionalidad que no será parte del proyecto._

El proyecto consiste en desarrollar e implantar un sistema informático que:

* Simplifique la administración y búsqueda en legajos de empleados.
* Realice el cálculo de vacaciones basada en la legislatura laboral argentina y el historial de ausencias/vacaciones de un empleado.
* Brinde información agregada y detallada sobre las licencias otorgadas a los empleados, para facilitar la toma de decisiones. 
* Administre los datos personales de postulantes, información de contacto, e historial de contactos con la empresa. 
* Facilite el seguimiento y la selección de candidatos a puestos de trabajo según sus capacidades y datos académicos.
* Simplifique otras tareas relacionadas con el área de recursos humanos, como permitir la carga de información por el propio postulante y la administración de los almuerzos diarios de la empresa.

## Relevamiento

_Ver [Relevamiento original](propuesta-trabajo#Relevamiento)._

### Cambios observados en el relevamiento original

* La denominación de la empresa cambio de _CommonSense_ a _Making Sense_. Aún así, el nombre propuesto del proyecto no será modificado.
* El proyecto de JIRA "Sales pipeline" para administrar las necesidades de nuevos profesionales según los potenciales proyectos quedó en desuso. El proceso de selección ahora se realiza de una manera más informal, en pos de la agilidad y el rápido cambio de la información disponible.
* El proyecto de JIRA "Recruit" quedó en desuso, se utiliza el e-mail y _LinkedIn_ para almacenar la información relacionada, ya no se ve la necesidad imperiosa de estandarizar el proceso (ver inciso anterior). Aún así, de forma lenta y gradual se está comenzando a utilizar CommonJobs para ello. (ver [nuevo objetivo sobre no interferir con los procesos de la empresa](actualizacion-propuesta-trabajo#Objetivos%20secundarios))
* Servicios como _ZonaJobs_, _Boomeran_ o _BuscoJobs_ ya no son tan utilizados, muchos de los postulantes son contactados por _LikedIn_ o por consultoras externas.
* Los términos y condiciones de la API _LinkedIn_ desalientan la integración con el servicio.
* En la empresa ya se busca inexorablemente sistematizar los procesos de selección o administración siempre que esto no les aporte un valor agregado. Por tanto, se permite un proceso más relajado en donde la información contenida en el sistema es suficiente para el uso del mismo, en lugar de extensiva y completa. La información incompleta, por tanto, no debería ser un obstáculo para poder continuar con dichos procesos.
* La administración de empleados era un proceso continuo sin repositorio central de información, y este llevaba a toda una serie de problemas de organización reflejados en ineficiencia o problemas por falta de comunicación. Se considera importante, entonces, que el proyecto ataque estos problemas.

## Actualización de las Características Mínimas (incluídas en el _límite del proyecto_)

_**Cambios:** Se modificaron las [Características Mínimas](propuesta-trabajo#Caracter%c3%adsticas_M%c3%adnimas_(inclu%c3%addas_en_el_%3cem%3el%c3%admite_del_proyecto%3c%2fem%3e)) quitando la migración de datos del proyecto JIRA "Recruit" ya que no es utilizado. Por otro lado, si bien se desarrollaron características relacionadas con las ofertas de trabajo o "búsquedas laborales", se quitaron de las características mínimas ya que no son las más prioritarias para el cliente. Se agregaron características relacionadas con administración de empleados._

* Reemplazar el actual legajo de empleados, almacenando los datos personales y datos laborales.
* Permitir búsquedas sobre los datos de empleados.
* Almacenamiento de documentación legal para los empleados.
* Realizar el cálculo de vacaciones.
* Administración de almuerzos de los empleados.
* Mantener información e histórico de los postulantes que hayan aplicado a la empresa.
* Registro de las entrevistas en la base de datos de postulantes
* Permitir búsquedas sobre los datos de postulantes.
* Seguridad para el acceso al sistema.

## Resumen de Características desarrolladas hasta ahora 

* Almacén de información de empleados.
   * Datos de contacto
   * Información para viajes
   * Datos bancarios
   * Beneficios brindados (obra social, almuerzo, etc.)
   * Información laboral (proyecto, seniority, carga horaria)
   * Ausencias
   * Vacaciones
   * Notas
   * Archivos adjuntos
* Almacén de información de postulantes.
   * Datos de contacto
   * Información académica
   * Habilidades técnicas con nivel estimado
   * Historial laboral
   * Notas genéricas y de entrevistas de recursos humanos y técnicas.
   * Archivos adjuntos
* Compartir información de postulantes para solo-lectura dentro de la empresa.
* Búsqueda full-text sobre datos relevantes de empleados y postulantes.
* Aprendizaje y sugerencia de datos que suelen repetirse (por ejemplo, universidad o título).
* Almacenamiento de archivos adjuntos y búsqueda full-text.
* Notificación de archivos requeridos para empleados.
* Cálculo de Vacaciones.
* Vista integral de datos comunes del staff de la empresa
* Seguridad para el acceso al sistema integrada con el sistema de seguridad de la empresa.
* Búsquedas laborales publicables para que el propio postulante cargue sus datos.

## Resumen del _Product Backlog_ hasta alcanzar el _límite del proyecto_

* Mostrar ausencias y vacaciones de forma gráfica.
* Definir las habilidades mínimas para búsqueda laboral.
* Permitir agregar y quitar tipos de entrevistas
* Indicación del flujo natural de los postulantes según entrevistas a las que asistieron
* Agregar habilidades no técnicas a empleados, postulantes y requerimientos mínimos de las búsquedas laborales.
* Sugerencia de postulantes según requerimientos de las búsquedas.
* Asociación entre postulantes y búsquedas laborales.
* Crear empleado a partir de postulante
* Permitir que el propio postulante complete sus habilidades y conocimientos
* Mejoras de UI y estilos

## Conformidad

En muestra de conformidad y aceptación de la actualización de la propuesta de trabajo, firman este documento Gabriel Buyatti (Director Funcional), Alejandro Fantini (Director Técnico) y por el equipo Matías José y Andrés Moschini.

