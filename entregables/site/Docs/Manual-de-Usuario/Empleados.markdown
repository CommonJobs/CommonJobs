# Empleados

La sección de empleados permite administrar a los empleados registrados con la empresa y su información. Permite dar seguimiento a cuestiones relacionadas con el empleado como también el historial de movimientos dentro de la empresa y sus cambios salariales.

## Listado de empleados

El listado de empleado brindará una visión global de los empleados que actualmente se encuentran en la empresa. Este listado se encuentra ordenado alfabéticamente, de izquierda a derecha y de arriba a abajo.

![Listado de empleados](Images/Empleados/01-listado.png)

Este listado permitirá interactuar con los empleados, o crear un nuevo empleado.

La cantidad de empleados mostrado en una línea es acorde al tamaño de la ventana, de forma que distintos dispositivos con distinto tamaño de pantalla deberían aún poder mostrar la información a un tamaño legible.

### Tarjetas

Cada tajeta de empleado muestra la siguiente información:

![Tarjetas de empleados](Images/Empleados/06-tarjeta-empleado.png)

- El avatar del empleado, si es que uno está registrado en el sistema. De otra forma, se mostrará un avatar genérico que indica que el avatar para este usuario no ha sido cargado.
- El nombre y apellido, en formato "Apellido, Nombre".
- El proyecto actual en el que este empleado se encuentra asignado.
- La posición actual que este empleado tiene en la empresa.
- Un listado de tags referentes a las habilidades (skills) principales de esta persona.
- Una indicación (arriba a la derecha) de si existen slots de adjuntos que se encuentren faltantes en el perfil de el empleado.

![Indicador de slots faltantes en tarjeta](Images/Empleados/07-indicador-slots-tarjeta.png)

El indicador de slots faltantes mostrará el detalle de los archivos faltantes si se le posiciona el cursor encima.

### Filtrado

El listado permite ser filtrado simplemente ingresando un texto por el cual filtrar. Este texto será comparado contra:

- Nombre
- Apellido
- Cualquier habilidad (skill) del empleado
- Cualquier habilidad técnica (skill técnico) del empleado
- Cualquier nombre de los archivos adjuntos del empleado
- Posición actual
- Proyecto actual
- Número de legajo
- Plataforma
- Fecha de contratación
- Fecha de nacimiento
- Cuenta bancaria
- Nivel de inglés
- Seniority
- Título
- Universidad

Comenzar a escribir automáticamente filtrará los resultados acorde.

También se pueden utilizar las casillas de selección debajo del campo de búsqueda para los siguientes efectos:

- **Buscar en notas**: El campo de búsqueda se comparará también contra cualquiera de las notas del empleado, incluyendo al empleado en los resultados de búsqueda si es que alguna de las notas contiene el texto introducido.
- **Buscar en adjuntos**: El campo de búsqueda se comparará también contra el contenido de cualquiera de los archivos adjuntos del empleado (siempre que esto fuera posible, ver documentación sobre adjuntos), incluyendo al empleado en los resultados de búsqueda si es que alguno de los archivos adjuntos contiene el texto introducido.
- **Incluir dados de baja**: Los empleados marcados como *dados de baja* serán incluidos entre los resultados, aplicando los filtros de búsqueda seleccionados.    

### Crear un nuevo empleado

Existen varias formas de crear un nuevo empleado desde esta pantalla.

#### Utilizar tarjeta de "Agregar nuevo..."

![Crear nuevo empleado desde agregar nuevo](Images/Empleados/02-utilizar-tarjeta-de-agregar-nuevo.png)

Haciendo click sobre la tarjeta de "Agregar nuevo..." la hará cambiar por una tarjeta en donde podemos introducir el nombre del nuevo empleado a crear. El nombre no tiene por qué estar en ningún formato particular. "Juan Pérez" y "Pérez, Juan" tendrán el mismo efecto. 

