using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Web.Common.Authentication
{
    /// <summary>
    /// Represent a base login model.
    /// </summary>
    public class LoginModel : ISupportUserName
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}