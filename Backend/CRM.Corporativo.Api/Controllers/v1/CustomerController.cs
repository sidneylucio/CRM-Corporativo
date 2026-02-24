using CRM.Corporativo.Api.Filters;
using CRM.Corporativo.Application.Services;
using CRM.Corporativo.Application.Validators;
using CRM.Corporativo.Domain.Base;
using CRM.Corporativo.Domain.Commands.Customer;
using CRM.Corporativo.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Corporativo.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IList<TError>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.GetByIdAsync(id, cancellationToken);

        return response.Match<ActionResult, CustomerResponse>(
            onSuccess: Ok,
            onFailure: NotFound);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _customerService.GetAllAsync(cancellationToken);

        return response.Match<ActionResult, List<CustomerResponse>>(
            onSuccess: Ok,
            onFailure: BadRequest);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IList<TError>), StatusCodes.Status400BadRequest)]
    [FluentValidationActionFilter<CreateCustomerCommand, CreateCustomerCommandValidator>()]
    public async Task<ActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var response = await _customerService.CreateAsync(command, cancellationToken);

        return response.Match<ActionResult, CustomerResponse>(
            onSuccess: (c) => CreatedAtAction(nameof(GetById), new { id = c.Id }, c),
            onFailure: BadRequest);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IList<TError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IList<TError>), StatusCodes.Status404NotFound)]
    [FluentValidationActionFilter<UpdateCustomerCommand, UpdateCustomerCommandValidator>()]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Id da URL não corresponde ao Id do corpo da requisição");

        var response = await _customerService.UpdateAsync(command, cancellationToken);

        return response.Match<ActionResult, CustomerResponse>(
            onSuccess: Ok,
            onFailure: (errors) => errors.Errors.Any(e => e.Code == "Customer.NotFound")
                ? NotFound(errors.Errors)
                : BadRequest(errors.Errors));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IList<TError>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.DeleteAsync(id, cancellationToken);

        return response.Match<ActionResult>(
            onSuccess: () => NoContent(),
            onFailure: NotFound);
    }

    [HttpGet("{id:guid}/events")]
    [ProducesResponseType(typeof(List<CustomerEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IList<TError>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetEvents(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.GetEventsAsync(id, cancellationToken);

        return response.Match<ActionResult, List<CustomerEventResponse>>(
            onSuccess: Ok,
            onFailure: NotFound);
    }
}
