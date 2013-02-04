using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Epnuke.OAuth.Mvc
{
    public class OAuthGenericResult : ActionResult
    {
        private readonly object _data;
        private readonly int _httpCode;

        public OAuthGenericResult(object data, int httpCode)
        {
            _data = data;
            _httpCode = httpCode;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = _httpCode;
            response.ContentType = "application/x-www-form-urlencoded";
            if (_data != null)
            {
                var sb = new StringBuilder();
                var props = _data.GetType().GetProperties();
                var idx = 0;
                foreach (var prop in props)
                {
                    sb.Append(string.Format("{0}={1}", prop.Name, prop.GetValue(_data, null)));
                    if (idx < props.Length -1) sb.Append('&');
                    idx++;
                }
                response.Output.Write(sb.ToString());
            }
        }
    }
}