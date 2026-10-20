using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using FirebirdMigration.Core;

namespace FirebirdMigration.Desktop;

public sealed class MainWindow : Window
{
    private readonly TextBox _masterHost = new() { Text = "localhost" };
    private readonly TextBox _masterPort = new() { Text = "3050" };
    private readonly TextBox _masterPath = new() { Watermark = "Alias ou caminho do banco master" };
    private readonly TextBox _masterUser = new() { Text = "SYSDBA" };
    private readonly TextBox _targetHost = new() { Text = "localhost" };
    private readonly TextBox _targetPort = new() { Text = "3055" };
    private readonly TextBox _targetPath = new() { Watermark = "Alias ou caminho do banco alvo" };
    private readonly TextBox _targetUser = new() { Text = "SYSDBA" };
    private readonly TextBlock _status = new()
    {
        Text = "Protótipo: nenhuma operação é executada no banco.",
        TextWrapping = Avalonia.Media.TextWrapping.Wrap
    };

    public MainWindow()
    {
        Title = "Firebird Migration Studio — Preview";
        Width = 980;
        Height = 740;
        MinWidth = 700;
        MinHeight = 580;

        var tabs = new TabControl
        {
            Items =
            {
                new TabItem { Header = "Conexões", Content = BuildConnectionPanel() },
                new TabItem { Header = "Migrar", Content = InformationPanel("Migrar Firebird 2.5 → 5.0", "Planejado: detectar versões, realizar backup e restore com utilitários oficiais, validar ODS e registros. Ainda indisponível.") },
                new TabItem { Header = "Comparar", Content = InformationPanel("Comparação master × alvo", "Planejado: diferenças de metadados e registros, leitura somente e relatório. Ainda indisponível.") },
                new TabItem { Header = "Sincronizar", Content = InformationPanel("Sincronização controlada", "Planejado: pré-visualização, aprovação, backup obrigatório e escrita exclusivamente no alvo. Ainda indisponível.") }
            }
        };

        Content = new DockPanel
        {
            Margin = new Thickness(20),
            Children =
            {
                new StackPanel
                {
                    Spacing = 5,
                    Children =
                    {
                        new TextBlock { Text = "Firebird Migration Studio", FontSize = 27, FontWeight = Avalonia.Media.FontWeight.Bold },
                        new TextBlock { Text = "Windows · Ubuntu · Fedora | Protótipo inicial sem operações destrutivas" }
                    }
                }.WithDock(Dock.Top),
                tabs
            }
        };
    }

    private Control BuildConnectionPanel()
    {
        var validate = new Button { Content = "Validar campos e isolamento do alvo", HorizontalAlignment = HorizontalAlignment.Left };
        validate.Click += (_, _) =>
        {
            var master = ReadProfile("Master", _masterHost, _masterPort, _masterPath, _masterUser);
            var target = ReadProfile("Alvo", _targetHost, _targetPort, _targetPath, _targetUser);
            var issues = TargetProtection.ValidateDistinctEndpoints(master, target);
            _status.Text = issues.Count == 0
                ? "Campos válidos e endpoints distintos. As conexões não foram testadas; nenhuma escrita foi realizada."
                : string.Join(Environment.NewLine, issues);
        };

        var panel = new StackPanel
        {
            Spacing = 12,
            Margin = new Thickness(0, 18, 0, 0),
            Children =
            {
                new TextBlock { Text = "MASTER — referência protegida", FontSize = 18 },
                Entry("Servidor", _masterHost), Entry("Porta", _masterPort),
                Entry("Banco/alias", _masterPath), Entry("Usuário", _masterUser),
                new Separator(),
                new TextBlock { Text = "ALVO — futuro destino de alterações", FontSize = 18 },
                Entry("Servidor", _targetHost), Entry("Porta", _targetPort),
                Entry("Banco/alias", _targetPath), Entry("Usuário", _targetUser),
                validate,
                _status
            }
        };
        return new ScrollViewer { Content = panel };
    }

    private static ConnectionProfile ReadProfile(string label, TextBox host, TextBox port, TextBox path, TextBox user)
        => new(label, host.Text?.Trim() ?? "", int.TryParse(port.Text, out int value) ? value : 0,
            path.Text?.Trim() ?? "", user.Text?.Trim() ?? "");

    private static Control Entry(string label, TextBox input)
        => new StackPanel { Spacing = 3, Children = { new TextBlock { Text = label }, input } };

    private static Control InformationPanel(string heading, string message)
        => new StackPanel
        {
            Margin = new Thickness(0, 24, 0, 0),
            Spacing = 15,
            Children =
            {
                new TextBlock { Text = heading, FontSize = 21 },
                new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                new TextBlock { Text = "Modo somente informativo — sem conexão ou alterações no banco." }
            }
        };
}

internal static class DockExtensions
{
    public static T WithDock<T>(this T control, Dock dock) where T : Control
    {
        DockPanel.SetDock(control, dock);
        return control;
    }
}
