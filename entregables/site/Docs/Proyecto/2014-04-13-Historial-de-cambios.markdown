# Historial de cambios

Este documento resume los cambios realizados durante el proyecto. La misma información puede verse de forma detallada en cada uno de los documentos de cierre y planificación de sprint, en la carpeta de Scrum del proyecto.

A continuación se detallan los avances logrados al momento de finalización de cada uno de los sprints.

## Sprint 0
(Terminado 26 de Enero 2012)

- Seleccionada herramienta de versionamiento de código (git - GitHub)
- Establecimiento y desglosamiento de un product backlog
- Seleccionada herramienta de gestión de tareas (Trello)
- Seleccionada motor de bases de datos a utilizar (RavenDB)
- Definición de una estrategia técnica y de arquitectura
- Selección de la herramienta de generación de documentación (markdown, mmd2pdf)

## Sprint 1
(Terminado 16 de Febrero de 2012)

- Instlado JIRA para gestión de product backlog
- Refinamiento del product backlog
- Documentación de métodos de generación de documentación
- Nueva funcionalidad: Legajo de empleados

## Sprint 2
(Terminado 7 de Marzo de 2012)

- Documentado por qué JIRA no fue apto para nuestro proceso
- Refinamiento de product backlog
- Entrega de documentación académica
- Instalación del sistema en entorno de producción
- Nueva funcionalidad
    - Postulantes
        - Ingreso de datos de postulantes
        - Búsqueda de postulantes
        - Registrar entrevistas a postulantes
        - Adjuntar CVs de postulantes
    - Empleados
        - Modo de edición para todos los campos
- Resueltos bugs
    - Uso de algunos controles rompen la aplicación
    - Mejoras de usabilidad en modo de edición
    - Mejoras de usabilidad para controles de fecha
    - Mejoras de usabilidad para controles numéricos

## Sprint 3
(Terminado 28 de Marzo de 2012)

- Documentación de Sprint 2 y comienzo de Sprint 3
- Documentación filtros IFilter
- Nueva funcionalidad
    - Identificación de usuarios
    - Búsquedas en contenido de archivos de postulantes (opcional)
    - Búsquedas en contenido de archivos de empleados (opcional)
    - Notas colapsadas por defecto
    - Sueldos colapsados por defecto
    - La tarjeta de un empleado ahora muestra el proyecto actual
    - Los campos editables ahora se encuentran resaltados
    - Estilos de edición consistentes entre postulantes y empleados
- Resueltos bugs
    - Los postulantes creados no se ven en la lista inmediatamente
    - "Volver al listado" no está en la barra de acciones
    - Los sueldos y algunas fechas no se formatean correctamente
    - La búsqueda rápida no funciona bien si se escribe demasiado rápido
    - No se ven las fotos de los empleados en el listado

## Sprint 4
(Terminado 20 de Abril de 2012)

- Documentación de Sprint 3 y comienzo de Sprint 4
- Nueva funcionalidad
    - Paginación de resultados en los listados de empleados y postulantes
- Resueltos bugs
    - Las URLs no se generan correctamente si el sitio no está instalado en el directorio raíz
    - Al editar una remuneración el cursor se va al final del campo

## Sprint 5
(Terminado 9 de Mayo de 2012)

- Documentación de Sprint 4
- Fundación para soporte de múltiples entornos
- Fundación para soporte de migraciones
- Actualización de RavenDB
- Nueva funcionalidad
    - Permitir recortar imágenes grandes
    - Migraciones: upgrades y downgrades
    - Vacaciones

## Sprint 6
(Terminado 30 de Mayo de 2012)

- Documentación de Sprint 5
- Documentado proceso de respaldo y restauración del sistema
- Resueltos problemas de backup de RavenDB
- Instalado de la nueva versión del sistema en producción
- Nueva funcionalidad
    - Script de backups automatizados
    - Restringido acceso a migraciones a ciertos usuarios
    - FavIcon
- Resueltos bugs
    - Mejor símbolo elegido para postulantes resaltados
    - Convertido campo de certificaciones en una lista
    - Los empleados no se buscan por nombre, sólo por apellido
    - Arreglada migración CleanVacationsStringData
    - Los días totales de vacaciones se calculan mal

