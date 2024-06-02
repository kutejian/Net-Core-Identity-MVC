
namespace Net_Core_Identity_MVC.Models.GitHubModel
{
    public interface IOAuth20Credential
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public interface IRedirectUri
    {
        public string RedirectUri { get; set; }
    }
    public class GithubCredential : IOAuth20Credential, IRedirectUri
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}
