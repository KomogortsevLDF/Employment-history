using System.Linq;

namespace Employment_history
{
    public class SecurityManager
    {
        public bool ValidateLogin(string login)
        {
            return login.Length >= 8;
        }

        public bool ValidatePassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(c => char.IsUpper(c));
            bool hasLowerCase = password.Any(c => char.IsLower(c));
            bool hasDigit = password.Any(c => char.IsDigit(c));
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }
}
