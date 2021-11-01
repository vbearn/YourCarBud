using System;
using System.Text.Json;
using AutoMapper;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Dtos
{
    public class OrderStepMapping : Profile
    {
        public OrderStepMapping()
        {
            CreateMap<OrderStepStatusUpdateDto, OrderStepStatusUpdateModel>()
                .ForMember(des => des.DetailCreationArgs, opt => opt.MapFrom<OrderStepDataResolver>())
                .ForMember(des => des.Status, opt => opt.MapFrom(src => Enum.Parse<Statuses>(src.Status)));

            CreateMap<DeliveryAppointmentCreationArgsDto, DeliveryAppointment>();
            CreateMap<ContactCreationArgsDto, ContactDetail>();
            CreateMap<PaymentCreationArgsDto, Payment>();
        }

        private class OrderStepDataResolver : IValueResolver<OrderStepStatusUpdateDto, OrderStepStatusUpdateModel,
            IOrderDetailCreationArgs>
        {
            public IOrderDetailCreationArgs Resolve(OrderStepStatusUpdateDto src, OrderStepStatusUpdateModel des,
                IOrderDetailCreationArgs destMember, ResolutionContext context)
            {
                // only have IOrderDetailCreationArgs if status is success 
                if (src.Status.ToLower() != "success")
                {
                    return null;
                }

                // empty DetailCreationArgs
                if (src.DetailCreationArgs.ValueKind == JsonValueKind.Undefined)
                {
                    return null;
                }

                var orderStepBehaviour =
                    context.Items[nameof(IOrderStepWorkflowBehaviour)] as IOrderStepWorkflowBehaviour;

                if (!orderStepBehaviour.DetailCreationArgsDtoType.IsAssignableTo(typeof(IOrderDetailCreationArgs)))
                {
                    throw new Exception("IOrderStepWorkflowBehaviour has incompatible IOrderDetailCreationArgs.");
                }

                return MapJsonElementToCreationArgs(src.DetailCreationArgs, orderStepBehaviour);
            }

            private static IOrderDetailCreationArgs MapJsonElementToCreationArgs(JsonElement jsonElement,
                IOrderStepWorkflowBehaviour orderStepBehaviour)
            {
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var json = jsonElement.GetRawText();

                var mappedData = JsonSerializer.Deserialize(json, orderStepBehaviour.DetailCreationArgsDtoType,
                    serializerOptions);

                return mappedData as IOrderDetailCreationArgs;
            }
        }
    }
}