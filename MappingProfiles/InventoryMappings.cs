using AutoMapper;
using InventoryApi.Dtos;
using InventoryApi.Entities;

namespace InventoryApi.MappingProfiles
{
    public class InventoryMappings : Profile
    {
        public InventoryMappings()
        {
            CreateMap<InventoryEntity, InventoryDto>().ReverseMap();
            CreateMap<InventoryEntity, InventoryUpdateDto>().ReverseMap();
            CreateMap<InventoryEntity, InventoryCreateDto>().ReverseMap();
        }
    }
}
