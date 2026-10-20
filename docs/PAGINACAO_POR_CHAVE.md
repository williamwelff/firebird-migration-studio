# Paginação por chave — próxima fase

`KeysetQueryBuilder` gera instruções **SELECT** com cursores lexicográficos para chaves primárias simples e compostas, eliminando a necessidade de `SKIP` na construção SQL. Usa parâmetros `@k0`, `@k1` etc. e rejeita identificadores fora de uma lista conservadora.

**Estado exato:** o construtor de SQL e seus testes foram publicados; o leitor real `RecordPageReader` **continua utilizando SKIP** até que a integração de cursor e os testes com bancos descartáveis estejam homologados. O código ainda não gerencia tipos dos valores do cursor, BLOBs grandes, isolamento entre páginas, collation ou concorrência. Não usar para sincronização ou produção.
