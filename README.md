# Firebird Migration Studio

Aplicativo desktop **open source e multiplataforma** para **migração, comparação e sincronização controlada** de bancos de dados Firebird.

> **Estado atual: protótipo inicial em desenvolvimento.** Ainda não existe uma versão pronta para operações reais em produção.

## Objetivos

- Atender Windows 10/11, Ubuntu e Fedora com C#, .NET 10 e Avalonia UI.
- Migrar bancos Firebird **2.5, 3.0 e 4.0 para Firebird 5.0**, usando as ferramentas oficiais adequadas a cada versão e validações prévias. As séries 3.5 e 4.5 não são versões oficiais do Firebird.
- Comparar um **banco master (referência)** com um **banco alvo**, tanto em estrutura quanto em registros.
- Gerar um plano revisável de correções; aplicar alterações aprovadas **somente no banco alvo**.
- Permitir que técnicos usem uma interface gráfica, sem digitar comandos.

## Módulos planejados

| Módulo | Responsabilidades |
| --- | --- |
| Interface gráfica | Perfis de conexão, assistentes, progresso, pré-visualização e relatórios |
| Migração | Verificações, backup da origem, restauração no destino e validação |
| Comparação estrutural | Tabelas, campos, índices, restrições, views, triggers, procedures e generators |
| Comparação de registros | Correspondência por chave estável, inserções, atualizações e exclusões opcionais |
| Sincronização | Simulação, aprovação, ordem de dependências, execução auditável e conferência |
| Relatórios | Diferenças, ações aprovadas, logs e pendências |

## Segurança

1. O banco **master nunca deve ser modificado** pela sincronização.
2. A comparação é somente leitura; operações de escrita ficam bloqueadas até revisão e autorização.
3. Exclusões no banco alvo são desativadas por padrão e exigem confirmação adicional.
4. Aplicação de mudanças exige backup recuperável e plano de reversão.
5. Tabelas sem chave primária ou outra chave única estável exigem mapeamento explícito.
6. A solução precisa considerar integridade referencial, triggers, BLOBs, charset, concorrência e volume de dados.
7. Não executar migrações reais antes de testes em bancos descartáveis.

## Plataformas prioritárias

- Windows 10/11 x64 — instalador planejado.
- Ubuntu LTS x64 — pacote `.deb` planejado.
- Fedora x64 — pacote `.rpm` planejado.

**Os instaladores ainda não estão disponíveis.** Compilar não equivale a homologar a interface em cada sistema.

## Tecnologias

- C# e .NET 10
- Avalonia UI
- Provedor .NET para Firebird, com versões compatíveis a validar
- Utilitários oficiais `gbak` e `gstat` para migração e conferência

## Documentação

- [Requisitos do produto](docs/PRODUCT_REQUIREMENTS.md)
- [Como executar o protótipo](docs/RUNNING_PREVIEW.md)

## Processo de design

**Qualquer nova proposta visual será apresentada para aprovação antes de ser incorporada ao aplicativo.** A interface atual é apenas um protótipo técnico, anterior a essa regra.

## Licença

MIT. Componentes e ferramentas de terceiros mantêm suas próprias licenças.
