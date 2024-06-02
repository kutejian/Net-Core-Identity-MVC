using Net_Core_Identity_MVC.Models.GitHubModel;

namespace Net_Core_Identity_MVC.Models.GoogleModel
{
    public class GoogleCredential : IOAuth20Credential, IRedirectUri
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}
