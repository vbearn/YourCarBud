using System;
using System.Linq;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services
{
    public class OrderStepBehaviourFactory : IOrderStepWorkflowBehaviourFactory
    {
        private readonly IOrderStepWorkflowBehaviour[] _orderStepBehaviours;


        public OrderStepBehaviourFactory(
            IOrderStepWorkflowBehaviour[] orderStepBehaviours)
        {
            _orderStepBehaviours = orderStepBehaviours;
        }

        public IOrderStepWorkflowBehaviour ResolveBehaviourViaStepName(string stepName)
        {
            var orderStepBehaviour = _orderStepBehaviours.SingleOrDefault(x => x.OrderStepName == stepName);

            if (orderStepBehaviour == null)
            {
                throw new ArgumentException($"Step Name {stepName} is not valid. No resolver has been registered for with this step name.");
            }

            IOrderStepWorkflowBehaviour iOrderStepBehaviour = orderStepBehaviour;

            return orderStepBehaviour;
        }

        public IOrderStepWorkflowBehaviour[] GetAllWorkflowBehaviours()
        {
            return _orderStepBehaviours;
        }
    }
}