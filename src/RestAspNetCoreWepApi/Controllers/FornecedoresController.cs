using Api.Application.DTOs;
using Api.Business.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Api.Business.Models;

namespace Api.Application.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            IMapper mapper)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorDTO>> ObterTodos()
        {
            var fornecedores = await _fornecedorRepository.ObterTodos();

            var fornecedoresDTO = _mapper.Map<IEnumerable<FornecedorDTO>>(fornecedores);

            return fornecedoresDTO;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<FornecedorDTO>> ObterPorId(Guid id)
        {
            var fornecedorDTO = await ObterFornecedorProdutosEndereco(id);

            if (fornecedorDTO is null)
                return NotFound();

            return fornecedorDTO;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<FornecedorDTO>> Adicionar(FornecedorDTO fornecedorDTO)
        {
            if (!ModelState.IsValid) 
                return CustomResponse(ModelState);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDTO);
            await _fornecedorService.Adicionar(fornecedor);

            return CustomResponse(fornecedorDTO);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<FornecedorDTO>> Atualizar(Guid id, FornecedorDTO fornecedorDTO)
        {
            if (id != fornecedorDTO.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDTO);

            var resultado = await _fornecedorService.Atualizar(fornecedor);

            if (!resultado)
                return BadRequest();

            return Ok(fornecedor);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<FornecedorDTO>> Deletar(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor is null)
                return NotFound();

            var resultado = await _fornecedorService.Remover(id);

            if (!resultado)
                return BadRequest();

            return Ok();
        }

        private async Task<FornecedorDTO> ObterFornecedorProdutosEndereco(Guid id)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorProdutosEndereco(id);

            var fornecedorDTO = _mapper.Map<FornecedorDTO>(fornecedor);

            return fornecedorDTO;
        }

        private async Task<FornecedorDTO> ObterFornecedorEndereco(Guid id)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorEndereco(id);

            var fornecedorDTO = _mapper.Map<FornecedorDTO>(fornecedor);

            return fornecedorDTO;
        }
    }
}
