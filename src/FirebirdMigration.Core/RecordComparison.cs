namespace FirebirdMigration.Core;

/// <summary>Snapshot de registros para testes e futura integração. Valores devem ser escalares imutáveis.</summary>
public sealed record RecordSnapshot(string Table, IReadOnlyList<string> KeyColumns,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows);

public enum RecordDifferenceKind { MissingInTarget, ExtraInTarget, DifferentValues }

public sealed record RecordDifference(RecordDifferenceKind Kind, string Table, string Key,
    IReadOnlyList<string> DifferentColumns);

/// <summary>
/// Comparador determinístico e somente leitura de snapshots já carregados.
/// Nunca gera SQL nem modifica bancos. Não adequado ainda a bancos de grande porte.
/// </summary>
public static class RecordComparer
{
    public static IReadOnlyList<RecordDifference> Compare(RecordSnapshot master, RecordSnapshot target)
    {
        ArgumentNullException.ThrowIfNull(master);
        ArgumentNullException.ThrowIfNull(target);
        if (!string.Equals(master.Table, target.Table, StringComparison.Ordinal))
            throw new ArgumentException("As tabelas precisam corresponder.");
        if (master.KeyColumns.Count == 0 || master.KeyColumns.Any(string.IsNullOrWhiteSpace) ||
            master.KeyColumns.Distinct(StringComparer.Ordinal).Count() != master.KeyColumns.Count ||
            !master.KeyColumns.SequenceEqual(target.KeyColumns, StringComparer.Ordinal))
            throw new ArgumentException("As chaves devem ser não vazias, únicas e idênticas.");
        var left = Index(master);
        var right = Index(target);
        var output = new List<RecordDifference>();
        foreach (var key in left.Keys.Union(right.Keys).OrderBy(x => x, StringComparer.Ordinal))
        {
            var hasMaster = left.TryGetValue(key, out var m);
            var hasTarget = right.TryGetValue(key, out var t);
            if (hasMaster && !hasTarget)
                output.Add(new(RecordDifferenceKind.MissingInTarget, master.Table, key, Array.Empty<string>()));
            else if (!hasMaster && hasTarget)
                output.Add(new(RecordDifferenceKind.ExtraInTarget, master.Table, key, Array.Empty<string>()));
            else if (hasMaster && hasTarget)
            {
                var names = m!.Keys.Union(t!.Keys, StringComparer.Ordinal)
                    .Except(master.KeyColumns, StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal).ToArray();
                var mismatches = names.Where(name =>
                    !m.TryGetValue(name, out var a) || !t.TryGetValue(name, out var b) ||
                    !SameValue(a, b)).ToArray();
                if (mismatches.Length != 0)
                    output.Add(new(RecordDifferenceKind.DifferentValues, master.Table, key, mismatches));
            }
        }
        return output;
    }

    private static Dictionary<string, IReadOnlyDictionary<string, object?>> Index(RecordSnapshot snapshot)
    {
        var result = new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.Ordinal);
        foreach (var row in snapshot.Rows)
        {
            if (row is null) throw new ArgumentException("Registro nulo.");
            var parts = new List<string>();
            foreach (var column in snapshot.KeyColumns)
            {
                if (!row.TryGetValue(column, out var value) || value is null || value is DBNull)
                    throw new ArgumentException("Chaves incompletas ou nulas.");
                parts.Add(EncodeValue(value));
            }
            var key = string.Concat(parts.Select(p => p.Length.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + p));
            if (!result.TryAdd(key, row))
                throw new ArgumentException("Chave duplicada detectada; comparação bloqueada.");
        }
        return result;
    }

    private static string EncodeValue(object value)
    {
        // Evita colisões entre valores de tipos diferentes e entre chaves compostas.
        return value switch
        {
            byte[] bytes => "bytes:" + Convert.ToHexString(bytes),
            string text => "string:" + text,
            Guid guid => "guid:" + guid.ToString("D"),
            DateTime dt => "datetime:" + dt.ToBinary().ToString(System.Globalization.CultureInfo.InvariantCulture),
            DateOnly date => "date:" + date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            TimeOnly time => "time:" + time.ToString("HH:mm:ss.fffffff", System.Globalization.CultureInfo.InvariantCulture),
            bool boolean => "bool:" + boolean,
            int or long or short or byte or uint or ulong or ushort or sbyte or decimal or float or double =>
                value.GetType().Name + ":" + Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture),
            _ => throw new NotSupportedException("Tipo de chave não homologado: " + value.GetType().Name)
        };
    }

    private static bool SameValue(object? a, object? b)
    {
        if (a is DBNull) a = null;
        if (b is DBNull) b = null;
        if (a is byte[] bytesA && b is byte[] bytesB)
            return bytesA.AsSpan().SequenceEqual(bytesB);
        return Equals(a, b);
    }
}
