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
                LastOrderDate = new DateTime(2000, 1, 1),
                StartDate = new DateTime(2013, 1, 7),
                EndDate = new DateTime(2020, 1, 1),
                Places = new StringKeyedCollection<Place>() 
                {
                    new Place() { Key = "place_larioja", Text = "La Rioja" },
                    new Place() { Key = "place_garay", Text = "Garay" }
                },
                Options = new StringKeyedCollection<Option>() 
                {
                    new Option() { Key = "menu_comun", Text = "Calórico" },
                    new Option() { Key = "menu_light", Text = "Light" },
                    new Option() { Key = "menu_vegetariano", Text = "Vegetariano" }
                },
                Foods = new WeekDayOptionKeyedCollection<MenuItem>() 
                {
                    new MenuItem() { WeekIdx = 0, DayIdx = 1, OptionKey = "menu_comun", Food = "Pollo al Verdeo con Purè de Papas" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 1, OptionKey = "menu_light", Food = "Milanesa de soja con calabaza" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesa de Zapallitos con Muzzarella, Tomate y Albahacas" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 2, OptionKey = "menu_comun", Food = "Panzottis de ricota y jamón con salsa bolognesa" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 2, OptionKey = "menu_light", Food = "Ensalada de Verdes, zanahaoria con Pollo y Citronette" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Pizza individual de Muzarella y vegetales gratinados" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 3, OptionKey = "menu_comun", Food = "Fajitas Mexicanas de Ternera con papas, batatas y cebolla" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 3, OptionKey = "menu_light", Food = "Ensalada de Hojas Verdes, Atún al Natural, Arroz, tomates y zanahoria" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Sorrentinos de Vegetales grillados y Ricota con salsa de Puerro y Tomates Frescos" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 4, OptionKey = "menu_comun", Food = "Suprema con puré de papas y calabaza" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 4, OptionKey = "menu_light", Food = "Pan de Carne Y vegetales al Wok" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Ensalada Capresse con Aceitunas negras y Rúcula" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 5, OptionKey = "menu_comun", Food = "Matambre relleno a la Pizza con papas al Oregano" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 5, OptionKey = "menu_light", Food = "Omelete de Claras, espinacas y calabaza Asadas con pan Integral con semillas" },
                    new MenuItem() { WeekIdx = 0, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Ñoquis de Calabaza salsa de tomates y espinaca" },

                    new MenuItem() { WeekIdx = 1, DayIdx = 1, OptionKey = "menu_comun", Food = "Calzon especial con jamon, morrones y Muzzarella" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 1, OptionKey = "menu_light", Food = "Wok de vegetales, Pollo y semillas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesa de Soja con vegetales al Wok" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 2, OptionKey = "menu_comun", Food = "Pastel de carne y papas con queso Gratinado" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 2, OptionKey = "menu_light", Food = "Pesca del Día Horneada al Limón y hierbas con Calabaza asada" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Sorrentinos Capresse Con salsa Rosa" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 3, OptionKey = "menu_comun", Food = "Milanesa con Arroz a la crema" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 3, OptionKey = "menu_light", Food = "Ensalada de espinacas frescas Champignones, Semillas, Verdeo, y Claras de Huevo Grilladas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Pizza Individual de Muzzarella, Rúcula, Parmesano y Aceitunas negras" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 4, OptionKey = "menu_comun", Food = "Hamburguesa Completa con papas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 4, OptionKey = "menu_light", Food = "Pollo al puerro acompañado con verduras asadas" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Quesadillas Mexicanas, con Queso y Cebolla" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 5, OptionKey = "menu_comun", Food = "Pollo Relleno al verdeo" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 5, OptionKey = "menu_light", Food = "Milanesa de ternera Horneada con ensalada tricolor" },
                    new MenuItem() { WeekIdx = 1, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Pizza integral, con verduras Grilladas y Salsa de Tomates Frescos" },

                    new MenuItem() { WeekIdx = 2, DayIdx = 1, OptionKey = "menu_comun", Food = "Pollo desguesado al Champignón con papas Doradas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 1, OptionKey = "menu_light", Food = "Wok de Vegetales y Pesca del Día" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Cintas caseras de Rúcula con wok de hongos y salsa de soja" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 2, OptionKey = "menu_comun", Food = "Pan de Carne con puré de papas" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 2, OptionKey = "menu_light", Food = "Ensalada de Rúcula, Champignones Frescos, Remolacha y Huevo" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Fajitas Mexicanas vegetarianas acompañadas con Batatas al Horno" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 3, OptionKey = "menu_comun", Food = "Ravioles de Pollo, Puerro y Muzaralla Con salsa mixta" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 3, OptionKey = "menu_light", Food = "Milanesa de Soja con Puré de Calabaza y Zanahoria" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Tomates rellenos acompañado con ensalada Jardinera" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 4, OptionKey = "menu_comun", Food = "Milanesa a la Suiza con papas y cebollas al horno" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 4, OptionKey = "menu_light", Food = "Pechuga sin Piel al Verdeo con Vegetales Grillados" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Sorrentinos de Calabaza y Muzzarella con salsa Mixta" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 5, OptionKey = "menu_comun", Food = "Matambre Arrollado al verdeo" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 5, OptionKey = "menu_light", Food = "Ñoquis de Ricota Magra, salsa de tomates Frescos y albahaca" },
                    new MenuItem() { WeekIdx = 2, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Pizza Individual de Espinaca y Muzzarella" },

                    new MenuItem() { WeekIdx = 3, DayIdx = 1, OptionKey = "menu_comun", Food = "Sorrentinos de Jamón y Queso con salsa Scarparo" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 1, OptionKey = "menu_light", Food = "Ensalada Roja. Tomate Remolacha Zanahoria, Repollo Colorado y Atún al Natural" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesa de Berengenas a la Napolitana con Puré de papas y calabaza" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 2, OptionKey = "menu_comun", Food = "Cuarto de pollo a la Mostaza con Papas doradas" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 2, OptionKey = "menu_light", Food = "Zapallitos Rellenos" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Wok de Vegetales, Hongos, Brotes, semillas y salsa de soja" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 3, OptionKey = "menu_comun", Food = "Milanesa Napolitana con puré Mixto" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 3, OptionKey = "menu_light", Food = "Pollo al puerro con Arroz Integral" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Ensalada de verdes Huevos duros, Parmesano, Croutons y aderesos Cesar" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 4, OptionKey = "menu_comun", Food = "Merluza a la milanesa con puré de papas y espinacas" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 4, OptionKey = "menu_light", Food = "Ensalada Cesar con pollo" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Ñoquis de Espinaca y papas, con salsa de Puerro y Tomates Frescos" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 5, OptionKey = "menu_comun", Food = "Fajitas Mixtas con papas provensal" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 5, OptionKey = "menu_light", Food = "Ravioles de Ricota en masa Integral, Acompañado con salsa de verduras y Tomates frescos al Wok" },
                    new MenuItem() { WeekIdx = 3, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Calzone de Muzarella, Tomates Albahaca, y Muzzarella" },

                    new MenuItem() { WeekIdx = 4, DayIdx = 1, OptionKey = "menu_comun", Food = "Canelones de Ricota y Verduras a la Bolognesa" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 1, OptionKey = "menu_light", Food = "Pollo al Limón acompañado con verduras asadas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 1, OptionKey = "menu_vegetariano", Food = "Milanesa de soja a la Napolitana con Puré Mixto" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 2, OptionKey = "menu_comun", Food = "Medallones de Pescado con puré de papas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 2, OptionKey = "menu_light", Food = "Ensalada de Hojas verdes Champignones, Verdeo, Claras de Huevo Grilladas y semillas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 2, OptionKey = "menu_vegetariano", Food = "Ñoquis de Papa y espinaca con salsa Roquefort" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 3, OptionKey = "menu_comun", Food = "Pollo Relleno con Jamón, queso y verduras asadas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 3, OptionKey = "menu_light", Food = "Merluza Horneada al Limón y hierbas con Calabaza asada	" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 3, OptionKey = "menu_vegetariano", Food = "Calabaza Asada con Queso gratinado Choclo y Puerro" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 4, OptionKey = "menu_comun", Food = "Pizza Individual Napolitana con Jamon y Aceitunas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 4, OptionKey = "menu_light", Food = "Wok de Vegetales, Brotes, Hongos y semillas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 4, OptionKey = "menu_vegetariano", Food = "Ravioles de espinaca y parmesano con salsa Rosa" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 5, OptionKey = "menu_comun", Food = "Carne asada al horno Con papas a las hierbas" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 5, OptionKey = "menu_light", Food = "Ñoquis de Ricota magra con tomates Frescos y albahaca" },
                    new MenuItem() { WeekIdx = 4, DayIdx = 5, OptionKey = "menu_vegetariano", Food = "Ensalada de Hojas verdes, Tomates, Muzarella, aceitunas Negras y Verdeo" }
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
