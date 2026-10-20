# Teste de conexão real — implementação inicial

## Estado
O aplicativo utiliza o provedor oficial `FirebirdSql.Data.FirebirdClient` versão 10.3.4 para **tentar abrir uma conexão real** a partir da tela **Conexões**. A operação está disponível para master e alvo, sem executar SQL de gravação.

O resultado apresenta o identificador de versão informado pelo provedor ou uma mensagem genérica de erro. Campos obrigatórios e portas inválidas são rejeitados antes da abertura.

## Segurança
- Senhas são mascaradas na interface e não são salvas em arquivo de configuração.
- A string de conexão não é registrada em logs e mensagens detalhadas do provedor não são exibidas ao operador.
- A conexão utiliza `Pooling = false` e tempo limite curto.
- O resultado da conexão **não prova** que a identidade física do banco é distinta ou que o usuário tem permissões somente leitura.
- **A aplicação ainda não realiza migração, comparação de registros ou sincronização.**

## Validação pendente
A compilação no GitHub Actions e os testes unitários **não demonstram** conectividade real com todas as versões. É necessário executar testes com bancos descartáveis e servidores Firebird **2.5, 3.0, 4.0 e 5.0** no Windows, Ubuntu e Fedora, inclusive cenários de credenciais incorretas, indisponibilidade e autenticação legada.

Esta etapa não implementa instalação automática do Firebird nem distribui senhas, bases ou executáveis proprietários.
