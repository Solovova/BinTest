using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace BinGuiDev.Components.Dev;

public partial class Dev3 : UserControl
{
    public Dev3()
    {
        InitializeComponent();
    }
    
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return new ValidationResult(false, "Значення не може бути пустим");

            if (!TimeSpan.TryParseExact(value.ToString(), 
                    "hh\\:mm\\:ss", 
                    CultureInfo.InvariantCulture, 
                    out TimeSpan timeSpan))
            {
                return new ValidationResult(false, "Неправильний формат часу. Використовуйте формат ГГ:ХХ:СС");
            }

            // Додаткові перевірки при необхідності
            if (timeSpan.Hours > 23)
                return new ValidationResult(false, "Години не можуть бути більше 23");
        
            if (timeSpan.Minutes > 59)
                return new ValidationResult(false, "Хвилини не можуть бути більше 59");
        
            if (timeSpan.Seconds > 59)
                return new ValidationResult(false, "Секунди не можуть бути більше 59");

            return ValidationResult.ValidResult;
        }
    }

}