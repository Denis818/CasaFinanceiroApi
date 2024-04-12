﻿using Domain.Dtos.Categoria;
using FluentValidation;

namespace Application.Validators.Finance
{
    public class CategoriaValidator : AbstractValidator<CategoriaDto>
    {
        public CategoriaValidator()
        {
            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage("A {PropertyName} é obrigatória.")
                .Length(3, 25)
                .WithMessage(
                    "A {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );
        }
    }
}
