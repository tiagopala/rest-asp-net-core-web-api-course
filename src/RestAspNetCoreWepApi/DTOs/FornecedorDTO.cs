﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Application.DTOs
{
    public class FornecedorDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [StringLength(14, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 11)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Documento { get; set; }

        public int TipoFornecedor { get; set; }

        public EnderecoDTO Endereco { get; set; }

        public bool Ativo { get; set; }

        public IEnumerable<ProdutoDTO> Produtos { get; set; }
    }
}
