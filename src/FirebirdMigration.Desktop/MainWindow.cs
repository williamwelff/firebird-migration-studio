using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using FirebirdMigration.Core;

namespace FirebirdMigration.Desktop;

/// <summary>Primeira implementação visual aprovada. Não realiza operações de banco.</summary>
public sealed class MainWindow : Window
{
    private static readonly SolidColorBrush Azul = new(Color.Parse("#1769E8"));
    private static readonly SolidColorBrush Cinza = new(Color.Parse("#586575"));
    private static readonly SolidColorBrush Fundo = new(Color.Parse("#F4F7FB"));
    private static readonly SolidColorBrush Branco = new(Color.Parse("#FFFFFF"));

    private readonly ContentControl _page = new();
    private readonly Dictionary<string, Button> _navigation = new();
    private readonly TextBox _masterHost = new() { Text = "localhost" };
    private readonly TextBox _masterPort = new() { Text = "3050" };
    private readonly TextBox _masterPath = new() { Watermark = "Caminho completo ou alias do banco" };
    private readonly TextBox _masterUser = new() { Text = "SYSDBA" };
    private readonly TextBox _targetHost = new() { Text = "localhost" };
    private readonly TextBox _targetPort = new() { Text = "3055" };
    private readonly TextBox _targetPath = new() { Watermark = "Caminho completo ou alias do banco" };
    private readonly TextBox _targetUser = new() { Text = "SYSDBA" };
    private readonly TextBlock _validation = new() { Text = "Nenhuma conexão foi testada.", TextWrapping = TextWrapping.Wrap };

    public MainWindow()
    {
        Title = "Firebird Migration Studio — Prévia de interface";
        Width = 1180;
        Height = 780;
        MinWidth = 850;
        MinHeight = 620;
        Background = Fundo;

        var sidebar = new StackPanel { Width = 210, Spacing = 7, Margin = new Thickness(12, 22, 12, 12) };
        foreach (var (name, glyph) in new[]
        {
            ("Início", "⌂"), ("Conexões", "▤"), ("Migrar", "⇧"),
            ("Comparar", "≡"), ("Sincronizar", "⇄"), ("Relatórios", "▣"),
            ("Configurações", "⚙")
        })
        {
            var key = name;
            var button = new Button
            {
                Content = $"{glyph}   {name}", HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left, Padding = new Thickness(14, 11),
                Background = Brushes.Transparent, BorderThickness = new Thickness(0)
            };
            button.Click += (_, _) => Navigate(key);
            sidebar.Children.Add(button);
            _navigation.Add(name, button);
        }

        var left = new Border { Background = Branco, Width = 234, Child = sidebar };
        var header = new DockPanel { LastChildFill = true, Margin = new Thickness(24, 18, 24, 16) };
        var status = new TextBlock
        {
            Text = "●  Protótipo seguro · sem operações reais",
            Foreground = Cinza, VerticalAlignment = VerticalAlignment.Center
        };
        DockPanel.SetDock(status, Dock.Right);
        header.Children.Add(status);
        header.Children.Add(new StackPanel
        {
            Spacing = 2,
            Children =
            {
                new TextBlock { Text = "Firebird Migration Studio", FontSize = 24, FontWeight = FontWeight.Bold },
                new TextBlock { Text = "Migração, comparação e sincronização de bancos Firebird", Foreground = Cinza }
            }
        });
        var right = new DockPanel();
        DockPanel.SetDock(header, Dock.Top);
        right.Children.Add(header);
        right.Children.Add(new ScrollViewer { Content = _page });
        var root = new DockPanel();
        DockPanel.SetDock(left, Dock.Left);
        root.Children.Add(left);
        root.Children.Add(right);
        Content = root;
        Navigate("Início");
    }

    private void Navigate(string page)
    {
        foreach (var item in _navigation)
        {
            item.Value.Background = item.Key == page ? Azul : Brushes.Transparent;
            item.Value.Foreground = item.Key == page ? Branco : Brushes.Black;
        }

        _page.Content = page switch
        {
            "Início" => Dashboard(),
            "Conexões" => Connections(),
            "Migrar" => Feature("Migração de bancos de dados", "Assistente de backup e restauração Firebird 2.5 → 5.0", "1. Origem   →   2. Backup   →   3. Destino   →   4. Restauração   →   5. Verificação"),
            "Comparar" => Feature("Comparação de bancos de dados", "Análise da estrutura e dos registros entre master e alvo", "Resumo   ·   Tabelas   ·   Procedures   ·   Triggers   ·   Índices   ·   Registros"),
            "Sincronizar" => Feature("Sincronização e correção", "Planejamento de alterações exclusivamente no banco alvo", "1. Revisar   →   2. Configurar   →   3. Autorizar   →   4. Verificar"),
            "Relatórios" => Feature("Relatórios", "Histórico e resultados das operações", "Nenhuma operação real foi executada."),
            _ => Feature("Configurações", "Preferências e ferramentas", "Recursos ainda indisponíveis nesta prévia.")
        };
    }

