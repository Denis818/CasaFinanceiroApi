using Application.Commands.Dtos;
using FluentValidation;

namespace Application.Commands.Validators.Finance
{
    public class DespesaValidator : AbstractValidator<DespesaCommandDto>
    {
        public DespesaValidator()
        {
            RuleFor(x => x.Item)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .Length(3, 50)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );

            RuleFor(x => (double)x.Preco)
                .InclusiveBetween(0.01, 9999.99)
                .WithMessage(
                    "O {PropertyName} não pode ser menor que 0.01, e maior que 9999."
                );

            RuleFor(x => x.Quantidade)
                .InclusiveBetween(1, 999)
                .WithMessage(
                    "A {PropertyName} não pode ser menor que 1, e maior que 9999."
                );

            RuleFor(x => x.Fornecedor)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .Length(3, 50)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );
        }
    }
}
