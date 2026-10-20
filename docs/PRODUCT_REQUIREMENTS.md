# Requisitos do produto — Firebird Migration Studio

## 1. Objetivo

Aplicação desktop multiplataforma para técnicos migrarem bancos Firebird 2.5, 3.0 e 4.0 para 5.0, compararem bancos independentes e, mediante autorização, sincronizarem um **banco alvo** com um **banco master**. Migração, comparação e sincronização são fluxos distintos.

## 2. Sistemas operacionais

- Windows 10/11 x64
- Ubuntu LTS x64
- Fedora x64

Implementação em C# e .NET 10 com Avalonia UI, sem dependência de APIs exclusivas do Windows. Cada plataforma deverá ser testada individualmente, inclusive a interface gráfica. Distribuição futura em instalador Windows, pacote `.deb` e pacote `.rpm`.

## 3. Migração

1. Selecionar banco Firebird **2.5, 3.0 ou 4.0** e destino **diferente** no Firebird 5.0. As versões 3.5 e 4.5 não serão oferecidas, por não serem séries oficiais.
2. Verificar versões, permissões, espaço, conexões e utilitários disponíveis.
3. Gerar backup lógico com o `gbak` adequado à versão de origem (2.5, 3.0 ou 4.0) e restaurar em um novo banco com `gbak` 5.0, sem pedir comandos ao operador.
4. Exibir progresso, códigos de saída, erros e logs sem revelar credenciais.
5. Conferir acesso ao destino, ODS, objetos e contagens de registros.
6. Alertar sobre migração separada de usuários, senhas, bibliotecas externas e drivers.

## 4. Comparação

Selecionar **Master (referência)** e **Alvo (destino)**. Permitir comparação somente leitura, sem sincronização automática.

### Estrutura

Comparar tabelas, campos, tipos, nulidade, valores padrão, chaves, restrições, índices, views, procedures, triggers, sequences/generators e dependências, conforme as versões suportadas. Ignorar diferenças cosméticas sem ocultar diferenças semânticas. Não sincronizar cegamente metadados internos ou características legítimas de versões distintas.

### Registros

- Identificar tabelas por nome ou mapeamento explícito.
- Identificar linhas por chave primária ou chave única estável aprovada pelo usuário.
- Apontar registros ausentes, diferentes e excedentes; exclusões são opcionais.
- Suportar BLOB, NULL, datas, valores numéricos, codificações e regras de equivalência.
- Processar grandes volumes em lotes, com cancelamento e progresso.
- Controlar concorrência: usar instantâneos transacionais adequados ou janela de manutenção.

## 5. Sincronização

- **Nunca escrever no master.**
- Propor um plano determinístico de alterações, com dependências e valores parametrizados.
- Mostrar SQL/ações propostas, quantidade estimada, riscos e simulação.
- Exigir consentimento e backup testado antes da execução.
- Bloquear exclusões por padrão e pedir confirmação específica.
- Detectar mudanças concorrentes entre análise e aplicação.
- Executar em transações apropriadas e explicar limites de rollback em DDL.
- Verificar resultado e registrar auditoria, sem alterar generators ou desativar triggers inadvertidamente.

## 6. Jornada do técnico

1. Escolher Migrar, Comparar ou Sincronizar.
2. Informar perfis master/origem e alvo/destino; validar.
3. Escolher escopo e regras.
4. Analisar diferenças e riscos.
5. Aprovar plano quando houver escrita.
6. Executar, validar e exportar relatório.

## 7. Critérios de aceitação

- Migração 2.5 → 5.0 testada em banco descartável com relacionamentos, triggers, procedures, BLOBs e generators.
- Comparação realiza **zero escritas** nos dois bancos.
- Diferenças de inserção, alteração e exclusão são classificadas corretamente.
- Sincronização modifica somente o alvo e exige autorização específica para exclusão.
- Falhas preservam o master, geram diagnóstico e indicam recuperação.
- Grandes tabelas não precisam caber inteiras em memória.
- Compilação e testes reais nos desktops Windows, Ubuntu e Fedora.

## 8. Etapas de implementação

1. Base multiplataforma, assistente de conexões, logs.
2. Detecção de versões e migração com backup/restore.
3. Comparação somente leitura de estrutura e registros.
4. Pré-visualização, aprovação e sincronização segura.
5. Instaladores, testes de plataforma e manual para técnicos.

## 9. Regra de aprovação visual

Propostas de layout, identidade visual, ícones, cores e fluxo de telas serão **apresentadas ao responsável antes de sua inclusão no código da interface**. A documentação e os módulos internos não dependem dessa aprovação.

**Este documento descreve requisitos, não funcionalidades já homologadas.**
