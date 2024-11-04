using Microsoft.ML.Data;

namespace ChllengePlusSoft.Models
{
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
