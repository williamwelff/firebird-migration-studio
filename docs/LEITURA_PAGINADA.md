# Leitura paginada de registros — experimental

`RecordPageReader` implementa uma leitura **somente SELECT**, com `FIRST` e `SKIP`, ordenada por colunas de chave definidas explicitamente. Cada chamada retorna no máximo 1.000 linhas e bloqueia identificadores SQL fora de uma lista conservadora de caracteres. O deslocamento é parametrizado e os valores de conexão não são registrados.

**Importante:** paginação via `SKIP` pode ser ineficiente em deslocamentos altos e páginas lidas em transações diferentes podem refletir estados diferentes se o banco receber gravações. A chave fornecida **ainda não é verificada nos metadados**; identidades com aspas e nomes especiais não são suportadas; `SELECT *` e BLOBs grandes ainda podem consumir muita memória. O módulo ainda não está vinculado à interface e não constitui comparação incremental completa.

**Validação:** testes unitários cobrem geração e rejeição de consultas inseguras. É necessário testar a sintaxe e o comportamento com servidores Firebird reais antes de marcar compatibilidade com Firebird 2.5, 3.0, 4.0 e 5.0.

Nenhuma gravação, correção ou exclusão foi habilitada.
