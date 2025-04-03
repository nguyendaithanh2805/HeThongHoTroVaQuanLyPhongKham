using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeThongHoTroVaQuanLyPhongKham.Dtos.HeThongHoTroVaQuanLyPhongKham.DTOs;
using HeThongHoTroVaQuanLyPhongKham.Dtos;
using HeThongHoTroVaQuanLyPhongKham.Mappers;
using HeThongHoTroVaQuanLyPhongKham.Models;
using HeThongHoTroVaQuanLyPhongKham.Repositories;
using HeThongHoTroVaQuanLyPhongKham.Services;
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using HeThongHoTroVaQuanLyPhongKham.Dtos.UpdateModels;

namespace LichHenServiceTests
{
    public class LichHenServiceTests
    {
        private readonly Mock<IRepository<TblLichHen>> _lichHenRepositoryMock;
        private readonly Mock<IMapper<LichHenDTO, TblLichHen>> _lichHenMappingMock;
        private readonly Mock<IRepository<TblBenhNhan>> _benhNhanRepositoryMock;
        private readonly Mock<IService<NhanVienDTO>> _nhanVienServiceMock;
        private readonly Mock<IRepository<TblDichVuYTe>> _dichVuYTeRepositoryMock;
        private readonly Mock<IService<PhongKhamDTO>> _phongKhamServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IRepository<TblHoaDon>> _hoaDonRepositoryMock;
        private readonly Mock<IRepository<TblDonThuoc>> _donThuocRepositoryMock;
        private readonly Mock<IHoaDonService> _hoaDonServiceMock;
        private readonly Mock<IRepository<TblHoSoYTe>> _hoSoYTeRepositoryMock;
        private readonly Mock<IMapper<HoaDonDTO, TblHoaDon>> _hoaDonMappingMock;
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly LichHenService _lichHenService;
        private readonly List<TblLichHen> _lichHenList;

