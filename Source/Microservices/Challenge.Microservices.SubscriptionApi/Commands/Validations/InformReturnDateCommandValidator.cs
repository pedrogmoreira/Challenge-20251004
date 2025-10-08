using FluentValidation;

namespace Challenge.Microservices.SubscriptionApi.Commands.Validations
{
    /// <summary>
    /// Validator for InformReturnDateCommand to ensure valid return date and subscription identifier
    /// </summary>
    public class InformReturnDateCommandValidator : AbstractValidator<InformReturnDateCommand>
    {
        public InformReturnDateCommandValidator()
        {
            RuleFor(x => x.SubscriptionIdentifier)
                .NotEmpty().WithMessage("Subscription ID is required.");

            RuleFor(x => x.ReturnDate)
                .NotEmpty().WithMessage("Return date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Return date cannot be in the future.");
        }
    }
}