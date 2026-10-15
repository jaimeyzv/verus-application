using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Verus.Application.UseCases.CompanyBoundedContext.CreateCompany;
using Verus.Infrastructure.Relevance;

namespace Verus.Tests.Application;

[TestFixture]
public class CreateCompanyValidatorTests
{
    private CreateCompanyValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        var evaluator = new RelevanceEvaluator(Options.Create(new RelevanceOptions()));
        _validator = new CreateCompanyValidator(evaluator);
    }

    [Test]
    public void Validate_RelevantNameAndWebsite_HasNoErrors()
    {
        var command = new CreateCompanyCommand("Acme Widgets", "https://www.acmewidgets.com");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_EmptyName_HasNameError()
    {
        var command = new CreateCompanyCommand("", "https://acmewidgets.com");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.Name);
    }

    [Test]
    public void Validate_SingleCharacterName_HasNameError()
    {
        var command = new CreateCompanyCommand("A", "https://a.com");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.Name);
    }

    [Test]
    public void Validate_MalformedWebsite_HasWebsiteError()
    {
        var command = new CreateCompanyCommand("Acme Widgets", "not-a-url");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.Website);
    }

    [Test]
    public void Validate_NameNotRelevantToWebsite_HasWebsiteError()
    {
        var command = new CreateCompanyCommand("Acme Widgets", "https://totallyunrelateddomain.com");

        var result = _validator.TestValidate(command);

        var failure = result.ShouldHaveValidationErrorFor(command => command.Website);
        Assert.That(failure.First().ErrorMessage, Does.Contain("does not appear relevant"));
    }
}
