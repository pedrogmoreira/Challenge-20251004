using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Commands.Validations
{
    /// <summary>
    /// Validator for motorbike registration command
    /// </summary>
    public class RegisterMotorbikeCommandValidator : AbstractValidator<RegisterMotorbikeCommand>
    {
        private readonly IMotorbikeRepository _repository;

        public RegisterMotorbikeCommandValidator(IMotorbikeRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.")
                .MinimumLength(3).WithMessage("Identifier must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Identifier must not exceed 50 characters.")
                .MustAsync(BeUniqueIdentifier).WithMessage("Identifier already exists.");

            RuleFor(x => x.Year)
                .NotEmpty().WithMessage("Year is required.")
                .GreaterThan(1900).WithMessage("Year must be greater than 1900.")
                .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year cannot be in the future.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MinimumLength(2).WithMessage("Model must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Model must not exceed 100 characters.");

            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("License plate is required.")
                .Matches(@"^[A-Z]{3}[0-9]{1}[A-Z0-9]{1}[0-9]{2}$")
                .WithMessage("License plate must follow the Brazilian format (ABC1D23 or ABC1234).")
                .MustAsync(BeUniqueLicensePlate).WithMessage("License plate already exists.");
        }

        /// <summary>
        /// Validates that the identifier is unique
        /// </summary>
        /// <param name="identifier">The identifier to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the identifier is unique, false if it already exists</returns>
        private async Task<bool> BeUniqueIdentifier(string? identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier)) return true;
            return !await _repository.IdentifierExistsAsync(identifier);
        }

        /// <summary>
        /// Validates that the license plate is unique
        /// </summary>
        /// <param name="licensePlate">The license plate to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the license plate is unique, false if it already exists</returns>
        private async Task<bool> BeUniqueLicensePlate(string? licensePlate, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(licensePlate)) return true;
            return !await _repository.LicensePlateExistsAsync(licensePlate);
        }
    }
}