Una vez que aceptemos el nombre del nuevo empleado, seremos llevado a la sección que nos permitirá editar su perfil.

#### Quick-attachment

En otras situaciones, es posible que simplemente se disponga de un documento sobre este empleado y se quiera con esta información ya incluirlo en el sistema. Para estos casos, se puede arrastrar el documento a la tarjeta de "Agregar nuevo...".

![Quick attachment en Agregar nuevo](Images/Empleados/03-agregar-nuevo-drop.png)

Tras arrastrar el archivo a la tarjeta de agregar nuevo y soltarlo, un cuadro de diálogo aparecerá para indicar qué tipo de archivo se puede considerar este. De forma similar, este cuadro de diálogo esperará el nombre del empleado para que sea creado. De forma similar al método anterior, el formato del nombre y apellido del empleado no es importante.

![Cuadro de diálogo sobre tipo de adjunto](Images/Empleados/04-tipo-de-adjunto-crear-nuevo.png)

Seleccionar uno de estos tipos de adjuntos asignará el archivo recién subido a el slot indicado, y creará el empleado con estos archivos ya asociados como adjuntos. 

En el caso de arrastrar múltiples archivos a la tarjeta de "Agregar Nuevo", solamente se podrán almacenar estos como archivos de parte de las notas, sin asociarse a ninguno de los slots.

![Cuadro de diálogo agregar nuevo con múltiples adjuntos](Images/Empleados/05-quick-attach-crear-nuevo-multiples-archivos.png)

### Editar un empleado

Haciendo click en cualquier parte de la tarjeta de un empleado, el sistema cargará el perfil del empleado para verlo o editarlo.

### Agregar un adjunto a un empleado

De forma similar en la que podemos arrastrar un adjunto para agregar un nuevo empleado, también es posible arrastrar un adjunto a la tarjeta de un empleado existente para añadir ese adjunto a su perfil.

![Agregar adjunto a empleado desde listado](Images/Empleados/08-quick-attachment.png)

El sistema mostrará deshabilitadas las opciones (slots) para las cuales el empleado ya posee un adjunto. También se pueden arrastrar varios archivos, en cuyo caso la única opción disponible será "Otros (en notas)".

En este punto, haciendo click sobre la opción "Ver empleado" se puede acceder al perfil del empleado, pero esto cancelará la operación de adjuntar los archivos.

## Perfil de empleado

El perfil de un empleado es una sección que permite editar la información del mismo. Esta sección se encuentra a su vez dividida en varias secciones las cuales también se encuentran divididas en sub-secciones.

Las secciones de un perfil de un empleado son:

- Menú principal de cambios (siempre accesible en la pantalla)
- Datos principales
- Datos personales
- Datos laborales
- Vacaciones
- Ausencias
- Archivos adjuntos
- Notas

Cada una de las secciones puede ser ocultada o mostrada para simplificar la vista actual.

### Menú principal de cambios

![Menú de cambios](Images/Empleados/09-menu-de-cambios.png)

El menú principal de cambios se refiere a acciones que son globales para el perfil del empleado.

La primera opción, *Guardar cambios* permitirá guardar cualquier cambio que se haya efectuado en el perfil. El botón se encontrará deshabilitado si es que no existen cambios para guardar, o si los cambios ya han sido guardados.

La segunda opción, *Descartar cambios* tiene la consecuencia opuesta: descartar todos los cambios que se hayan efectuado en el perfil de usuario y revertir los campos a sus valores anteriores. De igual forma, sólo estará habilitada esta opción si es que existen cambios que revertir.

La tercera opción, *Editar todos* nos permitirá cambiar todos los campos a modo de edición, en donde podemos cambiar su valor además de sólo observarlo. Esto es útil para cargar toda la información a la vez, en donde se podrá atravesar todo el formulario con la tecla tab y con el teclado llenar la información.

### Modos de edición

