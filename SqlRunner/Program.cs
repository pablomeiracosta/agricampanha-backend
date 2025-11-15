using System;
using System.IO;
using Microsoft.Data.SqlClient;

string connectionString = "Data Source=mssql.impulsoweb.uni5.net;Initial Catalog=impulsoweb;User Id=impulsoweb;Password=1mpuls0;TrustServerCertificate=True;";
string sqlFilePath = @"c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\CRIAR_TABELAS.sql";

try
{
    Console.WriteLine("Lendo arquivo SQL...");
    string sqlScript = File.ReadAllText(sqlFilePath);

    Console.WriteLine("Conectando ao banco de dados...");
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Conectado com sucesso!");

        // Dividir script em batches pelo GO
        string[] batches = sqlScript.Split(new string[] { "\r\nGO\r\n", "\nGO\n", "\rGO\r" }, StringSplitOptions.RemoveEmptyEntries);

        Console.WriteLine($"Executando {batches.Length} comandos SQL...\n");

        foreach (string batch in batches)
        {
            if (!string.IsNullOrWhiteSpace(batch))
            {
                using (SqlCommand command = new SqlCommand(batch, connection))
                {
                    command.CommandTimeout = 120;
                    command.ExecuteNonQuery();
                }
            }
        }

        Console.WriteLine("\nScript executado com sucesso!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERRO: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}
