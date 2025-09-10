# JSON Viewer e Query Tool

Uma aplicação .NET Console que permite carregar arquivos JSON e fazer queries baseadas em campos específicos.

## Funcionalidades

- ✅ Carregar arquivos JSON do sistema de arquivos
- ✅ Carregar JSON diretamente de string/texto
- ✅ Visualizar a estrutura hierárquica do JSON
- ✅ Listar todos os campos disponíveis no JSON
- ✅ Fazer queries simples por nome de campo
- ✅ Fazer queries avançadas com diferentes operações de filtro
- ✅ Suporte a campos aninhados (ex: `endereco.cidade`)
- ✅ Suporte a arrays e objetos complexos
- ✅ Múltiplas operações de comparação (igual, contém, regex, etc.)

## Requisitos

- .NET 9.0 ou superior
- Pacote Newtonsoft.Json (instalado automaticamente)

## Como usar

### 1. Compilar o projeto

```bash
dotnet build JsonViewer/JsonViewer.csproj
```

### 2. Executar a aplicação

#### Execução simples:
```bash
dotnet run --project JsonViewer/JsonViewer.csproj
```

#### Execução com arquivo JSON:
```bash
dotnet run --project JsonViewer/JsonViewer.csproj caminho/para/arquivo.json
```

### 3. Menu de opções

A aplicação oferece um menu interativo com as seguintes opções:

1. **Carregar arquivo JSON** - Carrega um arquivo JSON do sistema
2. **Carregar JSON de string** - Permite colar JSON diretamente
3. **Visualizar estrutura do JSON** - Mostra a hierarquia do JSON
4. **Listar todos os campos** - Lista todos os campos disponíveis
5. **Fazer query por campo** - Busca simples por nome de campo
6. **Query avançada com filtros** - Busca com operações de comparação
0. **Sair** - Encerra a aplicação

## Exemplos de uso

### Arquivo de exemplo

O projeto inclui um arquivo `exemplo.json` com dados de usuários para testes.

### Queries de exemplo

#### Buscar todos os valores de um campo:
- Campo: `nome`
- Resultado: Lista todos os nomes dos usuários

#### Buscar campos aninhados:
- Campo: `endereco.cidade`
- Resultado: Lista todas as cidades dos endereços

#### Buscar em arrays:
- Campo: `telefones.tipo`
- Resultado: Lista todos os tipos de telefone

#### Query com filtro:
- Campo: `idade`
- Valor: `30`
- Operação: `Maior que`
- Resultado: Usuários com idade maior que 30

#### Query com regex:
- Campo: `email`
- Valor: `.*@email\.com`
- Operação: `Expressão regular`
- Resultado: Emails que terminam com @email.com

## Operações de comparação disponíveis

1. **Igual** - Comparação exata (case-insensitive)
2. **Contém** - Verifica se o valor contém o texto
3. **Começa com** - Verifica se o valor inicia com o texto
4. **Termina com** - Verifica se o valor termina com o texto
5. **Expressão regular** - Usa regex para busca avançada
6. **Maior que** - Comparação numérica (ou alfabética)
7. **Menor que** - Comparação numérica (ou alfabética)
8. **Maior ou igual** - Comparação numérica (ou alfabética)
9. **Menor ou igual** - Comparação numérica (ou alfabética)

## Estrutura do projeto

```
JsonViewer/
├── JsonViewer.csproj          # Arquivo de projeto .NET
├── Program.cs                 # Ponto de entrada e interface do usuário
├── JsonLoader.cs              # Classe para carregar e parsear JSON
├── JsonQueryEngine.cs         # Engine de queries e filtros
├── exemplo.json              # Arquivo JSON de exemplo para testes
└── README.md                 # Este arquivo
```

## Classes principais

### JsonLoader
- Responsável por carregar JSON de arquivos ou strings
- Valida e parseia o conteúdo JSON
- Exibe a estrutura hierárquica do JSON

### JsonQueryEngine
- Executa queries nos dados JSON carregados
- Suporta busca em campos aninhados e arrays
- Implementa diferentes operações de comparação
- Lista todos os campos disponíveis no JSON

### QueryResult
- Representa o resultado de uma query
- Contém caminho, valor, tipo e contexto do resultado

## Tratamento de erros

- Validação de arquivos JSON malformados
- Verificação de existência de arquivos
- Tratamento de campos inexistentes
- Validação de expressões regulares
- Mensagens de erro claras e informativas
- **Suporte completo a caminhos com espaços** - Remove aspas automaticamente e normaliza caminhos

## Melhorias na versão atual

### ✅ Correção de caminhos com espaços
- A aplicação agora trata corretamente arquivos em pastas com espaços no nome
- Remove aspas automaticamente quando presentes no caminho
- Normaliza e expande caminhos relativos para absolutos
- Funciona tanto via linha de comando quanto via menu interativo

**Exemplos de caminhos suportados:**
```bash
# Com aspas
dotnet run --project JsonViewer/JsonViewer.csproj "Pasta com Espaços\arquivo.json"

# Sem aspas (também funciona)
dotnet run --project JsonViewer/JsonViewer.csproj Pasta com Espaços\arquivo.json

# No menu interativo, ambos funcionam:
# "C:\Meus Documentos\arquivo.json"
# C:\Meus Documentos\arquivo.json
```

## Contribuição

Para contribuir com o projeto:

1. Faça um fork do repositório
2. Crie uma branch para sua feature
3. Implemente suas mudanças
4. Teste thoroughly
5. Submeta um pull request

## Licença

Este projeto está sob licença MIT. Veja o arquivo LICENSE para mais detalhes.