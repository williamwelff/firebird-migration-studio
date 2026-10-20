using FirebirdSql.Data.FirebirdClient;

namespace FirebirdMigration.Core;

public sealed record ColumnDefinition(string Table, string Name, int Position, string FieldType,
    int? Length, int? Scale, bool NotNull);

public sealed record DatabaseSchema(IReadOnlyList<ColumnDefinition> Columns);

public enum SchemaDifferenceKind { MissingInTarget, ExtraInTarget, DifferentDefinition }

public sealed record SchemaDifference(SchemaDifferenceKind Kind, string Table, string Column,
    ColumnDefinition? Master, ColumnDefinition? Target);

/// <summary>Comparação em memória: não executa SQL nem altera bancos.</summary>
public static class SchemaComparer
{
    public static IReadOnlyList<SchemaDifference> Compare(DatabaseSchema master, DatabaseSchema target)
    {
        static Dictionary<string, ColumnDefinition> Index(DatabaseSchema schema) =>
            schema.Columns.ToDictionary(x => x.Table + "\u001f" + x.Name, StringComparer.Ordinal);
        var left = Index(master);
        var right = Index(target);
        var differences = new List<SchemaDifference>();
        foreach (var key in left.Keys.Union(right.Keys).OrderBy(x => x, StringComparer.Ordinal))
        {
            left.TryGetValue(key, out var a);
            right.TryGetValue(key, out var b);
            if (a is null && b is not null)
                differences.Add(new(SchemaDifferenceKind.ExtraInTarget, b.Table, b.Name, null, b));
            else if (a is not null && b is null)
                differences.Add(new(SchemaDifferenceKind.MissingInTarget, a.Table, a.Name, a, null));
            else if (a is not null && b is not null &&
                (a.Position != b.Position || a.FieldType != b.FieldType ||
                 a.Length != b.Length || a.Scale != b.Scale || a.NotNull != b.NotNull))
                differences.Add(new(SchemaDifferenceKind.DifferentDefinition, a.Table, a.Name, a, b));
        }
        return differences;
    }
}

/// <summary>
/// Consulta metadados documentados RDB$RELATIONS, RDB$RELATION_FIELDS e RDB$FIELDS.
/// Não faz DDL ou DML. O resultado inicial cobre colunas, não todos os atributos SQL.
/// </summary>
public static class SchemaReader
{
    private const string Sql = """
        SELECT TRIM(rf.RDB$RELATION_NAME),
               TRIM(rf.RDB$FIELD_NAME),
               rf.RDB$FIELD_POSITION,
               f.RDB$FIELD_TYPE,
               f.RDB$FIELD_LENGTH,
               f.RDB$FIELD_SCALE,
               COALESCE(rf.RDB$NULL_FLAG, f.RDB$NULL_FLAG, 0)
        FROM RDB$RELATION_FIELDS rf
        JOIN RDB$RELATIONS r ON r.RDB$RELATION_NAME = rf.RDB$RELATION_NAME
        JOIN RDB$FIELDS f ON f.RDB$FIELD_NAME = rf.RDB$FIELD_SOURCE
        WHERE COALESCE(r.RDB$SYSTEM_FLAG, 0) = 0
          AND r.RDB$VIEW_BLR IS NULL
        ORDER BY rf.RDB$RELATION_NAME, rf.RDB$FIELD_POSITION
        """;

    public static async Task<DatabaseSchema> ReadAsync(ConnectionProfile profile,
        string password, CancellationToken token = default)
    {
        var errors = profile.Validate();
        if (errors.Count != 0) throw new ArgumentException(string.Join(" ", errors), nameof(profile));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Senha obrigatória.", nameof(password));
        var settings = new FbConnectionStringBuilder
        {
            DataSource = profile.Host, Port = profile.Port, Database = profile.DatabasePath,
            UserID = profile.Username, Password = password, Pooling = false, ConnectionTimeout = 8
        };
        await using var connection = new FbConnection(settings.ToString());
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(System.Data.IsolationLevel.Snapshot, token);
        await using var command = connection.CreateCommand();
        command.Transaction = (FbTransaction)transaction;
        command.CommandText = Sql;
        await using var reader = await command.ExecuteReaderAsync(token);
        var columns = new List<ColumnDefinition>();
        while (await reader.ReadAsync(token))
        {
            columns.Add(new(
                reader.GetString(0).TrimEnd(),
                reader.GetString(1).TrimEnd(),
                Convert.ToInt32(reader.GetValue(2)),
                Convert.ToString(reader.GetValue(3), System.Globalization.CultureInfo.InvariantCulture) ?? "",
                reader.IsDBNull(4) ? null : Convert.ToInt32(reader.GetValue(4)),
                reader.IsDBNull(5) ? null : Convert.ToInt32(reader.GetValue(5)),
                Convert.ToInt32(reader.GetValue(6)) != 0));
        }
        await transaction.CommitAsync(token);
        return new DatabaseSchema(columns);
    }
}
