# Ambiente automatizado com servidores Firebird reais

O workflow [Firebird real integration](../.github/workflows/firebird-integration.yml) provisiona contêineres **descartáveis** a partir das imagens oficiais Firebird 3.0.14, 4.0.7 e 5.0.4, inicializa o banco `integration.fdb` com registros fictícios de `CLIENTES` e `PEDIDOS`, executa `FirebirdMigration.IntegrationTests` e remove os contêineres ao final. Cada versão executa em um runner Linux separado.

## Testes de integração
- Abertura de conexão real.
- Paginação por cursor com chave primária simples e composta.
- Página final vazia e ausência de repetição na continuação.
- Bloqueio de ordenação por coluna diferente da chave primária declarada.

## Resultado e limites
A primeira execução **Firebird 5.0.4** foi aprovada: [evidência no Actions](https://github.com/williamwelff/firebird-migration-studio/actions/runs/39696196515). As demais versões só devem ser declaradas aprovadas após a matriz concluir em verde.

O workflow **não** comprova interfaces Windows/Fedora, autenticação legada Firebird 2.5, BLOBs grandes, isolamentos entre várias transações, comportamento sob concorrência, backups e restores, segurança de produção ou sincronização. Os testes usam uma senha **exclusiva para dados públicos e descartáveis do CI**; jamais reutilizá-la em servidores reais.

## Execução
Abra GitHub > Actions > **Firebird real integration (disposable)** > Run workflow. O workflow também roda automaticamente quando o motor ou os arquivos de teste são alterados.

Todo o banco é criado pelo contêiner. Não conecte esse projeto de testes a bases de clientes.
