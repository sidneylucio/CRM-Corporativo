using CRM.Corporativo.Application.Services;
using CRM.Corporativo.Domain.Commands.Customer;
using CRM.Corporativo.Domain.Enums;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Domain.Services;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRM.Corporativo.UnitTests.Application;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repoMock = new();
    private readonly Mock<IEventStore> _eventStoreMock = new();
    private readonly Mock<IViaCepService> _viaCepMock = new();
    private readonly Mock<IRequestInfo> _requestInfoMock = new();
    private readonly Mock<ILogger<CustomerService>> _loggerMock = new();
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _requestInfoMock.Setup(r => r.Name).Returns("test-user");

        _viaCepMock
            .Setup(v => v.GetAddressAsync(It.IsAny<string>()))
            .ReturnsAsync((ViaCepAddress?)null);

        _eventStoreMock
            .Setup(e => e.AppendAsync(It.IsAny<CustomerEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new CustomerService(
            _repoMock.Object,
            _eventStoreMock.Object,
            _viaCepMock.Object,
            _requestInfoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Returns_Success_When_Valid()
    {
        _repoMock.Setup(r => r.GetByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.Insert(It.IsAny<Customer>(), It.IsAny<CancellationToken?>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var cmd = new CreateCustomerCommand(
            "Maria Souza", "11122233344", CustomerTypeEnum.PessoaFisica,
            DateTime.Today.AddYears(-25), "11999990000", "maria@email.com",
            "01310100", "Av. Paulista", "1", "Bela Vista", "São Paulo", "SP",
            null, false);

        var result = await _service.CreateAsync(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("maria@email.com", result.Value.Email);
    }

    [Fact]
    public async Task CreateAsync_Returns_Failure_When_Document_Duplicate()
    {
        _repoMock.Setup(r => r.GetByDocumentAsync("11122233344", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Document = "11122233344" });

        var cmd = new CreateCustomerCommand(
            "Maria Souza", "11122233344", CustomerTypeEnum.PessoaFisica,
            DateTime.Today.AddYears(-25), "11999990000", "maria@email.com",
            "01310100", "Av. Paulista", "1", "Bela Vista", "São Paulo", "SP",
            null, false);

        var result = await _service.CreateAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "Customer.DuplicateDocument");
    }

    [Fact]
    public async Task CreateAsync_Returns_Failure_When_Email_Duplicate()
    {
        _repoMock.Setup(r => r.GetByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.GetByEmailAsync("maria@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Email = "maria@email.com" });

        var cmd = new CreateCustomerCommand(
            "Maria Souza", "11122233344", CustomerTypeEnum.PessoaFisica,
            DateTime.Today.AddYears(-25), "11999990000", "maria@email.com",
            "01310100", "Av. Paulista", "1", "Bela Vista", "São Paulo", "SP",
            null, false);

        var result = await _service.CreateAsync(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "Customer.DuplicateEmail");
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Failure_When_Not_Found()
    {
        _repoMock.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken?>()))
            .ReturnsAsync((Customer?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "Customer.NotFound");
    }

    [Fact]
    public async Task DeleteAsync_Returns_Failure_When_Not_Found()
    {
        _repoMock.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken?>()))
            .ReturnsAsync((Customer?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "Customer.NotFound");
    }

    [Fact]
    public async Task CreateAsync_Appends_CustomerCreated_Event()
    {
        _repoMock.Setup(r => r.GetByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.Insert(It.IsAny<Customer>(), It.IsAny<CancellationToken?>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var cmd = new CreateCustomerCommand(
            "Carlos Lima", "99988877766", CustomerTypeEnum.PessoaFisica,
            DateTime.Today.AddYears(-30), "11988887777", "carlos@email.com",
            "01310100", "Rua X", "10", "Centro", "São Paulo", "SP",
            null, false);

        await _service.CreateAsync(cmd, CancellationToken.None);

        _eventStoreMock.Verify(
            e => e.AppendAsync(
                It.Is<CustomerEvent>(ev => ev.EventType == "CustomerCreated"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Uses_RequestInfo_Name_As_Actor()
    {
        _requestInfoMock.Setup(r => r.Name).Returns("usuario-teste");

        _repoMock.Setup(r => r.GetByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.Insert(It.IsAny<Customer>(), It.IsAny<CancellationToken?>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var cmd = new CreateCustomerCommand(
            "João Silva", "12312312300", CustomerTypeEnum.PessoaFisica,
            DateTime.Today.AddYears(-20), "11912345678", "joao@email.com",
            "01310100", "Rua Y", "5", "Centro", "São Paulo", "SP",
            null, false);

        await _service.CreateAsync(cmd, CancellationToken.None);

        _eventStoreMock.Verify(
            e => e.AppendAsync(
                It.Is<CustomerEvent>(ev => ev.OccurredBy == "usuario-teste"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
