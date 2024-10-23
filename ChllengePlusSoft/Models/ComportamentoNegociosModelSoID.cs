namespace ChllengePlusSoft.Models
{
    public class ComportamentoNegociosModelSoID
    {
        public long Id { get; set; }
        public long InteracoesPlataforma { get; set; }
        public long FrequenciaUso { get; set; }
        public string Feedback { get; set; }
        public string? UsoRecursosEspecificos { get; set; }
        public long EmpresaId { get; set; }
    }
}
