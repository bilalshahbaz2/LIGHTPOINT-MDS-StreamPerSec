using AutoMapper;
using CommandProtocol.Transferable;
using GrpcService.MDS;
using NLog;

namespace gRPCBaseCollector.Mappers
{
    /**
    * 
    * 
    */
    public class OutgoingGrpcMessageMapperImpl : IOutgoingMessageGrpcMapper
    {
        private readonly IMapper mapper;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public OutgoingGrpcMessageMapperImpl(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CommandProtocol.Transferable.OutgoingMessage Map(GrpcService.MDS.OutgoingMessage protobuff)
        {
            return mapper.Map<CommandProtocol.Transferable.OutgoingMessage>(protobuff);
        }


        public GrpcService.MDS.OutgoingMessage Map(CommandProtocol.Transferable.OutgoingMessage source)
        {
            //GrpcService.MDS.OutgoingMessage grpcMessage = new GrpcService.MDS.OutgoingMessage();
            //grpcMessage.CorrelationId = source.CorrelationId;
            //grpcMessage.Version = source.Version;
            //grpcMessage.Requestor = source.Requestor;
            //grpcMessage.Timestamp = source.Timestamp;
            //grpcMessage.Datasource = source.Datasource;
            //grpcMessage.UserId = source.UserId;
            //grpcMessage.CorrelationId = source.CorrelationId;
            //grpcMessage.Requesttype = (RequestType)source.RequestType;
            //grpcMessage.ResponseBag = new ResponseBagMessage();
            
            //foreach ( var bagItem in source.ResponseBag.Items)
            //{
            //    grpcMessage.ResponseBag.Items.Add(this.MapBagItem(bagItem));
            //}
            //return grpcMessage;
			/////////////////////*Using AutoMapper*///////////////////////

            return mapper.Map<GrpcService.MDS.OutgoingMessage>(source);
        }

        private ResponseBagItemMessage MapBagItem( ResponseBagItem bagItem)
        {
            var grpcItem = new ResponseBagItemMessage();
            grpcItem.Sequencenumber = bagItem.SequenceNo;
            grpcItem.Security = this.MapSecurityDefiniation(bagItem.Security);
            foreach( var field in bagItem.FieldValues)
            {
                grpcItem.FieldValues.Add( field.Key ,this.MapFeildDescription(field.Value));
            }
            return grpcItem;
            /**
             * 
             *  public SecurityDefinition Security { get; set; }
        public String SequenceNo { get; set; }
        public Dictionary<string, FieldDescriptor> FieldValues { get; set; } = new Dictionary<string, FieldDescriptor>();
        public Dictionary<String, String> FieldErrors { get; set; } = new Dictionary<string, string>();
            */
        }

        private SecurityDefinitionMessage MapSecurityDefiniation(SecurityDefinition securityDefinition)
        {
            var grpcItem = new SecurityDefinitionMessage();
            grpcItem.SecurityIdentifier = securityDefinition.SecurityIdentifier;
            grpcItem.IdentifierType = securityDefinition.IdentifierType;
            grpcItem.Message = securityDefinition.Message;

            return grpcItem;
        }

        private FieldDescriptorMessage MapFeildDescription( FieldDescriptor fieldDescriptor)
        {
            var grpcItem = new FieldDescriptorMessage();
            grpcItem.HasError = fieldDescriptor.HasError;
            grpcItem.Message = fieldDescriptor.Message ?? "";
            grpcItem.Timestamp = fieldDescriptor.Timestamp;
            grpcItem.OriginatingSource = fieldDescriptor.OriginatingSource;
            grpcItem.CollectorCode = fieldDescriptor.CollectorCode;
            grpcItem.Key = fieldDescriptor.Key;
            grpcItem.Value = fieldDescriptor.Value;
            return grpcItem;
        }

 
    }
}
