using FluentValidation;
using Verus.Application.Contracts.Relevance;

namespace Verus.Application.UseCases.CompanyBoundedContext.CreateCompany;

public sealed class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator(IRelevanceEvaluator relevanceEvaluator)
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MinimumLength(2).WithMessage("Company name must contain at least 2 characters.")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

        RuleFor(command => command.Website)
            .NotEmpty().WithMessage("Website URL is required.")
            .Must(BeAWellFormedHttpUrl)
            .WithMessage("Website must be a valid, well-formed URL (e.g. https://www.example.com).");

        RuleFor(command => command)
            .Custom((command, context) =>
            {
                if (string.IsNullOrWhiteSpace(command.Name) || !BeAWellFormedHttpUrl(command.Website))
                    return;

                var assessment = relevanceEvaluator.AssessNameWebsite(command.Name, command.Website);
                if (!assessment.IsRelevant)
                {
                    context.AddFailure(
                        nameof(command.Website),
                        $"Company name '{command.Name}' does not appear relevant to '{command.Website}'. {assessment.Explanation}");
                }
            });
    }

    private static bool BeAWellFormedHttpUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
