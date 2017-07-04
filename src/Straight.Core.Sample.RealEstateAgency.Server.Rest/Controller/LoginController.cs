using Microsoft.AspNetCore.Mvc;
using Straight.Core.Sample.RealEstateAgency.Server.Rest.Models.V1;

namespace Straight.Core.Sample.RealEstateAgency.Server.Rest.Controller
{
    [ApiVersion("1.0")]
    public class LoginController : Microsoft.AspNetCore.Mvc.Controller
    {
        public LoginController()
        {
        }

        [HttpPost]
        public IActionResult Connect([FromBody] ConnectionInformationDto connectionInfo)
        {
            var 
        }
    }
}
