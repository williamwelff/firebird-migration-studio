namespace FirebirdMigration.Core;

/// <summary>
/// Plano declarativo de redefinição. Nunca executa exclusões ou alterações em bancos.
/// </summary>
public enum ResetMode
{
    EmptyDatabase,
    SchemaOnly
}

public sealed record ResetRequest(
    string DatabaseLabel,
    ResetMode Mode,
    bool IsMaster,
    bool ConfirmedBackup,
    bool ExplicitConfirmation,
    bool HasVerifiedDestination);

public static class ResetSafety
{
    public static IReadOnlyList<string> Validate(ResetRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.DatabaseLabel))
            errors.Add("Selecione um banco para a operação.");
        if (request.IsMaster)
            errors.Add("A redefinição do banco master está bloqueada.");
        if (!request.ConfirmedBackup)
            errors.Add("É obrigatório confirmar um backup recuperável.");
        if (!request.HasVerifiedDestination)
            errors.Add("É obrigatório selecionar e verificar um destino diferente do banco atual.");
        if (!request.ExplicitConfirmation)
            errors.Add("É necessária uma confirmação explícita antes da redefinição.");
        return errors;
    }
}
