using System.Diagnostics.CodeAnalysis;

namespace ChllengePlusSoft.Models
{
    public class HistoricoInteracoes
    {
        public int Id { get; set; }
        public string? PropostaNegocio { get; set; }
        public string? ContratoAssinado { get; set; }
        public string? FeedbackServicosProdutos { get; set; }
        public EmpresasModel? Empresa { get; set; }
    }
}
