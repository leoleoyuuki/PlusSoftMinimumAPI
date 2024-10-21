using System.Runtime.InteropServices;

namespace ChllengePlusSoft.Models
{
    public class DesempenhoFinanceiroModel
    {
        public long Id { get; set; }
        public double Receita { get; set; }
        public double Lucro { get; set; }
        public double Crescimento { get; set; }
        public EmpresasModel? Empresa { get; set; }

    }
}
