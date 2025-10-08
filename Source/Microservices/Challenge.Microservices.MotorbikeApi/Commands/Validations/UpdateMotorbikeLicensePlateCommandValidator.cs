using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Common.Data.Interfaces;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using FluentValidation;
using MongoDB.Driver;

namespace Challenge.Microservices.MotorbikeApi.Commands.Validations
{
    /// <summary>
    /// Validator for license plate update command
    /// </summary>
    public class UpdateMotorbikeLicensePlateCommandValidator : AbstractValidator<UpdateMotorbikeLicensePlateCommand>
    {
        private readonly IMotorbikeRepository _repository;

        public UpdateMotorbikeLicensePlateCommandValidator(IMotorbikeRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Motorbike identifier is required.");

            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("License plate is required.")
                .Matches(@"^[A-Z]{3}[0-9]{1}[A-Z0-9]{1}[0-9]{2}$")
                .WithMessage("License plate must follow the Brazilian format (ABC1D23 or ABC1234).")
                .MustAsync(BeUniqueLicensePlate).WithMessage("License plate already exists.");
        }

        /// <summary>
        /// Validates that the new license plate is unique
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