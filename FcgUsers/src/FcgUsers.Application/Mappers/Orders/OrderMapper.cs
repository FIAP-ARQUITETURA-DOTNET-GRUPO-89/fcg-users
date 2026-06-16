using FcgUsers.Application.Commands.Orders;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace FcgUsers.Application.Mappers;

[Mapper]
public static partial class OrderMapper
{
    public static Order ToEntity(this CreateOrderCommand command)
        => new(command.Customer, command.TotalAmount, command.DeliveryAddress);

    public static CreateOrderResponse ToCreateOrderResponse(this Order order)
        => new(order.Id);

    [MapProperty("DeliveryAddress.Street", nameof(GetOrderByIdResponse.Street))]
    [MapProperty("DeliveryAddress.City", nameof(GetOrderByIdResponse.City))]
    [MapProperty("DeliveryAddress.State", nameof(GetOrderByIdResponse.State))]
    [MapProperty("DeliveryAddress.Cep", nameof(GetOrderByIdResponse.Cep))]
    public static partial GetOrderByIdResponse ToGetOrderByIdResponse(this Order order);

    [MapProperty("DeliveryAddress.Street", nameof(UpdateDeliveryAddressResponse.Street))]
    [MapProperty("DeliveryAddress.City", nameof(UpdateDeliveryAddressResponse.City))]
    [MapProperty("DeliveryAddress.State", nameof(UpdateDeliveryAddressResponse.State))]
    [MapProperty("DeliveryAddress.Cep", nameof(UpdateDeliveryAddressResponse.Cep))]
    public static partial UpdateDeliveryAddressResponse ToUpdateDeliveryAddressResponse(this Order order);

    [MapProperty("DeliveryAddress.Street", nameof(ForceOrderStatusResponse.Street))]
    [MapProperty("DeliveryAddress.City", nameof(ForceOrderStatusResponse.City))]
    [MapProperty("DeliveryAddress.State", nameof(ForceOrderStatusResponse.State))]
    [MapProperty("DeliveryAddress.Cep", nameof(ForceOrderStatusResponse.Cep))]
    public static partial ForceOrderStatusResponse ToForceOrderStatusResponse(this Order order);

    [MapperIgnoreSource(nameof(Order.UpdatedAt))]
    [MapperIgnoreSource(nameof(Order.DeliveryAddress))]
    public static partial GetOrdersResponse ToGetOrdersResponse(this Order order);

    public static partial IEnumerable<GetOrdersResponse> ToGetOrdersResponseList(this IEnumerable<Order> orders);
}
