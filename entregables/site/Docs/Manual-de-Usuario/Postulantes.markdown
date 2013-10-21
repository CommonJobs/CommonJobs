# Postulantes

## Listado de postulantes

La sección de postulante permite visualizar y administrar los postulantes a distintos puestos de trabajo o potenciales nuevos empleados de la familia de la empresa. Similar a otras pantallas de la aplicación, la primera pantalla que el sistema mostrará será un listado de tarjetas en las que cada una de ellas representa a un postulante particular.

![Listado de postulantes](Images/Postulantes/01-listado.png)

Cada una de las tarjetas que se muestran para los postulantes muestra la siguiente información:

- Apellido y nombre del postulante
- Si el postulante se encuentra resaltado
- Última empresa en la que estuvo trabajando
- Etapas por las que pasó el postulante en el proceso de entrevistado

### Búsqueda y filtrado

Este listado también permite efectuar búsquedas y filtrados. Para buscar postulantes, sólo hace falta comenzar a escribir en el campo de *Búsqueda de postulantes* y el sistema limitará los resultados a los postulantes que cumplen con el criterio ingresado. Los campos de un postulante que se comparan contra ese texto para incluirlos en los resultados de la búsqueda son:

- Nombre
- Apellido
- Cualquiera de las compañías en las que trabajó
- Cualquiera de las habilidades (*skills*) que posea
- Cualquiera de las habilidades técnicos (*technical skills*) que posea
- Cualquiera de los nombres de adjuntos que posea

Opcionalmente, se pueden activar modificadores a la búsqueda que agregan o relajan criterios de búsqueda. Estas son las cajas de selección debajo del campo de búsqueda de postulantes. Los modificadores disponibles y sus efectos son los siguientes:

- **Resaltado:** Incluye en el resultado de la búsqueda solamente a postulantes que se encuentren resaltados.
- **Buscar en adjuntos:** Busca también en el contenido de los archivos adjuntos que tengan los postulantes.
- **Incluir contratados:** Incluye en la búsqueda a postulantes que ya hayan sido marcados como contratados.
- **Solo contratados:** Restringe la búsqueda únicamente a postulantes que ya hayan sido marcados como contratados.

Similarmente, se puede filtrar el listado de resultados según las etapas que el postulante haya completado como parte de su proceso de contratación y entrevistado. Por defecto ninguna de esas opciones se encuentran habilitadas. Las opciones son:

- Entrevista técnica
- Entrevista Inglés
- Entrevista PM (Project Manager)
- Test Psicológico
- Entrevista RRHH (Recursos Humanos)
- Currículum Vitae  

### Agregar nuevo

Existen dos formas de crear un nuevo postulante, y ambas involucran la primera tarjeta de *"Agregar nuevo..."*.

#### Agregar nuevo postulante sin archivo adjunto

La forma más simple involucra hacer click en la tarjeta de *Agregar nuevo...*, en donde la tarjeta pedirá el nombre y apellido del postulante. El nombre y apellido pueden entrarse en el formato "&lt;Apellido&gt;, &lt;Nombre&gt;" o "&lt;Nombre&gt; &lt;Apellido&gt;". El sistema lo detectará automáticamente y asignará los valores a los campos correspondientes.

![Crear postulante sin archivo adjunto](Images/Postulantes/02-crear-postulante.png)

Tras ingresar el nombre y apellido del nuevo postulante, sólo basta con hacer click en el botón *Crear* para que el nuevo postulante ya forme parte de la base de postulantes. Inmediatamente el sistema presentará la pantalla de edición para este nuevo postulante.

#### Agregar nuevo postulante desde un archivo adjunto

Si se posee un archivo adjunto disponible para este postulante, es posible un flujo alternativo que simplifica la creación de un nuevo postulante y la adición del adjunto. Para eso, sólo hace falta arrastrar el archivo adjunto a la tarjeta de *Agregar nuevo...*.

![Crear postulante desde archivo adjunto](Images/Postulantes/03-crear-postulante-desde-adjunto.png)

En este caso el sistema mostrará una pantalla en donde se confirma la adición del adjunto y su nombre, junto con un campo para ingresar el nombre del postulante. Tras hacerlo, sólo es necesario hacer click en el botón *Crear* para que el postulante sea almacenado en la base del sistema y pasemos a la pantalla de edición del mismo.

### Agregar archivo adjunto a postulante desde el listado

De forma similar a la que se puede crear un postulante arrastrando un archivo a la tarjeta de *Agregar nuevo...*, también pueden adjuntarse archivos a postulantes existentes arrastrándolos hasta su tarjeta.

//TODO imagen de archivo siendo arrastrado a la aplicación

## Detalle de un postulante

La pantalla de detalle o edición de un postulante posee varias secciones que permiten administrar su información. A continuación se explicará en qué consiste dicha pantalla y qué información se dispone en ella.

### Sección principal del perfil

La sección principal del perfil posee la siguiente información del postulante:

- Imagen de postulante
- Apellido
- Nombre

El apellido y nombre pueden ser ingresados en un mismo campo, en e formato *&lt;Nombre Apellido&gt;* o *&lt;Apellido, Nombre&gt;* y el sistema automáticamente detectará cuál es cuál y lo mostrará apropiadamente.

Del lado derecho de esta sección se encuentra una sub-sección que permite administrar los siguientes datos:

- **Resaltado**: Este campo permite resaltar o des-resaltar al postulante. Esto hará que el postulante se vea con un color distinto en la pantalla del listado y que se encuentre listado antes que los demás. Por lo general, es útil resaltar postulantes a los cuales debe dárseles un seguimiento cercano o sobre el cual hay que actuar pronto, pero la funcionalidad de resaltado puede usarse según la utilidad que el usuario mejor disponga.
- **Links de acceso:** Esta sección permite la creación de links públicos para acceder al perfil de este postulante. Los links permiten también acceso de edición al mismo y son válidos sólo durante el tiempo que se mantengan asociados al perfil. Esto puede hacerse indicando un tiempo de expiración al mismo, tras el cual dicho link no estará más activo. La utilidad de esta funcionalidad permite a personas sin acceso al sistema contribuir con la construcción del perfil de un postulante (por ejemplo, entrevistadores o asociados).

### Datos personales

La siguiente sección posee esta información sobre un postulante:

- Fecha de nacimiento
- Estado civil
- Dirección
- Teléfonos
- Email
- Perfil de Linked In
- Nivel de inglés
- Título Universitario
- Recibido (graduado)
- Universidad
- Skills (Habilidades)
- Skills Técnicos (habilidades técnicas)

### Datos laborales

Los datos laborales permiten recavar información sobre la experiencia previa de trabajo que este postulante ha tenido.

Por cada uno de esos registros, se almacena la siguiente información:

- Desde
- Hasta
- Empresa
- Trabajo anterior / actual

### Notas

Finalmente, la sección de notas permite almacenar notas variadas y adjuntos sobre el postulante. Estas permiten también indicar su progreso sobre el proceso de contratación (entrevistas, tests, etc).

Por cada una de esas notas se registra la siguiente información:

- Fecha
- Tipo de nota (indicador de progreso)
- Archivo adjunto
- Notas

