using Api.Business.Interfaces;
using Api.Business.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Api.Application.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotifier _notifier;

        protected readonly IUserService _userService;
        
        protected Guid UsuarioId { get; }
        protected string UsuarioNome { get; }
        protected string UsuarioEmail { get; }
        protected IEnumerable<Claim> UsuarioClaims { get; }
        protected bool UsuarioAutenticado { get; }

        public MainController(INotifier notifier, IUserService userService)
        {
            _notifier = notifier;
            _userService = userService;

            UsuarioId = _userService.GetUserId();
            UsuarioNome = _userService.Name;
            UsuarioEmail = _userService.GetUserEmail();
            UsuarioClaims = _userService.GetClaimsIdentity();
            UsuarioAutenticado = _userService.IsAuthenticated();
        }

        protected bool OperacaoValida()
        {
            return !_notifier.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    Success = true,
                    Data = result
                });
            }

            return BadRequest(new
            {
                Success = false,
                Erros = _notifier.ObterNotificacao().Select(n => n.Mensagem)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) 
                NotificarErroModelStateInvalida(modelState);

            return CustomResponse();
        }

        protected void NotificarErroModelStateInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                var mensagemErro = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;

                NotificarErro(mensagemErro);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notifier.Handle(new Notification(mensagem));
        }
    }
}
