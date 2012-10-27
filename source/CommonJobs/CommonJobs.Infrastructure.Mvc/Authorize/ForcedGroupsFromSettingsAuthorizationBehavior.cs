using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    /// <summary>
    /// Al configurar el CommonJobsAuthorizeAttribute con un objeto de esta clase, se salta completamente 
    /// la autenticación por Active Directory, en su lugar se comparan los grupos requeridos contra los 
    /// especificados en el AppSettings correspondiente al settingKey.
    /// </summary>
    /// <remarks>
    /// Un setting vacío es interpretado como un usuario autenticado pero que no pertenece a ningún grupo,
    /// en cambio si el setting no existe se interpretará como un usuario no autenticado.
    /// Solo se tendrá en cuenta parámetro Roles del attributo, los demás parámetros serán ignorados.
    /// </remarks>
    /// <example>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Setting</term>
    ///         <description>Acciones permitidas</description>
    ///     </listheader>
    /// </list>
    ///     <item>
    ///         <term>No existente</term>
    ///         <description>Ninguna acción decorada con CommonJobsAuthorize será permitida.</description>
    ///     </item>
    ///     <item>
    ///         <term>"" (string vacío)</term>
    ///         <description>Solo las decoradas con [CommonJobsAuthorize] (que no requieran grupos)</description>
    ///     </item>
    ///     <item>
    ///         <term>"Grupo1, Grupo2"</term>
    ///         <description>
    ///         Decoradas de la siguientes formas [CommonJobsAuthorize], [CommonJobsAuthorize(Roles = "Grupo1")], 
    ///         [CommonJobsAuthorize(Roles = "Grupo2")], [CommonJobsAuthorize(Roles = "Grupo1, Grupo2")]
    ///         </description>
    ///     </item>
    /// </example>
    public class ForcedGroupsFromSettingsAuthorizationBehavior : ForcedGroupsAuthorizationBehaviorBase
    {
        private string settingKey;

        public override HashSet<string> ForcedGroups 
        {
            get 
            { 
                var setting = ConfigurationManager.AppSettings[settingKey];

                if (setting == null)
                    return null;

                return new HashSet<string>(setting.ToRoleList()); ; 
            }
        }

        public ForcedGroupsFromSettingsAuthorizationBehavior(string settingKey)
        {
            this.settingKey = settingKey;
        }
    }
}
