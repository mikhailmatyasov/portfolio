namespace WeSafe.Bus.Contracts.User
{
    public interface ICreateUserValidationResult
    {
        bool IsValid { get; set; }

        string ErrorMessage { get; set; }
    }
}
