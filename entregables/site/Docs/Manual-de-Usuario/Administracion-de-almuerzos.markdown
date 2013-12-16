# Administración de almuerzos

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