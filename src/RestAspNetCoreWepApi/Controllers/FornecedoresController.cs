using Api.Application.DTOs;
using Api.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Application.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedoresController(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorDTO>>> ObterTodos()
        {
            var fornecedores = await _fornecedorRepository.ObterTodos();

            return Ok(fornecedores);
        }
    }
}
