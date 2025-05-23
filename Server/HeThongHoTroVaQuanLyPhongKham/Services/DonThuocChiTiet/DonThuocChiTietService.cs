﻿using HeThongHoTroVaQuanLyPhongKham.Dtos;
using HeThongHoTroVaQuanLyPhongKham.Exceptions;
using HeThongHoTroVaQuanLyPhongKham.Mappers;
using HeThongHoTroVaQuanLyPhongKham.Models;
using HeThongHoTroVaQuanLyPhongKham.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HeThongHoTroVaQuanLyPhongKham.Services.DonThuocChiTiet
{
    public class DonThuocChiTietService : BaseService, IDonThuocChiTietService
    {
        private readonly IRepository<TblDonThuocChiTiet> _donThuocChiTietRepository;
        private readonly IMapper<DonThuocChiTietDTO, TblDonThuocChiTiet> _donThuocChiTietMapping;
        private readonly IService<ThuocDTO> _thuocService;
        private readonly IService<DonThuocDTO> _donThuocService;

        public DonThuocChiTietService(IRepository<TblDonThuocChiTiet> donThuocChiTietRepository, IMapper<DonThuocChiTietDTO, TblDonThuocChiTiet> donThuocChiTietMapping, IService<ThuocDTO> thuocService, IService<DonThuocDTO> donThuocService)
        {
            _donThuocChiTietRepository = donThuocChiTietRepository;
            _donThuocChiTietMapping = donThuocChiTietMapping;
            _thuocService = thuocService;
            _donThuocService = donThuocService;
        }

        public async Task AddAsync(IEnumerable<DonThuocChiTietDTO> dtos)
        {
            foreach (var chiTiet in dtos)
            {
                // Kiểm tra MaDonThuoc
                var donThuoc = await _donThuocService.GetByIdAsync(chiTiet.MaDonThuoc);
                if (donThuoc == null)
                    throw new NotFoundException($"Đơn thuốc với ID [{chiTiet.MaDonThuoc}] không tồn tại.");

                // Kiểm tra MaThuoc và lấy DonGia
                var thuoc = await _thuocService.GetByIdAsync(chiTiet.MaThuoc);
                if (thuoc == null)
                    throw new NotFoundException($"Thuốc với ID [{chiTiet.MaThuoc}] không tồn tại.");

                // Kiểm tra SoLuong
                if (chiTiet.SoLuong <= 0)
                    throw new ArgumentException("Số lượng thuốc phải lớn hơn 0.");

                // Kiểm tra xem chi tiết đã tồn tại chưa
                var existingChiTiet = await _donThuocChiTietRepository.GetQueryable()
                    .FirstOrDefaultAsync(ct => ct.MaDonThuoc == chiTiet.MaDonThuoc &&
                                              ct.MaThuoc == chiTiet.MaThuoc &&
                                              ct.SoLuong == chiTiet.SoLuong &&
                                              ct.CachDung == chiTiet.CachDung &&
                                              ct.LieuLuong == chiTiet.LieuLuong &&
                                              ct.TanSuat == chiTiet.TanSuat);

                if (existingChiTiet != null)
                    continue;

                // Tính ThanhTien
                chiTiet.ThanhTien = thuoc.DonGia * chiTiet.SoLuong;

                await _donThuocChiTietRepository.CreateAsync(
                    _donThuocChiTietMapping.MapDtoToEntity(chiTiet)
                );
            }
        }

        public Task<DonThuocChiTietDTO> AddAsync(DonThuocChiTietDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            await _donThuocChiTietRepository.DeleteAsync(
                _donThuocChiTietMapping.MapDtoToEntity(
                    await GetByIdAsync(id)));
        }

        public async Task<IEnumerable<DonThuocChiTietDTO>> GetAllAsync(int page, int pageSize)
        {
            var pageSkip = CalculatePageSkip(page, pageSize);
            var hoaDons = await _donThuocChiTietRepository.FindAllAsync(page, pageSize, pageSkip, "MaDonThuocChiTiet");
            return hoaDons.Select(t => _donThuocChiTietMapping.MapEntityToDto(t));
        }

        public async Task<DonThuocChiTietDTO> GetByIdAsync(int id)
        {
            var donThuocCt = await _donThuocChiTietRepository.FindByIdAsync(id, "MaDonThuocChiTiet");
            if (donThuocCt is null)
                throw new NotFoundException($"Đơn thuốc chi tiết với ID [{id}] không tồn tại.");

            return _donThuocChiTietMapping.MapEntityToDto(donThuocCt);
        }

        public async Task<DonThuocChiTietDTO> UpdateAsync(DonThuocChiTietDTO dto)
        {
            var donThuocCtUpdate = _donThuocChiTietMapping.MapDtoToEntity(
                await GetByIdAsync(dto.MaDonThuocChiTiet));

            await _thuocService.GetByIdAsync(dto.MaThuoc);
            await _donThuocService.GetByIdAsync(dto.MaDonThuoc);

            await _thuocService.GetByIdAsync(dto.MaThuoc);

            _donThuocChiTietMapping.MapDtoToEntity(dto, donThuocCtUpdate);

            return _donThuocChiTietMapping.MapEntityToDto(
                await _donThuocChiTietRepository.UpdateAsync(donThuocCtUpdate));
        }

        Task<(IEnumerable<DonThuocChiTietDTO> Items, int TotalItems, int TotalPages)> IService<DonThuocChiTietDTO>.GetAllAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
