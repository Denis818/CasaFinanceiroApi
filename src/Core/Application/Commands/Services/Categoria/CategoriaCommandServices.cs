using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;

namespace Application.Commands.Services
{
    public class CategoriaCommandServices(IServiceProvider service)
        : BaseCommandService<Categoria, CategoriaCommandDto, ICategoriaRepository>(service),
            ICategoriaCommandServices
    {
        protected override Categoria MapToEntity(CategoriaCommandDto entity) =>
            entity.MapToEntity();

        public async Task<bool> InsertAsync(CategoriaCommandDto categoriaDto)
        {
            if (Validator(categoriaDto))
                return false;

            if (await _repository.ExisteAsync(nome: categoriaDto.Descricao) != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "A categoria", categoriaDto.Descricao)
                );
                return false;
            }

            var categoria = categoriaDto.MapToEntity();
            await _repository.InsertAsync(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid code, CategoriaCommandDto categoriaDto)
        {
            if (Validator(categoriaDto))
                return false;

            var categoria = await _repository.GetByCodigoAsync(code);

            if (categoria is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Categoria", code)
                );
                return false;
            }

            if (categoria.Descricao == categoriaDto.Descricao)
                return false;

            if (_repository.IdentificarCategoriaParaAcao(categoria.Code))
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.AvisoCategoriaImutavel);
                return false;
            }

            if (
                await _repository.ExisteAsync(nome: categoriaDto.Descricao)
                is Categoria catergoriaExiste
            )
            {
                if (categoria.Code != catergoriaExiste.Code)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(
                            Message.RegistroExistente,
                            "A categoria",
                            categoriaDto.Descricao
                        )
                    );
                    return false;
                }
            }

            categoria.MapUpdateEntity(categoriaDto);

            _repository.Update(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );

                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            var categoria = await _repository.GetByCodigoAsync(code);

            if (categoria == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Categoria", code)
                );
                return false;
            }

            if (_repository.IdentificarCategoriaParaAcao(categoria.Code))
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.AvisoCategoriaImutavel);
                return false;
            }

            _repository.Delete(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Deletar")
                );
                return false;
            }

            return true;
        }
    }
}
