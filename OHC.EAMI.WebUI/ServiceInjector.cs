using AutoMapper;

namespace OHC.EAMI.WebUI
{
    public class ServiceInjector
    {
        private readonly MapperConfig _mapper;
        public ServiceInjector()
        {
            _mapper = new MapperConfig();
        }
        public IMapper InjectService()
        {
            var mapper = _mapper.RegisterMaps().CreateMapper();
            return mapper;
        }
    }
}