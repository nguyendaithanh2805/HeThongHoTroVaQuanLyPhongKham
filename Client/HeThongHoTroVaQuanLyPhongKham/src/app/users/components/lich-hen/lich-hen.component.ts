import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ApiResponse } from '../../../commons/ApiResponse';
import { LichHenService } from '../../../services/lich-hen/lich-hen.service';
import { DichVuYTeService } from '../../../services/dich-vu-y-te/dich-vu-yte.service';
import { NotificationService } from '../../../services/handle-error/NotificationService';
import { HttpErrorResponse } from '@angular/common/http';
import { NotificationComponent } from "../notification/notification.component";
import { CommonModule } from '@angular/common';
import { LichHenCreateDTO } from '../../../interfaces/LichHenCreateDTO';
import { DateFormatterService } from '../../../services/DateFormatterService';
import { AuthService } from '../../../services/Auth/AuthService';
import { BenhNhanService } from '../../../services/benh-nhan/benh-nhan.service';
import { ChatService, LichHenUpdate } from '../../../services/chat/chat.service';
import { IBenhNhan } from '../../../interfaces/benh-nhan/IBenhNhan';
import { IDichVuYTe } from '../../../interfaces/dich-vu-y-te/IDichVuYTe';

@Component({
  selector: 'app-lich-hen',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NotificationComponent
  ],
  templateUrl: './lich-hen.component.html',
  styleUrls: [
    './lich-hen.component.css',
    "/public/assets/users/bootstrap/owl.carousel.min.css",
    "/public/assets/users/bootstrap/tempusdominus-bootstrap-4.min.css",
    "/public/assets/users/bootstrap/bootstrap.min.css",
    "/public/assets/users/css/style.css"
  ],
  encapsulation: ViewEncapsulation.None
})
export class LichHenComponent implements OnInit, OnDestroy {
  lichHenForm: FormGroup;
  danhSachDichVu: IDichVuYTe[] = [];
  minDateTime: string;
  maxDateTime: string;
  isTenNull: boolean = true;
  maTaiKhoan: number | null = null;
  maBenhNhan: number | null = null;
  lichHenUpdateMessage: string | null = null; // Lưu thông báo từ SignalR
  private lichHenUpdateSubscription: Subscription | null = null; // Subscription để hủy lắng nghe SignalR

  constructor(
    private fb: FormBuilder,
    private lichHenService: LichHenService,
    private dichVuYTeService: DichVuYTeService,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private dateFormatter: DateFormatterService,
    private authService: AuthService,
    private benhNhanService: BenhNhanService,
    private router: Router,
    private chatService: ChatService
  ) {
    this.lichHenForm = this.fb.group({
      MaDichVuYTe: ['', [Validators.required, Validators.min(1)]],
      NgayHen: ['', Validators.required],
      Ten: ['']
    });

    const now = new Date();
    this.minDateTime = this.dateFormatter.formatToISOString(now).slice(0, 16);

    const maxDate = new Date(now);
    maxDate.setMonth(maxDate.getMonth() + 1);
    this.maxDateTime = this.dateFormatter.formatToISOString(maxDate).slice(0, 16);
  }

  ngOnInit(): void {
    this.loadDanhSachDichVu();
    this.maTaiKhoan = this.authService.getMaTaiKhoanFromToken();

    if (this.maTaiKhoan) {
      this.benhNhanService.getBenhNhanByMaTaiKhoan(this.maTaiKhoan).subscribe({
        next: (response: ApiResponse<IBenhNhan>) => {
          if (response.status && response.data) {
            this.maBenhNhan = response.data.maBenhNhan;
            this.isTenNull = response.data.ten == null;
            if (this.isTenNull) {
              this.lichHenForm.get('Ten')?.setValidators([Validators.required, Validators.minLength(2), Validators.maxLength(50)]);
            } else {
              this.lichHenForm.get('Ten')?.clearValidators();
            }
            this.lichHenForm.get('Ten')?.updateValueAndValidity();
          } else {
            this.notificationService.showError('Không tìm thấy thông tin bệnh nhân.');
          }
        },
        error: (err: HttpErrorResponse) => this.xuLyLoi(err)
      });
    } else {
      this.notificationService.showError('Không thể xác định mã tài khoản từ token.');
    }

    this.route.queryParams.subscribe(params => {
      const maDichVuYTe = params['maDichVuYTe'];
      if (maDichVuYTe) {
        this.lichHenForm.patchValue({ MaDichVuYTe: maDichVuYTe });
      }
    });

    // Lắng nghe thông báo lịch hẹn từ SignalR
    this.lichHenUpdateSubscription = this.chatService.lichHenUpdate$.subscribe(update => {
      if (update && this.maBenhNhan && update.maBenhNhan === this.maBenhNhan) {
        this.lichHenUpdateMessage = update.message;
        if (update.status === 'Success') {
          this.notificationService.showSuccess(update.message);
          this.lichHenForm.reset();
        } else {
          this.notificationService.showError(update.message);
        }
      }
    });
  }

