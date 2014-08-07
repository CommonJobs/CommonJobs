# Autenticación de CommonJobs

Como se explica en la [retrospectiva relacionada](../Proyecto/Retrospectiva/Diseno-de-autenticacion-y-autorizacion), al momento de la redacción de este documento, el sistema se encuentra configurado para utilizar _Google Authentication_ para la autenticación de usuarios. No se descarta volver a utilizar _Active Directory_ en un futuro.

En este documento se describe el diseño utilizado para ambos sistemas de autenticación y autorización, para detalles acerca de la configuración y grupos hay más información disponible en el [Manual de Implantación y Mantenimiento](../Manual-de-Implantacion-y-Mantenimiento/Autenticacion-y-Autorizacion).


## Uso de autenticación y autorización dentro de CommonJobs

En la interfaz encargada de la implementación de los accesos (la interfaz web, `CommonJobs.MVC.UI`), se usa en los distintos controladores el atributo `CommonJobsAuthorize`, pasando como parámetros los roles que los usuarios pueden tener para acceder a dicha funcionalidad.

Al tratarse de atributos de autenticación, estos pueden encontrarse tanto al nivel de controlador como al nivel de método de controlador (acción).

    public class AttachmentsController: CommonJobsController {
        [HttpPost]
        [CommonJobsAuthorize("Users,ApplicationManagers,EmployeeManagers")]
        public ActionResult Post(string id) {
            ...
        }   
    }

La configuración del sistema elegido para manejar la configuración de `CommonJobsAuthorize` se realiza seteando el valor de la propiedad estática `CommonJobsAuthorizeAttribute.AuthorizationBehavior` (de tipo `IAuthorizationBehavior`).

## Implementación

Al tratarse de funcionalidad compartida, la implementación de la funcionalidad descrita se encuentra en el proyecto `CommonJobs.Infrastructure.MVC`.

El atributo `CommonJobsAuthorize` no es más que una implementación de `AuthorizeAtribute`, cuya explicación está más allá del propósito de este documento. La funcionalidad se encuentra delegada a la interfaz `IAuthorizationBehavior`, para las cuales se encuentran implementadas tres clases:

- `PrefixAuthorizationBehaviorBase`: Clase abstracta que permitirá, tras un prefijo, identificar los roles o grupos de Active Directory. Esto permite, por ejemplo, ignorar por configuración cualquier grupo que comience con `CommonJobs_DEV`.
- `SessionAndExternalRolesAuthorizationBehavior`: Clase que delegará en otra el recuperado de roles (lo que permite fuentes externas distintas) y utilizará `httpContext.Session` para cachear los roles del usuario actual. En la aplicación actual, se enlaza la función `GetRoles` con el acceso a base de datos para que se obtenga desde RavenDB el registro del usuario con los permisos pertinentes.
- `SessionRolesAuthorizationBehavior`: Una versión menos parametrizable de `SessionAndExternalRolesAuthorizationBehavior` que utilizará `httpContext.Session` para recuperar información de los roles del usuario actual.

En el momento en que Active Directory se encontraba en uso como sistema principal de autenticación, estas llamadas faltantes simplemente debían obtener información del usuario desde `httpContext.User.Identity` y obtener desde Active Directory los grupos a los que esta persona pertenecía.

Esto era suficiente para que la aplicación pudiera identificar los roles actuales del usuario loggeado.

