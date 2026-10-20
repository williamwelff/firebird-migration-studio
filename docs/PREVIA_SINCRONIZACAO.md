# Prévia de sincronização de registros

O núcleo `RecordSyncPlanner` converte diferenças calculadas em **uma prévia não executável**. Distingue inserções previstas, atualizações previstas e registros adicionais no alvo que exigem análise humana. Registros adicionais não geram comandos `DELETE`.

A prévia contém somente metadados diagnósticos; **não gera SQL, não recebe conexão e não executa operações**. Sua propriedade `CanExecute` é sempre `false`.

Os testes utilizam diferenças sintéticas em memória. Ainda faltam validação de chaves contra metadados reais, leitura incremental de registros, resolução de conflitos, backup, aprovação explícita e homologação com servidores Firebird. **Não utilizar em produção.**
