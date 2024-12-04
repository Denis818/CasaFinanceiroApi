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

            RuleFor(x => (double)x.Desconto)
                .InclusiveBetween(0, 9999.99)
                .WithMessage(
                    "O {PropertyName} não pode ser menor que 0, e maior que 9999."
                );
        }
    }
}
