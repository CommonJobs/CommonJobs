# Autenticación de CommonJobs

La aplicación se encuentra configurada al momento de la entrega del proyecto para poder utilizar un sistema de autenticación: Google Auth, recuperando información del usuario desde la base de datos. Active Directory no es presentemente utilizado pero ha sido uno de los puntos fuertes en las primeras fases del proyecto.

A continuación se describirá el diseño utilizado para la integración de estos sistemas. La configuración de Active Directory, los distintos grupos y los permisos que estos otorgan se encuentran documentados en [la documentación de administración de Active Directory](../Manual-de-Sistema/Autenticacion-y-Autorizacion).

## Uso de autenticación y autorización dentro de CommonJobs

En la interfaz encargada de la implementación de los accesos (la interfaz web, `CommonJobs.MVC.UI`), se usa en los distintos controladores el atributo `CommonJobsAuthorize`, pasando como parámetros los roles que los usuarios pueden tener para acceder a dicha funcionalidad.

Al tratarse de atributos de autenticación, estos pueden encontrarse tanto al nivel de controlador como al nivel de método de controlador (acción).

```csharp
public class AttachmentsController: CommonJobsController {
    [HttpPost]
    [CommonJobsAuthorize("Users,ApplicationManagers,EmployeeManagers")]
    public ActionResult Post(string id) {
        ...
    }   
}
```

## Implementación

Al tratarse de funcionalidad compartida, la implementación de la funcionalidad descripta se encuentra en el proyecto `CommonJobs.Infrastructure.MVC`.

El atributo `CommonJobsAuthorize` no es más que una implementación de `AuthorizeAtribute`, cuya explicación está más allá del propósito de este documento. La funcionalidad se encuentra delegada a la interfaz `IAuthorizationBehavior`, para las cuales se encuentran implementadas tres clases:

- `PrefixAuthorizationBehaviorBase`: Clase abstracta que permitirá, tras un prefijo, identificar los roles o grupos de Active Directory. Esto permite, por ejemplo, ignorar por configuración cualquier grupo que comience con `CommonJobs_DEV`.
- `SessionAndExternalRolesAuthorizationBehavior`: Clase que delegará en otra el recuperado de roles (lo que permite fuentes externas distintas) y utilizará `httpContext.Session` para cachear los roles del usuario actual. En la aplicación actual, se enlaza la función `GetRoles` con el acceso a base de datos para que se obtenga desde RavenDB el registro del usuario con los permisos pertinentes.
- `SessionRolesAuthorizationBehavior`: Una versión menos parametrizable de `SessionAndExternalRolesAuthorizationBehavior` que utilizará `httpContext.Session` para recuperar información de los roles del usuario actual.

En el momento en que Active Directory se encontraba en uso como el sistema principal de autenticación, estas llamadas faltantes simplemente debían obtener información del usuario desde `httpContext.User.Identity` y obtener desde Active Directory los grupos a los que esta persona pertenecía.

Esto era suficiente para que la aplicación pudiera identificar los roles actuales del usuario loggeado.

## Migración a Google Auth / usuarios en base de datos

Tras que cambios estructurales en la empresa requirieran que no se pudiera utilizar más Active Directory como el punto principal de autenticación, la flexibilidad construida permitió hacer una migración gradual de un sistema de autenticación a otro.

El uso de estos distintos hooks permitía delegar la autenticación y la obtención de los roles del usuario, lo que resultó en un cambio que no afectaba al sistema en sí en su proceso de autenticación, sino simplemente en la implementación que recuperaría los datos del usuario.

Tras verificar que el usuario se encuentra loggeado con un email de MakingSense, gracias a la autenticación que Google provee (estándar OAuth y OAuth 2.0), el sistema puede identificar al usuario, y gracias a la información de usuario disponible en el sistema, se pueden identificar los roles asignados al usuario, e incluso el empleado correspondiente al usuario actualmente loggeado. Al momento de la escritura de este documento, dicha relación no se ha utilizado todavía, pero bien podría ser aprovechada la la implementación de un escenario en donde los empleados completan sus propios perfiles o lo usan como oportunidad para discusiones con el área de recursos humanos -- todas ideas tenidas en cuenta durante el desarrollo del proyecto, pero que no fueron parte del backlog a implementar.

La implementación de un sistema de autenticación con el estándar OAuth o la descripción de este estándar quedan fuera del propósito de este documento, pero de ser necesaria su consulta, puede observarse en `CommonJobs.MVC.UI.Controllers.GoogleAuthenticationController`.