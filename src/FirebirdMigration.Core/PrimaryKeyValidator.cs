using FirebirdSql.Data.FirebirdClient;

namespace FirebirdMigration.Core;

/// <summary>
/// Verifica se a seleção corresponde exatamente à PRIMARY KEY declarada pelo Firebird.
/// Não autoriza sincronização ou escrita.
/// </summary>
public static class PrimaryKeyValidator
{
    public static void ValidateSelection(IReadOnlyList<string> selected, IReadOnlyList<string> declared)
    {
        if (selected is null || declared is null || declared.Count == 0 ||
            selected.Count != declared.Count ||
            !selected.SequenceEqual(declared, StringComparer.Ordinal) ||
            selected.Any(string.IsNullOrWhiteSpace))
            throw new InvalidOperationException("A chave informada não corresponde à chave primária declarada.");
    }

    public static async Task<IReadOnlyList<string>> ReadDeclaredAsync(
        FbConnection connection, FbTransaction transaction, string table,
        CancellationToken token = default)
    {
        const string sql = """
            SELECT TRIM(s.RDB$FIELD_NAME)
            FROM RDB$RELATION_CONSTRAINTS rc
            JOIN RDB$INDEX_SEGMENTS s ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
            WHERE rc.RDB$RELATION_NAME = @table
              AND rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
            ORDER BY s.RDB$FIELD_POSITION
            """;
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sql;
        cmd.Parameters.Add("@table", FbDbType.Char).Value = table;
        var keys = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(token);
        while (await reader.ReadAsync(token))
            keys.Add(reader.GetString(0).TrimEnd());
        return keys;
    }
}
