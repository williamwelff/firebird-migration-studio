# Índices e restrições — implementação inicial

O núcleo `StructuralReader` consulta somente os catálogos `RDB$INDICES`, `RDB$INDEX_SEGMENTS`, `RDB$RELATION_CONSTRAINTS`, `RDB$REF_CONSTRAINTS` e `RDB$RELATIONS`. Reúne índices de colunas, chaves primárias, únicas e estrangeiras; compara nomes, segmentos e algumas propriedades em memória.

**Limitações:** ainda não identifica índices de expressão sem segmentos, CHECK/NOT NULL por constraint, ações ON UPDATE/DELETE de FK, índices inativos, nomes de restrições gerados automaticamente com equivalência semântica, entre outros detalhes. Pode produzir diferenças apenas nominais entre bancos equivalentes. **Não usar para gerar correções automáticas.**

Os testes automatizados verificam casos simulados, incluindo alteração de direção do índice e chave primária ausente. A leitura com servidores Firebird reais continua pendente.

A interface não foi modificada nesta etapa. Escritas em bancos permanecem desabilitadas.
