using Microsoft.ML;
using Microsoft.ML.Data;

namespace ChllengePlusSoft.Models
{
    public class DesempenhoPrediction
    {
        [ColumnName("Score")]
        public float Crescimento { get; set; }
    }
    public class MLModel
    {
        private readonly MLContext _mlContext;

        public MLModel()
        {
            _mlContext = new MLContext();
        }

        public ITransformer TrainModel(IEnumerable<DesempenhoPostModel> data)
        {
            // Converte os dados para o formato que o ML.NET entende
            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // Define a transformação do pipeline
            var pipeline = _mlContext.Transforms.Concatenate("Features", "Receita", "Lucro")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Crescimento", maximumNumberOfIterations: 100));

            // Treina o modelo
            var model = pipeline.Fit(trainingData);

            return model;
        }

        public float Predict(ITransformer model, DesempenhoPostModel input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<DesempenhoPostModel, DesempenhoPrediction>(model);
            var result = predictionEngine.Predict(input);
            return result.Crescimento;
        }
    }

}
