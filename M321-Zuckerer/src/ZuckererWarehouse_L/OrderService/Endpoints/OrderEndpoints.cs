using Microsoft.EntityFrameworkCore;
using MqLibrary;
using OrderService.Model;

namespace OrderService.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Order");

        }

    }
}
