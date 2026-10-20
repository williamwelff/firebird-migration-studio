# Comparador de registros — primeiro núcleo

O código `RecordComparer` compara registros já carregados em memória, por uma chave simples ou composta explicitamente definida. Classifica registros ausentes no alvo, adicionais no alvo e registros com campos divergentes. Diferencia valores nulos, compara BLOBs `byte[]` por conteúdo e recusa chaves nulas ou duplicadas.

**Proteção:** nenhuma conexão, leitura SQL, comando de escrita ou exclusão é executado por este comparador. Os resultados são somente diagnósticos.

**Limitações:** o leitor SQL de registros ainda não foi desenvolvido; a implementação recebe snapshots fornecidos por testes. Não há paginação, comparação incremental, identificação automática segura de chaves, tolerância semântica de tipos entre versões, normalização de charset, nem suporte a BLOBs em streaming. Não pode ser utilizada em produção nem para alimentar uma sincronização automática.

A homologação de conexões com servidores reais Firebird 2.5/3.0/4.0/5.0 continua adiada para uma etapa específica.
