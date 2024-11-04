using AuthService.Endpoints;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http;

namespace AuthService.Helpers
{
    public class RemoteAuthApi(IOptions<AppSettings> options, HttpClient client)
    {
        private readonly AppSettings _options = options.Value;
        private readonly HttpClient _httpClient = client;

        public async Task<bool> Authenticate(string username, string password)
        {
            //_httpClient.BaseAddress = new Uri(_options.ApiUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await _httpClient.PostAsJsonAsync(_options.ApiUrl + "checkpassword", new RemoteAuthApiRequestDTO(username, password));
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                var resultObj = await response.Content.ReadFromJsonAsync<RemoteAuthApiResponseDTO>();
                if (resultObj == null)
                {
                    return false;
                }
                if (!resultObj.Success)
                {
                    return false;
                }
                return true;
            }
            catch (HttpRequestException)
            {
                // unable to contact remote API
                return false;
            }
        }

    }

    public record RemoteAuthApiRequestDTO(string Username, string Password);

    public record RemoteAuthApiResponseDTO(bool Success, string Message);
}
