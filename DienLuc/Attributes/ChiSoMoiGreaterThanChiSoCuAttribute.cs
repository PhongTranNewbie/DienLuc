using DienLuc.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class ChiSoMoiGreaterThanChiSoCuAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var chiTietSddien = (ChiTietSddien)validationContext.ObjectInstance;

        if (chiTietSddien.ChiSoMoi < chiTietSddien.ChiSoCu)
        {
            return new ValidationResult("Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.");
        }

        return ValidationResult.Success;
    }
}
