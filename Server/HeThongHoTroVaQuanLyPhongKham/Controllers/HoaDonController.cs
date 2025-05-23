﻿using HeThongHoTroVaQuanLyPhongKham.Common;
using HeThongHoTroVaQuanLyPhongKham.Dtos;
using HeThongHoTroVaQuanLyPhongKham.Dtos.UpdateModels;
using HeThongHoTroVaQuanLyPhongKham.Exceptions;
using HeThongHoTroVaQuanLyPhongKham.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeThongHoTroVaQuanLyPhongKham.Controllers
{
    [Route("api/admin/invoices")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalItems, totalPages) = await _hoaDonService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<IEnumerable<HoaDonDTO>>.Success(items, page, pageSize, totalPages, totalItems, $"Đã lấy danh sách hóa đơn - trang {page} với {pageSize} bản ghi."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "QuanLy,KeToan,NhanVienHanhChinh,BenhNhan")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                return Ok(ApiResponse<HoaDonDTO>.Success(
                    await _hoaDonService.GetByIdAsync(id), $"Tìm thấy Hóa đơn với ID [{id}]."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "QuanLy,KeToan")]
        public async Task<IActionResult> Create([FromBody] HoaDonDTO hoaDonDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = hoaDonDTO.MaHoaDon },
                    ApiResponse<HoaDonDTO>.Success(
                        await _hoaDonService.AddAsync(hoaDonDTO), "Thêm hóa đơn thành công."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "QuanLy,KeToan")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] HoaDonDTO hoaDonDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (id != hoaDonDTO.MaHoaDon)
                    return BadRequest("Id không khớp");

                return Ok(ApiResponse<HoaDonDTO>.Success(
                    await _hoaDonService.UpdateAsync(hoaDonDTO), $"Cập nhật hóa đơn với ID [{id}] thành công."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "QuanLy,KeToan")]
        public async Task<IActionResult> UpdateTrangThaiThanhToan([FromRoute] int id, [FromBody] HoaDonUpdateDTO hoaDonDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != hoaDonDTO.MaHoaDon)
                    return BadRequest("Id không khớp");

                return Ok(ApiResponse<HoaDonDTO>.Success(
                    await _hoaDonService.UpdateTrangThaiAsync(hoaDonDTO), $"Cập nhật hóa đơn với mã hóa đơn [{id}] - trạng thái [{hoaDonDTO.TrangThaiThanhToan}] thành công"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }

        [HttpPatch("{id:int}/total-amount")]
        [Authorize(Roles = "QuanLy,KeToan")]
        public async Task<IActionResult> UpdateTongTien([FromRoute] int id, [FromBody] HoaDonUpdateDTO hoaDonDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != hoaDonDTO.MaHoaDon)
                    return BadRequest("Id không khớp");

                return Ok(ApiResponse<HoaDonDTO>.Success(
                    await _hoaDonService.UpdateTongTienAsync(hoaDonDTO), $"Cập nhật hóa đơn với mã hóa đơn [{id}] - tổng tiền [{hoaDonDTO.TongTien}] thành công"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "QuanLy,KeToan")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await _hoaDonService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<HoaDonDTO>.Fail(ex.Message));
            }
        }
    }
}
