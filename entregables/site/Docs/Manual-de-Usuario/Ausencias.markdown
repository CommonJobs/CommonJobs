# Ausencias

El módulo de ausencias permite tener una visión sumarizada de la asistencia del personal, y a la vez de períodos de ausencia. Esto permite una fácil planificación de vacaciones y períodos disponibles de vacaciones para distintos empleados de la empresa. A la vez permite identificar fácilmente empleados que han tenido poca asistencia durante el año, y las razones por las que eso haya ocurrido.

## Listado de ausencias

![Listado de ausencias](Images/Ausencias/01-listado.png)

El listado de ausencias será una primera vista a las ausencias registradas para los empleados en el corriente año. Todos los empleados se listarán por defecto y la siguiente información se mostrará para cada uno de ellos:

- **Nombre:** Apellido y nombre del empleado.
- **Completas (Compl.):** Cantidad de ausencias completas, en días.
- **Parciales (Parc.):** Cantidad de ausencias parciales, es decir, que no han comprendido la totalidad del día laboral.
- **Remoto (Rem.):** Cantidad de ausencias totales en las que el empleado ha trabajado de forma remota.
- **Vacaciones (Vac.):** Cantidad de días que el empleado se ha tomado como vacaciones durante el corriente año. (Nota: Esto no se relaciona al período al que corresponden las vacaciones, sino a la fecha real en donde fueron tomadas.)
- **Grilla de ausencias:** La grilla siguiente mostrará todo el corriente año, día por día, agrupado por mes. En cada una de las casillas del día, se mostrará la siguiente información:
	- Si la celda está **vacía:** el empleado acudió y trabajó de forma normal.
	- Si la celda está **resaltada levemente sin contenido:** se trató de un día no laboral. Al momento de escritura el sistema no está considerando feriados nacionales ni fiestas religiosas, sino sólo Sábados y Domingos.
	- Si la celda está **resaltada oscura sin contenido:** se trató de un día en el cual el empleado aún no estaba contratado.
	- Si la celda está **rellena hasta la mitad con un color:** se trató de una ausencia parcial. Cada color indica una razón particular.
	- Si la celda está **rellena totalmente con un color:** se trató de una ausencia parcial. Cada color indica una razón particular.
	- Si la celda está **marcada con una barra diagonal:** se trató de un día de trabajo remoto.
	- Si la celda está **marcada con una barra horizontal:** se trató de un día de vacaciones otorgado al empleado. 

![Referencia de tipos de ausencia](Images/Ausencias/02-referencias.png)

## Filtros y ordenamiento

El campo de búsqueda arriba a la derecha puede utilizarse para filtrar el listado de empleados que es visualizado en la pantalla.

Puede usarse cualquier parte del texto de los nombres y apellidos de los empleados, como también los totales que son mostrados en el año corriente.

Cuando la grilla se carga por primera vez, los empleados serán ordenados por apellido y nombre, pero es posible cambiar ese ordenamiento.

Haciendo click en la cabecera de una columna se puede indicar una dirección de ordenamiento para esa misma columna, que reemplazará el ordenamiento actual. La dirección del ordenamiento será indicado con una flecha próxima al título de la columna. Las celdas que contienen días no pueden ser utilizadas para el ordenamiento.

Una flecha hacia arriba indicará un ordenamiento de menor a mayor (números) y alfabético (para textos). Una flecha hacia abajo indica la dirección opuesta: mayor a menor (números) y alfabético inverso (textos).

![Lista filtrada y ordenada](Images/Ausencias/03-lista-filtrada-y-ordenada.png)

## Navegación

También pueden visualizarse otros años haciendo click en los links arriba y abajo, cerca del indicador del año actual. Estos nos mostrarán el mismo listado con la información correspondiente a años anteriores o posteriores al visualizado actualmente.

## Información sobre la ausencia

Para ver información sobre una ausencia en particular, sólo es necesario posicionar el cursor encima de la celda que indica esa ausencia. De esta forma, un *tooltip* se hará presente que mostrará la información cargada para dicha ausencia, junto con links al perfil del empleado y a los adjuntos que se hayan presentado relevantes a esa ausencia.

![Detalle de una ausencia](Images/Ausencias/04-detalle.png)