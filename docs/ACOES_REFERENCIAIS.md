# Comparação de ações referenciais

O leitor estrutural agora coleta as propriedades `RDB$UPDATE_RULE` e `RDB$DELETE_RULE` de chaves estrangeiras por meio de `RDB$REF_CONSTRAINTS`. O comparador detecta alterações em regras como `CASCADE`, `SET NULL` e `NO ACTION`, comparando os valores registrados no catálogo.

**Estado:** testes simulados adicionados, sem comandos de gravação. Ainda falta conferir os resultados contra servidores Firebird reais e validar as demais dependências, constraints CHECK, índices de expressão e equivalência de nomes gerados automaticamente. Nenhuma diferença deve ser usada para sincronização automática nesta fase.
