namespace FirebirdMigration.Core;

/// <summary>
/// Catálogo das séries oficiais admitidas como origem do assistente de migração.
/// A lista representa suporte planejado, não homologação de migração real.
/// </summary>
public static class MigrationVersions
{
    public static readonly IReadOnlyList<string> AllowedSourceVersions =
        Array.AsReadOnly(new[] { "2.5", "3.0", "4.0" });

    public const string TargetVersion = "5.0";

    public static bool IsAllowedSource(string? version) =>
        version is not null && AllowedSourceVersions.Contains(version.Trim());

    public static IReadOnlyList<string> Validate(string? sourceVersion, string? targetVersion)
    {
        var errors = new List<string>();
        if (!IsAllowedSource(sourceVersion))
            errors.Add("Versão de origem inválida. Utilize Firebird 2.5, 3.0 ou 4.0.");
        if (targetVersion?.Trim() != TargetVersion)
            errors.Add("Nesta etapa, a versão de destino deve ser Firebird 5.0.");
        return errors;
    }
}