    private Control Dashboard()
    {
        var actions = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var (label, description, destination) in new[]
        {
            ("Migrar", "Transferir bancos entre versões do Firebird.", "Migrar"),
            ("Comparar", "Encontrar diferenças de estrutura e registros.", "Comparar"),
            ("Sincronizar", "Revisar correções propostas para o alvo.", "Sincronizar"),
            ("Relatórios", "Consultar registros e resultados.", "Relatórios")
        })
        {
            var button = new Button { Content = "Abrir " + label + "  →", Background = Azul, Foreground = Branco };
            button.Click += (_, _) => Navigate(destination);
            actions.Children.Add(Card(label, description, button, 225));
        }
        return Page("Bem-vindo ao Firebird Migration Studio",
            "Selecione uma ferramenta. Esta versão apresenta somente a interface.",
            actions,
            Card("Segurança em primeiro lugar",
                "A comparação será somente leitura. A sincronização exigirá plano revisado, backup e autorização. Nenhum banco será modificado nesta prévia.", null, 930));
    }

    private Control Connections()
    {
        var validate = new Button
        {
            Content = "Validar campos", Background = Azul, Foreground = Branco,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        validate.Click += (_, _) =>
        {
            var master = ReadProfile("Master", _masterHost, _masterPort, _masterPath, _masterUser);
            var target = ReadProfile("Alvo", _targetHost, _targetPort, _targetPath, _targetUser);
            var issues = TargetProtection.ValidateDistinctEndpoints(master, target);
            _validation.Text = issues.Count == 0
                ? "Campos válidos e endpoints informados distintos. Conexões reais ainda não testadas."
                : string.Join(Environment.NewLine, issues);
        };
        var columns = new StackPanel { Spacing = 14 };
        columns.Children.Add(new TextBlock { Text = "Banco master — referência protegida", FontSize = 18, FontWeight = FontWeight.SemiBold });
        columns.Children.Add(Entry("Servidor", _masterHost));
        columns.Children.Add(Entry("Porta", _masterPort));
        columns.Children.Add(Entry("Banco ou alias", _masterPath));
        columns.Children.Add(Entry("Usuário", _masterUser));
        columns.Children.Add(new Separator());
        columns.Children.Add(new TextBlock { Text = "Banco alvo — destino futuro", FontSize = 18, FontWeight = FontWeight.SemiBold });
        columns.Children.Add(Entry("Servidor", _targetHost));
        columns.Children.Add(Entry("Porta", _targetPort));
        columns.Children.Add(Entry("Banco ou alias", _targetPath));
        columns.Children.Add(Entry("Usuário", _targetUser));
        columns.Children.Add(validate);
        columns.Children.Add(_validation);
        return Page("Conexões com bancos de dados",
            "Configure os dados de origem e destino. Nenhuma senha é armazenada nesta prévia.",
            Card("Dados das conexões", columns, null, 740));
    }

    private Control Feature(string title, string subtitle, string stages)
        => Page(title, subtitle,
            Card("Etapas e recursos previstos", stages, null, 930),
            Card("Disponibilidade", "Em desenvolvimento. A conexão, a comparação, o backup, o restore e a sincronização reais permanecem desabilitados.", null, 930));

    private static StackPanel Page(string title, string subtitle, params Control[] sections)
    {
        var result = new StackPanel { Margin = new Thickness(24, 10, 24, 24), Spacing = 15 };
        result.Children.Add(new TextBlock { Text = title, FontSize = 23, FontWeight = FontWeight.Bold });
        result.Children.Add(new TextBlock { Text = subtitle, Foreground = Cinza, TextWrapping = TextWrapping.Wrap });
        foreach (var section in sections) result.Children.Add(section);
        return result;
    }

    private static Border Card(string title, string description, Control? action, double width)
        => Card(title, (Control)new TextBlock
        { Text = description, TextWrapping = TextWrapping.Wrap, Foreground = Cinza }, action, width);

    private static Border Card(string title, Control content, Control? action, double width)
    {
        var stack = new StackPanel { Spacing = 13 };
        stack.Children.Add(new TextBlock { Text = title, FontSize = 18, FontWeight = FontWeight.SemiBold });
        stack.Children.Add(content);
        if (action is not null) stack.Children.Add(action);
        return new Border
        {
            Width = width, Background = Branco, CornerRadius = new CornerRadius(11),
            Margin = new Thickness(0, 0, 12, 12), Padding = new Thickness(20),
            BorderBrush = new SolidColorBrush(Color.Parse("#E3E8F0")),
            BorderThickness = new Thickness(1), Child = stack
        };
    }

    private static StackPanel Entry(string label, TextBox input)
        => new() { Spacing = 4, Children = { new TextBlock { Text = label }, input } };

    private static ConnectionProfile ReadProfile(string label, TextBox host, TextBox port, TextBox path, TextBox user)
        => new(label, host.Text?.Trim() ?? "", int.TryParse(port.Text, out var p) ? p : 0,
            path.Text?.Trim() ?? "", user.Text?.Trim() ?? "");
}
