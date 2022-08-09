namespace WeSafe.Web.Common.Exceptions.Abstract
{
    public interface IHttpException
    {
        int StatusCode { get; }

        string GetMessage();
    }
}
