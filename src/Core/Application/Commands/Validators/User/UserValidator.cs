using Application.Commands.Dtos;
using FluentValidation;

namespace Application.Commands.Validators
{
    public class UserValidator : AbstractValidator<UserCommandDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .EmailAddress()
                .WithMessage("O {PropertyName} deve ser válido.")
                .Length(3, 25)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("A {PropertyName} é obrigatória.")
                .Matches("[A-Z]")
                .WithMessage("A {PropertyName} deve conter ao menos uma letra maiúscula.")
                .Matches("[0-9]")
                .WithMessage("A {PropertyName} deve conter ao menos um número.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("A {PropertyName} deve conter ao menos um caractere especial.")
                .Length(4, 25)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );
        }
    }
}
