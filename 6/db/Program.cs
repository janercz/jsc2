using System;
using Microsoft.Data.SqlClient;

namespace LibraryApp
{
    class Program
    {
        static string connectionString = "Server=localhost,1433;Database=LibraryDb;User Id=sa;Password=SuperTajneHeslo123!;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== DATABÁZE KNIHOVNY ===");
                Console.WriteLine("1. Vypsat všechny knihy (Read)");
                Console.WriteLine("2. Vložit novou knihu (Create)");
                Console.WriteLine("3. Upravit knihu (Update)");
                Console.WriteLine("4. Smazat knihu (Delete)");
                Console.WriteLine("5. Konec");
                Console.Write("\nZadej: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ReadBooks();
                        break;
                    case "2":
                        CreateBook();
                        break;
                    case "3":
                        UpdateBook();
                        break;
                    case "4":
                        DeleteBook();
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Neplatná volba.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nStiskněte libovolnou klávesu pro pokračování...");
                    Console.ReadKey();
                }
            }
        }

        static void ReadBooks()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Author, [Year], IsAvailable FROM Books ORDER BY Title, [Year]";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"{"Id",-5} | {"Název",-30} | {"Autor",-25} | {"Rok",-5} | {"Dostupné"}");
                        Console.WriteLine(new string('-', 85));

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            string author = reader.GetString(2);
                            int year = reader.GetInt32(3);
                            bool isAvailable = reader.GetBoolean(4);

                            Console.WriteLine($"{id,-5} | {title,-30} | {author,-25} | {year,-5} | {(isAvailable ? "Ano" : "Ne")}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při čtení z databáze: {ex.Message}");
            }
        }

        static void CreateBook()
        {
            Console.Write("Zadejte název knihy: ");
            string title = Console.ReadLine();
            Console.Write("Zadejte autora: ");
            string author = Console.ReadLine();
            Console.Write("Zadejte rok vydání: ");
            
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Chyba: Rok musí být číslo.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Books (Title, Author, [Year], IsAvailable) VALUES (@Title, @Author, @Year, 1)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Author", author);
                        cmd.Parameters.AddWithValue("@Year", year);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Kniha byla úspěšně přidána. (Ovlivněno řádků: {rowsAffected})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při vkládání do databáze: {ex.Message}");
            }
        }

        static void UpdateBook()
        {
            Console.Write("Zadejte ID knihy, kterou chcete upravit: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Chyba: ID musí být číslo.");
                return;
            }

            Console.WriteLine("Co chcete upravit?");
            Console.WriteLine("1. Dostupnost (IsAvailable)");
            Console.WriteLine("2. Rok vydání");
            Console.Write("Volba: ");
            string choice = Console.ReadLine();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@Id", id);

                    if (choice == "1")
                    {
                        Console.Write("Je kniha dostupná? (ano/ne): ");
                        bool isAvailable = Console.ReadLine().ToLower() == "ano";
                        query = "UPDATE Books SET IsAvailable = @IsAvailable WHERE Id = @Id";
                        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                    }
                    else if (choice == "2")
                    {
                        Console.Write("Zadejte nový rok vydání: ");
                        if (!int.TryParse(Console.ReadLine(), out int year))
                        {
                            Console.WriteLine("Chyba: Rok musí být číslo.");
                            return;
                        }
                        query = "UPDATE Books SET [Year] = @Year WHERE Id = @Id";
                        cmd.Parameters.AddWithValue("@Year", year);
                    }
                    else
                    {
                        Console.WriteLine("Neplatná volba úpravy.");
                        return;
                    }

                    cmd.CommandText = query;
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Kniha byla úspěšně upravena.");
                    else
                        Console.WriteLine("Kniha s tímto ID nebyla nalezena.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při aktualizaci databáze: {ex.Message}");
            }
        }

        static void DeleteBook()
        {
            Console.Write("Zadejte ID knihy, kterou chcete smazat: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Chyba: ID musí být číslo.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Books WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            Console.WriteLine("Kniha byla úspěšně smazána.");
                        else
                            Console.WriteLine("Kniha s tímto ID nebyla nalezena.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při mazání z databáze: {ex.Message}");
            }
        }
    }
}