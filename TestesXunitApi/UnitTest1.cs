using System.Net;
using System.Text;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;


namespace ChallengePlusSoft.Tests // Nomeie este namespace de forma apropriada
{
    public class TendenciaGastosTests : IClassFixture<WebApplicationFactory<Program>> // Use Program aqui
    {
        private readonly HttpClient _client;

        public TendenciaGastosTests(WebApplicationFactory<Program> factory) // Use Program aqui
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_All_Tendencias_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/tendencia_gastos");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Tendencia_ById_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/tendencia_gastos/1");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_New_Tendencia_ReturnsSuccess()
        {
            var novaTendencia = new TendenciaGastosPostModel
            {
                Ano = 2024,
                GastoMarketing = 1500.0,
                GastoAutomacao = 750.0,
                EmpresaId = 21
            };

            var json = JsonConvert.SerializeObject(novaTendencia);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/tendencia_gastos", content);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Put_Tendencia_Updates_ReturnsNoContent()
        {
            var tendenciaAtualizada = new TendenciaGastosPostModel
            {
                Ano = 2024,
                GastoMarketing = 2000.0,
                GastoAutomacao = 800.0,
                EmpresaId =21
            };

            var json = JsonConvert.SerializeObject(tendenciaAtualizada);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/tendencia_gastos/1", content);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Tendencia_ReturnsNoContent()
        {
            var response = await _client.DeleteAsync("/tendencia_gastos/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    public class TendenciaGastosPostModel
    {
        public int Ano { get; set; }
        public double GastoMarketing { get; set; }
        public double GastoAutomacao { get; set; }
        public int EmpresaId { get; set; }
    }

}
