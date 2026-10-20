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
var masterSchema = new DatabaseSchema(new[]
{
    new ColumnDefinition("CLIENTES", "ID", 0, "8", 4, 0, true),
    new ColumnDefinition("CLIENTES", "NOME", 1, "37", 50, 0, false)
});
var targetSchema = new DatabaseSchema(new[]
{
    new ColumnDefinition("CLIENTES", "ID", 0, "8", 4, 0, true),
    new ColumnDefinition("CLIENTES", "NOME", 1, "37", 30, 0, false),
    new ColumnDefinition("CLIENTES", "LEGADO", 2, "37", 10, 0, false)
});
var differences = SchemaComparer.Compare(masterSchema, targetSchema);
Assert(differences.Count == 2, "Detectar duas diferenças");
Assert(differences.Any(x => x.Kind == SchemaDifferenceKind.DifferentDefinition && x.Column == "NOME"), "Detectar mudança de definição");
Assert(differences.Any(x => x.Kind == SchemaDifferenceKind.ExtraInTarget && x.Column == "LEGADO"), "Detectar campo extra");
Assert(SchemaComparer.Compare(masterSchema, masterSchema).Count == 0, "Esquemas idênticos");
var withCharacters = new DatabaseSchema(new[]
{
    new ColumnDefinition("CLIENTES", "NOME", 0, "37", 120, 0, false,
        CharacterLength: 30, SubType: 0, CharacterSet: 4, Collation: 0)
});
var withDifferentCharacterLength = new DatabaseSchema(new[]
{
    new ColumnDefinition("CLIENTES", "NOME", 0, "37", 120, 0, false,
        CharacterLength: 40, SubType: 0, CharacterSet: 4, Collation: 0)
});
Assert(SchemaComparer.Compare(withCharacters, withDifferentCharacterLength).Count == 1,
    "Comprimento em caracteres deve ser comparado, mesmo com bytes iguais");
var withDifferentCharset = new DatabaseSchema(new[]
{
    new ColumnDefinition("CLIENTES", "NOME", 0, "37", 120, 0, false,
        CharacterLength: 30, SubType: 0, CharacterSet: 3, Collation: 0)
});
Assert(SchemaComparer.Compare(withCharacters, withDifferentCharset).Count == 1,
    "Detectar charset diferente");
var structuralMaster = new StructuralSnapshot(
    new[] { new IndexDefinition("CLIENTES", "IDX_NOME", false, false, new[] { "NOME" }) },
    new[] { new KeyDefinition("CLIENTES", "PK_CLIENTES", "PRIMARY KEY", new[] { "ID" }, null) });
var structuralTarget = new StructuralSnapshot(
    new[] { new IndexDefinition("CLIENTES", "IDX_NOME", false, true, new[] { "NOME" }) },
    Array.Empty<KeyDefinition>());
var structuralDiff = StructuralComparer.Compare(structuralMaster, structuralTarget);
Assert(structuralDiff.Count == 2, "Duas diferenças de estrutura");
Assert(structuralDiff.Any(x => x.ObjectType == "Índice" && x.Kind == "Definição diferente"), "Direção do índice");
Assert(structuralDiff.Any(x => x.ObjectType == "Restrição" && x.Kind == "Ausente no alvo"), "PK ausente");
Assert(StructuralComparer.Compare(structuralMaster, structuralMaster).Count == 0, "Estruturas idênticas");
Console.WriteLine("23 verificações locais de perfis, versões, reset e metadados concluídas.");
static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