## Sprint 7
(Terminado 20 de Junio de 2012)

- Migración del proyecto a VS 2012
- Documentación de Sprint 6
- Sitio de documentación
- Nueva funcionalidad
    - Menú de navegación
    - Preservar una cantidad específica de respaldos anteriores
    - Banner de entorno (DEV / PROD)
    - Soporte HTTPS
    - Los listados muestran "Apellido, Nombre"
    - Los empleados están ordenados alfabéticamente
    - Búsqueda de archivos
    - Uso de Active Directory para permisos de usuario
- Resueltos bugs
    - Configuración de producción es errónea
    - Error al acceder a algunos empleados
    - No se puede usar backspace en los campos numéricos
    - Algunos archivos de estilos no se incluyen en el deploy automáticamente

## Sprint 8
(Terminado 11 de Julio de 2012)

- Re-priorización del product backlog
- Investigada la integración con LinkedIn
- Documentación de Sprint 7
- Actualización de RavenDB
- Nueva funcionalidad
    - Incluyendo archivos de imágenes en la búsqueda de archivos
    - Compartir vistas de postulantes
    - Mejoras al menú
- Resueltos bugs
    - Corregida documentación sobre grupos de Active Directory
    - Algunos archivos no tienen nombre en la búsqueda de archivos
    - La pantalla de resultados de búsqueda de archivos no permite seleccionar texto (genera una descarga)

## Sprint 9
(Terminado 1 de Agosto de 2012)

- Documentación de Sprint 8
- Revisión del product backlog
- Nueva funcionalidad
    - Postulantes aplicando directamente en el sistema
    - Ofertas de trabajo, postulaciones
    - Resaltando términos de búsqueda en la búsqueda de archivos
    - Los campos ahora aceptan cambios si el usuario simplemente hizo click fuera de ellos
    - Los postulantes ahora se muestran ordenados alfabéticamente
    - Historial de cambios en entidades
- Resueltos bugs
    - Cuando se llega a una URL incorrecta, no se indica eso al usuario
    - Cambiado símbolo de eliminación (-) por uno más obvio (x)
    - Mejorado el extracto de texto mostrado en la búsqueda de archivos
    - El archivo por defecto para empleados o postulantes sin imagen no se muestra
    - Al guardar, se pierden los datos pegados en el portapapeles
    - Error al agregar una nueva nota a un postulante

## Sprint 10
(Terminado el 24 de Agosto de 2012)

- Documentada la implantación del sitio de postulaciones
- Documentación de Sprint 9
- Nueva funcionalidad
    - Se resaltan los empleados que no tienen los adjntos requeridos
    - Quick-upload desde la página de listado
    - Links a LinkedIn para postulantes
    - El postulante ahora verá una página de "Búsqueda no disponible" si esa búsqueda ya no lo está
- Resueltos bugs
    - Las páginas de postulaciones no pueden scrollearse

## Sprint 11
(Terminado el 13 de Septiembre de 2012)

- Refinamiento del product backlog
- Planificación: página de vacaciones
- Planificación: nuevas prioridades
- Documentación Sprint 10
- Nueva funcionalidad
    - Agregar adjuntos a postulantes arrastrando un archivo a su página de detalle
    - Agregar adjuntos a empleados arrastrando un archivo a alguno de los slots disponibles en su página de detalle
- Resueltos bugs
    - Configuración errónea del sitio de postulaciones
    - Error en la sección de adjuntos de la página de un empleado

## Sprint 12
(Terminado el 4 de Octubre de 2012)

- Creación de un nuevo entorno en AppHarbor
- Nueva funcionalidad
    - Datos bancarios en la sección de detalle del empleado
    - Campo de CUIL para la página de detalle del empleado
    - Resumen de vacaciones para todo el staff
    - Vacaciones adeudadas para cada miembro del staff
    - Nuevo campo para usuario de Active Directory de un empleado
    - Nuevo campo para email corporativo de un empleado
- Resueltos bugs
    - No funcionan los links para compartir el acceso a postulantes
    - Error al arrastrar archivos a un empleado sin foto
    - Al intentar pegar un texto en una página de detalle (empleado/postulante) se mestra el diálogo de adjuntar archivos

