using FluentValidation;

namespace Challenge.Microservices.SubscriptionApi.Queries.Validations
{
    /// <summary>
    /// Validator for GetSubscriptionQuery to ensure valid identifier parameter
    /// </summary>
    public class GetSubscriptionQueryValidator : AbstractValidator<GetSubscriptionQuery>
    {
        public GetSubscriptionQueryValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.")
                .MinimumLength(3).WithMessage("Identifier must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Identifier must not exceed 50 characters.");
        }
    }
}