Durante la siguiente secciones se describirán los datos que pueden ser alterados en el perfil de un empleado. Sin embargo, para todos ellos, existen dos modos. El primero es el modo de visualización, en donde se verá el contenido del campo. Si este campo es editable, generalmente tendrá un subrayado punteado debajo suyo, y haciendo click sobre el mismo se podrá entrar en el modo de edición. Este modo de edición tendrá los controles apropiados para elegir un valor, y tras aceptar ese cambio (presionando Enter) se volverá al modo de visualización. También puede cancelarse el cambio presionando en la tecla "Escape" o haciendo click fuera del campo.

Cambiar el modo de visualización a edición y viceversa no tiene relación con que los cambios hayan sido guardados para este empleado.

### Datos principales

![Datos principales](Images/Empleados/10-datos-principales.png)

La sección de datos principales permite ver e interactuar con los siguientes datos del empleado:

- **Nombre y apellido**
- **Posición**: Posición actual que este empleado desempeña en la empresa.
- **Posición inicial**
- **Proyecto actual**
- **Título**: Título universitario que este empleado tiene.
- **Recibido / No recibido**: Si este empleado es un estudiante para el título descripto, será "no recibido". Si, por lo contrario, ya ha obtenido su título, será "Recibido".
- **Usuario de dominio**: Utilizado para que el empleado pueda acceder al sistema.
- **Email corporativo**

### Datos personales

![Datos personales](Images/Empleados/11-datos-personales.png)

La sección de Datos personales permite ver e interactuar con los siguientes datos del empleado:

- **Información personal**
    - **Fecha de nacimiento**
    - **Estado civil**: Soltero / Casado / Divorciado
    - **Dirección**
    - **Teléfonos**
    - **Email personal**
    - **Plataforma**: Lugar físico en el que se encuentra trabajando
    - **CUIL**
- **Información de viajero**
	- **Posee pasaporte argentino**: Sí / No
	- **Vencimiento del pasaporte argentino**
	- **Posee visa de EEUU**: Sí / No
	- **Vencimiento de la visa de EEUU**
	- **Posee ciudadanía europea**: Sí / No
	- **Posee pasaporte europeo**: Sí / No
	- **Vencimiento del pasaporte europeo**
- **Aptitudes**
	- **Seniority**: Nivel de desempeño de sus habilidades. En algunas empresas está relacionado con el nivel máximo que se le puede pagar a un empleado.
	- **Nivel de Inglés**
	- **Título**: Título universitario que este empleado tiene.
	- **Recibido / No recibido**: Si este empleado es un estudiante para el título descripto, será "no recibido". Si, por lo contrario, ya ha obtenido su título, será "Recibido".
	- **Skills**: Listado separado por comas de habilidades no técnicas que este empleado posee. Ejemplos: Project Management, Coordinación, Trabajo bajo presión.
	- **Certificaciones**: Listado de certificaciones que este empleado haya obtenido.
	- **Skills técnicos**: Listado de habilidades técnicas que este empleado posee. Cada habilidad, además de tener su nombre indicando qué es, también tendrá un indicador del nivel de dicha habilidad. Las posibilidades son: Basic, Intermediate, Advanced, Unknown.

### Datos laborales

![Datos laborales](Images/Empleados/12-datos-laborales.png)

La sección de Datos laborales permite ver e interactuar con los siguientes datos del empleado:

- **Relación con la empresa**
	- **Dado de baja**:  Fecha en la que el empleado ha sido dado de baja de la empresa.
	- **Legajo**: Número de legajo.
	- **Fecha de ingreso**: Fecha en la que el empleado ha ingresado en la empresa. El tiempo relativo a la actualidad es calculado automáticamente y mostrado junto con el dato ingresado.
	- **Carga horaria**: Carga horaria que el empleado está cumpliendo.
	- **Horario**: Horario que el empleado está cumpliendo.
	- **Acuerdo**: Acuerdo comercial con el que se regula la relación del empleado con la empresa.
