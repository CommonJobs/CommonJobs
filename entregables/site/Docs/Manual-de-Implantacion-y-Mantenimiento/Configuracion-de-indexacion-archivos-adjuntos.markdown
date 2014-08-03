# Configuracion de indexacion archivos adjuntos

El CommonJobs utiliza [IFilter](http://msdn.microsoft.com/en-us/library/ms691105) para la [indexación de archivos adjuntos](../Manual-de-Sistema/Indexacion-archivos-adjuntos), por ejemplo curriculums de postulantes, entrevistas técnicas, evaluaciones o cualquier otro documento subido al sistema.

IFilter es un plugin que permite extraer contenido y metadatos de diferentes formatos de archivos (documentos, emails, adjuntos de emails, entradas de bases de datos, metadatos de archivos de audio o imágenes), de esta manera hace posible indexar archivos que no son texto plano.

Es posible instalar filtros de manera de soportar más tipos de archivos, instalando [Citeknet IFilterExplorer](http://www.citeknet.com/Products/IFilters/IFilterExplorer/tabid/62/Default.aspx) en el servidor del sistema es posible verificar los filtros instalados.

Además de los filtros que Windows Server 2008 trae de serie, recomendamos la instalación de los siguientes filtros:

* [Adobe PDF IFilter v6.0](http://www.adobe.com/support/downloads/detail.jsp?ftpID=2611)
* [Office 2010 FilterPack](http://www.microsoft.com/download/en/details.aspx?id=17062)
* [Citeknet ZIP IFilter](http://www.citeknet.com/Products/IFilters/ZIPIFilter/tabid/69/Default.aspx)

![ ](Images/IFilters.jpg)

![ ](Images/IFilters2.jpg)

