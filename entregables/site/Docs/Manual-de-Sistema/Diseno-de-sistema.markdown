# Diseño del sistema

## Vista de paquetes

Distintos aspectos de CommonJobs se organizan internamente de distintas maneras. En esta parte se proveerá una introducción inicial al diseño del sistema, que se discutirá en mayor detalle más adelante.

Para la vista de paquetes podemos separar al sistema en dos grandes ramas, dependiendo de su dependencia con el dominio de la aplicación o de su reusabiliidad a través de otros sistemas. Esta clasificación divide los distintos componentes en _Domain dependent_ y _Domain independent_.

![Vista de paquetes](Images/ComponentDiagram.png)

## Descripción de paquetes

A continuación, una breve descripción de cada uno de los paquetes que conforman el sistema CommonJobs. Estos se corresponden tanto a namespaces como a proyectos existentes en la solución de Visual Studio.

- **CommonJobs:** Contenedor general para la aplicación.
    - **CommonJobs.Application:** Estructuras y clases comunes para su reuso dentro de la lógica de la aplicación. Búsquedas, queries comunes, actualización de registros.
        - **CommonJobs.Application.MyMenu:** Commandos y consultas comunes para el uso de la aplicación en la sección de MyMenu.
        - **CommonJobs.Application.Profiles:** Comandos y consultas comunes para el uso de la aplicación en respecto a perfiles de usuario.
        - **CommonJobs.Application.Test:** Tests de funcionalidad común a la aplicación.
    - **CommonJobs.Domain:** Contenedor de clases de dominio.
        - **CommonJobs.Domain.MyMenu:** Definición de clases particulares para la sección de MyMenu.
        - **CommonJobs.Domain.Profiles:** Definición de clases particulares para los perfiles de usuario.
        - **CommonJobs.Domain.Test:** Tests para el comportamiento de las clases de dominio.
    - **CommonJobs.Migrations:** Contenedor para la lógica de las migraciones de sistema de una versión a otra.
    - **CommonJobs.MVC:** Contenedor para las partes de la aplicación que interactúan con el usuario bajo la plataforma de ASP.NET MVC.
        - **CommonJobs.MVC.UI:** GUI ASP.NET MVC de la aplicación CommonJobs.
        - **CommonJobs.MVC.PublicUI:** GUI ASP.NET MVC de la versión pública de CommonJobs, utilizada para que los postulantes auto-completen sus perfiles.
    - **CommonJobs.ContentExtraction:** Contenedor y clases comunes para la extracción de contenidos desde archivos.
        - **CommonJobs.ContentExtraction.Extractors:** Implementaciones propias de CommonJobs de extractores de contenido.
        - **CommonJobs.ContentExtraction.IFilterExtraction:** Llamadas a APIs de Windows para obtener y utilizar extractores de contenidos IFilter instalados en el sistema.
            - **CommonJobs.ContentExtraction.IFilterExtraction.Test:** Pruebas de extractores y verificación de su comportamiento esperado.
    - **CommonJobs.Infrastructure:** Contenedor de clases necesarias para la estructura del sistema y la interacción con frameworks de terceros.
        - **CommonJobs.Infrastructure.Migrations:** Implementación de las clases necesarias para el uso de migraciones.
        - **CommonJobs.Infrastructure.MVC:** Implementación de las clases necesarias para corregir y ampliar el comportamiento de ASP.NET MVC.
            - **CommonJobs.Infrastructure.MVC.Test:** Pruebas de las clases de interacción con ASP.NET MVC.
        - **CommonJobs.Infrastructure.RavenDB:** Implementación de las clases necesarias para interactuar con RavenDB.
    - **CommonJobs.JavaScript:** Implementación de clases necesarias para la reutilización de lógica JavaScript del lado del servidor.
    - **CommonJobs.Utilities:** Utilidades genéricas reusadas por toda la aplicación, como construcción de expresiones, logging, manejo de cadenas y codificaciones, etc.
        - **CommonJobs.Utilities.Test:** Pruebas de las utilidades genéricas del sistema.

## CQRS

Gran parte del sistema se encuentra, en distintas secciones, separado bajo el concepto del patrón arquitectónico *CQRS* (*Command-Query Responsibility Segregation*). Esto permite utilizar en muchas secciones las mejores características propias de la API de RavenDB.

La mayoría de las clases presentes en *CommonJobs.Application* y los paquetes contenidos en este encapsulan la responsabilidad de estos comandos (ejemplo: *CommonJobs.Application.MyMenu.ProcessMenuCommand*) o de estas consultas (ejemplo: *CommonJobs.Application.ApplicantSearching.SearchApplicants*).

Las clases de la aplicación que exponen la funcionalidad en alguna forma de UI pueden entonces simplemente reutilizar estas clases, permitiendo a la aplicación extenderse simplemente a otras interfaces, o a aislar el comportamiento de forma necesaria para que pudieran actualizarse o cambiarse los sistemas con los que estos interactúan.

## Diseño de arquitectura del sistema

![Diagrama de Arquitectura](Images/ArchitectureDiagram.png)

Simplificando el conjunto de componentes que se ha descripto anteriormente, un diagrama de arquitectura del sistmema surge fácilmente en donde se pueden ver las dependencias de estas distintas capas del sistema.

Este diagrama, sin embargo, no habla sobre las dependencias que son independientes en una implantación y pueden comunicarse a través de distintos servidores.

## Diagrama de implantación

Si bien al momento de escritura el sistema se encuentra corriendo en un único servidor, CommonJobs soporta la separación de sus componentes en distintos servidores virtuales o físicos, siempre y cuando puedan comunicarse.

![Diagrama de implantación](Images/DeploymentDiagram.png)