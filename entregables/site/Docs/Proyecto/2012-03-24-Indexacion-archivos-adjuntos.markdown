# Indexación de archivos adjuntos

Dado que _CommonJobs_ debe permitir buscar en el contenido de los archivos adjuntos, por ejemplo curriculums, y estos pueden tener formatos muy diferentes se decidió realizar el siguiente diseño.

## Interface para extracción de contenido

Cuando se realiza el upload de un archivo, este es analizado por una lista de extractores que satisfacen la interface IContentExtractor:

    public interface IContentExtractor
    {
        bool TryExtract(string fullPath, Stream stream, string fileName, out ExtractionResult result);
    }

    public class ExtractionResult
    {
        public string ContentType { get; set; }
        public string PlainContent { get; set; }
    }

El sistema almacena el resultado del primer extractor exitoso de manera de que pueda ser indexado por nuestra base de datos.

De esta manera será posible crear distintos extractores de forma modular. Ahora mismo, la configuración se está realizando al inicio de la aplicación, pero en un futuro podrían utilizarse como plugins. 

## IFilter

_Microsoft_ utiliza en sus productos _Windows Indexing Service_, _Windows Desktop Search_ y _SQL Server_ la interface COM [IFilter](http://msdn.microsoft.com/en-us/library/ms691105) para la extracción de textos. 

Decidimos no utilizarla como interface principal de extracción de textos ya que nos limitaría demasiado al momento de intentar migrar el sistema a otras plataformas y además dificulta el desarrollo de filtros propios. Pero dada la gran disponibilidad de plugins IFilter, decidimos crear una implementación de IContentExtractor que lo soporte.

Para utilizar un nuevo plugin IFilter con _CommonJobs_, es suficiente con instalarlo en el equipo que hace las veces de servidor web. Para verificar que filtros están disponibles en los equipos estamos utilizando [Citeknet IFilterExplorer](http://www.citeknet.com/Products/IFilters/IFilterExplorer/tabid/62/Default.aspx). 

Pueden encontrarse gran cantidad de filtros gratutios en la Web, por ejemplo en [Citeknet](http://www.citeknet.com/Products/tabid/54/ctl/Edit/mid/381/Downloads/tabid/53/Default.aspx) o [IFilterShop](http://www.ifiltershop.com/).

### Filtros IFilter utilizados actualmente

Además de los filtros que Windows Server 2008 trae de serie, instalamos los siguientes:

* [Adobe PDF IFilter v6.0](http://www.adobe.com/support/downloads/detail.jsp?ftpID=2611)
* [Office 2010 FilterPack](http://www.microsoft.com/download/en/details.aspx?id=17062)
* [Citeknet ZIP IFilter](http://www.citeknet.com/Products/IFilters/ZIPIFilter/tabid/69/Default.aspx)

