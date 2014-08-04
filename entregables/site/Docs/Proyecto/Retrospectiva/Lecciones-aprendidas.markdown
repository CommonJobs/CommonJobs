# Lecciones aprendidas

Creemos que la reflexión sobre el proyecto amerita destacar los aspectos que nos permitieron aprender, tanto en aspectos que fueron de buen resultado como los que no.

## Qué salió bien y por qué

- La agilidad para tomar decisiones
- El proceso
- La coordinación del equipo
- La gestión del proyecto

### La agilidad para tomar decisiones

Elegimos un proceso simple que entre sus valores principales sostenía responder a cambios ante seguir planes. Esto se evidenció cuando notamos que las necesidades de nuestro cliente no estaban siendo satisfechas de la manera correcta, con lo que se replanteó el rumbo del proyecto, solucionando el problema de forma efectiva.

Dichas evaluaciones se tomaban sprint a sprint y se identificaban riesgos, lo que permitió atacarlos antes de que afectaran al proyecto de forma irreversible.

### El proceso

El proceso era lo suficientemente simple como para que pudiéramos seguirlo, incluso mientras nos encontrábamos en aprendizaje de herramientas y tecnología, pero era lo suficientemente complet como para que cada cambio fuera auditado por todo el grupo y las decisiones se tomaran en conjunto.

Las reuniones de sprint, las pláticas con el cliente, los cambios en el código y los code reviews eran parte de este proceso que aseguraban un producto formado y revisado que pudiera ser útil y de buena calidad.

### La coordinación del equipo

Los integrantes del equipo nos mantuvimos en constante comunicación y sin motivos de vergüenza o miedo, nunca se ocultaron riesgos o problemas afectando al desarrollo normal del proyecto. Este tipo de comunicación clara permitió tomar las decisiones apropiadas y que los miembros del equipo reaccionaran ante situaciones, logrando dar constante avance al proyecto y fomentar aún más el trabajo en equipo.

### La gestión del proyecto

Nuestro proceso y herramientas elegidas también permitió mantener un control ordenado de cada requerimiento, pedido, problema y mejora encontrada, cada hora utilizada en el proyecto y cada línea de código o documento que fue parte de él. Todo se encuentra documentado y todo está disponible públicamente, incluso con los cambios a lo largo del tiempo, lo que permite auditar el proyecto en su completitud.

Esto nos permitió siempre mantener un control claro de la situación del proyecto para detectar situaciones de riesgo u oportunidades de cambio.

## Qué podría haber salido mejor y por qué

- El diseño inconsistente entre subsistemas
- El involucramiento más activo de los stakeholders
- El testing automatizado

### El diseño inconsistente entre subsistemas

Tal como fue mencionado en la [Retrospectiva del diseño](***** TODO: LINK! *****), la agilidad de cambio de nuestro proyecto no permitió generar subsistemas que tuvieran una diseño interno homogéneo, aunque esto no afectara la forma en que se integraban.

Los módulos son relativamente desacoplados, por lo que no hay necesidad que cada uno de ellos se parezca a los otros (esto abre muchas posibilidades en cuanto a la funcionalidad que los módulos podrían implementar), pero tampoco es obvio en el código la separación de cada uno de ellos.

Creemos que un futuro trabajo de refactor podría mejorar mucho este área sin afectar en lo más mínimo la funcionalidad actual. Creemos que si bien el diseño puede mejorarse, no es necesario para el crecimiento del sistema.

### El testing automatizado

Nuestra cobertura de testing automatizado, tanto de unidad como end-to-end ha sido bastante pobre. Áreas particularmente complejas del sistema poseen este tipo de testing, pero no es una práctica que se ha desarrollado de forma constante a lo largo del proyecto.