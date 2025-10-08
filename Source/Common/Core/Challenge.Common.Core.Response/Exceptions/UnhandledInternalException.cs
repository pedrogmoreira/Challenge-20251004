namespace Challenge.Common.Core.Response.Exceptions
{
    /// <summary>
    /// Represents an unhandled internal server error (HTTP 500)
    /// </summary>
    public class UnhandledInternalException : Exception
    {
        public UnhandledInternalException()
            : base("Unhandled internal server error.")
        {
        }
    }
}
