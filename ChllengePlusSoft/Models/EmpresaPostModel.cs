using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChllengePlusSoft.Models
{
    public class EmpresaPostModel
    {
        public string? Nome { get; set; }
        public string? Tamanho { get; set; }
        public string? Setor { get; set; }
        public string? LocalizacaoGeografica { get; set; }
        public int NumeroFuncionarios { get; set; }
        public string? TipoEmpresa { get; set; }
        public int Cliente { get; set; }

    }
}
