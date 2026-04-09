using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperActionOAuthService helper;
        private string claveCifrado;

        public AuthController(RepositoryHospital repo, HelperActionOAuthService helper, IConfiguration conf)
        {
            this.repo = repo;
            this.helper = helper;
            this.claveCifrado = conf.GetValue<string>("CypherKeys:CryptoJson");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Empleado empleado = await this.repo.LogInEmpleadoAsync(model.UserName, int.Parse(model.Password));
            if(empleado == null)
            {
                return Unauthorized();
            } else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES CON NUESTRO TOKEN
                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken(),
                                            SecurityAlgorithms.HmacSha256);

                string jsonEmpleado = JsonConvert.SerializeObject(empleado);
                string jsonEncryptado = HelperCifrado.EncryptString(jsonEmpleado, Encoding.UTF8.GetBytes(this.claveCifrado));
                //CREAMOS UN ARRAY DE CLAIMS PARA EL TOKEN
                //AQUI ALMACENARIAMOS EL ROLE DEL USUARIO
                Claim[] information = new[]
                {
                    new Claim("UserData", jsonEncryptado),
                    new Claim(ClaimTypes.Role, empleado.Oficio)
                };
                //EL TOKEN SE GENERA CON UNA CLASE Y DEBEMOS ALMACENAR
                //LOS DATOS DE ISSUER, CREDENTIALS...
                JwtSecurityToken token = new JwtSecurityToken(
                    claims: information,
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    notBefore: DateTime.UtcNow
                    );
                //POR ULTIMO, DEVOLVEMOS LA RESPUESTA AFIRMATIVA
                //CON EL TOKEN
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
