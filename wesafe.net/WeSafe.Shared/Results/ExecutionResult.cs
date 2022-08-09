using System.Collections.Generic;
using System.Linq;

namespace WeSafe.Shared.Results
{
    /// <summary>
    /// Base abstract implementation for result of operation.
    /// </summary>
    public abstract class ExecutionResult : IExecutionResult
    {
        private static readonly IExecutionResult SuccessResult = new SuccessExecutionResult();
        private static readonly IExecutionResult FailedResult = new FailedExecutionResult(Enumerable.Empty<string>());

        /// <summary>
        /// Gets a successful result of operation.
        /// </summary>
        /// <returns>A IExecutionResult as a success result.</returns>
        public static IExecutionResult Success() => SuccessResult;

        /// <summary>
        /// Gets a failed result of operation.
        /// </summary>
        /// <returns>A IExecutionResult as a failed result.</returns>
        public static IExecutionResult Failed() => FailedResult;

        /// <summary>
        /// Gets a failed result of operation with a list of messages.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns>A IExecutionResult as a failed result.</returns>
        public static IExecutionResult Failed(IEnumerable<string> errors) => new FailedExecutionResult(errors);

        /// <summary>
        /// Gets a failed result of operation with a list of messages.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns>A IExecutionResult as a failed result.</returns>
        public static IExecutionResult Failed(params string[] errors) => new FailedExecutionResult(errors);

        /// <summary>
        /// Gets a successful result with a payload data.
        /// </summary>
        /// <typeparam name="T">The type of the payload data.</typeparam>
        /// <param name="value">Payload data.</param>
        /// <returns>A IExecutionResult as a success result.</returns>
        public static IExecutionResult Payload<T>(T value) => new PayloadExecutionResult<T>(value);

        /// <summary>
        /// Indicates that operation is successful.
        /// </summary>
        public abstract bool IsSuccess { get; }

        /// <summary>
        /// Represents result as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ExecutionResult - IsSuccess:{IsSuccess}";
        }
    }
}