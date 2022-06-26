using curso.api.Business.Entities;
using curso.api.Business.Repositories;
using curso.api.Configurations;
using curso.api.Models;
using curso.api.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace curso.api.Controllers
{
    [Route("api/v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {


        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthenticationService _authenticationService;
        private object usuarioViewModelOutput;

        public UsuarioController(
            IUsuarioRepository usuarioRepository, 
            IConfiguration configuration, 
            IAuthenticationService authenticationService)
        {
            _usuarioRepository = usuarioRepository;
            _authenticationService = authenticationService;
        }

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
        public  IActionResult Logar(LoginViewModelInput loginViewModelInput)
        {
            var usuario = _usuarioRepository.ObterUsuarioAsync(loginViewModelInput.Login);

            if (usuario == null)
            {
                return BadRequest("Houve um erro ao tentar acessar.");
            }
            var usuarioViewModelOutput = new UsuarioViewModelOutput()
            {
                Codigo = 1,
                Login = "Rodolfo",
                Email = "rodolfo@gmail.com"
            };

            var token = _authenticationService.GerarToken(usuarioViewModelOutput);

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

        [SwaggerResponse(statusCode: 200, description: "Registro Efetuado com Sucesso!", typeof(RegistroViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos Obrigatórios", typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro Interno", typeof(ErrorGenericoViewModel))]

        [HttpPost]
        [Route("registrar")]
        public IActionResult Registrar(RegistroViewModelInput registroViewModelInput)
        {

            var usuario = new Usuario()
            {
                Login = registroViewModelInput.Login,
                Senha = registroViewModelInput.Senha,
                Email = registroViewModelInput.Email
            };
            _usuarioRepository.Adicionar(usuario);
            _usuarioRepository.Commit();



            return Created("", registroViewModelInput);
        }
    }
}
