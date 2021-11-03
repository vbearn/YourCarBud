using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourCarBud.WebApi.Modules.OrderStepModule.Dtos;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Controllers
{
    [Route("api/orders/{orderId}/steps/{stepName}/status")]
    [ApiController]
    public class OrderStepStatusController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderStepWorkflowBehaviourFactory _orderStepBehaviourFactory;
        private readonly IOrderStepService _orderStepService;


        public OrderStepStatusController(
            IMapper mapper,
            IOrderStepService orderStepService,
            IOrderStepWorkflowBehaviourFactory orderStepBehaviourFactory
        )
        {
            _mapper = mapper;
            _orderStepService = orderStepService;
            _orderStepBehaviourFactory = orderStepBehaviourFactory;
        }

        [HttpPut("")]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateStepStatus(Guid orderId, string stepName,
            OrderStepStatusUpdateDto updateDto, CancellationToken cancellationToken)
        {
            // resolving the correct instance of OrderStep's WorkflowBehaviour (ContactDetail/Payment/...) from DI resolver using the stepName as key
            var orderStepBehaviour = _orderStepBehaviourFactory.ResolveBehaviourViaStepName(stepName);

            var updateModel = _mapper.Map<OrderStepStatusUpdateModel>(updateDto,
                opts => opts.Items.Add(nameof(IOrderStepWorkflowBehaviour), orderStepBehaviour));

            await _orderStepService.UpdateStatus(orderStepBehaviour, orderId, updateModel, cancellationToken);

            return Ok();
        }
    }
}