using System.Runtime.InteropServices;

namespace ChllengePlusSoft.Models
{
    public class TendenciaGastosModel
    {
        public long Id { get; set; }
        public int Ano { get; set; }
        public double GastoMarketing { get; set; }
        public double GastoAutomacao { get; set; }
        public EmpresasModel? Empresa { get; set; }




    }
}
