using Microsoft.AspNetCore.Mvc;
using Victor;
using Victor.models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

// FUNCIONALIDADE 1: Cadastrar Consumo
app.MapPost("/api/consumo/cadastrar", ([FromBody] ConsumoRequest request, [FromServices] AppDataContext ctx) =>
{
    // Validações obrigatórias
    if (request.Mes < 1 || request.Mes > 12)
    {
        return Results.Conflict("Mês deve estar entre 1 e 12.");
    }
    if (request.Ano < 2000)
    {
        return Results.Conflict("Ano deve ser maior ou igual a 2000.");
    }
    if (request.M3Consumidos <= 0)
    {
        return Results.Conflict("m3Consumidos deve ser maior que 0.");
    }

    var consumoExistente = ctx.Consumos.FirstOrDefault(c => c.Cpf == request.Cpf && c.Mes == request.Mes && c.Ano == request.Ano);
    if (consumoExistente is not null)
    {
        return Results.Conflict("Já existe um consumo para este CPF, mês e ano.");
    }

    // Cálculos
    var consumo = new Consumo
    {
        Cpf = request.Cpf,
        Mes = request.Mes,
        Ano = request.Ano,
        M3Consumidos = request.M3Consumidos,
        Bandeira = request.Bandeira,
        PossuiEsgoto = request.PossuiEsgoto
    };

    // 1) Consumo mínimo faturável
    consumo.ConsumoFaturado = consumo.M3Consumidos < 10 ? 10 : consumo.M3Consumidos;

    // 2) Tarifa por faixa
    if (consumo.ConsumoFaturado <= 10) consumo.Tarifa = 2.50;
    else if (consumo.ConsumoFaturado <= 20) consumo.Tarifa = 3.50;
    else if (consumo.ConsumoFaturado <= 50) consumo.Tarifa = 5.00;
    else consumo.Tarifa = 6.50;
    
    consumo.ValorAgua = consumo.ConsumoFaturado * consumo.Tarifa;

    // 3) Bandeira hídrica
    double percentualBandeira = consumo.Bandeira.ToLower() switch
    {
        "amarela" => 0.10,
        "vermelha" => 0.20,
        _ => 0 // Verde ou qualquer outro valor
    };
    consumo.AdicionalBandeira = consumo.ValorAgua * percentualBandeira;

    // 4) Taxa de esgoto
    consumo.TaxaEsgoto = consumo.PossuiEsgoto ? (consumo.ValorAgua + consumo.AdicionalBandeira) * 0.80 : 0;

    // 5) Total geral
    consumo.Total = consumo.ValorAgua + consumo.AdicionalBandeira + consumo.TaxaEsgoto;

    ctx.Consumos.Add(consumo);
    ctx.SaveChanges();
    return Results.Created($"/api/consumo/buscar/{consumo.Cpf}/{consumo.Mes}/{consumo.Ano}", consumo);
});

// FUNCIONALIDADE 2: Listar Consumos
app.MapGet("/api/consumo/listar", ([FromServices] AppDataContext ctx) =>
{
    var consumos = ctx.Consumos.ToList();
    if (consumos.Count == 0)
    {
        return Results.NotFound("Nenhum consumo encontrado.");
    }
    return Results.Ok(consumos);
});

// FUNCIONALIDADE 3: Buscar por CPF, Mês e Ano
app.MapGet("/api/consumo/buscar/{cpf}/{mes}/{ano}", ([FromRoute] string cpf, [FromRoute] int mes, [FromRoute] int ano, [FromServices] AppDataContext ctx) =>
{
    var consumo = ctx.Consumos.FirstOrDefault(c => c.Cpf == cpf && c.Mes == mes && c.Ano == ano);
    if (consumo is null)
    {
        return Results.NotFound("Consumo não encontrado para os parâmetros informados.");
    }
    return Results.Ok(consumo);

});

// FUNCIONALIDADE 4: Remover Leitura
app.MapDelete("/api/consumo/remover/{cpf}/{mes}/{ano}", ([FromRoute] string cpf, [FromRoute] int mes, [FromRoute] int ano, [FromServices] AppDataContext ctx) =>
{
    var consumo = ctx.Consumos.FirstOrDefault(c => c.Cpf == cpf && c.Mes == mes && c.Ano == ano);
    if (consumo is null)
    {
        return Results.NotFound("Consumo não encontrado para os parâmetros informados.");
    }
    ctx.Consumos.Remove(consumo);
    ctx.SaveChanges();

    return Results.Ok("Consumo removido com sucesso.");

});

// FUNCIONALIDADE 5: Total Geral de Faturas
app.MapGet("/api/consumo/total-geral", ([FromServices] AppDataContext ctx) =>
{
    var consumos = ctx.Consumos.ToList();
    if (consumos.Count == 0)
    {
        return Results.NotFound("Nenhum consumo encontrado.");
    }
    var totalGeral = consumos.Sum(c => c.Total);

    return Results.Ok(new { TotalGeral = Math.Round(totalGeral, 2) });
});



app.Run();

// Classe para representar a requisição de cadastro, contendo apenas os campos de entrada.
public class ConsumoRequest
{
    public string Cpf { get; set; } = string.Empty;
    public int Mes { get; set; }
    public int Ano { get; set; }
    public double M3Consumidos { get; set; }
    public string Bandeira { get; set; } = string.Empty;
    public bool PossuiEsgoto { get; set; }
}