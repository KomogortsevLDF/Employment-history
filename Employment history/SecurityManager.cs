using System.Globalization;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Employment_history
{
    public class SecurityManager
    {
        public bool ValidateLogin(string login)
        {
            return login.Length >= 8 && !login.Contains(" ");
        }

        public bool ValidatePassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(c => char.IsUpper(c));
            bool hasLowerCase = password.Any(c => char.IsLower(c));
            bool hasDigit = password.Any(c => char.IsDigit(c));
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));
            bool hasSpaces = password.Any(char.IsWhiteSpace);

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar && !hasSpaces;
        }

        public bool ValidateFIO(string fio)
        {
            return fio.Length >= 9 && fio.Count(c => c == ' ') == 2;
        }

        public bool ValidateDate(string input)
        {
            DateTime parsedDate;
            bool isValidFormat = DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            if (!isValidFormat || !IsDayAndMonthValid(input))
            {
                return false; // Неверный формат даты
            }

            DateTime today = DateTime.Today;
            return parsedDate <= today;
        }
        public static bool IsDayAndMonthValid(string inputDate)
        {
            string[] parts = inputDate.Split('.');
            if (parts.Length != 3)
            {
                return false; // Неверный формат даты
            }

            if (!int.TryParse(parts[0], out int day) || !int.TryParse(parts[1], out int month))
            {
                return false; // Неверный формат дня или месяца
            }

            if (month < 1 || month > 12 || day < 1 || day > 31)
            {
                return false; // Недопустимые значения для дня или месяца
            }

            return true;
        }
    }
}
