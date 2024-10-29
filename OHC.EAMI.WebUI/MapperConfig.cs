﻿using AutoMapper;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.CommonEntity.DataEntity;
using OHC.EAMI.WebUI.Models;

namespace OHC.EAMI.WebUI
{
    public class MapperConfig
    {
        public MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Fund, EAMIFundModel>().ReverseMap();
                cfg.CreateMap<ExclusivePmtType, EAMIExclusivePmtTypeModel>().ReverseMap();
                cfg.CreateMap<SCOPropertyEnumsLookup, SCOPropertyEnumsLookUpModel>().ReverseMap();
                cfg.CreateMap<SCOPropertyTypeLookUp, SCOPropertyTypeLookUpModel>().ReverseMap();
                cfg.CreateMap<SCOProperty, SCOPropertyModel>().ReverseMap();
                cfg.CreateMap<EAMIMasterData, EAMIMasterDataModel>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}