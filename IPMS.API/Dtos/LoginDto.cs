using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IPMS.API.Dtos
{
    public class LoginDto
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
    }
}
