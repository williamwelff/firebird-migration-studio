using FirebirdSql.Data.FirebirdClient;

namespace FirebirdMigration.Core;

/// <summary>Índice e suas colunas ordenadas. Não representa índices por expressão.</summary>
public sealed record IndexDefinition(string Table, string Name, bool Unique, bool Descending,
    IReadOnlyList<string> Columns);

public sealed record KeyDefinition(string Table, string Name, string Kind,
    IReadOnlyList<string> Columns, string? ReferencedTable,
    string? OnUpdate = null, string? OnDelete = null);

public sealed record StructuralSnapshot(IReadOnlyList<IndexDefinition> Indexes,
    IReadOnlyList<KeyDefinition> Keys);

public sealed record StructuralDifference(string ObjectType, string Table, string Name, string Kind);

/// <summary>Comparação pura e sem efeitos colaterais. Não produz comandos SQL.</summary>
public static class StructuralComparer
{
    public static IReadOnlyList<StructuralDifference> Compare(StructuralSnapshot master, StructuralSnapshot target)
    {
        var changes = new List<StructuralDifference>();
        CompareObjects(master.Indexes, target.Indexes, "Índice",
            x => x.Table + "\u001f" + x.Name,
            (a, b) => a.Unique == b.Unique && a.Descending == b.Descending &&
                      a.Columns.SequenceEqual(b.Columns, StringComparer.Ordinal),
            x => (x.Table, x.Name), changes);
        CompareObjects(master.Keys, target.Keys, "Restrição",
            x => x.Table + "\u001f" + x.Name,
            (a, b) => a.Kind == b.Kind && a.ReferencedTable == b.ReferencedTable &&
                      a.OnUpdate == b.OnUpdate && a.OnDelete == b.OnDelete &&
                      a.Columns.SequenceEqual(b.Columns, StringComparer.Ordinal),
            x => (x.Table, x.Name), changes);
        return changes.OrderBy(x => x.ObjectType, StringComparer.Ordinal)
            .ThenBy(x => x.Table, StringComparer.Ordinal)
            .ThenBy(x => x.Name, StringComparer.Ordinal).ToList();
    }

    private static void CompareObjects<T>(IReadOnlyList<T> master, IReadOnlyList<T> target,
        string category, Func<T, string> identity, Func<T, T, bool> equivalent,
        Func<T, (string Table, string Name)> display, List<StructuralDifference> changes)
    {
        var left = master.ToDictionary(identity, StringComparer.Ordinal);
        var right = target.ToDictionary(identity, StringComparer.Ordinal);
        foreach (var key in left.Keys.Union(right.Keys).OrderBy(x => x, StringComparer.Ordinal))
        {
            var inMaster = left.TryGetValue(key, out var a);
            var inTarget = right.TryGetValue(key, out var b);
            if (inMaster && !inTarget)
            {
                var (table, name) = display(a!);
                changes.Add(new(category, table, name, "Ausente no alvo"));
            }
            else if (!inMaster && inTarget)
            {
                var (table, name) = display(b!);
                changes.Add(new(category, table, name, "Adicional no alvo"));
            }
            else if (inMaster && inTarget && !equivalent(a!, b!))
            {
                var (table, name) = display(a!);
                changes.Add(new(category, table, name, "Definição diferente"));
            }
        }
    }
}

/// <summary>
/// Leitor experimental dos catálogos Firebird, apenas para índices de colunas
/// e PK/UNIQUE/FK. Inclui ações referenciais, mas não expressões ou CHECK.
/// </summary>
public static class StructuralReader
{
    private const string IndexSql = """
        SELECT TRIM(i.RDB$RELATION_NAME), TRIM(i.RDB$INDEX_NAME),
               COALESCE(i.RDB$UNIQUE_FLAG, 0), COALESCE(i.RDB$INDEX_TYPE, 0),
               TRIM(s.RDB$FIELD_NAME), s.RDB$FIELD_POSITION
        FROM RDB$INDICES i
        JOIN RDB$RELATIONS r ON r.RDB$RELATION_NAME = i.RDB$RELATION_NAME
        JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = i.RDB$INDEX_NAME
        WHERE COALESCE(r.RDB$SYSTEM_FLAG, 0) = 0
        ORDER BY i.RDB$RELATION_NAME, i.RDB$INDEX_NAME, s.RDB$FIELD_POSITION
        """;

