using System;
using Microsoft.Data.SqlClient;
using SafeVault.Models;

namespace SafeVault.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                                                    ?? "Server=localhost;Database=SafeVaultDb;Trusted_Connection=True;Encrypt=True;";

        public bool ValidateUserSecure(UserInputDto input)
        {
            string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Username = @Username";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 100).Value = input.Email;
                    command.Parameters.Add("@Username", System.Data.SqlDbType.VarChar, 50).Value = input.Username;

                    try
                    {
                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Database access error occurred safely: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}