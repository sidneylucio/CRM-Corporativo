using CRM.Corporativo.Domain.Commands.Customer;
using FluentValidation;

namespace CRM.Corporativo.Application.Validators;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MaximumLength(20).WithMessage("Telefone não pode exceder 20 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório")
            .EmailAddress().WithMessage("E-mail inválido");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Length(8).WithMessage("CEP deve ter 8 dígitos");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Logradouro é obrigatório");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Número é obrigatório");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("Bairro é obrigatório");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Cidade é obrigatória");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Estado é obrigatório")
            .Length(2).WithMessage("Estado deve ter 2 caracteres (sigla)");
    }
}
