# 🔑​ Food Core Auth

API serveless para authenticação e permissão de usuários de restaurantes fast-food, desenvolvida como parte do curso de Arquitetura de Software
da FIAP (Tech Challenge).

<div align="center">
  <a href="#visao-geral">Visão Geral</a> •
  <a href="#tecnologias">Tecnologias</a> •
  <a href="#fluxo-de-autenticacao">Fluxo de Autenticação</a> •
  <a href="#exemplo-de-fluxo">Exemplo de Fluxo</a> •
</div><br>

# 🔑 Lambda de Autenticação - Identificação via CPF (C# + Cognito)

## 📖 Visão Geral

A Lambda é responsável pela **identificação de clientes** no sistema de autoatendimento.
Ela recebe o **CPF** do cliente, consulta o **Cognito**, gera um **JWT** e retorna o token para o **API Gateway (APIM)**, que repassa a chamada para a **FoodCore API**.

## 🚀 Tecnologias

- **C# .NET 8 AWS Lambda Runtime**
- **Azure APIM** (API Gateway)
- **AWS Cognito** (identificação/autenticação sem senha, apenas CPF ou Email)
- **JWT** para comunicação segura
- **GitHub Actions + Terraform** para deploy

## 🔄 Fluxo de Autenticação

1. O **usuário** informa o **CPF** ou **CPF** no frontend. Caso o usuário não informe nada, uma requisição será enviada ao **APIM** solicitando um usuário temporário(GUEST)
2. A requisição chega no **APIM**, que redireciona para a **Azure Function (Lambda em C#)**.
3. A **Lambda**:
   - Valida o CPF ou Email caso forem enviados.
   - Consulta o **Cognito**.
   - Caso exista, gera um **JWT** assinado.
   - Retorna o token para o **APIM**.
4. O **APIM** repassa a requisição com o **JWT** no header para a **FoodCore API**.
5. A **API** valida o JWT e continua o fluxo (pedido, consulta etc.).

## 🧩 Exemplo de Fluxo

```mermaid
sequenceDiagram
    participant User
    participant APIM
    participant Lambda
    participant Cognito
    participant API

    User->>APIM: POST /auth {cpf}
    APIM->>Lambda: Invoca função com CPF
    Lambda->>Cognito: Consulta cliente
    Cognito-->>Lambda: Retorna dados
    Lambda-->>APIM: Retorna JWT
    APIM->>API: Chamada autenticada com JWT
    API-->>User: Retorna dados do pedido

