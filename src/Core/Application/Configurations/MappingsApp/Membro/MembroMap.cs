using Application.Commands.Dtos;
using Domain.Models.Membros;

namespace Application.Configurations.MappingsApp
{
    public static class MembroMap
    {
        public static Membro MapToEntity(this MembroCommandDto membroDto)
        {
            return new Membro { Nome = membroDto.Nome, Telefone = membroDto.Telefone };
        }
        public static void MapUpdateEntity(this Membro membro, MembroCommandDto membroDto)
        {
            membro.Nome = membroDto.Nome;
            membro.Telefone = membroDto.Telefone;
        }

    }
}
