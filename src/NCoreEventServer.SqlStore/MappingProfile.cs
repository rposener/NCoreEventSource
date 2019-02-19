using AutoMapper;
using NCoreEventServer.Models;
using NCoreEventServer.SqlStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.MapProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Subscription, SubscriptionEntity>()
                .ForMember(e => e.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.SubscriberId, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriber, opt => opt.Ignore());
            CreateMap<SubscriptionEntity, Subscription>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (SubscriptionTypes)Enum.Parse(typeof(SubscriptionTypes), src.Type)));

            CreateMap<Subscriber, SubscriberEntity>()
                .ForMember(dest => dest.BaseUri, opt => opt.MapFrom(src => src.BaseUri.AbsoluteUri))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()))
                .ForMember(dest => dest.SubscriberMessages, opt => opt.Ignore());
            CreateMap<SubscriberEntity, Subscriber>()
                .ForMember(dest => dest.BaseUri, opt => opt.MapFrom(src => new Uri(src.BaseUri)))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => (SubscriberStates)Enum.Parse(typeof(SubscriberStates), src.State)));

            CreateMap<ServerEventMessage, ServerEventMessageEntity>();
            CreateMap<ServerEventMessageEntity, ServerEventMessage>();

            CreateMap<PoisonEventMessageEntity, ServerEventMessageEntity>();
            CreateMap<ServerEventMessageEntity, PoisonEventMessageEntity>();

            CreateMap<SubscriberMessage, SubscriberMessageEntity>();
            CreateMap<SubscriberMessageEntity, SubscriberMessage>();
        }
    }
}
