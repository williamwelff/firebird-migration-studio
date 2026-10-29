using FirebirdMigration.Core;

var host = Environment.GetEnvironmentVariable("FB_TEST_HOST") ?? "localhost";
var port = int.TryParse(Environment.GetEnvironmentVariable("FB_TEST_PORT"), out var configuredPort)
    ? configuredPort : 3050;
var database = Environment.GetEnvironmentVariable("FB_TEST_DATABASE") ?? "integration.fdb";
var password = Environment.GetEnvironmentVariable("FB_TEST_PASSWORD");
if (string.IsNullOrWhiteSpace(password))
    throw new InvalidOperationException("FB_TEST_PASSWORD obrigatório: somente banco descartável de teste.");

var profile = new ConnectionProfile("Integração descartável", host, port, database, "SYSDBA");
var probe = await FirebirdConnectionProbe.TestAsync(profile, password);
Check(probe.Success, "Conexão real Firebird falhou. Confira os logs do serviço, sem divulgar credenciais.");

var first = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "CLIENTES", new[] { "ID" }, null, 2);
Check(first.Count == 2, "Primeira página deveria conter dois clientes.");
Check(Convert.ToInt32(first[0]["ID"]) == 1 && Convert.ToInt32(first[1]["ID"]) == 2,
    "Ordem da chave simples divergente.");

var second = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "CLIENTES", new[] { "ID" }, new object[] { first[^1]["ID"]! }, 2);
Check(second.Count == 1 && Convert.ToInt32(second[0]["ID"]) == 3,
    "Cursor deve ler o terceiro registro sem repetições.");

var final = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "CLIENTES", new[] { "ID" }, new object[] { second[^1]["ID"]! }, 2);
Check(final.Count == 0, "Última página deve estar vazia.");

var compound = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "PEDIDOS", new[] { "FILIAL", "ID" }, null, 2);
Check(compound.Count == 2, "Primeira página da chave composta.");
var nextCompound = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "PEDIDOS", new[] { "FILIAL", "ID" },
    new object[] { compound[^1]["FILIAL"]!, compound[^1]["ID"]! }, 2);
Check(nextCompound.Count == 1 && Convert.ToInt32(nextCompound[0]["FILIAL"]) == 2,
    "Cursor composto deve atravessar a mudança de filial.");

bool blocked = false;
try
{
    await RecordPageReader.ReadKeysetPageAsync(
        profile, password, "CLIENTES", new[] { "NOME" }, null, 2);
}
catch (InvalidOperationException) { blocked = true; }
Check(blocked, "Ordenação por coluna não PK deveria ser bloqueada.");

var documents = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "DOCUMENTOS", new[] { "ID" }, null, 2);
Check(documents.Count == 2, "Duas linhas com texto e BLOB.");
Check(Convert.ToString(documents[0]["TEXTO"]) == "Texto UTF8", "Leitura textual incorreta.");
Check(documents[0]["CONTEUDO"] is not null, "BLOB de teste não deveria ser nulo.");
Check(documents[1]["CONTEUDO"] is null, "BLOB NULL precisa ser preservado.");
var documentNext = await RecordPageReader.ReadKeysetPageAsync(
    profile, password, "DOCUMENTOS", new[] { "ID" },
    new object[] { documents[^1]["ID"]! }, 2);
Check(documentNext.Count == 0, "Cursor deve terminar na tabela com BLOB.");
Console.WriteLine("OK: conexão, cursor simples/composto, fim de página, chave inválida e BLOB/texto.");
Console.WriteLine("Servidor: " + probe.ServerVersion);

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
