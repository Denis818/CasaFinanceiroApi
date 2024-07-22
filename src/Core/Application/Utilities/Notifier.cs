using Domain.Enumeradores;
using Domain.Interfaces.Utilities;
using Domain.Utilities;

namespace Application.Utilities
{
    public class Notifier : INotifier
    {
        public List<Notificacao> ListNotificacoes { get; } = [];

        public void Clear()
        {
            ListNotificacoes.Clear();
        }

        public void Notify(EnumTipoNotificacao tipo, string message) =>
            ListNotificacoes.Add(new Notificacao(message, tipo));

        public bool HasNotifications(EnumTipoNotificacao tipo, out Notificacao[] notifications)
        {
            notifications = ListNotificacoes.Where(n => n.StatusCode == tipo).ToArray();
            return notifications.Length > 0;
        }

    }
}
