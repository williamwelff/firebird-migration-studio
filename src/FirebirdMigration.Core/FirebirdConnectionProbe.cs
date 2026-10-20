using FirebirdSql.Data.FirebirdClient;

namespace FirebirdMigration.Core;

/// <summary>Teste de conexão SOMENTE LEITURA, sem SQL de gravação.</summary>
public static class FirebirdConnectionProbe
{
    public sealed record ProbeResult(bool Success, string? ServerVersion, string Message);

    public static async Task<ProbeResult> TestAsync(
        ConnectionProfile profile, string password, CancellationToken cancellationToken = default)
    {
        var errors = profile.Validate();
        if (errors.Count > 0)
            return new(false, null, string.Join(" ", errors));
        if (string.IsNullOrEmpty(password))
            return new(false, null, "Informe uma senha para testar a conexão.");

        var builder = new FbConnectionStringBuilder
        {
            DataSource = profile.Host,
            Port = profile.Port,
            Database = profile.DatabasePath,
            UserID = profile.Username,
            Password = password,
            Pooling = false,
            ConnectionTimeout = 8
        };
        try
        {
            await using var connection = new FbConnection(builder.ToString());
            await connection.OpenAsync(cancellationToken);
            // Somente metadados oferecidos pela conexão; nenhuma consulta de alteração.
            return new(true, connection.ServerVersion, "Conexão estabelecida. Nenhum dado foi modificado.");
        }
        catch (OperationCanceledException)
        {
            return new(false, null, "Teste de conexão cancelado.");
        }
        catch (Exception e) when (e is FbException or InvalidOperationException or System.Net.Sockets.SocketException)
        {
            // Nunca devolver e.Message: pode revelar detalhes da conexão, caminhos ou credenciais.
            return new(false, null, "Não foi possível conectar. Verifique servidor, porta, banco, usuário, senha e autenticação.");
        }
    }
}
