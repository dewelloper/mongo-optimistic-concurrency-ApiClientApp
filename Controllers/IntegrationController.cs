using ApiClientApp.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ApiClientApp.Controllers
{

    //[ApiController]
    //[Route("[controller]")]
    public class IntegrationController : ControllerBase
    {
        private readonly HttpClient _httpClientFactory;

        public IntegrationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory.CreateClient("StockApi");
        }

        //[HttpGet("")]
        public async Task<IActionResult> GetStock()
        {
            IEnumerable<StockModel> result = null;
            var bearerToken = await CheckToken();

            try
            {
                if (bearerToken != null)
                {
                    //Token'ın başında / işareti vardı server tarafı ona göre Identity destekleyecek şekilde değiştirildi
                    // API projesini buna göre güncelleyelim (Repodan concurrency projesinde ilgili kısma not yazdım)
                    //bearer 'den sonra boşluk koymak gerekiyor aksi taktirde 401 verecektir
                    _httpClientFactory.DefaultRequestHeaders.Add("Authorization", "bearer "+bearerToken);
                }

                var getResult = await _httpClientFactory.GetStringAsync("/api/stock");
                result = Deserialize<IEnumerable<StockModel>>(getResult);

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> CheckToken()
        {
            try
            {
                HttpResponseMessage response = null;
                var ui = new UserInfo()
                {
                    Password = "Deneme123",
                    UserName = "testUser"
                };
                var json = JsonSerializer.Serialize<UserInfo>(ui);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _httpClientFactory.PostAsync("/api/token/new", content);
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
