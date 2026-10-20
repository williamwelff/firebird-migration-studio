# Comparação estrutural: primeiro núcleo implementado

**Estado:** código publicado, integração real ainda não homologada.

O núcleo `SchemaReader` consulta **somente leitura** as tabelas de catálogo `RDB$RELATIONS`, `RDB$RELATION_FIELDS` e `RDB$FIELDS` e coleta colunas de tabelas de usuário usando uma transação snapshot. `SchemaComparer` confronta dois resultados em memória e lista campos presentes apenas no master, presentes apenas no alvo e divergentes.

O resultado atual **não é um diff completo de DDL**: tipos internos do Firebird ainda devem ser normalizados para tipos SQL, e faltam subtipos, precisão, charset, collation, defaults, constraints, índices, procedures, triggers e views. Diferenças ainda não devem ser usadas para gerar correções.

A aplicação desktop ainda não aciona a leitura de esquema. A interface só receberá esse recurso quando os tratamentos de erro e as consultas estiverem testados.

## Pendência retomável
Testar leitura contra instâncias descartáveis Firebird 2.5, 3.0, 4.0 e 5.0 e validar as diferenças catalogadas, inclusive tipos de dados, NULL e colunas calculadas. A homologação real das conexões permanece pendente por decisão do projeto; não confundir a compilação automatizada com testes no servidor.

**Segurança:** não executar SQL de gravação no master ou no alvo. Nenhum script de sincronização é gerado nesta fase.

## Evolução: características adicionais

O leitor passa a coletar também `RDB$CHARACTER_LENGTH`, `RDB$FIELD_SUB_TYPE`, `RDB$FIELD_PRECISION`, `RDB$CHARACTER_SET_ID` e collation efetiva da definição do campo (metadados da relação/domínio). Isso permite detectar diferenças antes invisíveis, por exemplo duas colunas textuais com tamanho em bytes igual, mas limite de caracteres distinto. O código ainda **não** transforma esses valores em DDL SQL, tampouco valida semanticamente conjuntos de caracteres em bancos distintos. Sua leitura em servidores reais continua pendente.
