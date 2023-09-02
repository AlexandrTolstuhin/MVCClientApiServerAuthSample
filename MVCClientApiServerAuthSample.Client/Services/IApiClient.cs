using Refit;

namespace MVCClientApiServerAuthSample.Client.Services;

public interface IApiClient
{
    [Get("/getToken/{name}")]
    Task<string> GetTokenAsync(string name);

    [Get("/secret")]
    Task<string> GetSecretAsync([Header("Authorization")] string authorization);
}