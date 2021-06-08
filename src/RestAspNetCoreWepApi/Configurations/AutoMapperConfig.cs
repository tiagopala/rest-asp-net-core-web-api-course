using Api.Application.DTOs;
using Api.Business.Models;
using AutoMapper;

namespace Api.Application.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorDTO>().ReverseMap();
            CreateMap<Produto, ProdutoDTO>().ReverseMap();
            CreateMap<Endereco, EnderecoDTO>().ReverseMap();

            CreateMap<Produto, ProdutoImagemDTO>().ReverseMap();
            CreateMap<Produto, ProdutoDTO>()
                .ForMember(destination => destination.NomeFornecedor, options => options.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}
