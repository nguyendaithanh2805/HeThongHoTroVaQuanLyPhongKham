import { Routes } from '@angular/router';
import { TrangChuComponent } from './users/components/trang-chu/trang-chu.component';
import { GioiThieuComponent } from './users/components/gioi-thieu/gioi-thieu.component';
import { DichVuComponent } from './users/components/dich-vu/dich-vu.component';
import { LichHenComponent } from './users/components/lich-hen/lich-hen.component';
import { DangNhapComponent } from './users/components/dang-nhap/dang-nhap.component';
import { UserLayoutComponent } from './users/components/user-layout.component';
import { AdminLayoutComponent } from './admins/components/admin-layout/admin-layout.component';
import { PhongKhamComponent } from './admins/components/phong-kham/phong-kham.component';
import { DichVuYTeComponent } from './admins/components/dich-vu-y-te/dich-vu-y-te.component';
import { adminAuthGuard } from './guards/adminAuthGuard';
import { UnauthorizedComponent } from './users/components/unauthorized/unauthorized.component';
import { authGuard } from './guards/authGuards';

export const routes: Routes = [
  { 
    path: '',
    component: UserLayoutComponent,
    children: [
      { path: '', component: TrangChuComponent },
      { path: 'gioi-thieu', component: GioiThieuComponent },
      { path: 'dich-vu', component: DichVuComponent },
      { path: 'lich-hen', component: LichHenComponent, canActivate: [authGuard] },
      { path: 'dang-nhap', component: DangNhapComponent },
      { path: 'unauthorized', component: UnauthorizedComponent}
    ]
  },

  //Admins
  { 
    path: 'admin', 
    component: AdminLayoutComponent,
    canActivate: [adminAuthGuard],
    children: [
      // { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'phong-kham', component: PhongKhamComponent },
      { path: 'dich-vu-y-te', component: DichVuYTeComponent}
    ]
  }
];
