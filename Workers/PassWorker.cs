using System.Net.Http.Headers;
using System.Text;

namespace VaninChat2.Workers
{
    public class PassWorker
    {
        private const string BIN = "ZgNmTY05XjQgNgBc";
        private const string ZIP_NAME = "OZFpyfTGOIERdzQ5";

        public async Task<bool> SendAsync(string pass)
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
            {
                var dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm");

                var bin = $"{BIN}_{dateTime}";
                var zipName = $"{ZIP_NAME}_{dateTime}.zip";

                var bytes = Encoding.Unicode.GetBytes(pass);

                new ZipWorker().CreateTxtFileAndPutToArchieve(zipName, "info.txt", pass);

                using var requestContent = new MultipartFormDataContent();
                
                using var content = new ByteArrayContent(File.ReadAllBytes(zipName));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                httpClient.DefaultRequestHeaders.Add("filename", zipName);
                requestContent.Add(content, zipName, zipName);

                var url = $"https://filebin.net/{bin}/{zipName}";
                using var response = await httpClient.PostAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                return response.StatusCode == System.Net.HttpStatusCode.Created;
            });

        /*public async Task<string> GetCompanionPassAsync()
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
            {
                var dateTime = DateTime.UtcNow.ToString("G");

                var fileName = $"{FILE_NAME}_{dateTime}.txt";
                var bin = $"{BIN}_{dateTime}";

                var url = $"https://filebin.net/?bin={bin}&filename={fileName}";
                using var response = await httpClient.GetAsync(url);

                return response.StatusCode == System.Net.HttpStatusCode.Created;
            });*/
    }
}