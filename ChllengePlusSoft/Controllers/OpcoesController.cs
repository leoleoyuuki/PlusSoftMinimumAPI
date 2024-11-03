namespace ChllengePlusSoft.Controllers;
using Microsoft.AspNetCore.Mvc;


    [ApiController]
    [Route("[controller]")]
    public class OpcoesController : ControllerBase
    {
        [HttpGet("tipo_empresa")]
        public IActionResult GetTipoEmpresa()
        {
            var tipos = "SOCIEDADE_EMPRESARIA_LIMITADA, MICROEMPREENDEDOR_INDIVIDUAL, SOCIEDADE_LIMITADA_UNIPESSOAL, SOCIEDADE_ANONIMA, EMPRESARIO_INDIVIDUAL, SOCIEDADE_SIMPLES";
            return Ok(tipos);
        }

        [HttpGet("setor")]
        public IActionResult GetSetor()
        {
            var setores = "COMERCIAL, INDUSTRIAL, RURAL, SERVIÇOS, OUTROS";
            return Ok(setores);
        }

        [HttpGet("tamanho")]
        public IActionResult GetTamanho()
        {
            var tamanhos = "MICRO, PEQUENO, MEDIO, GRANDE, MULTINACIONAL";
            return Ok(tamanhos);
        }

        [HttpGet("uso_recursos_especificos")]
        public IActionResult GetUsoRecursosEspecificos()
        {
            var opcoes = "SIM ou NAO";
            return Ok(opcoes);
        }

        [HttpGet("frequencia_uso")]
        public IActionResult GetFrequenciaUso()
        {
            var opcoes = "De 0 a 10 (0-10)";
            return Ok(opcoes);
        }

        [HttpGet("contrato_assinado")]
        public IActionResult GetContratoAssinado()
        {
            var opcoes = "SIM ou NAO";
            return Ok(opcoes);
        }

        [HttpGet("ano")]
        public IActionResult GetAno()
        {
            var opcoes = "Entre 1900 e 2025 (1900-2025)";
            return Ok(opcoes);
        }
    }
