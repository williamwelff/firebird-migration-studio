# Executando o protótipo

Este primeiro protótipo contém **interface e validação local de campos**. Nenhuma conexão com Firebird, migração, comparação real ou sincronização foi implementada.

## Requisitos

- Windows 10/11, Ubuntu ou Fedora com ambiente gráfico.
- SDK .NET 10 e acesso aos pacotes NuGet.

## Executar

Abra um terminal **de desenvolvimento** na raiz do repositório e use:

```sh
dotnet run --project src/FirebirdMigration.Desktop/FirebirdMigration.Desktop.csproj
```

O usuário final não precisará utilizar terminal após a implementação dos instaladores. Os instaladores ainda não estão disponíveis.

## Testes de validação

```sh
dotnet run --project tests/FirebirdMigration.SmokeTests/FirebirdMigration.SmokeTests.csproj
```

GitHub Actions compila no Windows, Ubuntu e em um contêiner Fedora. Compilação em contêiner **não** substitui teste de interface gráfica em sessão desktop real.

## Limitações e próximos passos

- Ainda não conecta a bancos Firebird.
- A validação identifica campos vazios, portas inválidas e mesmo endpoint exato; **não detecta aliases diferentes para o mesmo banco**, DNS alternativos ou links simbólicos. A proteção final exigirá identificação real do banco.
- O fluxo da aplicação não deve solicitar nem salvar senhas em texto simples; o suporte à autenticação será implementado separadamente.
- Migração e sincronização estarão desabilitadas até haver testes com bancos descartáveis.
