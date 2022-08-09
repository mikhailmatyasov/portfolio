namespace WeSafe.Shared.Results
{
    /// <summary>
    /// Successful execution result implementation.
    /// </summary>
    public class SuccessExecutionResult : ExecutionResult
    {
        /// <summary>
        /// Indicates that operation is successful.
        /// </summary>
        public override bool IsSuccess { get; } = true;

        /// <summary>
        /// Represents result as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Successful execution";
        }
    }
}