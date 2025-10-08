using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Commands.Validations
{
    /// <summary>
    /// Validator for motorbike deletion command
    /// </summary>
    public class DeleteMotorbikeCommandValidator : AbstractValidator<DeleteMotorbikeCommand>
    {
        public DeleteMotorbikeCommandValidator()
        {
            RuleFor(x => x.Identifier)
               .NotEmpty().WithMessage("Identifier is required.")
               .MinimumLength(3).WithMessage("Identifier must be at least 3 characters.")
               .MaximumLength(50).WithMessage("Identifier must not exceed 50 characters.");
        }
    }
}