        public LichHenServiceTests()
        {
            _lichHenRepositoryMock = new Mock<IRepository<TblLichHen>>();
            _lichHenMappingMock = new Mock<IMapper<LichHenDTO, TblLichHen>>();
            _benhNhanRepositoryMock = new Mock<IRepository<TblBenhNhan>>();
            _nhanVienServiceMock = new Mock<IService<NhanVienDTO>>();
            _dichVuYTeRepositoryMock = new Mock<IRepository<TblDichVuYTe>>();
            _phongKhamServiceMock = new Mock<IService<PhongKhamDTO>>();
            _jwtServiceMock = new Mock<IJwtService>();
            _hoaDonRepositoryMock = new Mock<IRepository<TblHoaDon>>();
            _donThuocRepositoryMock = new Mock<IRepository<TblDonThuoc>>();
            _hoaDonServiceMock = new Mock<IHoaDonService>();
            _hoSoYTeRepositoryMock = new Mock<IRepository<TblHoSoYTe>>();
            _hoaDonMappingMock = new Mock<IMapper<HoaDonDTO, TblHoaDon>>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _lichHenList = new List<TblLichHen>();

            _lichHenService = new LichHenService(
                _lichHenRepositoryMock.Object,
                _lichHenMappingMock.Object,
                _benhNhanRepositoryMock.Object,
                _nhanVienServiceMock.Object,
                _dichVuYTeRepositoryMock.Object,
                _phongKhamServiceMock.Object,
                _jwtServiceMock.Object,
                _hoaDonRepositoryMock.Object,
                _donThuocRepositoryMock.Object,
                _hoaDonServiceMock.Object,
                _hoSoYTeRepositoryMock.Object,
                _hoaDonMappingMock.Object,
                _rabbitMQServiceMock.Object);

            // Thiết lập encoding UTF-8 để hiển thị tiếng Việt đúng
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Mock GetQueryable để trả về danh sách lichHenList
            var lichHenQueryable = _lichHenList.AsQueryable();
            var lichHenDbSetMock = new Mock<DbSet<TblLichHen>>();
            lichHenDbSetMock.As<IQueryable<TblLichHen>>().Setup(m => m.Provider).Returns(lichHenQueryable.Provider);
            lichHenDbSetMock.As<IQueryable<TblLichHen>>().Setup(m => m.Expression).Returns(lichHenQueryable.Expression);
            lichHenDbSetMock.As<IQueryable<TblLichHen>>().Setup(m => m.ElementType).Returns(lichHenQueryable.ElementType);
            lichHenDbSetMock.As<IQueryable<TblLichHen>>().Setup(m => m.GetEnumerator()).Returns(() => lichHenQueryable.GetEnumerator());
            _lichHenRepositoryMock.Setup(repo => repo.GetQueryable()).Returns(lichHenDbSetMock.Object);

            // Mock CreateAsync với lock để đảm bảo đồng bộ
            _lichHenRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<TblLichHen>()))
                .Callback<TblLichHen>(lh =>
                {
                    lock (_lichHenList)
                    {
                        lh.MaLichHen = _lichHenList.Count + 1; // Gán MaLichHen tăng dần
                        _lichHenList.Add(lh);
                    }
                })
                .ReturnsAsync((TblLichHen lh) => lh);
        }

        [Fact]
        public async Task ProcessLichHenMessage_ConcurrentRequests_SameDateAndService_DifferentPatients_LimitFive()
        {
            // Arrange
            const int MAX_APPOINTMENTS_PER_SLOT = 5;
            var ngayHen = DateTime.Now.AddDays(1);
            var messages = Enumerable.Range(1, 10).Select(i => new LichHenMessageDTO
            {
                MaDichVuYTe = 1,
                NgayHen = ngayHen,
                MaBenhNhan = i
            }).ToList();

            _lichHenRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<TblLichHen>()))
                .Callback<TblLichHen>(lh =>
                {
                    lock (_lichHenList)
                    {
                        var currentCount = _lichHenList.Count(l => l.MaDichVuYte == lh.MaDichVuYte && l.NgayHen == lh.NgayHen);
                        if (currentCount >= MAX_APPOINTMENTS_PER_SLOT)
                            throw new InvalidOperationException("Đã đạt giới hạn số lượng lịch hẹn cho khung giờ này.");

                        lh.MaLichHen = _lichHenList.Count + 1;
                        _lichHenList.Add(lh);
                    }
                })
                .ReturnsAsync((TblLichHen lh) => lh);

            // Act
            var tasks = messages.Select(m => Task.Run(() => _lichHenService.ProcessLichHenMessage(m))).ToList();
            var results = new List<(int MaBenhNhan, bool Success, string Message)>();

            foreach (var (message, task) in messages.Zip(tasks, (m, t) => (m, t)))
            {
                try
                {
                    await task;
                    results.Add((message.MaBenhNhan, true, "Thành công"));
                }
                catch (Exception ex)
                {
                    results.Add((message.MaBenhNhan, false, ex.Message));
                }
            }

            // Log results
            Console.WriteLine("\n=== Kết quả xử lý ===");
            foreach (var result in results.OrderBy(r => r.MaBenhNhan))
            {
                Console.WriteLine($"Yêu cầu (MaBenhNhan={result.MaBenhNhan}): {(result.Success ? "Thành công" : $"Thất bại - {result.Message}")}");
            }

            var successCount = results.Count(r => r.Success);
            var failureCount = results.Count(r => !r.Success);

            Console.WriteLine("\n\n=== Tóm tắt ===");
            Console.WriteLine($"Tổng số lịch hẹn đặt thành công: {successCount}");
            Console.WriteLine($"Tổng số lịch hẹn bị từ chối: {failureCount}");
            Console.WriteLine($"Danh sách lịch hẹn trong hệ thống:");
            foreach (var lh in _lichHenList.OrderBy(lh => lh.MaBenhNhan))
            {
                Console.WriteLine($"- MaLichHen={lh.MaLichHen}, MaBenhNhan={lh.MaBenhNhan}, MaDichVuYTe={lh.MaDichVuYte}, NgayHen={lh.NgayHen}, TrangThai={lh.TrangThai}");
            }

            // Assert
            Assert.Equal(MAX_APPOINTMENTS_PER_SLOT, successCount); // Expect 5 successes
            Assert.Equal(messages.Count - MAX_APPOINTMENTS_PER_SLOT, failureCount); // Expect 10 - 5 = 5 failures
            Assert.Equal(MAX_APPOINTMENTS_PER_SLOT, _lichHenList.Count); // Expect 5 appointments in the list
            Assert.Contains(results, r => !r.Success && r.Message == "Đã đạt giới hạn số lượng lịch hẹn cho khung giờ này.");
        }
    }
}