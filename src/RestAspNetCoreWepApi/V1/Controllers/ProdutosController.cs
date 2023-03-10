using Api.Application.Controllers;
using Api.Application.DTOs;
using Api.Business.Interfaces;
using Api.Business.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Api.Application.V1.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(
            IMapper mapper,
            IProdutoService produtoService,
            IProdutoRepository produtoRepository,
            INotifier notifier, IUserService userService) : base(notifier, userService)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoDTO>> ObterTodos()
        {
            var produtos = await _produtoRepository.ObterProdutosFornecedores();
            var produtoDTOs = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return produtoDTOs;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProdutoDTO>> ObterPorId(Guid id)
        {
            var produtoDTO = await ObterProduto(id);

            if (produtoDTO is null)
                return NotFound();

            return produtoDTO;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Adicionar(ProdutoDTO produtoDTO)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var imagemNome = $"{Guid.NewGuid()}_{produtoDTO.Imagem}"; 
            if(!UploadArquivo(produtoDTO.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoDTO);
            }

            produtoDTO.Imagem = imagemNome;
            var produto = _mapper.Map<Produto>(produtoDTO);
            await _produtoService.Adicionar(produto);

            return CustomResponse(produtoDTO);
        }

        [HttpPost("adicionar-alternativo")]
        [SwaggerOperation(
            Summary = "Criar um novo produto",
            Description = "Deve ser utilizado quando a imagem é muito grande. Quando não é possível serializar via base64."
        )]
        public async Task<ActionResult<ProdutoDTO>> AdicionarAlternativo(ProdutoImagemDTO produtoDTO)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var imgPrefixo = $"{Guid.NewGuid()}_";
            if (!await UploadArquivoAlternativo(produtoDTO.ImagemUpload, imgPrefixo))
            {
                return CustomResponse(produtoDTO);
            }

            produtoDTO.Imagem = imgPrefixo + produtoDTO.ImagemUpload.FileName;
            var produto = _mapper.Map<Produto>(produtoDTO);
            await _produtoService.Adicionar(produto);

            return CustomResponse();
        }

        [HttpPost("imagem")]
        [Obsolete("Esse endpoint está depreciado.")]
        //[DisableRequestSizeLimit]  // Desabilitar qualquer limite de tamanho
        [RequestSizeLimit(40000000)] // Habilitando arquivos de até 40 mbps
        public ActionResult AdicionarImagem(IFormFile file)
        {
            // Implementar upload do arquivo via IFormFile
            return Ok(file);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Atualizar(Guid id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.Id)
            {
                NotificarErro("Os Id's informados devem ser iguais");
                return CustomResponse();
            }

            var produtoAtualizacao = await ObterProduto(id);
            produtoDTO.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            if (produtoDTO.ImagemUpload != null)
            {
                var imagemNome = $"{Guid.NewGuid()}_{produtoDTO.Imagem}";
                if (!UploadArquivo(produtoDTO.ImagemUpload, imagemNome))
                    return CustomResponse(ModelState);

                produtoAtualizacao.Imagem = imagemNome;
            }

            produtoAtualizacao.Nome = produtoDTO.Nome;
            produtoAtualizacao.Descricao = produtoDTO.Descricao;
            produtoAtualizacao.Valor = produtoDTO.Valor;
            produtoAtualizacao.Ativo = produtoDTO.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return CustomResponse(produtoDTO);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Excluir(Guid id)
        {
            var produtoDTO = ObterProduto(id).Result;

            if (produtoDTO is null)
                return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse(produtoDTO);
        }

        // UploadAlternativo deve ser utilizado quando as imagens são muito grandes, não sendo possível o envio via base64.
        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo is null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para esse produto");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            using var stream = new FileStream(filePath, FileMode.Create);
            await arquivo.CopyToAsync(stream);
            return true;
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para esse produto");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(arquivo);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);
            return true;
        }

        private async Task<ProdutoDTO> ObterProduto(Guid id)
        {
            var produto = await _produtoRepository.ObterProdutoFornecedor(id);
            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return produtoDTO;
        }
    }
}
