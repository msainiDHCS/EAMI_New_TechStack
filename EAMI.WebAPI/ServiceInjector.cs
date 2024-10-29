using AutoMapper;

namespace EAMI.WebAPI
{
    public class ServiceInjector
    {
        private readonly MapperConfigSetup.MapperConfig _mapper;
        public ServiceInjector()
        {
            _mapper = new MapperConfigSetup.MapperConfig();
        }
        public IMapper InjectService()
        {
            var mapper = _mapper.RegisterMaps().CreateMapper();
            return mapper;
        }
    }
}