using Application.Configurations.MappingsApp;
using Application.Constantes;
using Application.Queries.Interfaces;
using Application.Queries.Interfaces.Telas;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Membros;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Application.Queries.Services
{
    public class MembroQueryServices(
        IDashboardQueryServices _dashboardConsultaServices,
        IServiceProvider service
    ) : BaseQueryService<Membro, MembroDto, IMembroRepository>(service), IMembroQueryServices
    {

        private readonly IServiceProvider _service = service;

        const string ConviteParaSite =
            "\r\n\r\nPara saber mais detalhes sobre os valores acesse:\r\n"
            + "https://casa-financeiro-app.netlify.app/portal"
            + "\r\n\r\nEntre com:"
            + "\r\nUsuário: *visitante*"
            + "\r\nSenha: *123456*";

        protected override MembroDto MapToDTO(Membro entity) => entity.MapToDTO();

        public async Task<IEnumerable<MembroDto>> GetAllAsync() =>
            await _repository
                .Get()
                .OrderBy(c => c.Nome)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();

        public async Task<MembroDto> GetByCodigoAsync(int id) => await GetByCodigoAsync(id);

        public async Task<string> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string titleMessage,
            bool isMoradia,
            string pix
        )
        {
            var membro = await _repository
                .Get(membro => membro.Nome == nome)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .FirstOrDefaultAsync();

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
                ? await MensagemValoresMoradiaDividos(pix, membro.Nome, titleMessage)
                : await MensagemValoresCasaDividosAsync(pix, membro, titleMessage);

            string encodedMessage = Uri.EscapeDataString(message);

            membro.Telefone = Regex.Replace(membro.Telefone, "[^0-9]", "");
            string whatsappUrl = $"https://wa.me/{membro.Telefone}?text={encodedMessage}";

            return whatsappUrl;
        }

        #region Metodos de Suporte

        private async Task<string> MensagemValoresCasaDividosAsync(
            string pix,
            MembroDto membro,
            string titleMessage
        )
        {
            var resumoMensal = await _dashboardConsultaServices.GetDespesasDivididasMensal();
            var membroIds = GetCods.MembroCod;

            double valorPorMembro =
                resumoMensal
                    .DespesasPorMembro.FirstOrDefault(m => m.Nome == membro.Nome)
                    ?.ValorDespesaCasa ?? 0;

            string title = titleMessage.IsNullOrEmpty()
                ? $"Olá {membro.Nome}, tudo bem? Essas são as despesas desse mês:\r\n\r\n"
                : titleMessage + "\r\n\r\n";

            string messageBody = "";

            if (membro.Code == membroIds.CodJhon)
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

        private async Task<string> MensagemValoresMoradiaDividos(
            string pix,
            string membroNome,
            string titleMessage
        )
        {
            var resumoMensal = await _dashboardConsultaServices.GetDespesasDivididasMensal();

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
