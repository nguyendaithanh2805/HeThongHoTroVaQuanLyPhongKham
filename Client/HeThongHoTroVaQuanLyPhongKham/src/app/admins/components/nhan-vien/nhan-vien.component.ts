import { Component, ViewChild, ViewEncapsulation } from '@angular/core';
import { INhanVien } from '../../../interfaces/nhan-vien/INhanVien';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { NhanVienService } from '../../../services/nhan-vien/nhan-vien.service';
import { NotificationService } from '../../../services/handle-error/NotificationService';
import { Router } from '@angular/router';
import { ApiResponse } from '../../../commons/ApiResponse';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NotificationComponent } from '../../../users/components/notification/notification.component';
import { HasPermissionDirective } from '../../../directive/has-per-mission.directive';
import { IVaiTro } from '../../../interfaces/vai-tro/IVaiTro';
import { VaiTroService } from '../../../services/vai-tro/vai-tro.service';
import { PhongKhamNhanVienService } from '../../../services/phong-kham-nhan-vien/phong-kham-nhan-vien.service';
import { IPhongKhamNhanVien } from '../../../interfaces/phong-kham-nhan-vien/IPhongKhamNhanVien';
import { IPhongKham } from '../../../interfaces/phong-kham/IPhongKham';
import { PhongKhamService } from '../../../services/phong-kham/phong-kham.service';

@Component({
  selector: 'app-nhan-vien',
  standalone: true,
  imports: [CommonModule, MatPaginator, NotificationComponent, ReactiveFormsModule, HasPermissionDirective],
  templateUrl: './nhan-vien.component.html',
    styleUrls: [
      './nhan-vien.component.css',
      '/public/assets/admins/css/styles.css',
      '/public/assets/admins/css/custom.css'
    ],
    encapsulation: ViewEncapsulation.None
})
export class NhanVienComponent {
  // Danh sách các dữ liệu chính
  danhSachNhanVien: INhanVien[] = [];
  danhSachPhongKham: IPhongKham[] = [];
  danhSachVaiTro: IVaiTro[] = [];
  danhSachPhanCong: IPhongKhamNhanVien[] = [];

  // Danh sách nhân viên đã và chưa phân công
  danhSachNhanVienDaPhanCong: INhanVien[] = [];
  danhSachNhanVienChuaPhanCong: INhanVien[] = [];

  // Phân trang cho nhân viên đã phân công
  soLuongNhanVienDaPhanCong: number = 0;
  soLuongMoiTrangDaPhanCong: number = 3;
  trangHienTaiDaPhanCong: number = 0;

  // Phân trang cho nhân viên chưa phân công
  soLuongNhanVienChuaPhanCong: number = 0;
  soLuongMoiTrangChuaPhanCong: number = 3;
  trangHienTaiChuaPhanCong: number = 0;

  // Trạng thái form và nhân viên
  hienThiFormNhanVien: boolean = false;
  cheDoChinhSuaNhanVien: boolean = false;
  nhanVienDangChon: INhanVien | null = null;
  formThongTinNhanVien: FormGroup;

  // Trạng thái form phân công
  hienThiFormPhanCong: boolean = false;
  formPhanCongNhanVien: FormGroup;
  nhanVienDangPhanCong: INhanVien | null = null;

  @ViewChild(MatPaginator) paginatorDaPhanCong!: MatPaginator;
  @ViewChild('unassignedPaginator') paginatorChuaPhanCong!: MatPaginator;

  constructor(
    private nhanVienService: NhanVienService,
    private phongKhamNhanVienService: PhongKhamNhanVienService,
    private phongKhamService: PhongKhamService,
    private vaiTroService: VaiTroService,
    private formBuilder: FormBuilder,
    private thongBaoService: NotificationService,
    private router: Router
  ) {
    this.formThongTinNhanVien = this.formBuilder.group({
      maNhanVien: [{ value: 0, disabled: true }],
      ten: ['', Validators.required],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      caLamViec: ['', Validators.required],
      chuyenMon: ['', Validators.required],
      tenDangNhap: [''],
      matKhau: [''],
      maVaiTro: ['']
    });

    this.formPhanCongNhanVien = this.formBuilder.group({
      maPhongKham: ['', [Validators.required, Validators.min(1)]],
      vaiTro: ['', [Validators.required]]
    });
  }

  ngOnInit() {
    this.taiDanhSachNhanVien();
    this.taiDanhSachPhongKham();
    this.taiDanhSachVaiTro();
    this.taiDanhSachPhanCong();
  }

  // Tải dữ liệu từ server
  taiDanhSachNhanVien() {
    this.nhanVienService.getAllServices(1, 1000).subscribe({
      next: (response: ApiResponse<INhanVien[]>) => {
        if (response.status && response.data) {
          this.danhSachNhanVien = response.data;
          this.phanChiaDuLieuNhanVien();
        } else {
          this.thongBaoService.showError(response.message || 'Không tải được danh sách nhân viên.');
        }
      },
      error: (err: HttpErrorResponse) => this.xuLyLoi(err)
    });
  }

