namespace WeSafe.Shared.Results
{
    /// <summary>
    /// Successful execution result with payload data implementation.
    /// </summary>
    public class PayloadExecutionResult<TPayload> : SuccessExecutionResult
    {
        /// <summary>
        /// Gets a payload data of type TPayload.
        /// </summary>
        public TPayload Payload { get; }

        /// <summary>
        /// Creates a successful execution result with payload data
        /// </summary>
        /// <param name="payload"></param>
        public PayloadExecutionResult(TPayload payload)
        {
            Payload = payload;
        }
    }
}