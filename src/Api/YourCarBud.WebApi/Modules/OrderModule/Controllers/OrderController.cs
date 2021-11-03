using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Dtos;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderModule.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;


        public OrderController(
            IMapper mapper,
            IOrderService orderService)
        {
            _mapper = mapper;
            _orderService = orderService;
        }

        [HttpGet("")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrderViewDto>))]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<OrderViewDto>>> GetAllOrders()
        {
            var orderEntities = await _orderService.GetAllOrdersWithChildren().AsNoTracking().ToListAsync();

            var orderMapped = _mapper.Map<IEnumerable<OrderViewDto>>(orderEntities);

            return Ok(orderMapped);
        }

        [HttpGet("{orderId}", Name = "GetOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderViewDto))]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<OrderViewDto>> Get(Guid orderId)
        {
            var orderEntity = await _orderService.GetOrderWithChildrenQuery(orderId).AsNoTracking()
                .SingleOrDefaultAsync();
            if (orderEntity == null)
            {
                throw new NotFoundException("Order not found.");
            }

            var orderMapped = _mapper.Map<OrderViewDto>(orderEntity);

            return Ok(orderMapped);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Post(OrderCreateDto createDto, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(createDto);
            await _orderService.CreateOrder(orderEntity, cancellationToken);

            var orderMapped = _mapper.Map<OrderViewDto>(orderEntity);

            return CreatedAtRoute("GetOrder", new { orderId = orderEntity.Id }, orderMapped);
        }
    }
}