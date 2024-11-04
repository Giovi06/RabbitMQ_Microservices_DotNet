using Microsoft.EntityFrameworkCore;

namespace OrderService.Model
{
    public enum OrderStatus
    {
        OrderedByCustomer = 1,
        BeingPacked = 2,
        ReadyForShipment = 3,
        WaitingForPickup = 4,
        InDelivery  = 5,
        Delivered = 6,
        Canceled = 7,
    }
}
