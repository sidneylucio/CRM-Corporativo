using CRM.Corporativo.Domain.Commands.Customer;
using CRM.Corporativo.Domain.Enums;
using FluentValidation;

namespace CRM.Corporativo.Application.Validators;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("CPF/CNPJ é obrigatório")
            .Must(BeValidDocument).WithMessage("CPF/CNPJ inválido");

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

        When(x => x.CustomerType == CustomerTypeEnum.PessoaFisica, () =>
        {
            RuleFor(x => x.BirthDate)
                .Must(BeAtLeast18YearsOld)
                .WithMessage("Pessoa Física deve ter no mínimo 18 anos");
        });

        When(x => x.CustomerType == CustomerTypeEnum.PessoaJuridica, () =>
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.StateRegistration) || x.IsStateRegistrationExempt)
                .WithMessage("Pessoa Jurídica deve informar a Inscrição Estadual ou marcar como Isento");
        });
    }

    private static bool BeAtLeast18YearsOld(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }

    private static bool BeValidDocument(string document)
    {
        var digits = new string(document.Where(char.IsDigit).ToArray());
        return digits.Length == 11 || digits.Length == 14;
    }
}
