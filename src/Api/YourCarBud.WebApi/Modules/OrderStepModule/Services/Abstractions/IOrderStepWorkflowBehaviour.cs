using System;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions
{
    public interface IOrderStepWorkflowBehaviour : IWorkflowBehaviour
    {
        public Type DetailCreationArgsDtoType { get; }
        public string OrderStepName { get; }
    }
}