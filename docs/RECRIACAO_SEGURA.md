# Recriação segura de banco de dados (funcionalidade planejada)

## Objetivo

Permitir ao usuário criar **uma nova base limpa** em Windows, Ubuntu e Fedora sem conhecimento de comandos administrativos. Esta função **não está implementada nem habilitada**; este documento estabelece o contrato de segurança.

## Modos

1. **Banco vazio:** criar um novo banco Firebird 5.0 sem tabelas nem dados.
2. **Estrutura sem registros:** criar um **novo** banco com a estrutura do modelo selecionado (tabelas, índices, procedures, triggers etc.), mas sem os dados de negócio. Um modelo desse tipo **depende do esquema da aplicação**: não existe um único arquivo Firebird genérico que sirva para todos os sistemas.

No futuro, o instalador poderá incluir um banco de exemplo identificável, exclusivamente para demonstração, sem informações reais e com licença adequada. Um modelo da estrutura do banco de cada cliente **não** deve ser empacotado em um instalador genérico.

## Proteções obrigatórias

- **Nunca redefinir automaticamente na instalação, atualização ou inicialização.**
- Nunca substituir ou modificar o banco master.
- Por padrão, criar um **novo banco em outro caminho**, mantendo o original intacto.
- Antes de permitir substituir um alvo existente, exigir backup testado, conferência de destino físico e confirmação explícita adicional; essa ação permanecerá desabilitada no MVP.
- Não incluir arquivos `.fdb` reais de clientes no repositório ou instalador.
- Alertar claramente que a recriação não migra senhas e usuários do servidor.
- Executar testes automatizados e teste manual com bancos descartáveis antes da liberação.
- Exibir resumo do resultado e registrar auditoria sem senhas.

## Interface proposta — ainda sujeita à aprovação visual

Na seção Configurações > Bancos de dados, oferecer **Criar banco limpo**, com opções de tipo, caminho de destino e resumo de segurança. O recurso é de criação de novo banco, **não um botão de apagar registros indiscriminadamente**.

A implementação visual deve ser apresentada para aprovação antes de ser incorporada ao aplicativo.
