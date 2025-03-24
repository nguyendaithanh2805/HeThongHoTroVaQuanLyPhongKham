﻿namespace HeThongHoTroVaQuanLyPhongKham.Services.HashPassword
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
