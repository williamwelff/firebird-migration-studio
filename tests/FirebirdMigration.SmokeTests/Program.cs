using FirebirdMigration.Core;

var master = new ConnectionProfile("Master", "localhost", 3050, "database25", "SYSDBA");
var target = new ConnectionProfile("Alvo", "localhost", 3055, "database50", "SYSDBA");
Assert(TargetProtection.ValidateDistinctEndpoints(master, target).Count == 0, "Destinos distintos");
Assert(TargetProtection.ValidateDistinctEndpoints(master, master).Count > 0, "Bloquear origem igual ao alvo");
Assert(new ConnectionProfile("", "", 0, "", "").Validate().Count == 5, "Campos obrigatórios");
Assert(MigrationVersions.Validate("2.5", "5.0").Count == 0, "2.5 para 5.0");
Assert(MigrationVersions.Validate("3.0", "5.0").Count == 0, "3.0 para 5.0");
Assert(MigrationVersions.Validate("4.0", "5.0").Count == 0, "4.0 para 5.0");
Assert(MigrationVersions.Validate("3.5", "5.0").Count > 0, "3.5 inválido");
Assert(MigrationVersions.Validate("4.5", "5.0").Count > 0, "4.5 inválido");
Assert(MigrationVersions.Validate("2.5", "4.0").Count > 0, "Destino inválido");
Console.WriteLine("9 verificações de perfis e versões concluídas.");
static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
