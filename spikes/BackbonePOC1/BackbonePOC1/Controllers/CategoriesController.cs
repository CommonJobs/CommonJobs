using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BackbonePOC1.Models;
using System.Collections;

namespace BackbonePOC1.Controllers
{
    public class CategoriesController : Controller
    {
        private IEnumerable EnumerationAsList<T>() where T : struct
        {
            return EnumerationAsList(typeof(T));
        }

        private IEnumerable EnumerationAsList(Type type) 
        {
            return Enum.GetValues(type).Cast<object>().Select(x => new { Value = (int)x, Name = x.ToString() });
        }

        private object Enumerations()
        {
            return new
            {
                ResultsViewType = EnumerationAsList<ResultsViewType>(),
                ProductType = EnumerationAsList<ProductType>(),
                FacetFiltersOrderType = EnumerationAsList<FacetFiltersOrderType>(),
                FacetFiltersOrderDirection = EnumerationAsList<FacetFiltersOrderDirection>()
            };
        }

        private ProductTypeConfiguration[] Categories()
        {
            return new[]
                {
                    new ProductTypeConfiguration { ProductType = ProductType.All, ResultsViewType = Models.ResultsViewType.List },
                    new ProductTypeConfiguration { ProductType = ProductType.Processor, ResultsViewType = Models.ResultsViewType.Grid },
                    new ProductTypeConfiguration { ProductType = ProductType.GraphicCard, ResultsViewType = Models.ResultsViewType.Grid },
                    new ProductTypeConfiguration { ProductType = ProductType.Motherboard },
                    new ProductTypeConfiguration { ProductType = ProductType.Desktop },
                    new ProductTypeConfiguration { ProductType = ProductType.Notebook },
                    new ProductTypeConfiguration { ProductType = ProductType.Server },
                    new ProductTypeConfiguration { ProductType = ProductType.Workstation },
                };
        }

        private void RegisterJsGlobal(string name, object obj)
        {
            var dict = ViewBag.JsGlobals as Dictionary<string, object>;
            if (dict == null)
                ViewBag.JsGlobals = new Dictionary<string, object>();
            ViewBag.JsGlobals[name] = obj;
        }

        public JsonResult JsonList()
        {
            return Json(
                new[]
                {
                    new ProductTypeConfiguration { ProductType = ProductType.All, ResultsViewType = Models.ResultsViewType.List },
                    new ProductTypeConfiguration { ProductType = ProductType.Processor, ResultsViewType = Models.ResultsViewType.Grid },
                    new ProductTypeConfiguration { ProductType = ProductType.GraphicCard, ResultsViewType = Models.ResultsViewType.Grid },
                    new ProductTypeConfiguration { ProductType = ProductType.Motherboard },
                    new ProductTypeConfiguration { ProductType = ProductType.Desktop },
                    new ProductTypeConfiguration { ProductType = ProductType.Notebook },
                    new ProductTypeConfiguration { ProductType = ProductType.Server },
                    new ProductTypeConfiguration { ProductType = ProductType.Workstation },
                },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            RegisterJsGlobal("App", new { });
            RegisterJsGlobal("App.Enumerations", Enumerations());
            RegisterJsGlobal("App._Categories", Categories());
            return View();
        }

    }
}
