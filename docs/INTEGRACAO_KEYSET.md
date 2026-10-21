# Integração do leitor por cursor

A função `RecordPageReader.ReadKeysetPageAsync` abre uma conexão real via provedor ADO.NET Firebird, verifica a chave primária declarada no catálogo e executa uma consulta `SELECT FIRST ...` construída por `KeysetQueryBuilder`. Na continuação, os valores da última chave são parâmetros `@k0`, `@k1` etc. A operação não usa `SKIP` nem executa escrita.

**Status:** código integrado e publicado, mas **não homologado com instâncias reais**. Os parâmetros de cursor são fornecidos pelo chamador e ainda não há sessão única snapshot entre páginas; tipagem de parâmetros, collations, transações concorrentes, paginação composta e tratamento de BLOBs grandes requerem testes de integração. A leitura antiga baseada em `SKIP` permanece disponível para fins experimentais.

## Homologação necessária

Executar um conjunto reproduzível de testes com instâncias descartáveis **Firebird 2.5, 3.0, 4.0 e 5.0**, cobrindo conexão, autenticação, catálogo, tabelas vazias, PK simples/composta, continuação de páginas, limites, tipos especiais, charsets e operações concorrentes. Validar Windows, Ubuntu e Fedora; anexar relatórios por versão e plataforma. Nenhum servidor real foi certificado com a entrega atual.
