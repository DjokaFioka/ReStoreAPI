using AutoMapper;
using ReStoreAPI.DTOs;
using ReStoreAPI.Entities;

namespace ReStoreAPI.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
