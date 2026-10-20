namespace FirebirdMigration.Core;

/// <summary>Plano de pré-visualização. Não contém nem executa SQL.</summary>
public enum ProposedRecordAction { Insert, Update, ReviewExtraRow }

public sealed record ProposedRecordChange(
    ProposedRecordAction Action, string Table, string Key,
    IReadOnlyList<string> ChangedColumns, bool RequiresManualReview);

public sealed record RecordSyncPreview(
    IReadOnlyList<ProposedRecordChange> Changes,
    int InsertCount, int UpdateCount, int ExtraTargetCount)
{
    public bool CanExecute => false;
}

/// <summary>
/// Conversão estritamente diagnóstica de diferenças em ações potenciais.
/// Nenhuma exclusão é produzida. Uma linha adicional no alvo requer revisão.
/// </summary>
public static class RecordSyncPlanner
{
    public static RecordSyncPreview Preview(IReadOnlyList<RecordDifference> differences)
    {
        ArgumentNullException.ThrowIfNull(differences);
        var items = new List<ProposedRecordChange>(differences.Count);
        foreach (var diff in differences)
        {
            ArgumentNullException.ThrowIfNull(diff);
            if (string.IsNullOrWhiteSpace(diff.Table) || string.IsNullOrWhiteSpace(diff.Key))
                throw new ArgumentException("Diferença de registro com identificação inválida.");
            var action = diff.Kind switch
            {
                RecordDifferenceKind.MissingInTarget => ProposedRecordAction.Insert,
                RecordDifferenceKind.DifferentValues => ProposedRecordAction.Update,
                RecordDifferenceKind.ExtraInTarget => ProposedRecordAction.ReviewExtraRow,
                _ => throw new ArgumentOutOfRangeException(nameof(differences))
            };
            items.Add(new ProposedRecordChange(action, diff.Table, diff.Key,
                diff.DifferentColumns.ToArray(), action == ProposedRecordAction.ReviewExtraRow));
        }
        var ordered = items.OrderBy(x => x.Table, StringComparer.Ordinal)
            .ThenBy(x => x.Key, StringComparer.Ordinal).ToArray();
        return new RecordSyncPreview(ordered,
            ordered.Count(x => x.Action == ProposedRecordAction.Insert),
            ordered.Count(x => x.Action == ProposedRecordAction.Update),
            ordered.Count(x => x.Action == ProposedRecordAction.ReviewExtraRow));
    }
}
