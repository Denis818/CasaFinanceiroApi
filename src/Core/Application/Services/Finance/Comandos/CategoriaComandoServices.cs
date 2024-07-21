using Application.Interfaces.Services.Finance.Comandos;
using Application.Resources.Messages;
using Application.Services.Base;
using Application.Services.Mappings;
using Domain.Dtos.Categorias;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;

namespace Application.Services.Finance.Comandos
{
    public class CategoriaComandoServices(IServiceProvider service)
        : BaseAppService<Categoria, ICategoriaRepository>(service), ICategoriaComandoServices
    {
        public async Task<Categoria> InsertAsync(CategoriaDto categoriaDto)
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

        public async Task<Categoria> UpdateAsync(int id, CategoriaDto categoriaDto)
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
