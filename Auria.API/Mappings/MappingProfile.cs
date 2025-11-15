using AutoMapper;
using Auria.Data.Entities;
using Auria.Dto.Categorias;
using Auria.Dto.Noticias;
using Auria.Dto.GaleriaFotos;
using Auria.Dto.Projetos;

namespace Auria.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeamento de CategoriaNoticia
        CreateMap<CategoriaNoticia, CategoriaNoticiaDto>();
        CreateMap<CategoriaNoticiaCreateDto, CategoriaNoticia>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.Noticias, opt => opt.Ignore());

        CreateMap<CategoriaNoticiaUpdateDto, CategoriaNoticia>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.Noticias, opt => opt.Ignore());

        // Mapeamento de Noticia
        CreateMap<Noticia, NoticiaDto>();
        CreateMap<NoticiaCreateDto, Noticia>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore());

        CreateMap<NoticiaUpdateDto, Noticia>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore());

        // Mapeamento de GaleriaFotos
        CreateMap<GaleriaFotos, GaleriaFotosDto>();
        CreateMap<GaleriaFotosCreateDto, GaleriaFotos>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Fotos, opt => opt.Ignore());

        CreateMap<GaleriaFotosUpdateDto, GaleriaFotos>()
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Fotos, opt => opt.Ignore());

        // Mapeamento de Foto
        CreateMap<Foto, FotoDto>();
        CreateMap<FotoCreateDto, Foto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataUpload, opt => opt.Ignore())
            .ForMember(dest => dest.GaleriaFotos, opt => opt.Ignore());

        CreateMap<FotoUpdateDto, Foto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IdGaleriaFotos, opt => opt.Ignore())
            .ForMember(dest => dest.Url, opt => opt.Ignore())
            .ForMember(dest => dest.NomeArquivo, opt => opt.Ignore())
            .ForMember(dest => dest.Tamanho, opt => opt.Ignore())
            .ForMember(dest => dest.DataUpload, opt => opt.Ignore())
            .ForMember(dest => dest.GaleriaFotos, opt => opt.Ignore());

        // Mapeamento de Projeto
        CreateMap<Projeto, ProjetoDto>();
        CreateMap<ProjetoCreateDto, Projeto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.GaleriaFotos, opt => opt.Ignore());

        CreateMap<ProjetoUpdateDto, Projeto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Ativo, opt => opt.Ignore())
            .ForMember(dest => dest.GaleriaFotos, opt => opt.Ignore());
    }
}
