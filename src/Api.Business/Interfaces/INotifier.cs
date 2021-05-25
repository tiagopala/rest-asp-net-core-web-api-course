using Api.Business.Notifications;
using System.Collections.Generic;

namespace Api.Business.Interfaces
{
    public interface INotifier
    {
        bool TemNotificacao();
        List<Notification> ObterNotificacao();
        void Handle(Notification notificacao);
    }
}
