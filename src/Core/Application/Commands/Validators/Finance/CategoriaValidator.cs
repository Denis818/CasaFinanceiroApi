using Application.Commands.Dtos;
using FluentValidation;

namespace Application.Commands.Validators.Finance
{
    public class CategoriaValidator : AbstractValidator<CategoriaCommandDto>
    {
        public CategoriaValidator()
        {
            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage("A {PropertyName} é obrigatória.")
                .Length(3, 50)
                .WithMessage(
                    "A {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );
        }
    }
}
