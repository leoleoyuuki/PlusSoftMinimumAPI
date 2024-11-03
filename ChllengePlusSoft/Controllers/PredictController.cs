using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;

namespace ChllengePlusSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly string _connectionString;

        public PredictController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        [HttpPost]
        public async Task<IActionResult> Predict([FromBody] DesempenhoPostModel input, MLModel mlModel)
        {
            // Treine o modelo uma vez (ou carregue um modelo pré-treinado)
            var data = new List<DesempenhoPostModel>
            {
                new DesempenhoPostModel { Receita = 10000, Lucro = 2000, Crescimento = 5, EmpresaId = 1 },
                new DesempenhoPostModel { Receita = 15000, Lucro = 3000, Crescimento = 7, EmpresaId = 2 },
                // Adicione mais dados conforme necessário
            };

            var model = mlModel.TrainModel(data); // Treina o modelo - isso deve ser feito uma vez, não a cada requisição

            // Faz a previsão
            var predictedCrescimento = mlModel.Predict(model, input);

            return Ok(new { PredictedCrescimento = predictedCrescimento });
        }
    }
}