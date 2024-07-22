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
        : BaseCommandService<Categoria, ICategoriaRepository>(service), ICategoriaCommandServices
    {
        public async Task<Categoria> InsertAsync(CategoriaCommandDto categoriaDto)
        {
            if (Validator(categoriaDto))
                return null;

            if (await _repository.ExisteAsync(nome: categoriaDto.Descricao) != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "A categoria", categoriaDto.Descricao)
                );
                return null;
            }

            var categoria = categoriaDto.MapToEntity();
            await _repository.InsertAsync(categoria);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            return categoria;
        }

        public async Task<Categoria> UpdateAsync(int id, CategoriaCommandDto categoriaDto)
        {
            if (Validator(categoriaDto))
                return null;

            var categoria = await _repository.GetByIdAsync(id);

            if (categoria is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Categoria", id)
                );
                return null;
            }

            if (categoria.Descricao == categoriaDto.Descricao)
                return categoria;

            if (_repository.IdentificarCategoriaParaAcao(categoria.Id))
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.AvisoCategoriaImutavel);
                return null;
            }

            if (
                await _repository.ExisteAsync(nome: categoriaDto.Descricao)
                is Categoria catergoriaExiste
            )
            {
                if (categoria.Id != catergoriaExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(
                            Message.RegistroExistente,
                            "A categoria",
                            categoriaDto.Descricao
                        )
                    );
                    return null;
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
                return null;
            }

            return categoria;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(id);

            if (categoria == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Categoria", id)
                );
                return false;
            }

            if (_repository.IdentificarCategoriaParaAcao(categoria.Id))
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
