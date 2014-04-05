# Introducción

Creemos que para asegurar el éxito de este proyecto, es decir que el sistema cumpla su cometido de ser útil al área de recursos humanos de la empresa, es muy importante la implantación temprana. De esta manera no solo comenzará a generar valor lo antes posible sino que además permitirá al cliente tener una visión mas cercana del producto, y podrá guiarnos mejor para satisfacer sus necesidades.


## Entornos

Además del entorno de desarrollo propio de cada uno de los desarrolladores del sistema, deberemos configurar un entorno para pruebas preliminares (en adelante DEV) y un entorno para el sistema en producción (en adelante PROD).

Los servidores y recursos para ambos entornos son provistos por la empresa.


## Flujo de desarrollo y despliegue

A medida que agreguemos características o resolvamos errores en el sistema, iremos _mergeando_ el código relacionado en la [rama `master` de nuestro repositorio de código](https://github.com/CommonJobs/CommonJobs). 

Al finalizar el _sprint_ o cuando nos parezca conveniente, realizaremos el despliegue de esa rama en nuestro entorno DEV (en algunos casos puntuales, por ejemplo para demos o consultas, podremos desplegar en DEV código aún no mergeado en la rama `master`).

El cliente evaluará los cambios en el entorno DEV y decidirá cuando serán desplegados en el entorno PROD.


## Consideraciones de Seguridad

Se asumirá que en el entorno DEV no se cargará ningún dato confidencial, privado o sensible, por lo que la seguridad del mismo no es crítica. Sin embargo, se tomarán consideraciones de seguridad para probar características relacionadas y emular de manera más fiel al entorno PROD.

En el entorno PROD, en cambio, se almacenarán datos confidenciales, privados y sensibles, de manera que solo serán accesibles a los usuarios reales del sistema y el administrador de sistemas de la empresa. Los desarrolladores no deberán tener acceso a los mismos.