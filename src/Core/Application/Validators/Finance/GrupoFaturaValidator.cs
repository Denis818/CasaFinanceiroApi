using Domain.Dtos.Despesas.Criacao;
using FluentValidation;

namespace Application.Validators.Despesas
{
    public class GrupoFaturaValidator : AbstractValidator<GrupoFaturaDto>
    {
        public GrupoFaturaValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O campo {PropertyName} é obrigatória.")
                .Length(3, 25)
                .WithMessage(
                    "O campo {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );
        }
    }
}
