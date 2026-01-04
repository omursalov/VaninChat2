using Dropbox.Api;
using VaninChat2.Exceptions;
using VaninChat2.Helpers.Internet;
using VaninChat2.Models;

namespace VaninChat2.Helpers.Dropbox
{
    public static class DropboxHelper
    {
        public static async Task<DropboxAccessTokenInfo> CheckAccessToken(string token)
        {
            DropboxAccessTokenInfo result = null;

            await new AttemptHelper(number: 10, delaySec: 0).ExecuteAsync(async () =>
            {
                await new ProxyHelper().ExecuteAsync(async (httpClient) =>
                {
                    using (var dbx = new DropboxClient(token, new DropboxClientConfig
                    {
                        HttpClient = httpClient
                    }))
                    {
                        try
                        {
                            await dbx.Users.GetCurrentAccountAsync();
                            result = new DropboxAccessTokenInfo();
                        }
                        catch (Exception ex)
                        {
                            var x = ex.GetType();
                            if (ex is AuthException)
                            {
                                if (ex.Message.Contains("invalid_access_token"))
                                {
                                    result = new DropboxAccessTokenInfo("Невалидный токен");
                                    throw new AttemptCancelException();
                                }
                                
                                if (ex.Message.Contains("expired_access_token"))
                                {
                                    result = new DropboxAccessTokenInfo("Токен просрочен");
                                    throw new AttemptCancelException();
                                }
                            }

                            throw;
                        }
                    }
                });
            });

            return await Task.FromResult(result);
        }
    }
}