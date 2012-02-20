# Generación de la documentación

Optamos entre parte de nuestro proceso, incorporar algún método de generación automática de formatos de documentación, para poder fomentar la generación de documentación de forma espontánea, sin la preocupación de tener que reformatear la documentación existente.

Para esto se evaluaron varias opciones, entre las cuales, una de las mejores oportunidades fue la utilización del formato [Markdown][Markdown] para la escritura de la documentación. La simpleza de este formato permite poder interactuar con los textos de una forma natural sin perder tiempo en el formateado de las mismas.

Por otro lado, las herramientas encontradas, específicamente [MultiMarkDown to PDF][mmd2pdf] (MMD2PDF) permitió la generación de documentos PDFs a partir de la estructura basada en los documentos originales.

Al momento de escritura, la documentación puede estilizarse con la utilización de estilos CSSs sobre archivos HTMLs intermedios generados desde el Markdown directamente. Tras esta conversión a CSS, la estructura jerárquica del HTML permite incorporar en la versión PDF una tabla de contenidos automatizada, basada en los niveles originales de títulos escritos en Markdown.

Aún está pendiente como tarea la estilización específica de estos CSSs y la personalización de la tabla de contenidos generada, tarea que requiere de cierto análisis debido a que esta capacidad ha sido una capacidad añadida a uno de los subsistemas de MMD2PDF ([wkhthmltopdf][wkhtmltopdf]) en una versión muy reciente, y por tanto se requiere su prueba y análisis de integración con el otro subsistema de MMD2PDF.
 
Cabe destacar que todas estas herramientas se encuentran en el dominio open source y en actual desarrollo activo, permitiendo la contribución de la comunidad, y del equipo mismo, si esto fuera necesario.

[Markdown]: http://en.wikipedia.org/wiki/Markdown
[mmd2pdf]: http://code.google.com/p/mmd2pdf/
[wkhtmltopdf]: http://code.google.com/p/wkhtmltopdf/