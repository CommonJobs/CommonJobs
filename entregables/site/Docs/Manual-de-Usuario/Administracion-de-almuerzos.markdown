# Seleccion de almuerzos

La funcionalidad denominada *MyMenu* dentro de CommonJobs permite la administración de almuerzos dentro de la empresa, teniendo en cuenta que existen varias opciones para ofrecer a los empleados, varias oficinas que realizan estos pedidos, y un horario restringido de posible elección para poder realizar los pedidos.

El acceso a *MyMenu* está cercanamente ligado a la identificación de los usuarios, y por tanto, es necesario que cada empleado que vaya a utilizarlo tenga cargado en su perfil la dirección de correo corporativo, y un usuario de dominio. De esta forma, el login podrá realizarse para dicho empleado con la cuenta de correo (gracias a la autenticación de Google) y el empleado podrá administrar los menúes por cuenta propia.

## Configuración

Una vez ingresado a la funcionalidad de *MyMenu*, lo primero que el empleado verá será la sección de configuración, que por el momento le permite configurar una oficina por defecto en la cual serán pedidos sus almuerzos. El empleado puede cambiar esta selección, y en el futuro esta pantalla dará más opciones de configuración.

![Configuración](Images/Administracion-de-almuerzos/01-configuracion.png)

## Días específicos

La opción de días específicos permite incluir excepciones particulares para las cuales el menú debe ser cambiado (contrario a la selección por defecto que el empleado haya realizado), y así indicar cambios específicos que deben realizarse, como el cambio de un menú por otro o la cancelación de un almuerzo para un día particular.

![Días específicos](Images/Administracion-de-almuerzos/02-dias-especificos.png)

## Menúes por semana

Las siguientes opciones disponibles en la aplicación permitirán al empleado configurar su elección por defecto según las semanas en las cuales los menúes se encuentran disponibles. Se asume que estos menúes cumplen un ciclo de determinadas semanas tras el cual vuelven a repetirse. En la imagen más abajo, se puede ver una configuración de un ciclo de 5 semanas, actualmente visualizando la semana 2.

![Configuración por semanas](Images/Administracion-de-almuerzos/03-semanas.png)

## Guardar cambios

Por supuesto, tras realizar la configuración, el empleado tiene la opción de descartar sus cambios si es que no está satisfecho con ellos, o guardarlos. Para esto puede utilizar los dos botones de acción que se encuentran disponibles en la sección superior derecha de la pantalla.

![Botones de acción](Images/Administracion-de-almuerzos/04-botones-de-accion.png)

# Administración de almuerzos

Por otro lado, los usuarios administrativos de la sección de *MyMenu* tendrán acceso a funcionalidad extra que les permitirá cambiar los almuerzos disponibles, a la vez de efectuar los pedidos correspondiente a cada día, basándose en las elecciones de cada uno de los empleados.

## Pedido del día

La primera de las nuevas funcionalidades disponibles es la visualización del pedido a efectuar cada día, que se calcula en base a las distintas elecciones de los empleados.

![Pedido del día](Images/Administracion-de-almuerzos/05-pedido-del-dia.png)

En la pantalla del pedido del día se puede observar:

- La fecha seleccionada del pedido. Es posible moverse días hacia adelante o hacia atrás para ver pedidos históricos o pedidos futuros en base a la selección actual de datos de los empleados.
- Si el pedido ha sido realizado o no: este dato se calcula automáticamente en base a la configuración de horarios de pedido. Esto permite generar un horario de corte a partir del cual los pedidos no pueden ser modificados.
- El pedido a efectuar: una versión sumarizada por oficina de la cantidad de menús a pedir y cuál es el contenido de cada menú.
- El detalle por empleado de los pedidos efectuados para el día y cualquier excepción o comentario que los empleados hayan hecho disponibles.

El listado puede ser también filtrado por oficina utilizando las pestañas con su nombre, permitiendo imprimir versiones independientes para cada una de ellas.

Igual que en otras partes del sistema, el listado puede ser filtrado con un criterio de búsqueda, ordenado con múltiples criterios, exportado a PDF o Excel, impreso individualmente o copiado al portapapeles.

## Definición de los menúes

Por supuesto, las distintas opciones de elección para los empleados debe ser cargada antes de que estas puedan ser seleccionadas. También es posible alterar los menúes con el tiempo si es que la empresa proveedora de menúes cambia sus opciones o cualquier otra razón pertinente.

Para estos casos la sección disponible para el administrador llamada *Definición de los menúes* permitirá configurar estos aspectos.

Para cada día de la semana (Lunes a Viernes), cada una de las semanas disponibles en el ciclo y cada uno de los posibles menúes, se puede cargar el contenido de este menú en particular.

Por ejemplo, en la imagen de más abajo podemos observar un menú cargado para una configuración de cinco semanas, y tres menúes disponibles.

![Definición de los menúes](Images/Administracion-de-almuerzos/06-definicion-de-los-menues.png)

## Configuración

Por último, es necesario poder configurar los parámetros mencionados en las secciones anteriores, por lo que se provee la sección de configuración entre las opciones de definición de los menúes. Las siguientes son:

![Configuración](Images/Administracion-de-almuerzos/07-configuracion.png)

- Título del menú
- Fecha de comienzo de este menú
- Fecha de finalización de este menú
- Fecha de último envío del pedido
- Horario hasta el cual es editable en cada día (hora de cierre de pedidos)
- Semana que comenzará el ciclo de pedidos
- La cantidad de semanas presentes en un ciclo de pedidos según el menú disponible
- Las distintas opciones (menús) disponibles
- Las distintas oficinas o localizaciones en las cuales los empleados pueden pedir sus menús

Tras guardados los cambios en estas configuraciones, se aplicarán inmediatamente en el resto del sistema.