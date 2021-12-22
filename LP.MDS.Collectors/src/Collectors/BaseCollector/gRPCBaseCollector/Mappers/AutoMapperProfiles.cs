using AutoMapper;
using GrpcService.MDS;
using CommandProtocol.Transferable;

namespace gRPCBaseCollector.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //GRPC incoming request Model to Requestable model
            CreateMap<IncomingRequest, CommandProtocol.Requestable.IncomingRequest>().ReverseMap();
            CreateMap<RequestBag, CommandProtocol.Requestable.RequestBag>().ReverseMap();
            CreateMap<SecurityDefinitionRequest, CommandProtocol.Requestable.SecurityDefinition>().ReverseMap();

            //Transferable model to GRPC Outgoing Model
            CreateMap<CommandProtocol.Transferable.OutgoingMessage, GrpcService.MDS.OutgoingMessage>().ReverseMap();
            CreateMap<ResponseBag, ResponseBagMessage>().ReverseMap();
            CreateMap<ResponseBagItem, ResponseBagItemMessage>()
                .ForMember(dest => dest.Sequencenumber, opt => opt.MapFrom(src => src.SequenceNo))
                .ForMember(dest => dest.ReferenceDataErrors, opt => opt.MapFrom(src => src.FieldErrors)).ReverseMap();
            CreateMap<SecurityDefinition, SecurityDefinitionMessage>().ReverseMap();
            CreateMap<FieldDescriptor, FieldDescriptorMessage>().ReverseMap();
        }
    }
}
