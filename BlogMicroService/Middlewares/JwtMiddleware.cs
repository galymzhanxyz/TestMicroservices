using RabbitMQ.EventBus.Core.Models;
using RabbitMQ.EventBus.Producer;

namespace BlogMicroService.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RpcClient _rpcClient;

        public JwtMiddleware(RequestDelegate next, RpcClient rpcClient)
        {
            _next = next;
            _rpcClient = rpcClient;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var res = _rpcClient.Call(new TransportMessageContract<string>()
            {
                MicroServiceName = "AuthMicroService",
                ServiceName = "ValidateToken",
                Message = token
            });

            if (string.IsNullOrEmpty(res.Message))
            {
                throw new Exception("Non Authorized!");
            }

            await _next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
