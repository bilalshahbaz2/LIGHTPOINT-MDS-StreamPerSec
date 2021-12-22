// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: MDS.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace GrpcService.MDS {
  public static partial class MdsGrpcService
  {
    static readonly string __ServiceName = "MDS.MdsGrpcService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrpcService.MDS.IncomingRequest> __Marshaller_MDS_IncomingRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrpcService.MDS.IncomingRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrpcService.MDS.OutgoingMessage> __Marshaller_MDS_OutgoingMessage = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrpcService.MDS.OutgoingMessage.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrpcService.MDS.AckOutgoingMessage> __Marshaller_MDS_AckOutgoingMessage = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrpcService.MDS.AckOutgoingMessage.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrpcService.MDS.UnSubscriptionRequest> __Marshaller_MDS_UnSubscriptionRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrpcService.MDS.UnSubscriptionRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Google.Protobuf.WellKnownTypes.Empty> __Marshaller_google_protobuf_Empty = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Google.Protobuf.WellKnownTypes.Empty.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.OutgoingMessage> __Method_GetReferenceData = new grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.OutgoingMessage>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetReferenceData",
        __Marshaller_MDS_IncomingRequest,
        __Marshaller_MDS_OutgoingMessage);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.AckOutgoingMessage> __Method_GetReferenceDataSubsciption = new grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.AckOutgoingMessage>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetReferenceDataSubsciption",
        __Marshaller_MDS_IncomingRequest,
        __Marshaller_MDS_AckOutgoingMessage);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.AckOutgoingMessage> __Method_Subscribe = new grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.AckOutgoingMessage>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Subscribe",
        __Marshaller_MDS_IncomingRequest,
        __Marshaller_MDS_AckOutgoingMessage);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrpcService.MDS.UnSubscriptionRequest, global::GrpcService.MDS.AckOutgoingMessage> __Method_UnSubscribe = new grpc::Method<global::GrpcService.MDS.UnSubscriptionRequest, global::GrpcService.MDS.AckOutgoingMessage>(
        grpc::MethodType.Unary,
        __ServiceName,
        "UnSubscribe",
        __Marshaller_MDS_UnSubscriptionRequest,
        __Marshaller_MDS_AckOutgoingMessage);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::GrpcService.MDS.OutgoingMessage> __Method_SubscriptionFeedHandler = new grpc::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::GrpcService.MDS.OutgoingMessage>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "SubscriptionFeedHandler",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_MDS_OutgoingMessage);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.AckOutgoingMessage> __Method_GetSearchData = new grpc::Method<global::GrpcService.MDS.IncomingRequest, global::GrpcService.MDS.AckOutgoingMessage>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetSearchData",
        __Marshaller_MDS_IncomingRequest,
        __Marshaller_MDS_AckOutgoingMessage);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::GrpcService.MDS.MDSReflection.Descriptor.Services[0]; }
    }

    /// <summary>Client for MdsGrpcService</summary>
    public partial class MdsGrpcServiceClient : grpc::ClientBase<MdsGrpcServiceClient>
    {
      /// <summary>Creates a new client for MdsGrpcService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public MdsGrpcServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for MdsGrpcService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public MdsGrpcServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected MdsGrpcServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected MdsGrpcServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.OutgoingMessage GetReferenceData(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetReferenceData(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.OutgoingMessage GetReferenceData(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetReferenceData, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.OutgoingMessage> GetReferenceDataAsync(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetReferenceDataAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.OutgoingMessage> GetReferenceDataAsync(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetReferenceData, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage GetReferenceDataSubsciption(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetReferenceDataSubsciption(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage GetReferenceDataSubsciption(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetReferenceDataSubsciption, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> GetReferenceDataSubsciptionAsync(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetReferenceDataSubsciptionAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> GetReferenceDataSubsciptionAsync(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetReferenceDataSubsciption, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage Subscribe(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Subscribe(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage Subscribe(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Subscribe, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> SubscribeAsync(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return SubscribeAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> SubscribeAsync(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Subscribe, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage UnSubscribe(global::GrpcService.MDS.UnSubscriptionRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return UnSubscribe(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage UnSubscribe(global::GrpcService.MDS.UnSubscriptionRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_UnSubscribe, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> UnSubscribeAsync(global::GrpcService.MDS.UnSubscriptionRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return UnSubscribeAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> UnSubscribeAsync(global::GrpcService.MDS.UnSubscriptionRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_UnSubscribe, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::GrpcService.MDS.OutgoingMessage> SubscriptionFeedHandler(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return SubscriptionFeedHandler(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::GrpcService.MDS.OutgoingMessage> SubscriptionFeedHandler(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_SubscriptionFeedHandler, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage GetSearchData(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetSearchData(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::GrpcService.MDS.AckOutgoingMessage GetSearchData(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetSearchData, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> GetSearchDataAsync(global::GrpcService.MDS.IncomingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetSearchDataAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::GrpcService.MDS.AckOutgoingMessage> GetSearchDataAsync(global::GrpcService.MDS.IncomingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetSearchData, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override MdsGrpcServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new MdsGrpcServiceClient(configuration);
      }
    }

  }
}
#endregion
