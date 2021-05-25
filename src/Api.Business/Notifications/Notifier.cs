using Api.Business.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Api.Business.Notifications
{
    public class Notifier : INotifier
    {
        private List<Notification> _notificacoes;

        public Notifier()
        {
            _notificacoes = new List<Notification>();
        }

        public void Handle(Notification notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        public List<Notification> ObterNotificacao()
        {
            return _notificacoes;
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}
