using AutoMapper;
using System.Linq;

namespace MahantInv.Infrastructure.Dtos.Product
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<destination, source>();
            // CreateMap<Entities.Product, ProductCreateDto>();
            //CreateMap<ProductCreateDto, Entities.Product>();
            CreateMap<Entities.Product, ProductSearchDto>()
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description ?? string.Empty))
                .ForMember(d => d.Company, m => m.MapFrom(s => s.Company ?? string.Empty))
                .ForMember(d => d.UnitTypeCode, m => m.MapFrom(s => s.UnitTypeCode ?? string.Empty))
                .ForMember(d => d.OrderBulkName, m => m.MapFrom(s => s.OrderBulkName ?? string.Empty))
                .ForMember(d => d.Storage, m => m.MapFrom(s => s.ProductStorages.Any()? string.Join(", ", s.ProductStorages.Select(ps => ps.Storage.Name)):string.Empty));
        }
    }
}
