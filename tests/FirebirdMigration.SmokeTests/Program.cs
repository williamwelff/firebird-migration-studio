using FirebirdMigration.Core;

var master = new ConnectionProfile("Master", "localhost", 3050, "database25", "SYSDBA");
var target = new ConnectionProfile("Alvo", "localhost", 3055, "database50", "SYSDBA");
Assert(TargetProtection.ValidateDistinctEndpoints(master, target).Count == 0, "Destinos distintos");
Assert(TargetProtection.ValidateDistinctEndpoints(master, master).Count > 0, "Bloquear origem igual ao alvo");
Assert(new ConnectionProfile("", "", 0, "", "").Validate().Count == 5, "Campos obrigatórios");
Console.WriteLine("3 testes de validação de perfis concluídos.");
static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
