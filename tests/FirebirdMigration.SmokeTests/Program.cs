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
Assert(ResetSafety.Validate(new ResetRequest("Master", ResetMode.EmptyDatabase, true, true, true, true)).Count > 0, "Bloqueio do master");
Assert(ResetSafety.Validate(new ResetRequest("Alvo", ResetMode.SchemaOnly, false, false, true, true)).Count > 0, "Backup obrigatório");
Assert(ResetSafety.Validate(new ResetRequest("Alvo", ResetMode.EmptyDatabase, false, true, true, false)).Count > 0, "Destino separado obrigatório");
Assert(ResetSafety.Validate(new ResetRequest("Novo", ResetMode.EmptyDatabase, false, true, true, true)).Count == 0, "Plano seguro com pré-requisitos");
Console.WriteLine("13 verificações de perfis, versões e requisitos de recriação concluídas.");
static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
