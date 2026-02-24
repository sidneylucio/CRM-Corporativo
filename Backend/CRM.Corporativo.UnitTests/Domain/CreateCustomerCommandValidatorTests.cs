using CRM.Corporativo.Application.Validators;
using CRM.Corporativo.Domain.Commands.Customer;
using CRM.Corporativo.Domain.Enums;
using FluentValidation.TestHelper;

namespace CRM.Corporativo.UnitTests.Domain;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    private static CreateCustomerCommand ValidPf() => new(
        Name: "João Silva",
        Document: "12345678901",
        CustomerType: CustomerTypeEnum.PessoaFisica,
        BirthDate: DateTime.Today.AddYears(-20),
        Phone: "11999990000",
        Email: "joao@example.com",
        ZipCode: "01310100",
        Street: "Av. Paulista",
        Number: "1000",
        Neighborhood: "Bela Vista",
        City: "São Paulo",
        State: "SP",
        StateRegistration: null,
        IsStateRegistrationExempt: false);

    private static CreateCustomerCommand ValidPj() => new(
        Name: "Empresa Teste LTDA",
        Document: "12345678000199",
        CustomerType: CustomerTypeEnum.PessoaJuridica,
        BirthDate: DateTime.Today.AddYears(-5),
        Phone: "1133334444",
        Email: "empresa@example.com",
        ZipCode: "01310100",
        Street: "Av. Paulista",
        Number: "500",
        Neighborhood: "Bela Vista",
        City: "São Paulo",
        State: "SP",
        StateRegistration: "123456789",
        IsStateRegistrationExempt: false);

    [Fact]
    public void Should_Pass_Valid_PessoaFisica()
    {
        var result = _validator.TestValidate(ValidPf());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_Valid_PessoaJuridica()
    {
        var result = _validator.TestValidate(ValidPj());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_PessoaFisica_Under18()
    {
        var cmd = ValidPf() with { BirthDate = DateTime.Today.AddYears(-17) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
    }

    [Fact]
    public void Should_Fail_PessoaJuridica_Without_IE_And_Not_Exempt()
    {
        var cmd = ValidPj() with { StateRegistration = null, IsStateRegistrationExempt = false };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Pass_PessoaJuridica_When_Exempt()
    {
        var cmd = ValidPj() with { StateRegistration = null, IsStateRegistrationExempt = true };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_InvalidEmail()
    {
        var cmd = ValidPf() with { Email = "not-an-email" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Fail_EmptyName()
    {
        var cmd = ValidPf() with { Name = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_InvalidZipCode()
    {
        var cmd = ValidPf() with { ZipCode = "123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ZipCode);
    }

    [Fact]
    public void Should_Fail_InvalidState()
    {
        var cmd = ValidPf() with { State = "São Paulo" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    [Fact]
    public void Should_Fail_InvalidDocument_TooShort()
    {
        var cmd = ValidPf() with { Document = "123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Document);
    }
}