## Sprint 13
(Terminado el 25 de Octubre de 2012)

- Documentación Sprint 11
- Documentación Sprint 12
- Documentación y entrega académica
- Nueva funcionalidad
    - Autocompletado en muchos de los campos disponibles
    - Posibilidad de considerar adelantos de vacaciones en el cálculo de adeudadas
    - Posibilidad de cargar ausencias para un empleado
    - Ver resumen de las vacaciones tomadas por un empleado al seleccionar un período de tiempo
- Resueltos bugs
    - Acceso desde redes externas no estaba funcionando
    - Configuración del entorno de PROD estaba errónea

## Sprint 14
(Terminado el 15 de Noviembre de 2012)

- Documentación Sprint 13
- Auditoría académica
- Formalizada retrospectiva de proyecto y reunión con Director Técnico
- Nueva funcionalidad
    - Información de viaje en la página de detalle de un empleado
    - Agregado un estado "Dado de baja" para empleados
    - Búsqueda por empleados dados de baja
    - Nuevo perfil con acceso a postulantes y no a empleados
- Resueltos bugs
    - Al hacer click en "Agregar nuevo" (empleado o postulante) y se cancelan los cambios, aparece un empleado con datos en blanco
    - Búsquedas en el sitio de documentación no funcionaban correctamente

## Sprint 15
(Terminado el 7 de Diciembre de 2012)

- Documentación Sprint 14
- Nueva funcionalidad
    - MyMenu
    - Permitir mostrar/ocultar en todas las secciones de detalle de un empleado/postulante
    - Permisos de MenuManager
    - Mejorada la experiencia de los autocomplete para conexiones lentas
    - Mejorada la interfaz de resultados de búsquedas de archivos
    - Grados de habilidad para habilidades técnicas
    - Impresión de pedidos por oficina en MyMenu
    - Mostrar la plataforma en la información de Legajos
    - MyMenu: mostrar el pedido del día en la página del administrador
    - Autocompletado para habilidades técnicas
    - Se ocultan las opciones a las que un usuario no tiene acceso
- Resueltos bugs
    - Al arrastrar un archivo a una tarjeta de un postulante existente, muestra un botón crear luego de adjuntado el archivo
    - Indicar claramente cuando una sección está colapsada o expandida
    - La página de migraciones no está funcionando
    - Solucionado error al querer agregar una habilidad técnica

## Sprint 16
(Terminado el 27 de Diciembre de 2012)

- Propuesta de presentación del proyecto
- Documentación Sprint 15
- Nueva funcionalidad
    - Nuevos estilos para todo el sitio, más consistentes
    - Nuevos estilos para las páginas de búsqueda
    - Ahora se pueden definir habilidades requeridas para una búsqueda
    - Gráfico de ausencias (incluyendo vacaciones)
    - Ahora se ve la fecha de la última actualización en la información de un postulante

## Sprint 17
(Terminado el 18 de Enero de 2013)

- Documentación Sprint 16
- Nueva funcionalidad
    - Mostrar estado de los postulantes según su flujo
    - Postulantes sugeridos, según sus habilidades, para la selección en una oferta
    - Tipo de entrevistas ahora se carga dinámicamente (pueden agregarse nuevos valores)
    - La búsqueda de postulantes dinámicamente muestra los tipos de entrevistas disponibles
- Resueltos bugs
    - Errores en el uso de MyMenu
    - Las tarjetas de los postulantes resaltados no se están resaltando
    - Si las vacaciones son al final de un año se calculan en ambos a la vez

## Sprint 18
(Terminado el 7 de Febrero de 2013)

- Documentación Sprint 17
- Estructura del Manual de Usuario
- Nueva funcionalidad
    - Links en la página principal a distintas secciones del sistema
    - Nuevos estilos en la edición de empleados y postulantes
    - Removidas las habilidades de las tarjetas de los postulantes
    - Nuevos estilos en el listado de búsquedas de trabajo
    - Nuevos estilos en la edición de una búsqueda de trabajo
    - Los postulantes pueden cargar habilidades por su propia cuenta
    - Google authentication usando el email corporativo del empleado
    - Convertir un postulante en un empleado (contratar)
- Resueltos bugs
    - No se pueden eliminar los links de compartir postulante

