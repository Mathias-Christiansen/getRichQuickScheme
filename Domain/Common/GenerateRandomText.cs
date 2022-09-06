using System.Text;

namespace Domain.Common;

public static class GenerateRandomText
{
    public static string Generate(int length)
    {
        if (length <= 0) return "";
        const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var random = new Random();
        var builder = new StringBuilder();
        foreach (var c in Enumerable.Range(0,length).Select(_ => random.Next(0, chars.Length)).Select(x => chars[x]))
        {
            builder.Append(c);
        }

        return builder.ToString();
    }
}