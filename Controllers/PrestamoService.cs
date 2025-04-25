using BiblioApp.Controllers;
using BiblioApp.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;

namespace BiblioApp.Controllers
{
    public class PrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PrestamoService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<PrestamosModel>> GetAllPrestamosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Prestamo");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<PrestamosModel>>(content);
        }

        public async Task<PrestamosModel> GetPrestamoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Prestamo/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PrestamosModel>(content);
        }

        public async Task<PrestamosModel> CreatePrestamoAsync(PrestamosModel prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Prestamo", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<PrestamosModel> UpdatePrestamoAsync(int id, PrestamosModel prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Prestamo/{id}", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<bool> DeletePrestamoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Prestamo/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
}
