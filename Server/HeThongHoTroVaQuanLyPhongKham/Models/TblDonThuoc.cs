﻿using System;
using System.Collections.Generic;

namespace HeThongHoTroVaQuanLyPhongKham.Models;

public partial class TblDonThuoc
{
    public int MaDonThuoc { get; set; }

    public int MaHoSoYte { get; set; }

    public int? MaHoaDon { get; set; }

    public DateTime NgayKeDon { get; set; }

    public virtual TblHoSoYTe MaHoSoYteNavigation { get; set; } = null!;

    public virtual TblHoaDon? MaHoaDonNavigation { get; set; }

    public virtual ICollection<TblDonThuocChiTiet> TblDonThuocChiTiets { get; set; } = new List<TblDonThuocChiTiet>();

    public virtual ICollection<TblKetQuaDieuTri> TblKetQuaDieuTris { get; set; } = new List<TblKetQuaDieuTri>();
}
