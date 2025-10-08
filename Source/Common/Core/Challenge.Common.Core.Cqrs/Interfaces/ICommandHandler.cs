using Challenge.Common.Core.Response.Models.Interfaces;

namespace Challenge.Common.Core.Cqrs.Interfaces
{
    /// <summary>
    /// Command handler interface for commands that return a response
    /// </summary>
    /// <typeparam name="TCommand">The command type to be handled</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Handles the command execution
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>A response indicating the result of the operation</returns>
        Task<IResponse> Handle(TCommand command);
    }
}
