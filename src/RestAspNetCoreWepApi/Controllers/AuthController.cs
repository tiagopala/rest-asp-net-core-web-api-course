//using Api.Application.DTOs;
//using Api.Business.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace Api.Application.Controllers
//{
//    [Route("api/[controller]")]
//    public class AuthController : MainController
//    {
//        private readonly SignInManager<IdentityUser> _signInManager;
//        private readonly UserManager<IdentityUser> _userManager;

//        public AuthController(
//            SignInManager<IdentityUser> signInManager,
//            UserManager<IdentityUser> userManager,
//            INotifier notifer) : base(notifer)
//        {
//            _signInManager = signInManager;
//            _userManager = userManager;
//        }

//        [HttpPost("criar")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesDefaultResponseType]
//        public async Task<ActionResult> Criar(CreateUserDTO createUserDTO)
//        {
//            if (!ModelState.IsValid)
//                return CustomResponse(ModelState);

//            var user = new IdentityUser
//            {
//                UserName = createUserDTO.Email,
//                Email = createUserDTO.Email,
//                EmailConfirmed = true
//            };

//            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

//            if (result.Succeeded)
//            {
//                await _signInManager.SignInAsync(user, false);
//                return CustomResponse(createUserDTO);
//            }

//            foreach (var error in result.Errors)
//            {
//                NotificarErro(error.Description);
//            }

//            return CustomResponse(createUserDTO);
//        }

//        [HttpPost("entrar")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesDefaultResponseType]
//        public async Task<ActionResult> Login(LoginUserDTO loginUserDTO)
//        {
//            if (!ModelState.IsValid)
//                return CustomResponse(ModelState);

//            var result = await _signInManager.PasswordSignInAsync(loginUserDTO.Email, loginUserDTO.Password, false, true);

//            if (result.Succeeded)
//                return CustomResponse(loginUserDTO);

//            if(result.IsLockedOut)
//            {
//                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
//                return CustomResponse(loginUserDTO);
//            }

//            NotificarErro("Usuário ou senha incorretos");
//            return CustomResponse(loginUserDTO);
//        }
//    }
//}
