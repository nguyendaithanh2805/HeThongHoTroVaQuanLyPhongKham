
using System.Text.Json;
using HeThongHoTroVaQuanLyPhongKham.Dtos.UpdateModels;
using HeThongHoTroVaQuanLyPhongKham.Hubs;
using HeThongHoTroVaQuanLyPhongKham.Services;
using Microsoft.AspNetCore.SignalR;

namespace HeThongHoTroVaQuanLyPhongKham.Workers
{
    public class LichHenConsumer : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<ChatHub> _chatHub;

        public LichHenConsumer(
            IRabbitMQService rabbitMQService,
            IServiceScopeFactory serviceScopeFactory,
            IHubContext<ChatHub> chatHub)
        {
            _rabbitMQService = rabbitMQService;
            _serviceScopeFactory = serviceScopeFactory;
            _chatHub = chatHub;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQService.ConsumeMessage(async (message) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // Create a new scope
                {
                    var lichHenService = scope.ServiceProvider.GetRequiredService<ILichHenService>();

                    var lichHenMessage = JsonSerializer.Deserialize<LichHenMessageDTO>(message);
                    try
                    {
                        if (lichHenMessage != null)
                        {
                            await lichHenService.ProcessLichHenMessage(lichHenMessage);
                            // Gửi thông báo qua SignalR
                            await _chatHub.Clients.All.SendAsync("ReceiveLichHenUpdate", new
                            {
                                MaBenhNhan = lichHenMessage.MaBenhNhan,
                                Status = "Success",
                                Message = "Lịch hẹn đã được tạo thành công."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Gửi thông báo lỗi qua SignalR
                        await _chatHub.Clients.All.SendAsync("ReceiveLichHenUpdate", new
                        {
                            MaBenhNhan = lichHenMessage?.MaBenhNhan,
                            Status = "Error",
                            Message = ex.Message
                        });
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}
