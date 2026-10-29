# Homologação inicial — Firebird 2.5.9

O workflow [Firebird 2.5 legacy integration](../.github/workflows/firebird-legacy-integration.yml) foi executado com sucesso em servidor Firebird 2.5.9 real e descartável: https://github.com/williamwelff/firebird-migration-studio/actions/runs/40877281511.

O teste utiliza a imagem legada `jacobalberty/firebird:v2.5.9-ss`, cujo projeto foi arquivado em 2025, e deve ficar **restrito ao CI isolado**, sem acesso a redes ou dados de clientes. O teste cria suas próprias tabelas fictícias e verifica conexão, chave primária simples e composta, paginação, leitura de dados e rejeição de chave incompatível.

**Não está homologado:** migração 2.5 → 5, backup/restore, integridade de dados em larga escala, comportamento sob concorrência, aplicativos Windows/Fedora, BLOBs grandes, segurança em produção. Nunca executar scripts de teste em bancos de clientes.
