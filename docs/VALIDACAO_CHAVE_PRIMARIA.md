# Validação da chave primária declarada

A leitura experimental de registros paginados agora consulta `RDB$RELATION_CONSTRAINTS` e `RDB$INDEX_SEGMENTS` dentro da **mesma transação snapshot** para recuperar a chave primária declarada e sua ordem. A leitura é bloqueada quando não há PK, quando as colunas informadas diferem ou quando a ordem de uma PK composta é incompatível.

Essa verificação impede o uso de uma ordenação arbitrária como se fosse uma chave única, mas não resolve o custo de SKIP alto, BLOBs grandes, SQL dinâmico ainda não homologado com todas as versões, nem a comparação consistente de múltiplas páginas em uma mesma transação. É uma etapa de proteção, **não homologação de produção**. Nenhum SQL de alteração foi habilitado.
