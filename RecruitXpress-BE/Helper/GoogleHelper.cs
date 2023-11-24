using System.Text;

namespace RecruitXpress_BE.Helper;

public class GoogleHelper
{
    public static string urlEncodeForGoogle(string url) {
        const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.~";
        var result = new StringBuilder();
        foreach(var symbol in url) {
            if (unreservedChars.IndexOf(symbol) != -1) {
                result.Append(symbol);
            } else {
                result.Append("%" + ((int) symbol).ToString("X2"));
            }
        }

        return result.ToString();

    }
}