using Api.Business.Interfaces;
using Api.Business.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Api.Application.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotifier _notifier;
        
        public MainController(INotifier notifier)
        {
            _notifier = notifier;
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
