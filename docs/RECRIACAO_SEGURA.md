# Restauração de fábrica do banco — especificação

**Estado: planejado; não implementado.**

## Comportamento esperado

A opção **Restaurar configurações de fábrica** cria um banco Firebird 5.0 novo, com a **mesma estrutura de um modelo aprovado**, mas sem dados operacionais de clientes.

Deve preservar, conforme o modelo: tabelas, colunas, tipos, chaves primárias e estrangeiras, índices, views, triggers, procedures, packages, sequences/generators, domains, constraints e permissões necessárias.

**Não** é criar um banco vazio sem tabelas, nem executar `DELETE FROM` indiscriminadamente.

## Modelo e dados iniciais

A estrutura deve vir de um **modelo de banco específico da aplicação** fornecido ou aprovado pelo responsável. O Firebird Migration Studio é uma ferramenta genérica e não possui como adivinhar a estrutura de cada cliente.

Separar:
- **Estrutura:** objetos do banco que devem permanecer.
- **Dados operacionais:** clientes, vendas, histórico etc., que não devem permanecer.
- **Dados essenciais de inicialização:** parâmetros, perfis, tabelas de referência ou cadastros indispensáveis, preservados conforme uma lista explícita e aprovada no modelo.

Validar valores iniciais de generators/sequences e dependências de dados essenciais.

Sem um modelo homologado, o botão deve ficar indisponível, com explicação clara.

## Distribuição

Uma versão homologada do modelo poderá ser disponibilizada na instalação **somente quando houver autorização e compatibilidade da aplicação**. Não incluir bancos de clientes, credenciais nem arquivos proprietários sem permissão. Modelos poderão também ser importados posteriormente.

## Segurança

1. Jamais executar automaticamente ao instalar, atualizar ou iniciar.
2. Jamais modificar o master.
3. Criar por padrão um **novo arquivo** em caminho distinto, sem apagar o original.
4. Exigir seleção do modelo, pré-visualização do que será preservado/removido, backup recuperável e confirmação explícita.
5. Substituição de banco existente fica desabilitada no MVP; depois exigirá salvaguardas adicionais.
6. Validar a estrutura resultante e os dados essenciais antes de disponibilizar para uso.
7. Emitir logs sem senhas nem dados pessoais.

## Interface

A funcionalidade poderá aparecer como **Restaurar configurações de fábrica** (EN: **Restore factory defaults**). Qualquer mudança visual deverá ser mostrada para aprovação antes de inclusão no aplicativo.

## Decisão de produto confirmada

A restauração de fábrica **deve manter dados essenciais de configuração e inicialização**, além de todos os objetos estruturais previstos no modelo homologado. Registros operacionais (por exemplo, clientes, vendas e históricos) não serão transferidos. A seleção dos dados essenciais será explícita por modelo, com conferência de dependências e relatório da operação. Na ausência de classificação confiável, a operação deverá ser bloqueada, nunca excluir dados por suposição.
