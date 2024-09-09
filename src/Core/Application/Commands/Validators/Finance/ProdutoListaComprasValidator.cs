using Application.Commands.Dtos.Despesa;
using FluentValidation;

namespace Application.Commands.Validators.Finance
{
    public class ProdutoListaComprasValidator : AbstractValidator<ProdutoListaComprasCommandDto>
    {
        public ProdutoListaComprasValidator()
        {
            RuleFor(x => x.Item)
                .NotEmpty()
                .WithMessage("A {PropertyName} é obrigatória.")
                .Length(3, 50)
                .WithMessage(
                    "A {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );
        }
    }
}
