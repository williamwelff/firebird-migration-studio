# Homologação inicial: Firebird 2.5.9 → 5.0.4

**Resultado:** aprovado em GitHub Actions com servidores reais descartáveis em Linux, [execução 40884148600](https://github.com/williamwelff/firebird-migration-studio/actions/runs/40884148600).

## Procedimento testado
1. Iniciar dois contêineres de teste isolados: Firebird 2.5.9 e Firebird 5.0.4.
2. Criar tabelas fictícias CLIENTES e PEDIDOS com PK simples e composta e FK.
3. Inserir registros fictícios e confirmar commit.
4. Executar backup pelo `gbak` do Firebird 2.5.
5. Executar restore do mesmo backup pelo `gbak` do Firebird 5.0.
6. Usar `isql` e `EXECUTE BLOCK` para falhar o teste caso a quantidade de linhas, soma decimal, valores ou presença da FK seja diferente.

O workflow executa [os SQLs de origem e validação](../tests/integration/migration/) e destrói contêineres ao final.

## Limites
- Aprovação **somente para o conjunto sintético testado**. Não foi validado um backup real de cliente nem a fidelidade de procedures, triggers, domains, generators, views, índices por expressão, BLOBs e charsets variados.
- A migração foi realizada por comandos automáticos no CI; **a interface gráfica ainda não orquestra essa migração**.
- Não prova compatibilidade com todos os bancos Firebird 2.5 e não autoriza uso em produção.
- Imagem Firebird 2.5 comunitária legada usada apenas em ambiente isolado; validar proveniência e riscos antes de ampliar.
- Próxima prioridade: banco fictício avançado, conferências de integridade e contagens maiores, validação de backup/restore por versão e assistente GUI com proteção da origem.
