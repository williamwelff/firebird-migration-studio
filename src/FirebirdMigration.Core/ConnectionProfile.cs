namespace FirebirdMigration.Core;

public sealed record ConnectionProfile(
    string Label,
    string Host,
    int Port,
    string DatabasePath,
    string Username)
{
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Label)) errors.Add("Informe o nome da conexão.");
        if (string.IsNullOrWhiteSpace(Host)) errors.Add("Informe o servidor.");
        if (Port is < 1 or > 65535) errors.Add("A porta deve estar entre 1 e 65535.");
        if (string.IsNullOrWhiteSpace(DatabasePath)) errors.Add("Informe o caminho ou alias do banco.");
        if (string.IsNullOrWhiteSpace(Username)) errors.Add("Informe o usuário.");
        return errors;
    }
}

public static class TargetProtection
{
    public static IReadOnlyList<string> ValidateDistinctEndpoints(
        ConnectionProfile master,
        ConnectionProfile target)
    {
        var errors = master.Validate().Concat(target.Validate()).ToList();
        if (master.Host.Equals(target.Host, StringComparison.OrdinalIgnoreCase)
            && master.Port == target.Port
            && master.DatabasePath.Equals(target.DatabasePath, StringComparison.OrdinalIgnoreCase))
            errors.Add("O master e o alvo não podem apontar para o mesmo banco.");
        return errors;
    }
}
