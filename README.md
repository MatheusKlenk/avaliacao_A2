# API de Consumo de Água - Avaliação A2

## Endpoints da API

A aplicação mostra os seguintes endpoints para gerenciamento do consumo:

### 1. Cadastrar Consumo

* Endpoint: POST /api/consumo/cadastrar
* Descrição: Registra um nova leitura de consumo. Todos os calculos são feitos internamente antes de salvar.
* Corpo da Requisição (Exemplo):
    json
    {
      "cpf": "123.456.789-00",
      "mes": 10,
      "ano": 2025,
      "m3Consumidos": 37.5,
      "bandeira": "Vermelha",
      "possuiEsgoto": true
    }

### 2. Listar Todos os Consumos

* Endpoint: GET /api/consumo/listar
* Descrição: Retorna uma lista com todos os registros de consumo de agua cadastrados.

### 3. Buscar Consumo Específico

* Endpoint: GET /api/consumo/buscar/{cpf}/{mes}/{ano}
* Descrição: Retorna um registro de consumo específico com base no CPF, mês e ano.
* Exemplo: GET http://localhost:5099/api/consumo/buscar/123.456.789-00/10/2025

### 4. Remover Consumo

* Endpoint: DELETE /api/consumo/remover/{cpf}/{mes}/{ano}
* Descrição: Remove um registro de consumo com base no CPF, mês e ano.
* Exemplo: DELETE http://localhost:5099/api/consumo/remover/123.456.789-00/10/2025

### 5. Obter Total Geral Faturado

* Endpoint: GET /api/consumo/total-geral
* Descrição: Retorna a soma do valor total de todas as faturas cadastradas no banco de dados.
* Resposta (Exemplo):
    json
    {
      "totalGeral": 457.5
    }
    

Projeto desenvolvido por:

Matheus Klenk de Lima
Victor Borges Blanc

*(Solução: `Victor.sln`, Projeto: `Matheus.csproj`)*
