﻿using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Application.Queries.Services
{
    public class MembroQueryServices(
        IDashboardQueryServices _dashboardConsultaServices,
        IServiceProvider service) : BaseQueryService<Membro, IMembroRepository>(service), IMembroQueryServices
    {
        const string ConviteParaSite =
            "\r\n\r\nPara saber mais detalhes sobre os valores acesse:\r\n"
            + "https://casa-financeiro-app.netlify.app/portal"
            + "\r\n\r\nEntre com:"
            + "\r\nUsuário: *visitante*"
            + "\r\nSenha: *123456*";

        public async Task<IEnumerable<Membro>> GetAllAsync() =>
            await _repository.Get().OrderBy(c => c.Nome).AsNoTracking().ToListAsync();

        public async Task<Membro> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<string> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string titleMessage,
            bool isMoradia,
            string pix
        )
        {
            var membro = await _repository.Get(membro => membro.Nome == nome).AsNoTracking().FirstOrDefaultAsync();

            if (membro is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.AcaoNaoInvalida, "Membro não encontrado")
                );

                return null;
            }

            if (isMoradia && membro.Nome.Contains("Jhon"))
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.AcaoNaoInvalida, "o Jhon não paga aluguel")
                );

                return null;
            }

            string message = isMoradia
                ? await MensagemValoresMoradiaDividosAsync(pix, membro.Nome, titleMessage)
                : await MensagemValoresCasaDividosAsync(pix, membro, titleMessage);

            string encodedMessage = Uri.EscapeDataString(message);

            membro.Telefone = Regex.Replace(membro.Telefone, "[^0-9]", "");
            string whatsappUrl = $"https://wa.me/{membro.Telefone}?text={encodedMessage}";

            return whatsappUrl;
        }

        #region Metodos de Suporte

        private async Task<string> MensagemValoresCasaDividosAsync(
            string pix,
            Membro membro,
            string titleMessage
        )
        {
            var resumoMensal = await _dashboardConsultaServices.GetDespesasDivididasMensalAsync();
            var membroIds = _repository.GetMembersIds();

            double valorPorMembro =
                resumoMensal
                    .DespesasPorMembro.FirstOrDefault(m => m.Nome == membro.Nome)
                    ?.ValorDespesaCasa ?? 0;

            string title = titleMessage.IsNullOrEmpty()
                ? $"Olá {membro.Nome}, tudo bem? Essas são as despesas desse mês:\r\n\r\n"
                : titleMessage + "\r\n\r\n";

            string messageBody = "";

            if (membro.Id == membroIds.IdJhon)
            {
                messageBody =
                    $"Sua parte no almoço ficou esse valor: *R$ {valorPorMembro:F2}*."
                    + $"\r\n\r\nMeu pix: *{pix}*.";
            }
            else
            {
                messageBody =
                    $"As despesas de casa dividido para cada vai ficar: *R$ {valorPorMembro:F2}*."
                    + $"\r\n\r\nMeu pix: *{pix}*.";
            }

            return title + messageBody + ConviteParaSite;
        }

        private async Task<string> MensagemValoresMoradiaDividosAsync(
            string pix,
            string membroNome,
            string titleMessage
        )
        {
            var resumoMensal = await _dashboardConsultaServices.GetDespesasDivididasMensalAsync();

            double valorPorMembro =
                resumoMensal
                    .DespesasPorMembro.Where(membro => membro.Nome == membroNome)
                    .FirstOrDefault()
                    ?.ValorDespesaMoradia ?? 0;

            string title = titleMessage.IsNullOrEmpty()
                ? $"Olá {membroNome}, tudo bem? Essas são as despesas desse mês:\r\n\r\n"
                : titleMessage + "\r\n\r\n";

            string messageBody =
                $"O valor do Aluguel, comdomínio e conta de luz para cada ficou: *R$ {valorPorMembro:F2}*."
                + $"\r\n\r\nMeu pix: *{pix}*.";

            return title + messageBody + ConviteParaSite;
        }

        #endregion
    }
}
