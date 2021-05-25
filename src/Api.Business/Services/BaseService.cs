using Api.Business.Interfaces;
using Api.Business.Models;
using Api.Business.Notifications;
using FluentValidation;
using FluentValidation.Results;

namespace Api.Business.Services
{
    public abstract class BaseService
    {
        private readonly INotifier _notificador;

        public BaseService(INotifier notificador)
        {
            _notificador = notificador;
        }

        protected void Notificar(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                Notificar(erro.ErrorMessage);
            }
        }

        protected void Notificar(string mensagem)
        {
            _notificador.Handle(new Notification(mensagem));
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid) return true;

            Notificar(validator);

            return false;
        }
    }
}