    private const string KeySql = """
        SELECT TRIM(rc.RDB$RELATION_NAME), TRIM(rc.RDB$CONSTRAINT_NAME),
               TRIM(rc.RDB$CONSTRAINT_TYPE), TRIM(s.RDB$FIELD_NAME),
               s.RDB$FIELD_POSITION, TRIM(reftable.RDB$RELATION_NAME),
               TRIM(ref.RDB$UPDATE_RULE), TRIM(ref.RDB$DELETE_RULE)
        FROM RDB$RELATION_CONSTRAINTS rc
        JOIN RDB$RELATIONS r ON r.RDB$RELATION_NAME = rc.RDB$RELATION_NAME
        JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
        LEFT JOIN RDB$REF_CONSTRAINTS ref ON ref.RDB$CONSTRAINT_NAME = rc.RDB$CONSTRAINT_NAME
        LEFT JOIN RDB$RELATION_CONSTRAINTS reftable
               ON reftable.RDB$CONSTRAINT_NAME = ref.RDB$CONST_NAME_UQ
        WHERE COALESCE(r.RDB$SYSTEM_FLAG, 0) = 0
          AND rc.RDB$CONSTRAINT_TYPE IN ('PRIMARY KEY', 'UNIQUE', 'FOREIGN KEY')
        ORDER BY rc.RDB$RELATION_NAME, rc.RDB$CONSTRAINT_NAME, s.RDB$FIELD_POSITION
        """;

    public static async Task<StructuralSnapshot> ReadAsync(
        ConnectionProfile profile, string password, CancellationToken token = default)
    {
        var errors = profile.Validate();
        if (errors.Count > 0) throw new ArgumentException(string.Join(" ", errors), nameof(profile));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Senha obrigatória.", nameof(password));

        var settings = new FbConnectionStringBuilder
        {
            DataSource = profile.Host, Port = profile.Port, Database = profile.DatabasePath,
            UserID = profile.Username, Password = password, Pooling = false, ConnectionTimeout = 8
        };
        await using var connection = new FbConnection(settings.ToString());
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(System.Data.IsolationLevel.Snapshot, token);

        var indexRows = new Dictionary<string, (string Table, string Name, bool Unique, bool Descending, List<string> Columns)>(StringComparer.Ordinal);
        await using (var cmd = connection.CreateCommand())
        {
            cmd.Transaction = (FbTransaction)transaction;
            cmd.CommandText = IndexSql;
            await using var reader = await cmd.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                var table = reader.GetString(0).TrimEnd();
                var name = reader.GetString(1).TrimEnd();
                var key = table + "\u001f" + name;
                if (!indexRows.TryGetValue(key, out var item))
                    item = (table, name, Convert.ToInt32(reader.GetValue(2)) != 0,
                        Convert.ToInt32(reader.GetValue(3)) != 0, new List<string>());
                item.Columns.Add(reader.GetString(4).TrimEnd());
                indexRows[key] = item;
            }
        }

        var keyRows = new Dictionary<string, (string Table, string Name, string Kind, string? Referenced, string? UpdateRule, string? DeleteRule, List<string> Columns)>(StringComparer.Ordinal);
        await using (var cmd = connection.CreateCommand())
        {
            cmd.Transaction = (FbTransaction)transaction;
            cmd.CommandText = KeySql;
            await using var reader = await cmd.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                var table = reader.GetString(0).TrimEnd();
                var name = reader.GetString(1).TrimEnd();
                var key = table + "\u001f" + name;
                if (!keyRows.TryGetValue(key, out var item))
                    item = (table, name, reader.GetString(2).TrimEnd(),
                        reader.IsDBNull(5) ? null : reader.GetString(5).TrimEnd(),
                        reader.IsDBNull(6) ? null : reader.GetString(6).TrimEnd(),
                        reader.IsDBNull(7) ? null : reader.GetString(7).TrimEnd(), new List<string>());
                item.Columns.Add(reader.GetString(3).TrimEnd());
                keyRows[key] = item;
            }
        }

        await transaction.CommitAsync(token);
        return new StructuralSnapshot(
            indexRows.Values.Select(x => new IndexDefinition(x.Table, x.Name, x.Unique, x.Descending, x.Columns)).ToList(),
            keyRows.Values.Select(x => new KeyDefinition(x.Table, x.Name, x.Kind, x.Columns, x.Referenced, x.UpdateRule, x.DeleteRule)).ToList());
    }
}
