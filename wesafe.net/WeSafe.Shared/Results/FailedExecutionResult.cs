using System;
using System.Collections.Generic;
using System.Linq;

namespace WeSafe.Shared.Results
{
    /// <summary>
    /// Failed execution result implementation.
    /// </summary>
    public class FailedExecutionResult : ExecutionResult
    {
        /// <summary>
        /// Error messages.
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; }

        /// <summary>
        /// Creates a failed execution result with errors
        /// </summary>
        /// <param name="errors">Error messages.</param>
        public FailedExecutionResult(IEnumerable<string> errors)
        {
            Errors = (errors ?? Enumerable.Empty<string>()).ToList();
        }

        /// <summary>
        /// Creates a failed execution result with errors
        /// </summary>
        /// <param name="errors">Error messages.</param>
        public FailedExecutionResult(params string[] errors)
        {
            Errors = (errors ?? Enumerable.Empty<string>()).ToList();
        }

        /// <summary>
        /// Indicates that operation is successful.
        /// </summary>
        public override bool IsSuccess { get; } = false;

        /// <summary>
        /// Represents result as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Errors.Any()
                ? $"Failed execution due to: {String.Join(", ", Errors)}"
                : "Failed execution";
        }
    }
}