using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonJobs.Raven.Mvc.Authorize
{
    /// <summary>
    /// Al configurar el CommonJobsAuthorizeAttribute con un objeto de esta clase, los grupos de
    /// Active Directory requeridos se corresponderán con el prefijo indicado en AppSetings concatenado
    /// con el rol requerido por el atributo. 
    /// </summary>
    /// <remarks>
    /// El uso de está clase permite configurar diferentes grupos de un mismo dominio de Active
    /// Directory para distintos entornos, por ejemplo definiendo el prefijo "CommonJobsDEV_"
    /// para el entorno de pruebas y "CommonJobs_" para el entorno de producción.
    /// </remarks>
    public class PrefixFromSettingsAuthorizationBehavior : PrefixAuthorizationBehaviorBase
    {
        private string settingKey;

        public override string Prefix 
        {
            get { return ConfigurationManager.AppSettings[settingKey] ?? string.Empty; }
        }

        public PrefixFromSettingsAuthorizationBehavior(string settingKey)
        {
            this.settingKey = settingKey;
        }
    }
}
