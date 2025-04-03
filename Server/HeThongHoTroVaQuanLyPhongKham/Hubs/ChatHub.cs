using HeThongHoTroVaQuanLyPhongKham.Exceptions;
using HeThongHoTroVaQuanLyPhongKham.Models;
using HeThongHoTroVaQuanLyPhongKham.Repositories;
using HeThongHoTroVaQuanLyPhongKham.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace HeThongHoTroVaQuanLyPhongKham.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<int, string> _userConnections = new ConcurrentDictionary<int, string>();
        private static readonly ConcurrentDictionary<int, List<ChatMessage>> _chatSessions = new ConcurrentDictionary<int, List<ChatMessage>>();
        private static readonly ConcurrentDictionary<int, bool> _activePatients = new ConcurrentDictionary<int, bool>();

        private readonly IUserService _userService;
        private readonly IRepository<TblBenhNhan> _benhNhanRepository;
        private readonly IRepository<TblNhanVien> _nhanVienRepository;

        public ChatHub(
            IUserService userService,
            IRepository<TblBenhNhan> benhNhanRepository,
            IRepository<TblNhanVien> nhanVienRepository)
        {
            _userService = userService;
            _benhNhanRepository = benhNhanRepository;
            _nhanVienRepository = nhanVienRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdentifier = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdentifier) || !int.TryParse(userIdentifier, out var maTaiKhoan))
            {
                await Clients.Caller.SendAsync("Error", "Không thể xác định mã tài khoản.");
                return;
            }

            _userConnections[maTaiKhoan] = Context.ConnectionId;

            try
            {
                var maBenhNhan = await _userService.GetMaBenhNhanFromTaiKhoan(maTaiKhoan);
                var benhNhan = await _benhNhanRepository.GetQueryable()
                    .FirstOrDefaultAsync(bn => bn.MaBenhNhan == maBenhNhan);
                if (benhNhan != null)
                {
                    _activePatients[maTaiKhoan] = true;
                    if (_chatSessions.TryGetValue(maTaiKhoan, out var messages))
                    {
                        await Clients.Caller.SendAsync("LoadChatHistory", messages);
                    }
                    await NotifyActivePatients();
                }
            }
            catch (NotFoundException)
            {
                try
                {
                    var maNhanVien = await _userService.GetMaNhanVienFromTaiKhoan(maTaiKhoan);
                    var nhanVien = await _nhanVienRepository.GetQueryable()
                        .FirstOrDefaultAsync(nv => nv.MaNhanVien == maNhanVien);
                    if (nhanVien != null)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, "Staff");
                        await Clients.Caller.SendAsync("UpdateActivePatients", _activePatients.Keys.ToList());
                        var chatPatientIds = _chatSessions.Keys.ToList();
                        await Clients.Caller.SendAsync("LoadChatPatients", chatPatientIds);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("Error", "Không tìm thấy nhân viên.");
                    }
                }
                catch (NotFoundException)
                {
                    await Clients.Caller.SendAsync("Error", "Không thể xác định vai trò người dùng.");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userIdentifier = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userIdentifier) && int.TryParse(userIdentifier, out var maTaiKhoan))
            {
                _userConnections.TryRemove(maTaiKhoan, out _);

                try
                {
                    var maBenhNhan = await _userService.GetMaBenhNhanFromTaiKhoan(maTaiKhoan);
                    var benhNhan = await _benhNhanRepository.GetQueryable()
                        .FirstOrDefaultAsync(bn => bn.MaBenhNhan == maBenhNhan);
                    if (benhNhan != null)
                    {
                        _activePatients.TryRemove(maTaiKhoan, out _);
                        await NotifyActivePatients();
                    }
                }
                catch (NotFoundException)
                {
                    throw;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToStaff(int maTaiKhoan, string message)
        {
            try
            {
                var maBenhNhan = await _userService.GetMaBenhNhanFromTaiKhoan(maTaiKhoan);
                var benhNhan = await _benhNhanRepository.GetQueryable()
                    .FirstOrDefaultAsync(bn => bn.MaBenhNhan == maBenhNhan);
                if (benhNhan == null) return;

                var chatMessage = new ChatMessage
                {
                    SenderId = maTaiKhoan,
                    SenderName = benhNhan.Ten ?? "Không xác định",
                    Message = message,
                    Timestamp = DateTime.Now,
                    IsStaff = false
                };

                var session = _chatSessions.GetOrAdd(maTaiKhoan, _ => new List<ChatMessage>());
                session.Add(chatMessage);

                Console.WriteLine($"Sending message to Staff: {message}"); // Debug
                await Clients.Group("Staff").SendAsync("ReceiveMessage", chatMessage);
                if (_userConnections.TryGetValue(maTaiKhoan, out var patientConnectionId))
                {
                    await Clients.Client(patientConnectionId).SendAsync("ReceiveMessage", chatMessage);
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
        }

        public async Task JoinChat(int maNhanVien, int maTaiKhoan)
        {
            try
            {
                var nhanVien = await _nhanVienRepository.GetQueryable()
                    .FirstOrDefaultAsync(nv => nv.MaNhanVien == maNhanVien);
                if (nhanVien == null) return;

                await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{maTaiKhoan}");

                var notification = $"Nhân viên {nhanVien.Ten ?? "Không xác định"} (Mã: {maNhanVien}) đã tham gia chat";
                if (_userConnections.TryGetValue(maTaiKhoan, out var patientConnectionId))
                {
                    await Clients.Client(patientConnectionId).SendAsync("StaffJoined", notification);
                }

                if (_chatSessions.TryGetValue(maTaiKhoan, out var messages))
                {
                    Console.WriteLine($"Loading chat history for maTaiKhoan {maTaiKhoan}: {messages.Count} messages"); // Debug
                    await Clients.Caller.SendAsync("LoadChatHistory", messages);
                }
            }
            catch (NotFoundException)
            {
                // Không tìm thấy nhân viên, không làm gì
            }
        }

        public async Task SendMessageToPatient(int maNhanVien, int maTaiKhoan, string message)
        {
            try
            {
                var nhanVien = await _nhanVienRepository.GetQueryable()
                    .FirstOrDefaultAsync(nv => nv.MaNhanVien == maNhanVien);
                if (nhanVien == null) return;

                var chatMessage = new ChatMessage
                {
                    SenderId = maNhanVien,
                    SenderName = nhanVien.Ten ?? "Không xác định",
                    Message = message,
                    Timestamp = DateTime.Now,
                    IsStaff = true
                };

                var session = _chatSessions.GetOrAdd(maTaiKhoan, _ => new List<ChatMessage>());
                session.Add(chatMessage);

                if (_userConnections.TryGetValue(maTaiKhoan, out var patientConnectionId))
                {
                    await Clients.Client(patientConnectionId).SendAsync("ReceiveMessage", chatMessage);
                }
                await Clients.Caller.SendAsync("ReceiveMessage", chatMessage);
            }
            catch (NotFoundException)
            {
                // Không tìm thấy nhân viên, không làm gì
            }
        }

        private async Task NotifyActivePatients()
        {
            var activePatientIds = _activePatients.Keys.ToList();
            await Clients.Group("Staff").SendAsync("UpdateActivePatients", activePatientIds);
        }
    }

    public class ChatMessage
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsStaff { get; set; }
    }
}