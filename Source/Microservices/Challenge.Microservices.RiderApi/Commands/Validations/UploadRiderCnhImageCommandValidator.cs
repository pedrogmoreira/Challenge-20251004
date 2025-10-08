using FluentValidation;

namespace Challenge.Microservices.RiderApi.Commands.Validations
{
    public class UploadRiderCnhImageCommandValidator : AbstractValidator<UploadRiderCnhImageCommand>
    {
        private const int MAX_FILE_SIZE_MB = 50;
        private const int BYTES_PER_MB = 1_048_576;
        private readonly string[] _allowedFormats = ["image/png", "image/bmp"];

        public UploadRiderCnhImageCommandValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Identifier is required.");

            RuleFor(x => x.CnhImage)
                .NotEmpty().WithMessage("The CNH image is required for upload.")
                .Must(BeValidBase64String).WithMessage("Invalid Base64 format.")
                .Must(BeValidImageFormat).WithMessage($"Image format not allowed. Accepted formats: {string.Join(", ", _allowedFormats)}")
                .Must(BeWithinSizeLimit).WithMessage($"Image exceeds maximum size of {MAX_FILE_SIZE_MB}MB.");
        }

        private bool BeValidBase64String(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            try
            {
                var cleanBase64 = RemoveBase64Prefix(base64String);
                Convert.FromBase64String(cleanBase64);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string RemoveBase64Prefix(string base64String)
        {
            if (base64String.Contains(','))
            {
                return base64String[(base64String.IndexOf(',') + 1)..];
            }
            return base64String;
        }

        private bool BeValidImageFormat(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            try
            {
                var cleanBase64 = RemoveBase64Prefix(base64String);
                var imageBytes = Convert.FromBase64String(cleanBase64);

                if (imageBytes.Length < 4)
                    return false;

                if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                    return true;

                if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool BeWithinSizeLimit(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            try
            {
                var cleanBase64 = RemoveBase64Prefix(base64String);
                var imageBytes = Convert.FromBase64String(cleanBase64);
                return imageBytes.Length <= BYTES_PER_MB * MAX_FILE_SIZE_MB;
            }
            catch
            {
                return false;
            }
        }
    }
}
