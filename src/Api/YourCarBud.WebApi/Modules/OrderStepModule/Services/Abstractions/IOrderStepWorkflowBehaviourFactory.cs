namespace YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions
{
    public interface IOrderStepWorkflowBehaviourFactory
    {
        IOrderStepWorkflowBehaviour ResolveBehaviourViaStepName(string stepName);
        IOrderStepWorkflowBehaviour[] GetAllWorkflowBehaviours();
    }
}