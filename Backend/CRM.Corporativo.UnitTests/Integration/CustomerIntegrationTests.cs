using CRM.Corporativo.Application.Services;
using CRM.Corporativo.Domain.Commands.Customer;
using CRM.Corporativo.Domain.Enums;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Domain.Services;
using CRM.Corporativo.Infra.Data.Context;
using CRM.Corporativo.Infra.Data.EventStore;
using CRM.Corporativo.Infra.Data.Interfaces;
using CRM.Corporativo.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CRM.Corporativo.UnitTests.Integration;

public class CustomerIntegrationTests : IDisposable
{
    private readonly ApplicationContext _context;
    private readonly ICustomerRepository _repository;
    private readonly IEventStore _eventStore;
    private readonly CustomerService _service;

    public CustomerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationContext(options, null);
        _repository = new CustomerRepository(_context);
        _eventStore = new InMemoryEventStore(_context);

        var viaCepMock = new Mock<IViaCepService>();
        viaCepMock.Setup(v => v.GetAddressAsync(It.IsAny<string>())).ReturnsAsync((ViaCepAddress?)null);

        var requestInfoMock = new Mock<IRequestInfo>();
        requestInfoMock.Setup(r => r.Name).Returns("tester");

        _service = new CustomerService(
            _repository,
            _eventStore,
            viaCepMock.Object,
            requestInfoMock.Object,
            NullLogger<CustomerService>.Instance);
    }

    [Fact]
    public async Task Create_Then_Get_Returns_Customer()
    {
        var cmd = BuildCreateCommand("Ana Costa", "44455566677", "ana@test.com");

        var createResult = await _service.CreateAsync(cmd, CancellationToken.None);
        Assert.True(createResult.IsSuccess);

        var getResult = await _service.GetByIdAsync(createResult.Value.Id, CancellationToken.None);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("ana@test.com", getResult.Value.Email);
    }

    [Fact]
    public async Task Create_Duplicate_Document_Returns_Failure()
    {
        var cmd = BuildCreateCommand("Pedro Alves", "55566677788", "pedro@test.com");
        await _service.CreateAsync(cmd, CancellationToken.None);

        var cmd2 = cmd with { Email = "outro@test.com" };
        var result = await _service.CreateAsync(cmd2, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "Customer.DuplicateDocument");
    }

    [Fact]
    public async Task Create_Duplicate_Email_Returns_Failure()
    {
        var cmd = BuildCreateCommand("Luiza Melo", "66677788899", "luiza@test.com");
        await _service.CreateAsync(cmd, CancellationToken.None);

        var cmd2 = cmd with { Document = "99988866655" };
        var result = await _service.CreateAsync(cmd2, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == "Customer.DuplicateEmail");
    }

    [Fact]
    public async Task Create_Records_Event_In_EventStore()
    {
        var cmd = BuildCreateCommand("Bruno Neves", "33344455566", "bruno@test.com");
        var createResult = await _service.CreateAsync(cmd, CancellationToken.None);

        Assert.True(createResult.IsSuccess);

        var eventsResult = await _service.GetEventsAsync(createResult.Value.Id, CancellationToken.None);
        Assert.True(eventsResult.IsSuccess);
        Assert.Single(eventsResult.Value);
        Assert.Equal("CustomerCreated", eventsResult.Value[0].EventType);
    }

    [Fact]
    public async Task Update_Then_Delete_Records_Events()
    {
        var cmd = BuildCreateCommand("Fernanda Lima", "11122233344", "fernanda@test.com");
        var createResult = await _service.CreateAsync(cmd, CancellationToken.None);
        var id = createResult.Value.Id;

        var updateCmd = new UpdateCustomerCommand(
            id, "Fernanda Lima Atualizada", "11988880000", "fernanda@test.com",
            "01310100", "Av. Nova", "200", "Centro", "São Paulo", "SP", null, false);

        var updateResult = await _service.UpdateAsync(updateCmd, CancellationToken.None);
        Assert.True(updateResult.IsSuccess);

        var deleteResult = await _service.DeleteAsync(id, CancellationToken.None);
        Assert.True(deleteResult.IsSuccess);

        var eventsResult = await _service.GetEventsAsync(id, CancellationToken.None);
        Assert.Equal(3, eventsResult.Value.Count);
        Assert.Equal("CustomerCreated", eventsResult.Value[0].EventType);
        Assert.Equal("CustomerUpdated", eventsResult.Value[1].EventType);
        Assert.Equal("CustomerDeleted", eventsResult.Value[2].EventType);
    }

    private static CreateCustomerCommand BuildCreateCommand(string name, string document, string email) =>
        new(name, document, CustomerTypeEnum.PessoaFisica,
            DateTime.Today.AddYears(-25), "11999990000", email,
            "01310100", "Av. Paulista", "100", "Bela Vista", "São Paulo", "SP",
            null, false);

    public void Dispose() => _context.Dispose();
}
