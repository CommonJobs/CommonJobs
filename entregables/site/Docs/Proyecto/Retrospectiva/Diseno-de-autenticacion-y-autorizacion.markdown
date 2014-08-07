# Diseño de autenticación y autorizacion (retrospectiva)

Si bien este es solo uno más de los aspectos del diseño de la aplicación, creemos que se justifica su inclusión en la sección de retrospectiva ya que su capacidad de adaptación fue puesta a prueba antes de terminar el desarrollo del sistema. 

Originalmente era un _importante_ requerimiento de seguridad del cliente la utilización de _Active Directory_ como proveedor de autenticación. El sistema ya estaba en producción utilizando este sistema de autenticación cuando por un problema en la infraestructura del cliente, se requirió urgentemente utilizar otro tipo de autenticación.

La flexibilidad del diseño implementado permitía realizar una migración ordenada hacia otra tecnología. Se decidió utilizar _Google Authentication_ ya que cada empleado tiene un email con el dominio de la empresa utilizando el servicio de _Google Apps_. Además, como backup, permitiríamos autenticación con usuario y password almacenados en el sistema.

El uso de estos distintos hooks permitía delegar la autenticación y la obtención de los roles del usuario, lo que resultó en un cambio que no afectaba al sistema en sí en su proceso de autenticación, sino simplemente en la implementación que recuperaría los datos del usuario.

Tras verificar que el usuario se encuentra loggeado con su email corporativo, gracias a la autenticación que Google provee (estándar OAuth y OAuth 2.0), el sistema puede identificar al usuario, y gracias a la información de usuario disponible en la base de datos, se pueden identificar los roles asignados al usuario, e incluso el empleado correspondiente al usuario actualmente loggeado. 

Al momento de la escritura de este documento, la relación entre usuario y empleado solo se ha utilizado para que los empleados completen sus preferencias en el sub-sistema de administración de almuerzos, en un futuro podría ser aprovechada para la implementación de una interfaz donde los empleados completan sus propios perfiles o lo usan como oportunidad para discusiones con el área de recursos humanos -- todas ideas tenidas en cuenta durante el desarrollo del proyecto, pero que no fueron parte del backlog a implementar.

La implementación de un sistema de autenticación con el estándar OAuth o la descripción de este estándar quedan fuera del propósito de este documento, pero de ser necesaria su consulta, puede observarse en `CommonJobs.MVC.UI.Controllers.GoogleAuthenticationController`.

Estamos muy satisfechos con el resultado, ya que nuestro diseño demostró respetar el [principio de Open/Closed](http://en.wikipedia.org/wiki/Open/closed_principle) como puede observase en el documento sobre el [Diseño de autenticación y autorización](../../manual-de-sistema/diseno-autenticacion-y-autorizacion).
