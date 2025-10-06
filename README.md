# 🔑​ Food Core Auth

API serveless para authenticação e permissão de usuários de restaurantes fast-food, desenvolvida como parte do curso de Arquitetura de Software
da FIAP (Tech Challenge).

<div align="center">
  <a href="#visao-geral">Visão Geral</a> •
  <a href="#tecnologias">Tecnologias</a> •
  <a href="#fluxo-de-autenticacao">Fluxo de Autenticação</a> •
  <a href="#exemplo-de-fluxo">Exemplo de Fluxo</a>
</div><br>

# 🔑 Lambda de Autenticação - Identificação via CPF (C# + Cognito)

## 📖 Visão Geral

A Lambda é responsável pela **identificação de clientes** no sistema de autoatendimento.
Ela recebe o **CPF** do cliente, consulta o **Cognito**, gera um **JWT** e retorna o token para o **API Gateway (APIM)**, que repassa a chamada para a **FoodCore API**.

## 🚀 Tecnologias

- **C# .NET 9 AWS Lambda Runtime**
- **Azure APIM** (API Gateway)
- **AWS Cognito** (identificação/autenticação sem senha, apenas CPF ou Email)
- **JWT** para comunicação segura
- **GitHub Actions + Terraform** para deploy

## 🔄 Fluxo de Autenticação de clientes

1. O usuário informa **CPF ou EMAIL** no frontend.
2. A requisição chega no **APIM**, que redireciona para a **Azure Function (Lambda em C#)**.
3. O Cognito gera um **JWT**.
4. A **Azure Function** valida:
   - Assinatura do token via **JWKS público da AWS**
   - Se o usuário tem permissão de acessar o path solicitado (com base na Role)
   - O mecanismo é **implicit deny** (qualquer falha = acesso negado).
   - Se o token for válido, a Function retorna um body semelhante a esse:

    ```json
    {
      "subject": "a1b2c3d4-e5f6-7890-abcd-1234567890ef",
      "name": "João da Silva",
      "email": "joao.silva@example.com",
      "cpf": "12345678900",
      "role": "ADMIN",
      "createdAt": "2025-10-02T09:30:00Z"
    }
    ```

5. O **APIM** repassa a requisição com o **JWT** e todos os atributos retornados pela lambda em headers HTTP para a **FoodCore API**.

## 🧩 Exemplo de Fluxo (cliente)

```mermaid
sequenceDiagram
    participant User
    participant APIM
    participant Lambda
    participant Cognito
    participant API

    User->>APIM: POST /login {cpf} ou {email}
    APIM->>Lambda: Invoca função com CPF ou EMAIL
    Lambda->>Cognito: Consulta cliente
    Cognito-->>Lambda: Retorna dados
    Lambda-->>APIM: Retorna JWT
    APIM->>API: Chamada autenticada com JWT
    API-->>User: Retorna dados do pedido

