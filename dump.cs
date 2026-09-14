using System;
using System.IO;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        var dbPath = @"C:\Users\alext\source\repos\TheBestBean\TheBestBean\coffee.db";
        var connectionString = $"Data Source={dbPath}";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Title, Content FROM BlogPosts";

            try {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Title: {reader.GetString(0)}");
                        Console.WriteLine($"Content: {reader.GetString(1)}");
                        Console.WriteLine("-------------------");
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
