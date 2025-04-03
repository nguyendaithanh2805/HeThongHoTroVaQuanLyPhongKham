namespace HeThongHoTroVaQuanLyPhongKham.Services
{
    public interface IRabbitMQService
    {
        void SendMessage<T>(T message);
        void ConsumeMessage(Func<string, Task> onMessageReceived);
    }
}
