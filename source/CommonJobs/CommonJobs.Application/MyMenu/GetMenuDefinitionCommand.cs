using CommonJobs.Domain;
using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
using NLog;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class GetMenuDefinitionCommand : Command<Menu>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public string Id { get; set; }
        private string IdOrDefault
        {
            get { return string.IsNullOrWhiteSpace(Id) ? Common.DefaultMenuId : Id; }
        }

        public GetMenuDefinitionCommand(string id = null)
        {
            this.Id = id;
        }

        public override Menu ExecuteWithResult()
        {
            var menu = RavenSession.Load<Menu>(IdOrDefault);
            if (menu == null)
            {
                menu = IdOrDefault == Common.DefaultMenuId ? CreateDefaultMenu(IdOrDefault) : CreateNewMenu(IdOrDefault);
                ExecuteCommand(new UpdateMenuDefinitionCommand(menu, DateTime.Now));
            }

            return menu;
        }

        private static Menu CreateDefaultMenu(string id)
        {
            return new Menu()
            {
                Id = id,
                Title = "Menú primaveral",
                FirstWeekIdx = 2,
                WeeksQuantity = 5,
                DeadlineTime = "9:30",
                LastSentDate = new DateTime(2000, 1, 1),
                StartDate = new DateTime(2012, 9, 21),
                EndDate = new DateTime(2020, 1, 1),
                Places = new StringKeyedCollection<Place>() 
                {
                    new Place() { Key = "place_larioja", Text = "La Rioja" },
                    new Place() { Key = "place_garay", Text = "Garay" }
                },
                Options = new StringKeyedCollection<Option>() 
                {
                    new Option() { Key = "menu_comun", Text = "Común" },
                    new Option() { Key = "menu_light", Text = "Light" },
                    new Option() { Key = "menu_vegetariano", Text = "Vegetariano" }
                },
                Foods = new WeekDayOptionKeyedCollection<MenuItem>() 
                {
                    new MenuItem() { WeekIdx = 0, DayIdx = 1, OptionKey = "menu_comun", Food = "Cuarto de pollo deshuesado al champignonge c/papas" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 1, OptionKey = "menu_light", Food = "Wok de vegetales y atun al natural" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Cintas caseras de rucula c/wok de hongos  y salsa de soja" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 2, OptionKey = "menu_comun", Food = "Albondigas a la portuguesa c/pure de papas" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 2, OptionKey = "menu_light", Food = "Ensalada de rucula,champig frescos,remolacha y huevo" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Fajitas Mexicanas veg c/ batatas al horno" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 3, OptionKey = "menu_comun", Food = "Ravioles de pollo,puerro y muzarella c/ salsa mixta" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 3, OptionKey = "menu_light", Food = "Milanesas de soja c/puré de calabaza y zanahoria" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Brusquetas de verduras gratinadas,aceitunas,tomates y espinaca" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 4, OptionKey = "menu_comun", Food = "Milanesa a la suiza c/papas y cebollas al horno" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 4, OptionKey = "menu_light", Food = "Pechuga sin piel al puerro c/vegetales grillados" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Sorrentinos de calabaza y muzzarella c/salsa mixta" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 5, OptionKey = "menu_comun", Food = "Filet de merluza al roquefort c/papas al natural" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 5, OptionKey = "menu_light", Food = "Ñoquis de ricota,salsa de tomates frescos y albahaca" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Pizza individual de espinaca y muzzarella" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 1, OptionKey = "menu_comun", Food = "Sorrentinos jamon y quso c/salsa scarparo" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 1, OptionKey = "menu_light", Food = "Ensalada roja de tomate,remolacha,zanahoria,repollo colorado y atun al natural" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesa de berenjena a la napolitana c/puré mixto" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 2, OptionKey = "menu_comun", Food = "Cuarto de pollo al verdeo c/papas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 2, OptionKey = "menu_light", Food = "Brusquetas de verduras grilladas,tomates y albahaca en pan integral y semillas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Wok de vegetales hongos,brotes,semillas y salsa de soja" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 3, OptionKey = "menu_comun", Food = "Milanesa napolitana c/puré mixto" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 3, OptionKey = "menu_light", Food = "Pollo a la mostaza c/arroz integral" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Ñoquis de espicas y papas c/ salsa de puerro y tomates frescos" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 4, OptionKey = "menu_comun", Food = "Merluza horneada caprese c/puré de papas y espinacas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 4, OptionKey = "menu_light", Food = "Risotto Integral de Verduras y espinaca" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Ensalada de verdes,huevos duros,parmesano,croutons y aderezos Cesar" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 5, OptionKey = "menu_comun", Food = "Fajitas Mixtas c/papas provenzal" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 5, OptionKey = "menu_light", Food = "Ravioles de ricota en masa integral c/salsa de verduras y tomates frescos" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Calzone de muzzarella,tomates,albahaca y muzzarella" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 1, OptionKey = "menu_comun", Food = "Canelones de ricota y verduras a la bolognesa" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 1, OptionKey = "menu_light", Food = "Pollo al puerro acompañado c/verduras asadas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesas de soja a la napolitana c/puré mixto" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 2, OptionKey = "menu_comun", Food = "Arroz c/calamares" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 2, OptionKey = "menu_light", Food = "Ensalada de hojas verdes, champignones, verdeo, claras de huevo grilladas y semillas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Ñoquis de papa y espinaca c/salsa roquefort" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 3, OptionKey = "menu_comun", Food = "Muslo relleno c/ jamon y queso y verduras asadas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 3, OptionKey = "menu_light", Food = "Niños envueltos c/puré de calabaza y hierbas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Calabaza asada c/queso gratinado,choclo y puerro" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 4, OptionKey = "menu_comun", Food = "Pizza individual napolitana c/jamon y aceitunas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 4, OptionKey = "menu_light", Food = "Merluza horneada al limon y hierbas c/calabaza asada" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Ensalada de hojas verdes,tomates,muzzarella,aceitunas negras y verdeo" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 5, OptionKey = "menu_comun", Food = "Carne asada al horno con papas a las hierbas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 5, OptionKey = "menu_light", Food = "Wok de vegetales, brotes, hongos y semillas." },
                    new MenuItem() { WeekIdx = 2, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Ravioles de espinaca y parmesano c/salsa rosa" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 1, OptionKey = "menu_comun", Food = "Pollo al verdeo c/calabaza asada" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 1, OptionKey = "menu_light", Food = "Roulete integral de vegetales,brotes y semillas" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesa de zapallitos c/muzzarella.tomate y albahaca" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 2, OptionKey = "menu_comun", Food = "Lasagna a la bolognesa" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 2, OptionKey = "menu_light", Food = "Ensalada de verdes c/pollo y citronete" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Pizza individual de muzzarella y vegetales gratinados c/pan casero" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 3, OptionKey = "menu_comun", Food = "Fajitas mexicanas de ternera c/papas y batatas" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 3, OptionKey = "menu_light", Food = "Ensalada de hojas verdes, atún al natural tomates y zanahoria" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Sorrentinos de vegetales grillados y ricota c/salsa de puerro y tomates" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 4, OptionKey = "menu_comun", Food = "Suprema napolitana c/puré de papas y calabaza" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 4, OptionKey = "menu_light", Food = "Pan de carne y vegetales al wok" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Ensalada caprese c/aceitunas negras y rucula" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 5, OptionKey = "menu_comun", Food = "Matambre a la Pizza con Papas al orégano" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 5, OptionKey = "menu_light", Food = "Omelete de claras,espinacas y calabaza asada c/pan integral" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Ñoquis de calabaza c/salsa de tomates y espinacas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 1, OptionKey = "menu_comun", Food = "Calzon especial c/jamon,morrones y muzzarella" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 1, OptionKey = "menu_light", Food = "Wok de vegetales,pollo y semillas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milhojas de verdura,queso y muzzarella" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 2, OptionKey = "menu_comun", Food = "Pastel de carne y papas con queso gratinado" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 2, OptionKey = "menu_light", Food = "Pesca del día horneada al limon y hierbas c/calabaza asada" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Sorrentinos Caprese c/salsa de quesos" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 3, OptionKey = "menu_comun", Food = "Milanesa con arroz a la crema" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 3, OptionKey = "menu_light", Food = "Ensalada de espinacas frescas,champignones,semillas,verdeo y claras de huevo grilladas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Pizza individual de muzzarella,rucula y parmesano y aceitunas negras" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 4, OptionKey = "menu_comun", Food = "Fajitas mexicanas de pollo c/papas a la hierba" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 4, OptionKey = "menu_light", Food = "Pollo al puerro c/verduras asadas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Quesadillas mexicanas,c/queso y cebolla" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 5, OptionKey = "menu_comun", Food = "Suprema a la milanesa c/puré de papas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 5, OptionKey = "menu_light", Food = "Pizza integral,c/verduras grilladas y salsa de tomates frescos" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Lasagna de ricota,vegetales,espinacas y salsa mixta gratinada" }
                }
            };
        }

        private static Menu CreateNewMenu(string id)
        {
            return new Menu()
            {
                Id = id,
                Title = string.Format("Nuevo menú ({0})", id),
                FirstWeekIdx = 0,
                WeeksQuantity = 2,
                DeadlineTime = "9:30",
                StartDate = DateTime.Now.Date.AddDays(7),
                EndDate = DateTime.Now.Date.AddYears(1),
                Places = new StringKeyedCollection<Place>(),
                Options = new StringKeyedCollection<Option>() 
                {
                    new Option() { Key = "menu_comun", Text = "Común" },
                    new Option() { Key = "menu_light", Text = "Light" },
                    new Option() { Key = "menu_vegetariano", Text = "Vegetariano" }
                },
                Foods = new WeekDayOptionKeyedCollection<MenuItem>()
            };
        }
    }
}
