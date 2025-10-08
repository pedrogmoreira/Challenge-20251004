using FluentValidation.Results;

namespace Challenge.Common.Core.Validation.Extensions
{
    /// <summary>
    /// Extension methods for FluentValidation results
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Converts a FluentValidation result into a dictionary of errors grouped by property name
        /// </summary>
        /// <param name="result">The validation result to convert</param>
        /// <returns>Dictionary where keys are property names and values are lists of error messages</returns>
        public static Dictionary<string, List<string>> ToErrorDictionary(this ValidationResult result)
        {
            return result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToList()
                );
        }

        /// <summary>
        /// Creates a single error dictionary for a specific field
        /// </summary>
        /// <param name="key">The field name or error key</param>
        /// <param name="message">The error message</param>
        /// <returns>Dictionary containing a single error entry</returns>
        public static Dictionary<string, List<string>> SingleError(string key, string message) =>
            new() { [key] = [message] };
    }
}
