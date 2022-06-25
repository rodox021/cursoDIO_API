using curso.api.Models.Usuario;
using curso.api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using curso.api.Infraestruture.Data;
using Microsoft.EntityFrameworkCore;
using curso.api.Business.Entities;

namespace curso.api.Controllers
{
    [Route("api/v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private object usuarioViewModelOutput;

        /// <summary>
        /// Este Serviço permite autenticar um usuário cadastrado e ativo.
        /// </summary>
        /// <param name="loginViewModelInput"></param>
        /// <returns>Retornar Status ok, dados do usuario e o token em caso de sucesso</returns>
        /// 

        [SwaggerResponse(statusCode: 200, description: "Sucesso ao autenticar", typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos Obrigatórios", typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro Interno", typeof(ErrorGenericoViewModel))]

        [HttpPost]
        [Route("logar")]
        public IActionResult Logar(LoginViewModelInput loginViewModelInput)
        {
            var usuarioViewModelOutput =  new UsuarioViewModelOutput()
            {
                Codigo = 1,
                Login = "Rodolfo",
                Email = "rodolfo@gmail.com"
            };
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(new ValidaCampoViewModelOutput(ModelState.SelectMany(msgs => msgs.Value.Errors).Select(s => s.ErrorMessage)));
            //}



            //--------------------

            var secret = Encoding.ASCII.GetBytes("RodolfolealBraga");//("MzfsT&d9gprP>!9$Es(X!5g@;ef!5sbk:jH\\2.}8ZP'qY#7");
            var symmetricSecurityKey = new SymmetricSecurityKey(secret);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioViewModelOutput.Codigo.ToString()),
                    new Claim(ClaimTypes.Name, usuarioViewModelOutput.Login.ToString()),
                    new Claim(ClaimTypes.Email, usuarioViewModelOutput.Email.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenGenerated = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(tokenGenerated);

            //-----------------



            return Ok(new
            { 
                Token = token,
                usuario = usuarioViewModelOutput
            });
        }


        /// <summary>
        /// Este Serviço permite cadastrar usuario no banco de dados
        /// </summary>
        /// <param name="registroViewModelInput"></param>
        /// <returns>Retornar Status Created e usuário cadastrado </returns>
        /// 

        [SwaggerResponse(statusCode: 200, description: "Registro Efetuado com Sucesso!", typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos Obrigatórios", typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro Interno", typeof(ErrorGenericoViewModel))]

        [HttpPost]
        [Route("registrar")]
        public IActionResult Registrar(RegistroViewModelInput registroViewModelInput)
        {

            var options = new DbContextOptionsBuilder<CursoDbContext>();
            options.UseSqlite("Data source = cusro.db");

            CursoDbContext contexto = new CursoDbContext(options.Options);

            var migracoesPendentes = contexto.Database.GetPendingMigrations();

            if (migracoesPendentes.Count() >0)
            {
                contexto.Database.Migrate();
            }

            var usuario = new Usuario() { 
                Login = registroViewModelInput.Login,
                Senha = registroViewModelInput.Senha,
                Email = registroViewModelInput.Email
            };
            contexto.Usuario.Add(usuario);
            contexto.SaveChanges();

            return Created("", registroViewModelInput);
        }
    }
}
