using Application.Commands.Dtos;
using FluentValidation;

namespace Application.Commands.Validators.Finance
{
    public class GrupoFaturaValidator : AbstractValidator<GrupoFaturaCommandDto>
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