  taiDanhSachPhongKham() {
    this.phongKhamService.getAllServicesNotPaginator().subscribe({
      next: (response: ApiResponse<IPhongKham[]>) => {
        if (response.status && response.data) {
          this.danhSachPhongKham = response.data;
        } else {
          this.thongBaoService.showError('Không tải được danh sách phòng khám.');
        }
      },
      error: (err: HttpErrorResponse) => this.xuLyLoi(err)
    });
  }

  taiDanhSachVaiTro() {
    this.vaiTroService.getAllServices().subscribe({
      next: (response: ApiResponse<IVaiTro[]>) => {
        if (response.status && response.data) {
          this.danhSachVaiTro = response.data.filter(vaiTro => vaiTro.ten !== 'QuanLy');
        } else {
          this.thongBaoService.showError('Không tải được danh sách vai trò.');
        }
      },
      error: (err: HttpErrorResponse) => this.xuLyLoi(err)
    });
  }

  taiDanhSachPhanCong() {
    this.phongKhamNhanVienService.getAllServices(1, 1000).subscribe({
      next: (response: ApiResponse<IPhongKhamNhanVien[]>) => {
        if (response.status && response.data) {
          this.danhSachPhanCong = response.data;
          this.phanChiaDuLieuNhanVien();
        } else {
          this.thongBaoService.showError('Không tải được danh sách phân công.');
        }
      },
      error: (err: HttpErrorResponse) => this.xuLyLoi(err)
    });
  }

  // Phân chia dữ liệu nhân viên
  phanChiaDuLieuNhanVien() {
    const danhSachMaDaPhanCong = new Set(this.danhSachPhanCong.map(a => a.maNhanVien));
    this.danhSachNhanVienDaPhanCong = this.danhSachNhanVien.filter(item => danhSachMaDaPhanCong.has(item.maNhanVien));
    this.danhSachNhanVienChuaPhanCong = this.danhSachNhanVien.filter(item => !danhSachMaDaPhanCong.has(item.maNhanVien));
    this.soLuongNhanVienDaPhanCong = this.danhSachNhanVienDaPhanCong.length;
    this.soLuongNhanVienChuaPhanCong = this.danhSachNhanVienChuaPhanCong.length;
  }

  // Xử lý phân trang
  xuLyThayDoiTrangDaPhanCong(event: PageEvent) {
    this.trangHienTaiDaPhanCong = event.pageIndex;
    this.soLuongMoiTrangDaPhanCong = event.pageSize;
  }

  xuLyThayDoiTrangChuaPhanCong(event: PageEvent) {
    this.trangHienTaiChuaPhanCong = event.pageIndex;
    this.soLuongMoiTrangChuaPhanCong = event.pageSize;
  }

  // Quản lý form nhân viên
  themNhanVienMoi() {
    this.cheDoChinhSuaNhanVien = false;
    this.nhanVienDangChon = null;
    this.formThongTinNhanVien.reset({ maNhanVien: 0, caLamViec: '' });
    this.hienThiFormNhanVien = true;
  }

  chinhSuaNhanVien(nhanVien: INhanVien) {
    this.cheDoChinhSuaNhanVien = true;
    this.nhanVienDangChon = { ...nhanVien };
    this.formThongTinNhanVien.patchValue({
      maNhanVien: nhanVien.maNhanVien,
      ten: nhanVien.ten,
      soDienThoai: nhanVien.soDienThoai,
      caLamViec: nhanVien.caLamViec,
      chuyenMon: nhanVien.chuyenMon,
      maVaiTro: nhanVien.maVaiTro,
      maTaiKhoan: nhanVien.maTaiKhoan || 0
    });
    this.hienThiFormNhanVien = true;
  }

  luuThongTinNhanVien() {
    const formValue = this.formThongTinNhanVien.value;
  
    const maNhanVien = this.formThongTinNhanVien.get('maNhanVien')?.value || 0;
  
    const nhanVien: INhanVien = {
      maNhanVien: maNhanVien,
      ten: formValue.ten,
      soDienThoai: formValue.soDienThoai,
      caLamViec: formValue.caLamViec || undefined,
      chuyenMon: formValue.chuyenMon,
      tenDangNhap: formValue.tenDangNhap || '',
      matKhau: formValue.matKhau || null,
      maVaiTro: Number(formValue.maVaiTro) || 0,
      maTaiKhoan: 0
    };
  
    if (this.cheDoChinhSuaNhanVien) {
      // Chế độ chỉnh sửa
      this.nhanVienService.updateService(maNhanVien, nhanVien).subscribe({
        next: (response: ApiResponse<INhanVien>) => {
          if (response.status) {
            this.thongBaoService.showSuccess('Cập nhật nhân viên thành công!');
            this.datLaiFormNhanVien();
            this.taiDanhSachNhanVien();
          } else {
            this.thongBaoService.showError(response.message || 'Cập nhật thất bại.');
          }
        },
        error: (err: HttpErrorResponse) => this.xuLyLoi(err)
      });
    } else {
      // Chế độ thêm mới
      this.nhanVienService.createService(nhanVien).subscribe({
        next: (response: ApiResponse<INhanVien>) => {
          if (response.status) {
            this.thongBaoService.showSuccess('Thêm nhân viên thành công!');
            this.datLaiFormNhanVien();
            this.taiDanhSachNhanVien();
          } else {
            this.thongBaoService.showError(response.message || 'Thêm thất bại.');
          }
        },
        error: (err: HttpErrorResponse) => this.xuLyLoi(err)
      });
    }
  }

