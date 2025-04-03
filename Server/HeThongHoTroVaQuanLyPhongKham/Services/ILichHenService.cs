using HeThongHoTroVaQuanLyPhongKham.Dtos.HeThongHoTroVaQuanLyPhongKham.DTOs;
using HeThongHoTroVaQuanLyPhongKham.Dtos.UpdateModels;

namespace HeThongHoTroVaQuanLyPhongKham.Services
{
    public interface ILichHenService : IService<LichHenDTO>
    {
        Task<(IEnumerable<LichHenDTO> Items, int TotalItems, int TotalPages)> GetAllAsync(
            int page, int pageSize,
            DateTime? ngayHen = null,
            int? maNhanVien = null,
            int? maPhong = null
        );
        Task<IEnumerable<LichHenDTO>> GetAllAsync();
        Task<LichHenDTO> UpdateTrangThaiAsync(LichHenUpdateDTO dto);
        Task<LichHenDTO> AddForPatientAsync(LichHenCreateDTO dto);
        Task<LichHenDTO> UpdateAsync(LichHenForUpdateDTO dto);
        Task<LichHenDTO> GetByMaBenhNhan(int maBenhNhan);
        Task ProcessLichHenMessage(LichHenMessageDTO message);
    }
}
