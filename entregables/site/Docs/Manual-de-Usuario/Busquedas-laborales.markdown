# Búsquedas Laborales

La sección de **Búsquedas** de CommonJobs permitirá administrar el contenido público de búsquedas laborales, ver qué postulantes presentados son los más aptos según los requerimientos de cada búsqueda, y proveer URLs amigables para compartir en distintas redes sociales.

## Pantalla principal

La pantalla principal muestra las búsquedas que han sido cargadas en el sistema, lo que no necesariamente implica que todas ellas se encuentren disponibles públicamente.

![Pantalla principal](Images/Busquedas-laborales/01-pantalla-principal.png)

Cada una de las búsquedas se encuentra identificada por una tarjeta, que muestra la siguiente información:

- El título de la búsqueda
- La URL amigable elegida para esta búsqueda (si es que posee alguna)
- Si la búsqueda laboral se encuentra publicada o no

Haciendo click sobre cada una de ellas, podremos acceder a su detalle.

## Agregar búsqueda laboral

Haciendo click en la tarjeta de *Agregar nueva...* podremos agregar una nueva búsqueda laboral. Inmediatamente seremos llevados a la página de detalle para completar los datos de la misma.

![Nueva búsqueda](Images/Busquedas-laborales/02-nueva-busqueda.png)

## Pantalla de detalle

La pantalla de detalle de una búsqueda posee campos para la inserción de los distintos datos sobre la misma y el cálculo de los requerimientos necesarios para sugerir postulantes adecuados.

Estos campos se encuentran divididos en distintas secciones que veremos a continuación.

### Acciones

![Acciones](Images/Busquedas-laborales/12-acciones.png)

En el detalle de una búsqueda, algunas acciones se puede realizar que afecten a la totalidad de la búsqueda. Las primeras tres se relacionan a los campos de la búsqueda y sus datos:

- **Guardar cambios** sólo estará disponible si se han efectuado cambios en los datos de la búsqueda. En este caso, presionarlo guardará esos cambios como parte del perfil.
- **Descartar cambios**, por otro lado, descartará cualquier cambio actual efectuado en los datos de la búsqueda y restaurará los campos a los datos que se encuentran almacenados para la misma.
- **Editar todos** cambiará el estado de estos campos para comenzar la edición de los mismos. Esta posibilidad es útil para editarlos todos a la vez sólo con el uso del teclado.  

Por otro lado, la siguiente acción afectan a la búsqueda en su totalidad:

- **Eliminar búsqueda:** eliminará la búsqueda de la base de datos.

### Sección principal

![Detalle: Sección principal](Images/Busquedas-laborales/03-detalle-principal.png)

La sección principal permite visualizar y modificar los siguientes datos:

- **Título de la búsqueda:** Título interno utilizado para identificar a esta búsqueda. Es sólo para referencia de los usuarios del sistema CommonJobs.
- **URL amigable de la búsqueda:** Comúnmente referido en inglés como *slug*, la última porción de la URL es editable a gusto para tener una búsqueda más alcanzable a través de redes sociales.
- **Publicada:** La marca a la derecha indicará si la búsqueda se encuentra actualmente publicada o no. Se verá en gris y vacía si no lo está, se verá en verde y rellena si es que lo está.

### Notas públicas

![Detalle: Notas públicas](Images/Busquedas-laborales/04-detalle-notas-publicas.png)

La sección de notas públicas permite ingresar detalles sobre la búsqueda que se desea sea visto por los postulantes que lleguen al sitio público. Este detalle puede ser tan extenso o tan escueto como se prefiera, y se permite la edición con formato para la misma. 

### Información privada

![Detalle: Información privada](Images/Busquedas-laborales/05-detalle-informacion-privada.png)

La sección de información privada contiene dos campos: el de conocimientos técnicos requeridos, que permitirá determinar los requerimientos mínimos para esta búsqueda (agregando, quitando o modificando requerimientos técnicos) y notas: que permitirá mantener notas internas relativas a la búsqueda.

### Postulantes sugeridos

![Detalle: Postulantes sugeridos](Images/Busquedas-laborales/06-postulantes-sugeridos.png)

Por último, hay una sección de sólo lectura denominada *Postulantes sugeridos*. Esta sección mostrará postulantes del bancos de postulantes del sistema que cumplan con los requerimientos necesarios para esta búsqueda. Los postulantes estarán ordenados de más a menos recomendado, estando el postulante con más habilidades primero.

Junto a ellos, se enumera cada una de las habilidades técnicas que el postulante tiene, junto a un indicador del nivel de dicha habilidad. Actualmente estas son las referencias:

- Verde: avanzado
- Amarillo: Intermedio
- Rojo: básico
- Vacío: no posee esa habilidad / no ha sido evaluada

Cada nombre de postulante es un hipervínculo que permite navegar a la pantalla del perfil del postulante para ver o editar sus datos.

## Publicación de una búsqueda

Cuando una búsqueda es indicada como pública, se encuentra disponible desde una subsección de los sitios públicos. Estos sitios públicos deberían estar amalgamados con los sitios institucionales.

![Búsqueda pública](Images/Busquedas-laborales/07-busqueda-publica.png)

La búsqueda pública permitirá al postulante que llegue a ella ver la información publicada y enviar sus propios datos para ser evaluados para el puesto.

![Búsqueda pública: rellena](Images/Busquedas-laborales/08-busqueda-rellena.png)

Cuando un postulante rellena el formulario de contacto y envía los datos, verá un mensaje de confirmación indicando que su operación fue exitosa.

![Búsqueda pública: agradecimiento](Images/Busquedas-laborales/09-busqueda-gracias.png)

Por otro lado, el postulante será creado en el sistema, los datos serán grabados tal cual han sido ingresados por el postulante y se añadirá al postulante con sus habilidades a los postulantes sugeridos de la búsqueda (si es que cumple con los requisitos mínimos).

![Búsqueda pública: añadido a los postulantes sugeridos](Images/Busquedas-laborales/10-sugeridos-adicion.png)

![Búsqueda pública: perfil autocreado](Images/Busquedas-laborales/11-perfil-autocreado.png)