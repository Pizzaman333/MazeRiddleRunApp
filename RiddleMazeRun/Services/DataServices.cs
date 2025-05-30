using RiddleMazeRun.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiddleMazeRun.Services;

class DataServices
{
    public async Task SaveUserToFileAsync(User newUser, string filePath)
    {
        List<User> users = new();

        try
        {
            if (File.Exists(filePath))
            {
                string existingJson = await File.ReadAllTextAsync(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    users = JsonSerializer.Deserialize<List<User>>(existingJson) ?? new List<User>();
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            }

            users.Add(newUser);
            string updatedJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving user: {ex.Message}");
        }
    }

    public bool IsEmailInUse(string email, string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                var users = JsonSerializer.Deserialize<List<User>>(existingJson);
                if (users != null)
                {
                    return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking email: {ex.Message}");
        }

        return false;
    }

    public async Task<bool> UpdateUserCompletedLevelsAsync(string email, string password, int newCompletedLevels, string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return false;

            string existingJson = await File.ReadAllTextAsync(filePath);
            List<User> users = JsonSerializer.Deserialize<List<User>>(existingJson) ?? new List<User>();

            string hashedPassword = password; // Use your existing method

            var user = users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.HashedPassword == hashedPassword);

            if (user != null)
            {
                user.CompletedLevels = newCompletedLevels;

                string updatedJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, updatedJson);

                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating user: {ex.Message}");
        }

        return false;
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

}
