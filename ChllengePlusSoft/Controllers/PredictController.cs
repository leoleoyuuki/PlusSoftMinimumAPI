using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using ChllengePlusSoft.Models;
using System.Collections.Generic;

namespace ChllengePlusSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public PredictController()
        {
            _mlContext = new MLContext();
            TrainModel(); // Treina o modelo ao iniciar o controlador
        }

        private void TrainModel()
        {
            // Dados de exemplo para treinamento
            var data = new List<InputData>
            {
                new InputData { GastoMarketing = 5000, GastoAutomacao = 3000, Receita = 10000 },
                new InputData { GastoMarketing = 7000, GastoAutomacao = 4000, Receita = 15000 },
                new InputData { GastoMarketing = 8000, GastoAutomacao = 5000, Receita = 20000 },
                new InputData { GastoMarketing = 6000, GastoAutomacao = 2000, Receita = 12000 },
                new InputData { GastoMarketing = 9000, GastoAutomacao = 6000, Receita = 25000 },
                new InputData { GastoMarketing = 10000, GastoAutomacao = 7000, Receita = 30000 },
                new InputData { GastoMarketing = 11000, GastoAutomacao = 8000, Receita = 35000 },
                new InputData { GastoMarketing = 12000, GastoAutomacao = 9000, Receita = 40000 },
                new InputData { GastoMarketing = 13000, GastoAutomacao = 10000, Receita = 45000 },
                new InputData { GastoMarketing = 14000, GastoAutomacao = 11000, Receita = 50000 }
            };

            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(InputData.GastoMarketing), nameof(InputData.GastoAutomacao))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(InputData.Receita), maximumNumberOfIterations: 100));

            _model = pipeline.Fit(trainingData);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Prediction), 200)]
        [ProducesResponseType(400)]
        public IActionResult Predict([FromBody] TendenciaGastosPostModel input)
        {
            // Criar um objeto de entrada para previsão
            var inputData = new InputData
            {
                GastoMarketing = (float)input.GastoMarketing,
                GastoAutomacao = (float)input.GastoAutomacao
            };

            // Criar um motor de previsão
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<InputData, Prediction>(_model);

            // Fazer a previsão
            var prediction = predictionEngine.Predict(inputData);

            return Ok(new { ReceitaPrevista = prediction.PredictedReceita });
        }
    }

    // Definição das classes InputData e Prediction
    public class InputData
    {
        public float GastoMarketing { get; set; }
        public float GastoAutomacao { get; set; }
        public float Receita { get; set; }
    }

    public class Prediction
    {
        [ColumnName("Score")]
        public float PredictedReceita { get; set; }
    }
}