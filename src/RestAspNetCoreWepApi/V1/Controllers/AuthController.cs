using Api.Application.Configurations;
using Api.Application.Controllers;
using Api.Application.DTOs;
using Api.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Application.V1.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettingsConfig _appSettings;

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettingsConfig> appSettings,
            INotifier notifier, IUserService userService) : base(notifier, userService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("criar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Criar(CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = createUserDTO.Email,
                Email = createUserDTO.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return CustomResponse(GerarJwt(createUserDTO.Email).Result);
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost("entrar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Login(LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUserDTO.Email, loginUserDTO.Password, false, true);

            if (result.Succeeded)
                return CustomResponse(GerarJwt(loginUserDTO.Email).Result);

            if (result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            NotificarErro("Usuário ou senha incorretos");
            return CustomResponse();
        }

        private async Task<LoginResponseDTO> GerarJwt(string email)
        {
            // Identificando o usuário, obtendo e atribuindo Claims referentes a ele.
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            // Configuração das Claims a serem adicionadas no Token.
            claims.Add(new Claim(ClaimTypes.Name, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id)); // Sub -> User
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // Jti -> Id Próprio do Token
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            // Adição da role do usuário via Claims.
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            // "Conversão" da Lista de Claims em ClaimsIdentity, sendo a propriedade aceita no SecurityTokenDescriptor.Subject.
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            // Criando o retorno para uma aplicação SPA
            return new LoginResponseDTO
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UserToken = new UserTokenDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Where(c => c.Type.Length > 3).Select(c => new ClaimDTO { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
