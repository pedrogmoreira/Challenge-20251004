using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Queries.Validations
{
    /// <summary>
    /// Validator for GetMotorbikeByIdentifierQuery to ensure valid identifier parameter
    /// </summary>
    public class GetMotorbikeByIdentifierQueryValidator : AbstractValidator<GetMotorbikeByIdentifierQuery>
    {
        public GetMotorbikeByIdentifierQueryValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.")
                .MinimumLength(3).WithMessage("Identifier must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Identifier must not exceed 50 characters.");
        }
    }
}
