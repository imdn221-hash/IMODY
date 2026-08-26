using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace IMODY
{
    public class UserAccount
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
    }

    public static class AccountService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "IMODY",
            "accounts.json");

        public static bool Create(string name, string email, string password)
        {
            List<UserAccount> accounts = LoadAccounts();

            if (accounts.Any(x =>
                x.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }


            accounts.Add(new UserAccount
            {
                Name = name,
                Email = email,
                PasswordHash = Hash(password)
            });

            SaveAccounts(accounts);
            return true;
        }

        public static bool ResetPassword(
    string email,
    string newPassword)
        {
            List<UserAccount> accounts = LoadAccounts();

            UserAccount? account = accounts.FirstOrDefault(x =>
                x.Email.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase));

            if (account == null)
                return false;

            account.PasswordHash = Hash(newPassword);

            SaveAccounts(accounts);

            return true;
        }
        public static UserAccount? Login(string email, string password)
        {

            return LoadAccounts().FirstOrDefault(x =>
                x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                x.PasswordHash == Hash(password));
        }

        private static List<UserAccount> LoadAccounts()
        {
            if (!File.Exists(FilePath))
                return new List<UserAccount>();

            string json = File.ReadAllText(FilePath);

            return JsonSerializer.Deserialize<List<UserAccount>>(json)
                   ?? new List<UserAccount>();
        }

        private static void SaveAccounts(List<UserAccount> accounts)
        {
            string? folder = Path.GetDirectoryName(FilePath);

            if (folder != null)
                Directory.CreateDirectory(folder);

            string json = JsonSerializer.Serialize(accounts);
            File.WriteAllText(FilePath, json);
        }

        private static string Hash(string password)
        {
            byte[] bytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(password));

            return Convert.ToHexString(bytes);
        }
    }
}