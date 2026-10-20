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
var fkMaster = new StructuralSnapshot(Array.Empty<IndexDefinition>(),
    new[] { new KeyDefinition("PEDIDOS", "FK_CLIENTE", "FOREIGN KEY", new[] { "CLIENTE_ID" },
        "CLIENTES", "NO ACTION", "CASCADE") });
var fkTarget = new StructuralSnapshot(Array.Empty<IndexDefinition>(),
    new[] { new KeyDefinition("PEDIDOS", "FK_CLIENTE", "FOREIGN KEY", new[] { "CLIENTE_ID" },
        "CLIENTES", "NO ACTION", "SET NULL") });
Assert(StructuralComparer.Compare(fkMaster, fkTarget).Count == 1,
    "Detectar diferença na regra ON DELETE");
Assert(StructuralComparer.Compare(fkMaster, fkMaster).Count == 0,
    "Não sinalizar FK idêntica");
var recordMaster = new RecordSnapshot("CLIENTES", new[] { "ID" }, new IReadOnlyDictionary<string, object?>[]
{
    new Dictionary<string, object?> { ["ID"] = 1, ["NOME"] = "Ana", ["FOTO"] = new byte[] { 1, 2 } },
    new Dictionary<string, object?> { ["ID"] = 2, ["NOME"] = null }
});
var recordTarget = new RecordSnapshot("CLIENTES", new[] { "ID" }, new IReadOnlyDictionary<string, object?>[]
{
    new Dictionary<string, object?> { ["ID"] = 1, ["NOME"] = "Ana Maria", ["FOTO"] = new byte[] { 1, 2 } },
    new Dictionary<string, object?> { ["ID"] = 3, ["NOME"] = "Carlos" }
});
var recordDiff = RecordComparer.Compare(recordMaster, recordTarget);
Assert(recordDiff.Count == 3, "Inserção, remoção e atualização previstas");
Assert(recordDiff.Any(x => x.Kind == RecordDifferenceKind.MissingInTarget), "Registro ausente no alvo");
Assert(recordDiff.Any(x => x.Kind == RecordDifferenceKind.ExtraInTarget), "Registro adicional no alvo");
Assert(recordDiff.Single(x => x.Kind == RecordDifferenceKind.DifferentValues).DifferentColumns.SequenceEqual(new[] { "NOME" }),
    "Comparação de BLOBs byte a byte");
Assert(RecordComparer.Compare(recordMaster, recordMaster).Count == 0, "Snapshots idênticos");
var duplicated = new RecordSnapshot("CLIENTES", new[] { "ID" }, new IReadOnlyDictionary<string, object?>[]
{
    new Dictionary<string, object?> { ["ID"] = 1 },
    new Dictionary<string, object?> { ["ID"] = 1 }
});
AssertThrows(() => RecordComparer.Compare(duplicated, recordMaster), "Bloquear chaves duplicadas");
var nullKey = new RecordSnapshot("CLIENTES", new[] { "ID" }, new IReadOnlyDictionary<string, object?>[]
{
    new Dictionary<string, object?> { ["ID"] = null }
});
AssertThrows(() => RecordComparer.Compare(nullKey, recordMaster), "Bloquear chave nula");
var preview = RecordSyncPlanner.Preview(recordDiff);
Assert(preview.InsertCount == 1, "Prévia de inserção");
Assert(preview.UpdateCount == 1, "Prévia de atualização");
Assert(preview.ExtraTargetCount == 1, "Linha excedente exige revisão");
Assert(preview.Changes.Single(x => x.Action == ProposedRecordAction.ReviewExtraRow).RequiresManualReview,
    "Registro adicional deve exigir revisão");
Assert(!preview.CanExecute, "A prévia nunca pode executar SQL");
Assert(RecordSyncPlanner.Preview(Array.Empty<RecordDifference>()).Changes.Count == 0,
    "Plano vazio é permitido");
var query = RecordPageReader.BuildQuery("CLIENTES", new[] { "ID" }, 100);
Assert(query.Contains("FIRST 100 SKIP @skip", StringComparison.Ordinal), "Limite e paginação");
Assert(query.Contains("ORDER BY " + (char)34 + "ID" + (char)34, StringComparison.Ordinal), "Ordenação por chave");
AssertThrows(() => RecordPageReader.BuildQuery("CLIENTES; DROP TABLE CLIENTES", new[] { "ID" }, 100),
    "Bloquear nome de tabela inseguro");
AssertThrows(() => RecordPageReader.BuildQuery("CLIENTES", new[] { "ID DESC" }, 100),
    "Bloquear injeção em colunas de chave");
AssertThrows(() => RecordPageReader.BuildQuery("CLIENTES", new[] { "ID" }, 0),
    "Bloquear tamanho de página zero");
AssertThrows(() => RecordPageReader.BuildQuery("CLIENTES", new[] { "ID" }, 1001),
    "Bloquear tamanho acima do máximo");
Console.WriteLine("44 verificações locais de perfis, versões, reset, metadados e registros concluídas.");
static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static void AssertThrows(Action action, string message)
{
    try { action(); }
    catch (ArgumentException) { return; }
    throw new InvalidOperationException(message);
}
