using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000/api";
var client = new HttpClient();

// Primeiro, fazer login para obter o token
Console.WriteLine("=== 1. Fazendo login para obter token ===");
var loginData = new
{
    login = "admin",
    senha = "admin123"
};

var loginResponse = await client.PostAsJsonAsync($"{baseUrl}/Auth/login", loginData);
var loginResult = await loginResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {loginResponse.StatusCode}");
Console.WriteLine($"Response: {loginResult}");

if (!loginResponse.IsSuccessStatusCode)
{
    Console.WriteLine("ERRO: Não foi possível fazer login. Usando token de teste...");
    // Usar um token de teste ou continuar sem autenticação
}
else
{
    var loginObj = JsonSerializer.Deserialize<JsonElement>(loginResult);
    if (loginObj.TryGetProperty("token", out var tokenElement))
    {
        var token = tokenElement.GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine($"Token obtido: {token.Substring(0, 20)}...");
    }
}

Console.WriteLine("\n=== 2. Criando uma nova galeria ===");
var galeriaData = new
{
    titulo = "Galeria de Teste - Notícias",
    descricao = "Galeria criada para testes de funcionalidade",
    idReferencia = 1, // 1 = Notícias
    idRegistroRelacionado = 1
};

var createGaleriaResponse = await client.PostAsJsonAsync($"{baseUrl}/GaleriaFotos", galeriaData);
var createGaleriaResult = await createGaleriaResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {createGaleriaResponse.StatusCode}");
Console.WriteLine($"Response: {createGaleriaResult}");

if (!createGaleriaResponse.IsSuccessStatusCode)
{
    Console.WriteLine("ERRO ao criar galeria!");
    return 1;
}

var galeriaObj = JsonSerializer.Deserialize<JsonElement>(createGaleriaResult);
var galeriaId = galeriaObj.GetProperty("id").GetInt32();
Console.WriteLine($"Galeria criada com ID: {galeriaId}");

Console.WriteLine("\n=== 3. Listando todas as galerias (paginado) ===");
var listGaleriasResponse = await client.GetAsync($"{baseUrl}/GaleriaFotos?pageNumber=1&pageSize=10");
var listGaleriasResult = await listGaleriasResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {listGaleriasResponse.StatusCode}");
Console.WriteLine($"Response: {listGaleriasResult}");

Console.WriteLine("\n=== 4. Buscando galeria por ID ===");
var getGaleriaResponse = await client.GetAsync($"{baseUrl}/GaleriaFotos/{galeriaId}");
var getGaleriaResult = await getGaleriaResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {getGaleriaResponse.StatusCode}");
Console.WriteLine($"Response: {getGaleriaResult}");

Console.WriteLine("\n=== 5. Adicionando foto à galeria (URL existente) ===");
var fotoData = new
{
    idGaleriaFotos = galeriaId,
    url = "https://res.cloudinary.com/demo/image/upload/sample.jpg",
    nomeArquivo = "sample.jpg",
    legenda = "Foto de teste 1",
    tamanho = 150000,
    ordem = 1
};

var addFotoResponse = await client.PostAsJsonAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/adicionar-existente", fotoData);
var addFotoResult = await addFotoResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {addFotoResponse.StatusCode}");
Console.WriteLine($"Response: {addFotoResult}");

