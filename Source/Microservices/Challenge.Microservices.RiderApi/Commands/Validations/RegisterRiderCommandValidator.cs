using Challenge.Microservices.RiderApi.Infra.Data.Repositories;
using DocumentValidator;
using FluentValidation;

namespace Challenge.Microservices.RiderApi.Commands.Validations
{
    public class RegisterRiderCommandValidator : AbstractValidator<RegisterRiderCommand>
    {
        private readonly IRiderRepository _repository;
        public RegisterRiderCommandValidator(IRiderRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.")
                .Length(3, 50).WithMessage("Identifier must be between 3 and 50 characters.")
                .MustAsync(BeUniqueIdentifier).WithMessage("Identifier already exists.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(5, 100).WithMessage("Username must be between 5 and 100 characters.");

            RuleFor(x => x.Cnpj)
                .NotEmpty().WithMessage("CNPJ is required.")
                .Must(BeAValidCNPJ).WithMessage("CNPJ is invalid.")
                .MustAsync(BeUniqueCnpj).WithMessage("CNPJ already exists.");

            RuleFor(x => x.Birthdate)
                .NotEmpty().WithMessage("Birthdate is required.")
                .LessThan(DateTime.Now).WithMessage("Birthdate must be in the past.");

            RuleFor(x => x.CnhNumber)
                .NotEmpty().WithMessage("CNH Number is required.")
                .Must(BeAValidCNH).WithMessage("CNH is invalid.")
                .MustAsync(BeUniqueCnhNumber).WithMessage("CNH number already exists.");

            RuleFor(x => x.CnhType)
                .NotEmpty().WithMessage("CNH Type is required.")
                .Must(type => type?.ToString() is "A" or "B" or "AB")
                    .WithMessage("CNH Type must be 'A', 'B', or 'AB'.");


        }

        private bool BeAValidCNPJ(string? cnpj) => CnpjValidation.Validate(cnpj);

        private bool BeAValidCNH(string? cnh) => CnhValidation.Validate(cnh);

        private async Task<bool> BeUniqueIdentifier(string? identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier)) return true;
            return !await _repository.IdentifierExistsAsync(identifier);
        }

        private async Task<bool> BeUniqueCnpj(string? cnpj, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cnpj)) return true;
            return !await _repository.CnpjExistsAsync(cnpj);
        }

        private async Task<bool> BeUniqueCnhNumber(string? cnhNumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cnhNumber)) return true;
            return !await _repository.CnhNumberExistsAsync(cnhNumber);
        }
    }
}
