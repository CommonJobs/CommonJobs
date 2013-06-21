# Legajos

La sección de legajos permite una visión sumarizada de la información que se posee de los empleados presentes y pasados.

Dicha sección puede accederse desde el menú principal, bajo el nombre de "Legajos". Si dicha opción se encuentra deshabilitada, póngase en contacto con el administrador de su sistema para asegurarse de poseer los permisos necesarios.

![Listado de legajos](Images/Legajos/01-listado.png)

## Columnas incluidas

Las columnas que por defecto se pueden ver en este listado son:

1. **Número de Legajo:** Este es el número de legajo que se puede completar en el perfil del empleado, bajo la sección *Relación con la empresa*.
2. **Plataforma:** Esta es la plataforma indicada para el empleado, en la sección *Información personal*.
3. **Nombre:** Este es el nombre completo del empleado, como se escribe en la sección principal de su perfil. Si el empleado no dispone de un nombre o de un apellido actualmente, se indicará lo siguiente con el texto *Sin nombre* señalado en letras itálicas.
4. **CUIL:** Esta es la Clave Única de Identificación Laboral, que se indica en la sección de *Información personal* en el perfil del empleado.
5. **Ingreso:** La fecha de ingreso del empleado en la compañía, indicada en la sección de *relación con la empresa* en el perfil del empleado.
6. **Banco:** El banco del empleado, según lo indicado en su perfil en la sección *Datos bancarios*.
7. **Sucursal:** La sucursal del banco del empleado, según lo indicado en su perfil en la sección *Datos bancarios*.
8. **Nº Cuenta:** El número de cuenta del banco del empleado, según lo indicado en su perfil en la sección *Datos bancarios*.
9. **CBU:** La Clave Bancaria Uniforme de la cuenta del empleado, según lo indicado en su perfil en la sección *Datos bancarios*.
10. **Convenio:** El convenio laboral que la empresa tiene con el empleado, según lo indicado en el campo *Acuerdo* de la sección *Relación con la empresa* del perfil del empleado.
11. **Tarea:** El proyecto en el que el empleado está actualmente trabajando, según lo indicado en la sección principal del perfil de usuario.
12. **Nacimiento:** Fecha de nacimiento del empleado, según lo indicado en la sección de *Información personal* de su perfil.

## Ordenamiento

Cuando la grilla se carga por primera vez, los empleados serán ordenados por apellido y nombre, pero es posible cambiar ese ordenamiento.

Haciendo click en la cabecera de una columna se puede indicar una dirección de ordenamiento para esa misma columna, que reemplazará el ordenamiento actual. La dirección del ordenamiento será indicado con una flecha próxima al título de la columna.

Una flecha hacia arriba indicará un ordenamiento de menor a mayor (números), alfabético (para textos) y de más antiguo a más reciente (fechas). Una flecha hacia abajo indica la dirección opuesta: mayor a menor (números), alfabético inverso (textos) y de más reciente a más antiguo (fechas).

![Lista re-ordenada por ingresos recientes a antiguos](./Images/Legajos/02-ordenamiento.png)

### Ordenamiento múltiple

La grilla permite ser ordenada por múltiples criterios. Tras haber elegido y haber indicado un primer ordenamiento, sólo es necesario presionar *Shift* (o *Mayúsculas*) y hacer click sobre la segunda columna sobre la que quiere ordenarse. La grilla ordenará las filas según esa segunda columna, manteniendo como prioridad el orden impuesto por la primer columna. Esto es útil cuando el primer ordenamiento encuentra valores repetidos.

![Lista ordenada por múltiples criterios](./Images/Legajos/03-ordenamiento-multiple.png)

Mientras se mantiene presionada la tecla *Shift*, el ordenamiento de las columnas variará de menor a mayor, luego mayor a menor, y luego removerá el ordenamiento por esa columna, dejándola sin ningún ordenamiento en particular. A consiguientes clicks, se repite este comportamiento. Esto permite re-decidir ordenamientos múltiples si necesidad de perder los ya establecidos.

## Paginación

Al final del listado, se encontrará una leyenda que indica cuántos empleados se están visualizando actualmente, y si existen más empleados que no se encuentran en la vista actual. Si ese fuera el caso, también serían visible unos controles que permitirían cambiar de página visualizada.

![Leyenda de paginación](./Images/Legajos/04-paginacion.png)

<!---
Nota: sería muy bueno poder mostrar una imagen de esto. ¿Cuál es el límite de la paginación por defecto?
--->

## Búsqueda

Para buscar algún valor en particular, puede escribirse sobre la caja de búsqueda que se encuentra en la esquina superior derecha. Al comenzar a escribir, los valores serán filtrados automáticamente para mostrar los resultados.

![Búsqueda](./Images/Legajos/05-busqueda.png)

De la misma forma que cuando se muestran todos los resultados, al efectuar una búsqueda de filtrado, se verá una leyenda que indica cuántos registros se encuentran actualmente visibles del total, y se indicará además que se trata de un filtrado.

El filtrado se aplica sobre cualquiera de las columnas de la tabla.

## Botones de exportación

En la esquina superior izquierda se encuentran varios botones que permiten la exportación de los datos visualizados en distintos formatos. Estos son: *Imprimir*, *Copiar*, *PDF*, y *Excel*.

### Imprimir

Al presionar el botón imprimir se mostrará un mensaje indicando que la tabla se ha acomodado para una óptima impresión. Los menúes y mensajes adicionales serán retirados de la pantalla, para permitir imprimir los contenidos de la tabla.

Para efectuar la impresión, se debe iniciar la impresión desde el navegador que se esté utilizando.

![Visualización de impresión](./Images/Legajos/06-impresion.png)

Una vez que se haya efectuado la impresión, puede cancelarse este modo de visualización presionando la tecla *Escape*.

### Copiar

Para copiar la información visualizada en pantalla y poder compartirla con otros programas, el botón de Copiar permitirá hacerlo, a través del portapapeles.

Una vez que el botón se presiona, una confirmación de la información copiada se mostrará en pantalla. Esta confirmación incluirá el número de filas copiadas, que coincidirá con el número de filas visualizadas actualmente.

![Confirmación de copiado](./Images/Legajos/07-copiar.png)

Una vez realizado esto, la información se encontrará disponible en el portapapeles para ser pegada en otra aplicación. El formato de esta información copiada será de texto separado por tabulaciones.

### PDF

La exportación a PDF permitirá generar un archivo en formato de documento portable (PDF). La información en pantalla será llevada a ese formato en orientación vertical.

![Ejemplo de exportación PDF](./Images/Legajos/08-pdf.png)

Notesé que el formato del PDF generado actualmente no se encuentra optimizado para datos de longitud ni para mostrar información de contexto (como el día en el que se generó, o la búsqueda efectuada). Esta funcionalidad se agregará en un futuro al sistema.

### Excel

La opción de exportación a Excel permitirá descargar un archivo *.csv*, siglas de *Comma Separated Values* (Valores separados por comas). Este formato estándar permite interactuar con varios programas de manejo de datos, incluyendo el popular Microsoft Excel.

![Exportación por defecto a CSV](./Images/Legajos/09-csv.png)

Una vez que estos archivos se encuentren en su programa, usted puede manipularlos a gusto, generar reportes útiles, incluso filtrar o manipular la información y darle formato.

![Ejemplo de utilización de datos exportados a Excel](./Images/Legajos/10-csv-reporte.png)

En el ejemplo anterior, se puede apreciar cómo se le ha dado formato a la exportación generada y cómo se ha generado un reporte automático utilizando tablas pivote en base a los datos de la tabla.