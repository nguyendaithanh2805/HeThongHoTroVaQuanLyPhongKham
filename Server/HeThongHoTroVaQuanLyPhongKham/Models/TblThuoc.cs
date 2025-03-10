﻿using System;
using System.Collections.Generic;

namespace HeThongHoTroVaQuanLyPhongKham.Models;

public partial class TblThuoc
{
    public int MaThuoc { get; set; }

    public string Ten { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public string DonVi { get; set; } = null!;

    public virtual ICollection<TblDonThuocChiTiet> TblDonThuocChiTiets { get; set; } = new List<TblDonThuocChiTiet>();
}
