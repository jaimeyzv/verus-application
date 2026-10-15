using MediatR;
using Microsoft.AspNetCore.Mvc;
using Verus.Application.UseCases.CompanyBoundedContext.CreateCompany;
using Verus.Application.UseCases.CompanyBoundedContext.GetAllCompanies;
using Verus.Application.UseCases.CompanyBoundedContext.GetCompanyById;
using Verus.Application.UseCases.CompanyBoundedContext.SearchCompanies;
using Verus.WebAPI.Contracts.Companies;

namespace Verus.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var companies = await mediator.Send(new GetAllCompaniesQuery(), cancellationToken);
        return Ok(companies);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? domain,
        [FromQuery] string? term,
        CancellationToken cancellationToken)
    {
        var companies = await mediator.Send(new SearchCompaniesQuery(name, domain, term), cancellationToken);
        return Ok(companies);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var company = await mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCompanyCommand(request.Name, request.Website);
        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
