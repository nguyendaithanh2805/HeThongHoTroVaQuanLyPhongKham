﻿using HeThongHoTroVaQuanLyPhongKham.Dtos.HeThongHoTroVaQuanLyPhongKham.DTOs;

namespace HeThongHoTroVaQuanLyPhongKham.Services
{
    public interface ILichHenService : IService<LichHenDTO>
    {
        Task<IEnumerable<LichHenDTO>> GetAllAsync(
            int page, int pageSize,
            DateTime? ngayHen = null,
            int? maNhanVien = null,
            int? maPhong = null
        );
        Task<LichHenDTO> UpdateTrangThaiAsync(int maLichHen, string trangThai);
    }
}