  ngOnDestroy(): void {
    if (this.lichHenUpdateSubscription) {
      this.lichHenUpdateSubscription.unsubscribe();
    }
  }

  loadDanhSachDichVu(): void {
    this.dichVuYTeService.getAllServices(1, 1000).subscribe({
      next: (response: ApiResponse<IDichVuYTe[]>) => {
        if (response.status && response.data) {
          this.danhSachDichVu = response.data;
        } else {
          this.notificationService.showError('Không tải được danh sách dịch vụ y tế.');
        }
      },
      error: (err: HttpErrorResponse) => this.xuLyLoi(err),
    });
  }

  datLichHen(): void {
    if (this.lichHenForm.invalid) {
      this.notificationService.showError('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    if (!this.maTaiKhoan) {
      this.notificationService.showError('Không thể xác định tài khoản người dùng.');
      return;
    }

    this.benhNhanService.getBenhNhanByMaTaiKhoan(this.maTaiKhoan).subscribe({
      next: (response: ApiResponse<IBenhNhan>) => {
        if (response.status && response.data) {
          const maBenhNhan = response.data.maBenhNhan;
          this.maBenhNhan = maBenhNhan;

          if (this.isTenNull) {
            const tenBenhNhan = this.lichHenForm.get('Ten')?.value;
            this.benhNhanService.updateForTen(maBenhNhan, { ...response.data, ten: tenBenhNhan }).subscribe({
              next: () => this.createLichHen(maBenhNhan),
              error: (err: HttpErrorResponse) => this.notificationService.handleError(err)
            });
          } else {
            this.createLichHen(maBenhNhan);
          }
        } else {
          this.notificationService.showError('Không tìm thấy thông tin bệnh nhân.');
        }
      },
      error: (err: HttpErrorResponse) => this.notificationService.handleError(err)
    });
  }

  private createLichHen(maBenhNhan: number): void {
    const lichHen: LichHenCreateDTO = {
      MaBenhNhan: maBenhNhan,
      MaDichVuYTe: Number(this.lichHenForm.get('MaDichVuYTe')?.value),
      NgayHen: this.dateFormatter.formatToISOString(this.lichHenForm.get('NgayHen')?.value),
    };

    this.lichHenService.createForPatient(lichHen).subscribe({
      next: (lichHenResponse: ApiResponse<any>) => {
        if (lichHenResponse.status) {
          this.notificationService.showSuccess('Đang xử lý lịch hẹn... Vui lòng chờ thông báo.');
        } else {
          this.notificationService.showError(lichHenResponse.message || 'Đặt lịch hẹn thất bại.');
        }
      },
      error: (err: HttpErrorResponse) => this.notificationService.handleError(err)
    });
  }

  sendMessageToStaff(message: string): void {
    if (this.maTaiKhoan) {
      console.log('Sending message with MaTaiKhoan:', this.maTaiKhoan); // Debug
      this.chatService.sendMessageToStaff(this.maTaiKhoan, message);
      this.notificationService.showSuccess('Đã gửi tin nhắn đến nhân viên hỗ trợ.');
    } else {
      this.notificationService.showError('Không thể gửi tin nhắn: Không xác định được mã tài khoản.');
    }
  }

  xuLyLoi(err: HttpErrorResponse): void {
    this.notificationService.handleError(err);
  }
}