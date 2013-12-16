# Migraciones

La sección de migraciones permitirá administrar las actualizaciones graduales a la estructura de la base de datos que requieran distintas actualizaciones de la aplicación. De esta forma, siempre se puede actualizar el sistema preservando los datos y se puede volver a versiones anteriores con una pérdida de datos mínima.

## Acceder al área de migraciones

A diferencia de otras áreas de la aplicación, el área de migraciones no se encuentra disponible desde los menúes de la aplicación. Para acceder a él, hace falta ingresar manualmente la ruta `/Migrations` luego de la ruta base a través del la cual se pueda acceder a la aplicación.

Es necesario estar autenticado como un administrador para poder acceder al área de migraciones.

## Listado de migraciones

La pantalla de migraciones mostrará un listado de las migraciones disponibles en el sistema. Para obtener nuevas migraciones es necesario instalar versiones más actualizadas del código en el servidor que se encuentra ejecutando esta aplicación.

![Listado de migraciones](Images/Migraciones/01-listado.png)

El listado de migraciones muestra la siguiente información para cada una de las migraciones disponibles:

- **Key:** clave identificadora de la migración. Por convención del equipo se ha decidido hacer a esta la fecha de implementación de dicha migración.
- **Class:** nombre de la clase que implementa los cambios de esta migración. Esta es la clase que tiene la lógica necesaria para aplicar los cambios en la estructura de los datos, tanto para instalar como para desinstalar la migración.
- **Description:** Una descripción breve de los cambios que esta migración incluye.
- **Status:** Estado según ha sido detectado en la estructura de datos actual, permitiendo identificar si la migración ha sido instalada o no.
- **Action:** Listado de acciones posibles según el estado actual de la migración.

## Aplicar cambios de migraciones

Para aplicar cambios de las migraciones, sólo hace falta seleccionar la acción correspondiente (*Install* / *Reinstall*) en el listado de acciones disponibles y hacer click en el botón de *Submit* de abajo.

De la misma forma, elegir la acción *Uninstall* de una migración aplicará los cambios reversos para remover los cambios introducidos por una migración.

## Ver información de errores

En el caso particular en el que una migración haya fallado, su estado será indicado como tal y se podrá ver un icono cercano a su estado indicando la razón del fallo de aplicación de la migración. Colocando el puntero sobre este icono permitirá ver detalles de por qué falló la migración para poder resolver el problema.

![Información de error](Images/Migraciones/02-error.png)