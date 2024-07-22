using Application.Commands.Dtos;
using FluentValidation;

namespace Application.Commands.Validators.Finance
{
    public class MembroValidator : AbstractValidator<MembroCommandDto>
    {
        public MembroValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .Length(3, 25)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );

            RuleFor(x => x.Telefone)
                .NotEmpty()
                .Length(11, 17)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                )
                .WithMessage("O telefone é obrigatório.")
                .Matches(@"^[\d\s()+-]*$")
                .WithMessage("Por favor, insira um número de telefone válido.");
        }
    }
}
