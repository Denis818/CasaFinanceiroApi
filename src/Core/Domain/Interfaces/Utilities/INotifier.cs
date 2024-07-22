using Domain.Enumeradores;
using Domain.Utilities;

namespace Domain.Interfaces.Utilities
{
    public interface INotifier
    {
        List<Notificacao> ListNotificacoes { get; }
        void Notify(EnumTipoNotificacao tipo, string message);
        bool HasNotifications(EnumTipoNotificacao tipo, out Notificacao[] notifications);
        void Clear();
    }
}