  xoaNhanVien(maNhanVien: number) {
    if (confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) {
      this.nhanVienService.deleteService(maNhanVien).subscribe({
        next: () => {
          this.thongBaoService.showSuccess('Xóa nhân viên thành công!');
          this.taiDanhSachNhanVien();
          this.taiDanhSachPhanCong();
        },
        error: (err: HttpErrorResponse) => this.xuLyLoi(err)
      });
    }
  }

  // Quản lý form phân công
  phanCongNhanVien(nhanVien: INhanVien) {
    this.nhanVienDangPhanCong = nhanVien;
    this.hienThiFormPhanCong = true;
    this.formPhanCongNhanVien.reset();

    // Nếu nhân viên đã có phân công, điền dữ liệu hiện tại vào form
    const phanCongHienTai = this.danhSachPhanCong.find(pc => pc.maNhanVien === nhanVien.maNhanVien);
    if (phanCongHienTai) {
      this.formPhanCongNhanVien.patchValue({
        maPhongKham: phanCongHienTai.maPhongKham,
        vaiTro: phanCongHienTai.vaiTro
      });
    }
  }

  luuPhanCongNhanVien() {
    const giaTriForm = this.formPhanCongNhanVien.value;
    const thongTinPhanCong: IPhongKhamNhanVien = {
      maPhongKhamNhanVien: 0,
      maPhongKham: Number(giaTriForm.maPhongKham),
      maNhanVien: this.nhanVienDangPhanCong?.maNhanVien || 0,
      vaiTro: giaTriForm.vaiTro
    };

    // Kiểm tra xem nhân viên đã có phân công chưa
    const phanCongHienTai = this.danhSachPhanCong.find(pc => pc.maNhanVien === thongTinPhanCong.maNhanVien);

    if (phanCongHienTai) {
      // Nếu đã có phân công, thực hiện update
      thongTinPhanCong.maPhongKhamNhanVien = phanCongHienTai.maPhongKhamNhanVien;
      this.phongKhamNhanVienService.updateService(phanCongHienTai.maPhongKhamNhanVien, thongTinPhanCong).subscribe({
        next: (response: ApiResponse<IPhongKhamNhanVien>) => {
          if (response.status) {
            this.thongBaoService.showSuccess('Cập nhật phân công thành công!');
            this.huyFormPhanCong();
            this.taiDanhSachPhanCong();
            this.taiDanhSachNhanVien();
          } else {
            this.thongBaoService.showError(response.message || 'Cập nhật phân công thất bại.');
          }
        },
        error: (err: HttpErrorResponse) => this.xuLyLoi(err)
      });
    } else {
      // Nếu chưa có phân công, thực hiện create
      this.phongKhamNhanVienService.createService(thongTinPhanCong).subscribe({
        next: (response: ApiResponse<IPhongKhamNhanVien>) => {
          if (response.status) {
            this.thongBaoService.showSuccess('Phân công nhân viên thành công!');
            this.huyFormPhanCong();
            this.taiDanhSachPhanCong();
            this.taiDanhSachNhanVien();
          } else {
            this.thongBaoService.showError(response.message || 'Phân công thất bại.');
          }
        },
        error: (err: HttpErrorResponse) => this.xuLyLoi(err)
      });
    }
  }

  huyFormPhanCong() {
    this.hienThiFormPhanCong = false;
    this.nhanVienDangPhanCong = null;
    this.formPhanCongNhanVien.reset();
  }

  huyFormNhanVien() {
    this.datLaiFormNhanVien();
  }

  datLaiFormNhanVien() {
    this.hienThiFormNhanVien = false;
    this.cheDoChinhSuaNhanVien = false;
    this.nhanVienDangChon = null;
    this.formThongTinNhanVien.reset({ maNhanVien: 0, caLamViec: '' });
  }

  // Lấy thông tin phòng khám và vai trò
  layPhongKhamCuaNhanVien(maNhanVien: number): string {
    const phanCong = this.danhSachPhanCong.find(a => a.maNhanVien === maNhanVien);
    if (phanCong) {
      const phongKham = this.danhSachPhongKham.find(p => p.maPhongKham === phanCong.maPhongKham);
      return phongKham ? phongKham.loai : 'Không tìm thấy phòng';
    }
    return 'Chưa được phân công';
  }

  layVaiTroPhanCong(maNhanVien: number): string {
    const phanCong = this.danhSachPhanCong.find(pc => pc.maNhanVien === maNhanVien);
    return phanCong ? phanCong.vaiTro : 'Chưa phân công';
  }

  // Xử lý lỗi
  xuLyLoi(err: HttpErrorResponse) {
    this.thongBaoService.handleError(err);
  }
}