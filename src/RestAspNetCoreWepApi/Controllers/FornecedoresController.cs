using Api.Application.DTOs;
using Api.Business.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Api.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Extensions;

namespace Api.Application.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IMapper mapper,
            IFornecedorService fornecedorService,
            IEnderecoRepository enderecoRepository,
            IFornecedorRepository fornecedorRepository,
            INotifier notifier, IUserService userService) : base(notifier, userService)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<FornecedorDTO>> ObterTodos()
        {
            var fornecedores = await _fornecedorRepository.ObterTodos();

            var fornecedoresDTO = _mapper.Map<IEnumerable<FornecedorDTO>>(fornecedores);

            return fornecedoresDTO;
        }

        [HttpGet("{id:guid}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FornecedorDTO>> ObterPorId(Guid id)
        {
            var fornecedorDTO = await ObterFornecedorProdutosEndereco(id);

            if (fornecedorDTO is null)
                return NotFound();

            return fornecedorDTO;
        }

        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ClaimsAuthorize("Fornecedor", "Adicionar")]
        public async Task<ActionResult<FornecedorDTO>> Adicionar(FornecedorDTO fornecedorDTO)
        {
            if (UsuarioAutenticado)
            {
                // Exemplo de como capturar informações de usuário de forma simples, se necessário.
                var userId = UsuarioId;
                var userName = UsuarioNome;
                var userEmail = UsuarioEmail;
                var userClaims = UsuarioClaims;
            }

            if (!ModelState.IsValid) 
                return CustomResponse(ModelState);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDTO);
            await _fornecedorService.Adicionar(fornecedor);

            return CustomResponse(fornecedorDTO);
        }

        [HttpPut("{id:guid}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        public async Task<ActionResult<FornecedorDTO>> Atualizar(Guid id, FornecedorDTO fornecedorDTO)
        {
            if (id != fornecedorDTO.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado no body da requisição");
                return CustomResponse(fornecedorDTO);
            }

            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDTO);
            await _fornecedorService.Atualizar(fornecedor);

            return CustomResponse(fornecedorDTO);
        }

        [HttpDelete("{id:guid}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        public async Task<ActionResult<FornecedorDTO>> Excluir(Guid id)
        {
            var fornecedorDTO = await ObterFornecedorEndereco(id);

            if (fornecedorDTO is null)
                return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse();
        }

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoDTO>> ObterEnderecoPorId(Guid id)
        {
            var endereco = await _enderecoRepository.ObterPorId(id);
            var enderecoDTO = _mapper.Map<EnderecoDTO>(endereco);
            return enderecoDTO;
        } 

        [HttpPut("atualizar-endereco/{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoDTO enderecoDTO)
        {
            if (id != enderecoDTO.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado no body da requisição");
                return CustomResponse(enderecoDTO);
            }

            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var endereco = _mapper.Map<Endereco>(enderecoDTO);
            await _fornecedorService.AtualizarEndereco(endereco);

            return CustomResponse(enderecoDTO);
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
