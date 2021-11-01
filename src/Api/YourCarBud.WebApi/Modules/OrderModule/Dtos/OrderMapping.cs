using System;
using AutoMapper;
using YourCarBud.WebApi.Modules.OrderModule.Entities;

namespace YourCarBud.WebApi.Modules.OrderModule.Dtos
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, OrderViewDto>()
                .ForMember(des => des.Status, opt => opt.MapFrom(src => Enum.GetName(src.Status)))
                .ForMember(des => des.Steps, opt => opt.MapFrom(src => src.OrderSteps));

            CreateMap<OrderStep, OrderStepViewDto>()
                .ForMember(des => des.Status, opt => opt.MapFrom(src => Enum.GetName(src.Status)));


            CreateMap<OrderCreateDto, Order>()
                .ForMember(des => des.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));
        }
    }
}