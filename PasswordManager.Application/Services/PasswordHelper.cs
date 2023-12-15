using PasswordmanagerApp.Application.Model;
using System.Text.RegularExpressions;

public static class PasswordHelper
{
    public static Strongpwd CalculateSafeness(string password)
    {
        int score = 0;

        if (Regex.IsMatch(password, "[A-Z]"))
            score++;
        if (Regex.IsMatch(password, "[a-z]"))
            score++;
        if (Regex.IsMatch(password, "[0-9]")) 
            score++;
        if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) 
            score++;

        switch (score)
        {
            case 1:
            case 2:
                return Strongpwd.Weak;
            case 3:
                return Strongpwd.Middle;
            default:
                return Strongpwd.Strong;
        }
    }
}
