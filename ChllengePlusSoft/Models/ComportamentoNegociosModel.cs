using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ChllengePlusSoft.Models
{
    public class ComportamentoNegociosModel
    {
        public long Id { get; set; }
        public long InteracoesPlataforma { get; set; }
        public long FrequenciaUso { get; set; }
        public string Feedback { get; set; }
        public string? UsoRecursosEspecificos { get; set; }
        public EmpresasModel? Empresa { get; set; }
    }
}
