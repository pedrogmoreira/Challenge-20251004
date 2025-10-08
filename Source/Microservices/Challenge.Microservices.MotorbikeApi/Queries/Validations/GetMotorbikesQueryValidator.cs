using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Queries.Validations
{
    /// <summary>
    /// Validator for GetMotorbikesQuery to ensure valid pagination parameters
    /// </summary>
    public class GetMotorbikesQueryValidator : AbstractValidator<GetMotorbikesQuery>
    {
        public GetMotorbikesQueryValidator()
        {
            RuleFor(x => x.LicensePlate)
                .Matches(@"^[A-Z]{3}[0-9]{1}[A-Z0-9]{1}[0-9]{2}$")
                .WithMessage("License plate must follow the Brazilian format (ABC1D23 or ABC1234).");
        }
    }
}
