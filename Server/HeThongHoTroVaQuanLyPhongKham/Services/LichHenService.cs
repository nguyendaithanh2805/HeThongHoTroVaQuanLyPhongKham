﻿using HeThongHoTroVaQuanLyPhongKham.Dtos;
using HeThongHoTroVaQuanLyPhongKham.Dtos.HeThongHoTroVaQuanLyPhongKham.DTOs;
using HeThongHoTroVaQuanLyPhongKham.Dtos.UpdateModels;
using HeThongHoTroVaQuanLyPhongKham.Exceptions;
using HeThongHoTroVaQuanLyPhongKham.Mappers;
using HeThongHoTroVaQuanLyPhongKham.Models;
using HeThongHoTroVaQuanLyPhongKham.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HeThongHoTroVaQuanLyPhongKham.Services
{
    public class LichHenService : BaseService, ILichHenService
    {
        private readonly IRepository<TblLichHen> _lichHenRepository;
        private readonly IMapper<LichHenDTO, TblLichHen> _lichHenMapping;
        private readonly IService<BenhNhanDTO> _benhNhanService;
        private readonly IService<NhanVienDTO> _nhanVienService;
        private readonly IService<DichVuYTeDTO> _dichVuYTeService;
        private readonly IService<PhongKhamDTO> _phongKhamService;

        public LichHenService(IRepository<TblLichHen> lichHenRepository, IMapper<LichHenDTO, TblLichHen> lichHenMapping, IService<BenhNhanDTO> benhNhanService, IService<NhanVienDTO> nhanVienService, IService<DichVuYTeDTO> dichVuYTeService, IService<PhongKhamDTO> phongKhamService)
        {
            _lichHenRepository = lichHenRepository;
            _lichHenMapping = lichHenMapping;
            _benhNhanService = benhNhanService;
            _nhanVienService = nhanVienService;
            _dichVuYTeService = dichVuYTeService;
            _phongKhamService = phongKhamService;
        }

        public async Task<LichHenDTO> AddAsync(LichHenDTO dto)
        {
            await _benhNhanService.GetByIdAsync(dto.MaBenhNhan);
            await _nhanVienService.GetByIdAsync(dto.MaNhanVien);
            await _dichVuYTeService.GetByIdAsync(dto.MaDichVuYTe);
            await _phongKhamService.GetByIdAsync(dto.MaPhongKham);

            return _lichHenMapping.MapEntityToDto(
                await _lichHenRepository.CreateAsync(
                    _lichHenMapping.MapDtoToEntity(dto)));
        }

        public async Task DeleteAsync(int id)
        {
            await _lichHenRepository.DeleteAsync(
                _lichHenMapping.MapDtoToEntity(
                    await GetByIdAsync(id)));
        }

        public async Task<IEnumerable<LichHenDTO>> GetAllAsync(
            int page, int pageSize, 
            DateTime? ngayHen = null, 
            int? maNhanVien = null, 
            int? maPhong = null)
        {
            var query = _lichHenRepository.GetQueryable();

            if (ngayHen.HasValue)
                query = query.Where(lh => lh.NgayHen.Date == ngayHen.Value.Date);

            if (maNhanVien.HasValue)
                query = query.Where(lh => lh.MaNhanVien == maNhanVien.Value);

            if (maPhong.HasValue)
                query = query.Where(lh => lh.MaPhongKham == maPhong.Value);

            var pageSkip = CalculatePageSkip(page, pageSize);
            var lichHens = await _lichHenRepository.FindAllWithQueryAsync(query, page, pageSize, pageSkip, "MaLichHen");

            return lichHens.Select(lh => _lichHenMapping.MapEntityToDto(lh));
        }

        public Task<IEnumerable<LichHenDTO>> GetAllAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<LichHenDTO> GetByIdAsync(int id)
        {
            var lichHen = await _lichHenRepository.FindByIdAsync(id, "MaLichHen");
            if (lichHen is null)
                throw new NotFoundException($"Lịch hẹn với ID [{id}] không tồn tại.");

            return _lichHenMapping.MapEntityToDto(lichHen);
        }

        public async Task<LichHenDTO> UpdateAsync(LichHenDTO dto)
        {
            var lichHenUpdate = _lichHenMapping.MapDtoToEntity(
                await GetByIdAsync(dto.MaLichHen));
            var lichHen = await GetByIdAsync(dto.MaLichHen);
            await _benhNhanService.GetByIdAsync(dto.MaBenhNhan);
            await _nhanVienService.GetByIdAsync(dto.MaNhanVien);
            await _dichVuYTeService.GetByIdAsync(dto.MaDichVuYTe);
            await _phongKhamService.GetByIdAsync(dto.MaPhongKham);

            _lichHenMapping.MapDtoToEntity(dto, lichHenUpdate);

            return _lichHenMapping.MapEntityToDto(
                await _lichHenRepository.UpdateAsync(lichHenUpdate));
        }

        public async Task<LichHenDTO> UpdateTrangThaiAsync(LichHenUpdateDTO dto)
        {
            var lichHen = await GetByIdAsync(dto.MaLichHen);

            lichHen.TrangThai = dto.TrangThai;

            return _lichHenMapping.MapEntityToDto(
                await _lichHenRepository.UpdateAsync(
                _lichHenMapping.MapDtoToEntity(lichHen)));
        }
    }
}
