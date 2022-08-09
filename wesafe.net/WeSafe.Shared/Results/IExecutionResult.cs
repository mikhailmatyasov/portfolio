namespace WeSafe.Shared.Results
{
    /// <summary>
    /// Provides an abstraction for result of operation.
    /// </summary>
    public interface IExecutionResult
    {
        /// <summary>
        /// Indicates that operation is successful.
        /// </summary>
        bool IsSuccess { get; }
    }
}