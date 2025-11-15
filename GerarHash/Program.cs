using System;

var senha = args.Length > 0 ? args[0] : "admin123";
var hash = BCrypt.Net.BCrypt.HashPassword(senha, 11);

Console.WriteLine($"Senha: {senha}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine();
Console.WriteLine("SQL para atualizar:");
Console.WriteLine($"UPDATE AGRICAMPANHA_USUARIO SET SenhaHash = '{hash}' WHERE Login = 'admin';");
