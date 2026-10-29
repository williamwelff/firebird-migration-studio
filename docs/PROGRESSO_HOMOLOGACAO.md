# Progresso da homologação por integração real

## Executado com evidência anterior
- Firebird 3.0.14, 4.0.7 e 5.0.4: conexão, leitura por cursor de PK simples/composta, última página e rejeição de chave errada em contêineres descartáveis Linux. Execução aprovada: https://github.com/williamwelff/firebird-migration-studio/actions/runs/39696326398

## Nova cobertura em validação
- Tabela DOCUMENTOS: coluna VARCHAR UTF8, BLOB SUB_TYPE TEXT, BLOB NULL e término da leitura por cursor. Testes adicionados ao workflow; aprovação depende da execução mais recente.
- Estes testes **não comprovam** suporte a BLOBs grandes, comparação de conteúdo binário, texto com caracteres fora do ASCII, nem consumo limitado de memória.

## Pendências
- Firebird 2.5: obter ambiente isolado confiável e documentar versão/imagem de origem, licenciamento, autenticação legada e cadeia de instalação antes de ativar um job. Não substituir por imagem não verificada.
- Testes do aplicativo real nos sistemas Windows e Fedora.
- Comparação consistente entre páginas sob concorrência; atualmente cada página usa transação independente.
- Validação de memória, BLOBs muito grandes, conjuntos de caracteres, isolamento e falhas de rede.
- O produto não está homologado para produção nem para sincronização de dados.

Não adicionar bancos nem credenciais de clientes aos testes.