- **Remuneración**: Esta sección por defecto se encuentra oculta.
	- **Remuneración actual**: Sueldo que actualmente se le paga al empleado.
	- **Remuneración inicial**: Sueldo que se le pagaba al empleado cuando comenzó su relación con la empresa.
	- **Cambios salariales**: Conjunto de cambios que resultaron en incrementos o decrementos del salario pagado al empleado. Cada cambio tendrá una fecha asociada, un nuevo sueldo (que actualizará la remuneración actual automáticamente), y una nota opcional.
- **Datos bancarios**: Datos sobre la cuenta bancaria en la que la empresa le paga al empleado.
	- **Banco**
	- **Sucursal**
	- **CBU**
	- **Número de cuenta** 
- **Otros datos**
	- **Obra social**
	- **Almuerzo** 

### Vacaciones

![Vacaciones](Images/Empleados/13-vacaciones.png)

La sección de Vacaciones permite ver e interactuar con un listado de periodos de vacaciones que el empleado se ha tomado. Cada uno de estas vacaciones tendrá un periodo asignado (generalmente, un año), una fecha de comienzo y una fecha de fin. El sistema calculará automáticamente la cantidad de días de vacaciones que corresponden a dichas fechas y mostrará el total de días a esas vacaciones como el total de días completo que el empleado se ha tomado.

### Ausencias

![Ausencias](Images/Empleados/14-ausencias.png)

La sección de ausencias permite ver e interactuar con un listado de períodos de ausencias que el empleado se ha tomado. Cada uno de estos períodos contendrá la siguiente información:

- **Desde**: Fecha de comienzo de la ausencia
- **Hasta**: Fecha de finalización de la ausencia
- **Tipo**: Tipo de ausencia. Todo el día / Parte del día / Trabajo remoto.
- **Razón**: Razón por la que el empleado se ha tomado este período ausente.
- **Certificado**: Con certificado / sin certificado.
- **Adjunto**: Archivo adjunto que sirve como certificado de la ausencia.
- **Nota**: Notas relativas a la ausencia del empleado.

### Archivos adjuntos

![Archivos adjuntos](Images/Empleados/15-archivos-adjuntos.png)

La sección de archivos adjuntos permite ver e interactuar los distintos archivos adjuntos que están asignados a slots particulares. Estos slots son indicadores de documentos particulares que deben estar presentes para un empleado, si es que el slot es _requerido_, o que es generalmente utilizado pero puede no estar presente, como es el caso de los _slots opcionales_.

Un ejemplo de _slot requerido_ es el documento de alta, que todo empleado debe tener en el sistema de legajos de la empresa. Un ejemplo de _slot opcional_ es el documento de baja, que un empleado debe tener solamente si es que se ha dado de baja en la empresa.

Esta sección mostrará qué documentos se encuentran cargados y asignados a slots (un documento por slot) y cuáles slots se encuentran vacíos aún. Cada slot mostrará su nombre y se mostrará debajo el nombre del archivo. También se intentará indicar el tipo del archivo si es que este ha sido reconocido por el sistema, pero de todas formas será accesible haciendo click en él.

![Eliminar adjunto de slot](Images/Empleados/16-eliminar-archivo.png)

Poniendo el cursor encima de un archivo se puede eliminar el mismo, ya que una cruz con dicha finalidad será mostrada.

### Notas

![Notas](Images/Empleados/17-notas.png)

La sección de notas permite ver e interactuar con un listado de notas referentes a este empleado. Cada nota posee una fecha, un archivo adjunto (opcional) con información relacionada a la nota, y el contenido rico en formato (Markdown) de la nota.

Las notas pueden editarse con un editor rico que ayuda al usuario a ingresar el formato Markdown correcto para el tipo de edición que el usuario desea hacer.

![Editar notas](Images/Empleados/18-editar-notas.png)
