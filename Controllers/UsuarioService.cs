using BiblioApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace BiblioApp.Controllers
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public UsuarioService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<UsuariosModel>> GetAllUsuariosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Usuario");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<UsuariosModel>>(content);
        }

        public async Task<UsuariosModel> GetUsuarioByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UsuariosModel>(content);
        }

        public async Task<UsuariosModel> CreateUsuarioAsync(UsuariosModel usuario)
        {
            var content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Usuario", content);
            response.EnsureSuccessStatusCode();
            return usuario;
        }

        public async Task<UsuariosModel> UpdateUsuarioAsync(int id, UsuariosModel usuario)
        {
            var content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Usuario/{id}", content);
            response.EnsureSuccessStatusCode();
            return usuario;
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Usuario/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
}
