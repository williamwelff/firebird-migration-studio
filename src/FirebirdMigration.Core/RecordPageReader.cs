using System.Data;
using System.Text.RegularExpressions;
using FirebirdSql.Data.FirebirdClient;

namespace FirebirdMigration.Core;

/// <summary>
/// Leitura paginada e exclusivamente de consulta para diagnósticos.
/// Cada página ocupa memória limitada, mas não é um comparador incremental completo.
/// </summary>
public static class RecordPageReader
{
    public const int MaximumPageSize = 1000;
    private static readonly Regex Identifier = new(@"^[A-Z_][A-Z0-9_$]*$", RegexOptions.CultureInvariant);

    public static string BuildQuery(string table, IReadOnlyList<string> keyColumns, int pageSize)
    {
        if (pageSize is < 1 or > MaximumPageSize)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Tamanho de página não permitido.");
        ValidateIdentifiers(table, keyColumns);
        var ordering = string.Join(", ", keyColumns.Select(QuoteIdentifier));
        // FIRST/SKIP é suportado nas versões Firebird suportadas. Ordenação por chave estável.
        return $"SELECT FIRST {pageSize} SKIP @skip * FROM {QuoteIdentifier(table)} ORDER BY {ordering}";
    }

    private static string QuoteIdentifier(string identifier) => "\"" + identifier + "\"";

    private static void ValidateIdentifiers(string table, IReadOnlyList<string> keys)
    {
        if (string.IsNullOrEmpty(table) || !Identifier.IsMatch(table))
            throw new ArgumentException("Nome de tabela não homologado.", nameof(table));
        if (keys is null || keys.Count == 0 || keys.Count != keys.Distinct(StringComparer.Ordinal).Count() ||
            keys.Any(x => string.IsNullOrEmpty(x) || !Identifier.IsMatch(x)))
            throw new ArgumentException("Chaves inválidas; somente identificadores SQL simples são aceitos.", nameof(keys));
    }

    public static async Task<IReadOnlyList<IReadOnlyDictionary<string, object?>>> ReadPageAsync(
        ConnectionProfile profile, string password, string table,
        IReadOnlyList<string> keyColumns, int skip, int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (profile.Validate().Count != 0) throw new ArgumentException("Perfil inválido.", nameof(profile));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Senha obrigatória.", nameof(password));
        if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));
        var sql = BuildQuery(table, keyColumns, pageSize);
        var settings = new FbConnectionStringBuilder
        {
            DataSource = profile.Host, Port = profile.Port, Database = profile.DatabasePath,
            UserID = profile.Username, Password = password, Pooling = false, ConnectionTimeout = 8
        };
        await using var connection = new FbConnection(settings.ToString());
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);
        var declaredKey = await PrimaryKeyValidator.ReadDeclaredAsync(connection, (FbTransaction)transaction, table, cancellationToken);
        PrimaryKeyValidator.ValidateSelection(keyColumns, declaredKey);
        await using var command = connection.CreateCommand();
        command.Transaction = (FbTransaction)transaction;
        command.CommandText = sql;
        command.Parameters.Add("@skip", FbDbType.Integer).Value = skip;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var rows = new List<IReadOnlyDictionary<string, object?>>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var values = new Dictionary<string, object?>(StringComparer.Ordinal);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                object? value = await reader.IsDBNullAsync(i, cancellationToken)
                    ? null : reader.GetValue(i);
                if (value is byte[] blob) value = blob.ToArray();
                values.Add(reader.GetName(i), value);
            }
            rows.Add(values);
        }
        await transaction.CommitAsync(cancellationToken);
        return rows;
    }
}
