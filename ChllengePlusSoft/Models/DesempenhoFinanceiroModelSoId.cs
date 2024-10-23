namespace ChllengePlusSoft.Models
{
    public class DesempenhoFinanceiroModelSoID
    {
        public long Id { get; set; }
        public double Receita { get; set; }
        public double Lucro { get; set; }
        public double Crescimento { get; set; }
        public long EmpresaId { get; set; }
    }
}
