using AutoMapper;
using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.DTOs.ViewDetailsMunicipe;
using EdirSalesBancoDeDados.Domain;

namespace EdirSalesBancoDeDados.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Grupo, GrupoDto>().ReverseMap();
            CreateMap<Grupo, DetalheGrupo>().ReverseMap();
            CreateMap<Municipe, MunicipeDto>().ReverseMap();
            CreateMap<Municipe, DetalheMunicipe>().ReverseMap();
            CreateMap<Solicitacao, SolicitacaoDto>().ReverseMap();
            CreateMap<Oficio, OficioDto>().ReverseMap();
            CreateMap<Telefone, TelefoneDto>().ReverseMap();
            CreateMap<Agente, AgenteDto>().ReverseMap();

            CreateMap<SolicitacaoDto, Solicitacao>()
           .ForMember(dest => dest.Oficios, opt => opt.MapFrom(src => src.Oficios.Select(o => new Oficio { NumeroOficio = o })));

            // Mapeamento Solicitacao -> SolicitacaoDTO
            CreateMap<Solicitacao, SolicitacaoDto>()
                .ForMember(dest => dest.Oficios, opt => opt.MapFrom(src => src.Oficios.Select(o => o.NumeroOficio)));



            CreateMap<AgenteDto, Agente>()
                .ForMember(dest => dest.Contatos, opt => opt.Ignore()).ReverseMap();

            CreateMap<Agente, AgenteDto>()
                .ForMember(dest => dest.Contatos, opt => opt.MapFrom(src => src.Contatos.Select(c => c.Contato)));

            CreateMap<Municipe, MunicipeDto>()
                .ForMember(dest => dest.Grupos, opt => opt.MapFrom(src => src.Grupos.Select(g => g.Id).ToList()));

            CreateMap<MunicipeDto, Municipe>()
                .ForMember(dest => dest.Grupos, opt => opt.Ignore()); // Ignoramos os grupos para tratá-los manualmente
            CreateMap<Municipe, DetalheMunicipe>()
                .ForMember(dest => dest.Solicitacoes, opt => opt.Ignore());
        }
    }
}
