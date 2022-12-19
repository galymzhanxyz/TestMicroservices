namespace RabbitMQ.EventBus.Core.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public Response()
        {
            Message = "";
            ErrorCode = "";
        }
    }
}
