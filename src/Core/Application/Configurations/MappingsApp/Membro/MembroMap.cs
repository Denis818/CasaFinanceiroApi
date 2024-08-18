using Application.Commands.Dtos;
using Domain.Dtos;
using Domain.Models.Membros;

namespace Application.Configurations.MappingsApp
{
    public static class MembroMap
    {
        public static MembroQueryDto MapToDTO(this Membro membro)
        {
            return new MembroQueryDto
            {
                Nome = membro.Nome,
                Telefone = membro.Telefone,
                DataInicio = membro.DataInicio,
                Code = membro.Code
            };
        }

        public static Membro MapToEntity(this MembroCommandDto membroDto)
        {
            return new Membro
            {
                Nome = membroDto.Nome,
                Telefone = membroDto.Telefone,
            };
        }

        public static void MapUpdateEntity(this Membro membro, MembroCommandDto membroDto)
        {
            membro.Nome = membroDto.Nome;
            membro.Telefone = membroDto.Telefone;
        }
    }
}
