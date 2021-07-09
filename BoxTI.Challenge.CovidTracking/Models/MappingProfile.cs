                                                                                                                                                                                                  using AutoMapper;
using System;
using BoxTI.Challenge.CovidTracking.API.Models;
using BoxTI.Challenge.CovidTracking.API.Models.Dto;

namespace BoxTI.Challenge.CovidTracking.Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CountryDto, Country>()
                    .ForMember(dest => dest.TotalCases,
                                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.TotalCases)
                                    ? 0 : decimal.Parse(src.TotalCases)));

            CreateMap<CountryDto, Country>()
            .ForMember(dest => dest.ActiveCases,
                     opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.ActiveCases)
                         ? 0 : decimal.Parse(src.ActiveCases)));


            CreateMapMod<Country, CountryDto>();
            CreateMapMod<CountryDto, Country>();


        }

        Tuple<IMappingExpression<T, U>, IMappingExpression<U, T>>
        CreateMapMod<U, T>()
        {
            return new Tuple<IMappingExpression<T, U>, IMappingExpression<U, T>>
                (CreateMap<T, U>(), CreateMap<U, T>());
        }


    }
}

