using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class GetEvaluationTemplateCommand : Command<Template>
    {
        public string Id { get; set; }
        private string IdOrDefault
        {
            get { return string.IsNullOrWhiteSpace(Id) ? Template.DefaultTemplateId : Id; }
        }

        public GetEvaluationTemplateCommand(string id = null)
        {
            this.Id = id;
        }

        public override Template ExecuteWithResult()
        {
            var template = RavenSession.Load<Template>(IdOrDefault);
            if (template == null && IdOrDefault == Template.DefaultTemplateId)
            {
                template = CreateDefaultTemplate(IdOrDefault);
                ExecuteCommand(new UpdateTemplateCommand(template));
            }

            return template;
        }

        private static Template CreateDefaultTemplate(string id)
        {
            var template = new Template();
            template.Groups.AddRange(
                new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>( "jobperformance", "Desempeño Laboral"),
                    new KeyValuePair<string, string>( "humanfactor", "Factor Humano / Actitud"),
                    new KeyValuePair<string, string>( "skills", "Habilidades"),
                }
            );
            template.Items.AddRange(
                new List<TemplateItem>()
                {
                    ///JOB PERFORMANCE
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "responsibility",
                        Text = "Responsabilidad",
                        Description = "Grado de compromiso que asume para el cumplimiento de las metas. Grado de tranquilidad que el nivel de compromiso le genera a sus supervisores directos (ya sea responsable de un área, project manager y/o líder técnico) y a los involucrados en la tarea o equipo de trabajo."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "accuracy",
                        Text = "Exactitud y calidad de trabajo",
                        Description = "Coherencia entre el trabajo solicitado y el efectivamente realizado. Grado de efectividad que demuestra en el trabajo. El trabajo realizado cumple con lo requerido y es de buena calidad."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "datecompliance",
                        Text = "Cumplimiento de fechas",
                        Description = "Cumplimiento de las fechas de entrega pautadas. Se aplica al análisis, desarrollo, documentación, reporte y demás actividades relacionadas al cumplimiento de la tarea."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "productivity",
                        Text = "Productividad",
                        Description = "Volumen de trabajo que realiza por unidad de tiempo. Proporción del tiempo dedicado al trabajo exclusivamente. Eficiencia."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "workorder",
                        Text = "Orden y claridad del trabajo",
                        Description = "Sus trabajos pueden ser abordados con facilidad por otras personas. (Por ejemplo, para un desarrollador: los nombres de las variables son claros, el código es ordenado y legible, de ser necesario modificar su código es posible hacerlo; para un diseñador: dejar documentados estilos, formatos de texto, colores. Para ambos documentación en Jira, el repositorio)."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "workplanification",
                        Text = "Planificación del trabajo",
                        Description = "Planificación de sus tareas. Conocimiento exacto del estado de sus tareas. Analiza sus tareas contemplando el tiempo que requiere para llevarlas adelante y sabe estimar lo que falta."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "documentation",
                        Text = "Documentación que genera",
                        Description = "Documentación relacionada a la tarea, como del código, del diseño, de la estrategia, campaña, minutas, documentación de alcances. Aplica metodologías coherentes de documentación. Validez y calidad de la misma."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "taskreport",
                        Text = "Reporta avances de tareas",
                        Description = "Frecuencia, constancia, exactitud y calidad en los reportes (por email, orales, informes escritos, etc.). Capacidad de extraer la información relevante al elevar un informe."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "accomplishment",
                        Text = "Capacidad de realización",
                        Description = "Practicidad, autonomía, pragmatismo. Posibilidad de llegar a la última instancia de una meta superando los obstáculos. Capacidad de interactuar con otros en búsqueda de alcanzar las metas."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "situationcomprehension",
                        Text = "Comprensión de situaciones",
                        Description = "Capacidad de entender conceptos y situaciones rápidamente. Capacidad de adaptarse a los cambios y comprender objetivos globales."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "commonsense",
                        Text = "Sentido Común",
                        Description = "Capacidad para ubicarse en las situaciones de manera coherente. Capacidad de elegir alternativas convenientes con visión estratégica a futuro y siendo realista."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "existingprocedures",
                        Text = "Cumplimiento de los procedimientos existentes",
                        Description = "Grado de cumplimiento de los procedimientos generales de trabajo, y pasos de cada tarea en particular. Cumplimiento de las normas de la empresa cuando aplica."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "technicalknowledge",
                        Text = "Grado de conocimiento técnico",
                        Description = "Conocimiento de las distintas herramientas necesarias para desarrollar sus labores."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "communication",
                        Text = "Comunicación",
                        Description = "Eficacia de la comunicación con otros integrantes del equipo de trabajo, la empresa y el cliente."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "oralenglish",
                        Text = "Inglés oral",
                        Description = "Grado de conocimiento de Inglés Oral."
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "writtenenglish",
                        Text = "Inglés escrito",
                        Description = "Grado de conocimiento de Inglés Escrito."
                    },
                    ///HUMAN FACTOR
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "attitudecompany",
                        Text = "Actitud hacia la empresa",
                        Description = "Capacidad de defender los intereses de la Empresa y adherirse a sus lineamientos. Lealtad para con la Empresa. Voluntad y flexibilidad para extender el horario de trabajo ante una necesidad puntual."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "attitudecoworkers",
                        Text = "Actitud hacia los compañeros",
                        Description = "Forma en la que se maneja con sus compañeros inmediatos. Camaradería."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "attitudeclient",
                        Text = "Actitud hacia el cliente",
                        Description = "Claridad en la comunicación con el cliente (interno y externo). Respeto, cooperación y cordialidad. Manejo de situaciones conflictivas, tanto en reuniones como por mail o telefónicamente."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "teamwork",
                        Text = "Cooperación con el equipo",
                        Description = "Colaboración en el desarrollo de trabajos de integrantes de otros grupos. Trabajo en equipo. Capacidad de compartir conocimiento y habilidades."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "abbilitytoacceptcriticism",
                        Text = "Capacidad de aceptar críticas",
                        Description = "Capacidad de recibir críticas constructivas en forma abierta. Grado de adaptación a las mismas. Capacidad de no ofenderse y aprovechar las críticas para mejorar."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "abbilitytogeneratesuggestions",
                        Text = "Capacidad de generar sugerencias constructivas",
                        Description = "Cantidad de sugerencias que realiza para mejorar el trabajo. Calidad de las mismas. Capacidad de elevar las sugerencias oportunamente a quien corresponde."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "personalpresentation",
                        Text = "Pautas de convivencia",
                        Description = "Mantenimiento del área de trabajo en orden, consideración para con sus compañeros de área (por ejemplo volumen de la música, tono de voz, vocabulario, ruidos molestos)."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "predisposition",
                        Text = "Predisposición",
                        Description = "Se muestra predispuesto hacia la tarea. Manifestación de una actitud positiva frente a los diferentes requerimientos. Entusiasmo y Motivación."
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "puntuality",
                        Text = "Puntualidad",
                        Description = "Puntualidad en reuniones. Aviso con tiempo si no se puede ser puntual."
                    },
                    ///SKILLS
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "initiative",
                        Text = "Iniciativa/Proactividad",
                        Description = "Inquietud por avanzar y mejorar. Facilidad para ofrecerse como ejecutor de sus propuestas. Tiene empuje."
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "creativity",
                        Text = "Creatividad",
                        Description = "Ofrece alternativas innovadoras para solucionar problemas. Capacidad de vincular distintos conocimientos para una nueva aplicación de los mismos."
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "underpressureresponse",
                        Text = "Respuesta bajo presión",
                        Description = "Capacidad de mantener la calma y transmitirla a sus compañeros. Capacidad de tomar decisiones correctas bajo presión. Capacidad de sacar provecho de situaciones adversas. Capacidad de realización en estos casos."
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "multitasking",
                        Text = "Capacidad de manejar múltiples tareas",
                        Description = "Mantiene en orden sus tareas incluso cuando maneja múltiples temas. Sabe priorizar. Tiempo que le insume la conmutación entre un tema y el otro. Capacidad de realización en estos casos."
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "leadership",
                        Text = "Coordinación y liderazgo",
                        Description = "Carisma, liderazgo natural, capacidad de mediar en los conflictos internos y capacidad de mediar en los conflictos con pares, clientes y/o proveedores."
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "potentiality",
                        Text = "Potencialidad - capacidad de aprendizaje",
                        Description = "Inquietud y capacidad para conocer, adquirir y aplicar distintas habilidades necesarias para el trabajo."
                    }
                }
            );
            return template;
        }
    }
}
