using FluentValidation;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;

namespace Challenge.Microservices.SubscriptionApi.Commands.Validations
{
    /// <summary>
    /// Validator for CreateSubscriptionCommand to ensure valid rental parameters including rider eligibility and plan selection
    /// </summary>
    public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
    {
        public CreateSubscriptionCommandValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.")
                .MinimumLength(3).WithMessage("Identifier must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Identifier must not exceed 50 characters.");

            RuleFor(x => x.RiderIdentifier)
                .NotEmpty().WithMessage("Rider ID is required.");

            RuleFor(x => x.MotorbikeIdentifier)
                .NotEmpty().WithMessage("Motorbike ID is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(BeAtLeastTomorrow).WithMessage("Start date must be at least the first day after creation.");

            RuleFor(x => x.ExpectedEndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x.PredictedEndDate)
                .NotEmpty().WithMessage("Predicted end date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("Predicted end date must be after start date.");

            RuleFor(x => x.PlanDays)
                .NotEmpty().WithMessage("Plan is required.")
                .Must(BeValidPlan).WithMessage("Invalid plan. Valid plans: 7, 15, 30, 45, or 50 days.");
        }

        private bool BeAtLeastTomorrow(DateTime startDate)
        {
            return startDate.Date >= DateTime.UtcNow.Date.AddDays(1);
        }

        private bool BeValidPlan(int planDays)
        {
            return RentalPlans.Plans.ContainsKey(planDays);
        }
    }
}