if (!addFotoResponse.IsSuccessStatusCode)
{
    Console.WriteLine("ERRO ao adicionar foto!");
}
else
{
    var fotoObj = JsonSerializer.Deserialize<JsonElement>(addFotoResult);
    var fotoId1 = fotoObj.GetProperty("id").GetInt32();
    Console.WriteLine($"Foto 1 adicionada com ID: {fotoId1}");

    // Adicionar segunda foto
    Console.WriteLine("\n=== 6. Adicionando segunda foto à galeria ===");
    var fotoData2 = new
    {
        idGaleriaFotos = galeriaId,
        url = "https://res.cloudinary.com/demo/image/upload/sample2.jpg",
        nomeArquivo = "sample2.jpg",
        legenda = "Foto de teste 2",
        tamanho = 200000,
        ordem = 2
    };

    var addFoto2Response = await client.PostAsJsonAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/adicionar-existente", fotoData2);
    var addFoto2Result = await addFoto2Response.Content.ReadAsStringAsync();
    Console.WriteLine($"Status: {addFoto2Response.StatusCode}");
    Console.WriteLine($"Response: {addFoto2Result}");

    if (addFoto2Response.IsSuccessStatusCode)
    {
        var foto2Obj = JsonSerializer.Deserialize<JsonElement>(addFoto2Result);
        var fotoId2 = foto2Obj.GetProperty("id").GetInt32();
        Console.WriteLine($"Foto 2 adicionada com ID: {fotoId2}");

        // Listar fotos da galeria
        Console.WriteLine("\n=== 7. Listando fotos da galeria ===");
        var listFotosResponse = await client.GetAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos");
        var listFotosResult = await listFotosResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {listFotosResponse.StatusCode}");
        Console.WriteLine($"Response: {listFotosResult}");

        // Definir foto principal
        Console.WriteLine("\n=== 8. Definindo foto 1 como principal ===");
        var setPrincipalResponse = await client.PutAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/{fotoId1}/definir-principal", null);
        var setPrincipalResult = await setPrincipalResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {setPrincipalResponse.StatusCode}");
        Console.WriteLine($"Response: {setPrincipalResult}");

        // Buscar foto principal
        Console.WriteLine("\n=== 9. Buscando foto principal da galeria ===");
        var getPrincipalResponse = await client.GetAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/principal");
        var getPrincipalResult = await getPrincipalResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {getPrincipalResponse.StatusCode}");
        Console.WriteLine($"Response: {getPrincipalResult}");

        // Alterar foto principal
        Console.WriteLine("\n=== 10. Alterando foto principal para foto 2 ===");
        var setPrincipal2Response = await client.PutAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/{fotoId2}/definir-principal", null);
        var setPrincipal2Result = await setPrincipal2Response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {setPrincipal2Response.StatusCode}");
        Console.WriteLine($"Response: {setPrincipal2Result}");

        // Verificar novamente foto principal
        Console.WriteLine("\n=== 11. Verificando nova foto principal ===");
        var getPrincipal2Response = await client.GetAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/principal");
        var getPrincipal2Result = await getPrincipal2Response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {getPrincipal2Response.StatusCode}");
        Console.WriteLine($"Response: {getPrincipal2Result}");

        // Atualizar galeria
        Console.WriteLine("\n=== 12. Atualizando galeria ===");
        var updateGaleriaData = new
        {
            titulo = "Galeria de Teste - Notícias (Atualizada)",
            descricao = "Descrição atualizada",
            idReferencia = 1,
            idRegistroRelacionado = 1
        };

        var updateGaleriaResponse = await client.PutAsJsonAsync($"{baseUrl}/GaleriaFotos/{galeriaId}", updateGaleriaData);
        var updateGaleriaResult = await updateGaleriaResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {updateGaleriaResponse.StatusCode}");
        Console.WriteLine($"Response: {updateGaleriaResult}");

        // Deletar uma foto
        Console.WriteLine("\n=== 13. Deletando foto 1 ===");
        var deleteFotoResponse = await client.DeleteAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/{fotoId1}");
        var deleteFotoResult = await deleteFotoResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {deleteFotoResponse.StatusCode}");
        Console.WriteLine($"Response: {deleteFotoResult}");

        // Verificar foto principal após deletar
        Console.WriteLine("\n=== 14. Verificando foto principal após deletar foto 1 ===");
        var getPrincipal3Response = await client.GetAsync($"{baseUrl}/galeria-fotos/{galeriaId}/fotos/principal");
        var getPrincipal3Result = await getPrincipal3Response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {getPrincipal3Response.StatusCode}");
        Console.WriteLine($"Response: {getPrincipal3Result}");
    }
}

// Deletar galeria
Console.WriteLine("\n=== 15. Deletando galeria (cascade delete de fotos) ===");
var deleteGaleriaResponse = await client.DeleteAsync($"{baseUrl}/GaleriaFotos/{galeriaId}");
var deleteGaleriaResult = await deleteGaleriaResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {deleteGaleriaResponse.StatusCode}");
Console.WriteLine($"Response: {deleteGaleriaResult}");

// Buscar por referência
Console.WriteLine("\n=== 16. Buscando galerias por IdReferencia (1 - Notícias) ===");
var getByRefResponse = await client.GetAsync($"{baseUrl}/GaleriaFotos/por-referencia/1");
var getByRefResult = await getByRefResponse.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {getByRefResponse.StatusCode}");
Console.WriteLine($"Response: {getByRefResult}");

Console.WriteLine("\n=== TESTES CONCLUÍDOS ===");
return 0;
