
namespace ChllengePlusSoft.Models
{
    public class EmpresasModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Tamanho { get; set; }
        public string? Setor { get; set; }
        public string? LocalizacaoGeografica { get; set; }
        public int NumeroFuncionarios { get; set; }
        public string? TipoEmpresa { get; set; }
        public bool Cliente { get; set; }
        public List<TendenciaGastosModel>? Tendencias { get; set; }
        public List<DesempenhoFinanceiroModelSoID>? Desempenhos { get; set; }
        public List<HistoricoInteracoes>? Historicos { get; set; }
        public List<ComportamentoNegociosModel>? Comportamentos { get; set; }

        public static implicit operator EmpresasModel(EmpresaModelComId v)
        {
            throw new NotImplementedException();
        }
    }
}
