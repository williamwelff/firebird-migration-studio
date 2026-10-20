using System.Text.RegularExpressions;

namespace FirebirdMigration.Core;

/// <summary>
/// Gera somente SELECT por cursor lexicográfico com parâmetros.
/// Não executa SQL, não aplica alterações e não valida compatibilidade de tipos.
/// </summary>
public static class KeysetQueryBuilder
{
    public const int MaximumPageSize = 1000;
    private static readonly Regex SafeName = new(
        @"\A[A-Z_][A-Z0-9_$]*\z", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static string Build(string table, IReadOnlyList<string> keyColumns, int pageSize,
        bool hasCursor)
    {
        if (pageSize is < 1 or > MaximumPageSize)
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (!IsSafe(table) || keyColumns is null || keyColumns.Count == 0 ||
            keyColumns.Any(x => !IsSafe(x)) ||
            keyColumns.Distinct(StringComparer.Ordinal).Count() != keyColumns.Count)
            throw new ArgumentException("Identificadores SQL não permitidos.");

        var quoted = keyColumns.Select(x => "\"" + x + "\"").ToArray();
        var conditions = new List<string>();
        if (hasCursor)
        {
            // PK composta: (A > :k0) OR (A = :k0 AND B > :k1) ...
            for (var i = 0; i < quoted.Length; i++)
            {
                var conjuncts = new List<string>();
                for (var j = 0; j < i; j++)
                    conjuncts.Add($"{quoted[j]} = @k{j}");
                conjuncts.Add($"{quoted[i]} > @k{i}");
                conditions.Add("(" + string.Join(" AND ", conjuncts) + ")");
            }
        }
        var where = hasCursor ? " WHERE " + string.Join(" OR ", conditions) : "";
        return $"SELECT FIRST {pageSize} * FROM \"{table}\"{where} ORDER BY " +
            string.Join(", ", quoted);
    }

    private static bool IsSafe(string? value) =>
        value is not null && SafeName.IsMatch(value);
}
