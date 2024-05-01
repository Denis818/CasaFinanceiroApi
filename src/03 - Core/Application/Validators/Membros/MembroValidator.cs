﻿using Domain.Dtos.Membros;
using FluentValidation;

namespace Application.Validators.Membros
{
    public class MembroValidator : AbstractValidator<MembroDto>
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
                .Length(11, 16)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                )
                .WithMessage("O telefone é obrigatório.")
                .Matches(@"^[\d\s()+-]*$")
                .WithMessage("Por favor, insira um número de telefone válido.");
        }
    }
}
