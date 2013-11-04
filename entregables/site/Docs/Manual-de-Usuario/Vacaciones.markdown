# Vacaciones

La sección de vacaciones permite una visualización sumarizada de la información de vacaciones que se dispone de los empleados.

Esta información se encuentra detallada para cada empleado, en la sección de Vacaciones de su perfil. Para más información, ver la sección de [Empleados](Empleados.markdown).

![Listado de vacaciones](Images\Vacaciones\01-listado.png)

## Columnas incluídas

Las columnas que por defecto se pueden ver en este listado son:

1. **Nombre:** Apellido y nombre del empleado. También es una fácil de acceder fácilmente al perfil del empleado, haciendo click sobre su nombre.
2. **Ingreso:** Fecha de ingreso del empleado a la empresa, como método relativo de medición de la cantidad de vacaciones adeudada al empleado.
3. **Adeudadas:** Cantidad de vacaciones, medida en días, que la empresa le debe al empleado.
4. **Tomadas:** Cantidad de vacaciones, en días, que el empleado se ha tomado en total desde su ingreso a la empresa.
5. **Adelantadas:** Cantidad de vacaciones, en días, que la empresa le ha dado al empleado en adelanto por un período futuro.
6. **(Periodo):** Cantidad de vacaciones, en días, que el empleado se ha tomado en cada período. Cada período está indicado por un año, desde el actual hasta 6 años anteriores. 
7. **Anteriores:** Cantidad de vacaciones, en días, que el empleado se ha tomado en períodos anteriores a los años indicados en la pantalla.

Las vacaciones por período se calculan como 14 días por año, pero si el empleado es contratado en ese mismo año (período), las vacaciones se consideran proporcionales a la porción del año que el empleado fue contratado en la empresa. A la vez, las vacaciones ascienden a 21 días por año si es que el empleado tiene 6 o más años de antigüedad, y posteriormente a 28 días si es que el empleado tiene 10 o más años de antigüedad.

## Ordenamiento

Cuando la grilla se carga por primera vez, los empleados serán ordenados por apellido y nombre, pero es posible cambiar ese ordenamiento.

Haciendo click en la cabecera de una columna se puede indicar una dirección de ordenamiento para esa misma columna, que reemplazará el ordenamiento actual. La dirección del ordenamiento será indicado con una flecha próxima al título de la columna.

Una flecha hacia arriba indicará un ordenamiento de menor a mayor (números), alfabético (para textos) y de más antiguo a más reciente (fechas). Una flecha hacia abajo indica la dirección opuesta: mayor a menor (números), alfabético inverso (textos) y de más reciente a más antiguo (fechas).

![Lista re-ordenada por ingresos recientes a antiguos](./Images/Vacaciones/02-ordenamiento.png)

### Ordenamiento múltiple

La grilla permite ser ordenada por múltiples criterios. Tras haber elegido y haber indicado un primer ordenamiento, sólo es necesario presionar *Shift* (o *Mayúsculas*) y hacer click sobre la segunda columna sobre la que quiere ordenarse. La grilla ordenará las filas según esa segunda columna, manteniendo como prioridad el orden impuesto por la primer columna. Esto es útil cuando el primer ordenamiento encuentra valores repetidos.

![Lista ordenada por múltiples criterios](./Images/Vacaciones/03-ordenamiento-multiple.png)

Mientras se mantiene presionada la tecla *Shift*, el ordenamiento de las columnas variará de menor a mayor, luego mayor a menor, y luego removerá el ordenamiento por esa columna, dejándola sin ningún ordenamiento en particular. A consiguientes clicks, se repite este comportamiento. Esto permite re-decidir ordenamientos múltiples si necesidad de perder los ya establecidos.

## Paginación

Al final del listado, se encontrará una leyenda que indica cuántos empleados se están visualizando actualmente, y si existen más empleados que no se encuentran en la vista actual. Si ese fuera el caso, también serían visible unos controles que permitirían cambiar de página visualizada.

![Leyenda de paginación](./Images/Vacaciones/04-paginacion.png)

<!---
Nota: sería muy bueno poder mostrar una imagen de esto. ¿Cuál es el límite de la paginación por defecto?
--->

## Búsqueda

Para buscar algún valor en particular, puede escribirse sobre la caja de búsqueda que se encuentra en la esquina superior derecha. Al comenzar a escribir, los valores serán filtrados automáticamente para mostrar los resultados.

![Búsqueda](./Images/Vacaciones/05-busqueda.png)

De la misma forma que cuando se muestran todos los resultados, al efectuar una búsqueda de filtrado, se verá una leyenda que indica cuántos registros se encuentran actualmente visibles del total, y se indicará además que se trata de un filtrado.

El filtrado se aplica sobre cualquiera de las columnas de la tabla. En el caso de la fecha de ingreso, también incluye el día de la fecha de ingreso, aunque este no se encuentre directamente visible.

## Botones de exportación

En la esquina superior izquierda se encuentran varios botones que permiten la exportación de los datos visualizados en distintos formatos. Estos son: *Imprimir*, *Copiar*, *PDF*, y *Excel*.

### Imprimir

Al presionar el botón imprimir se mostrará un mensaje indicando que la tabla se ha acomodado para una óptima impresión. Los menúes y mensajes adicionales serán retirados de la pantalla, para permitir imprimir los contenidos de la tabla.

Para efectuar la impresión, se debe iniciar la impresión desde el navegador que se esté utilizando.

![Visualización de impresión](./Images/Vacaciones/06-impresion.png)

Una vez que se haya efectuado la impresión, puede cancelarse este modo de visualización presionando la tecla *Escape*.

### Copiar

Para copiar la información visualizada en pantalla y poder compartirla con otros programas, el botón de Copiar permitirá hacerlo, a través del portapapeles.

Una vez que el botón se presiona, una confirmación de la información copiada se mostrará en pantalla. Esta confirmación incluirá el número de filas copiadas, que coincidirá con el número de filas visualizadas actualmente.

![Confirmación de copiado](./Images/Vacaciones/07-copiar.png)

Una vez realizado esto, la información se encontrará disponible en el portapapeles para ser pegada en otra aplicación. El formato de esta información copiada será de texto separado por tabulaciones.

### PDF

La exportación a PDF permitirá generar un archivo en formato de documento portable (PDF). La información en pantalla será llevada a ese formato en orientación vertical.

![Ejemplo de exportación PDF](./Images/Vacaciones/08-pdf.png)

Notesé que el formato del PDF generado actualmente no se encuentra optimizado para datos de longitud ni para mostrar información de contexto (como el día en el que se generó, o la búsqueda efectuada). Esta funcionalidad se agregará en un futuro al sistema.

### Excel

La opción de exportación a Excel permitirá descargar un archivo *.csv*, siglas de *Comma Separated Values* (Valores separados por comas). Este formato estándar permite interactuar con varios programas de manejo de datos, incluyendo el popular Microsoft Excel.

![Exportación por defecto a CSV](./Images/Vacaciones/09-csv.png)

Una vez que estos archivos se encuentren en su programa, usted puede manipularlos a gusto, generar reportes útiles, incluso filtrar o manipular la información y darle formato.