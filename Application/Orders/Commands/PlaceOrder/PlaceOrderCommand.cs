using Application.Cart;
using MediatR;

namespace Application.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(
    string UserId,
    string ShippingAddress,
    string ShippingCity,
    string ShippingZip,
    List<CartItem> Items
) : IRequest<int>;
