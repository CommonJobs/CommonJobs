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

            template.Items.AddRange(
                new List<TemplateItem>()
                {
                    ///JOB PERFORMANCE
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "responsibility",
                        Text = "Responsabilidad",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "accuracy",
                        Text = "Exactitud y claridad de trabajo",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "datecompliance",
                        Text = "Cumplimiento de fechas",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "productivity",
                        Text = "Productividad",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "workorder",
                        Text = "Orden y claridad del trabajo",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "workplanification",
                        Text = "Planificación del trabajo",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "documentation",
                        Text = "Documentación que genera",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "taskreport",
                        Text = "Reporta avances de tareas",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "accomplishment",
                        Text = "Capacidad de realización",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "situationcomprehension",
                        Text = "Comprensión de situaciones",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "commonsense",
                        Text = "Sentido Común",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "existingprocedures",
                        Text = "Cumplimiento de los procedimientos existentes",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "technicalknowledge",
                        Text = "Grado de conocimiento técnico",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "communication",
                        Text = "Comunicación",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "oralenglish",
                        Text = "Inglés oral",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "jobperformance",
                        Key = "writtenenglish",
                        Text = "Inglés escrito",
                        Description = ""
                    },
                    ///HUMAN FACTOR
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "attitudecompany",
                        Text = "Actitud hacia la empresa",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "attitudecoworkers",
                        Text = "Actitud hacia los compañeros",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "attitudeclient",
                        Text = "Actitud hacia el cliente",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "teamwork",
                        Text = "Cooperación con el equipo",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "abbilitytoacceptcriticism",
                        Text = "Capacidad de aceptar críticas",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "abbilitytogeneratesuggestions",
                        Text = "Capacidad de generar sugerencias constructivas",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "personalpresentation",
                        Text = "Presentación personal",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "predisposition",
                        Text = "Predisposición",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "humanfactor",
                        Key = "puntuality",
                        Text = "Punctuality",
                        Description = ""
                    },
                    ///SKILLS
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "initiative",
                        Text = "Iniciativa/Proactividad",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "creativity",
                        Text = "Creatividad",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "underpressureresponse",
                        Text = "Respuesta bajo presión",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "multitasking",
                        Text = "Capacidad de manejar múltiples tareas",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "leadership",
                        Text = "Coordinación y liderazgo",
                        Description = ""
                    },
                    new TemplateItem(){
                        GroupKey = "skills",
                        Key = "potentiality",
                        Text = "Potencialidad - capacidad de aprendizaje",
                        Description = ""
                    }
                }
            );
            return template;
        }
    }
}
