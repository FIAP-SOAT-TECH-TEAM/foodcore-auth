# 🔑​ Food Core Auth

API serveless para authenticação e permissão de usuários de restaurantes fast-food, desenvolvida como parte do curso de Arquitetura de Software
da FIAP (Tech Challenge).

<div align="center">
  <a href="#visao-geral">Visão Geral</a> •
  <a href="#tecnologias">Tecnologias</a> •
  <a href="#autenticacao-de-clientes">Autenticação de clientes</a> •
  <a href="#autenticacao-de-administradores">Autenticação de administradores</a>
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

## 🔄 Autenticação de clientes

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

### Exemplo de Fluxo (cliente)

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
```

## 🧑‍💼 Autenticação de Administradores

Diferente dos clientes, administradores não autenticam via Lambda.
Eles utilizam diretamente a Hosted UI do AWS Cognito, onde realizam login com usuário e senha.

Os links da Hosted UI são expostos como outputs do Terraform e podem ser consultados no pipeline de CD (GitHub Actions) após o deploy.

### 🔗 Recuperando os links de autenticação

Nos outputs do Terraform, dois links são disponibilizados:

- **Hosted UI (Implicit Flow)**
 Realiza login e retorna o **JWT diretamente na URL** após a autenticação.

- **Hosted UI (Authorization Code Flow)**
Retorna um **código de autorização**, que deve ser trocado por um **JWT** via requisição de back-end.

### 🧭 Fluxos de Autenticação Cognito

🔸 Implicit Flow

Fluxo mais simples, retorna o token diretamente após o login.

Exemplo:

```mermaid
sequenceDiagram
    participant Admin
    participant Cognito
    participant APIM
    participant API

    Admin->>Cognito: Acessa Hosted UI (implicit URL)
    Cognito-->>Admin: Retorna JWT na URL (fragment)
    Admin->>APIM: Chamada autenticada com JWT
    APIM->>API: Repassa token válido
    API-->>Admin: Retorna dados administrativos
```

Exemplo de URL (output Terraform):

```bash
https://foodcore-auth-domain.auth.us-east-1.amazoncognito.com/login?
client_id=xxxxxxx&
response_type=token&
scope=email+openid+profile&
redirect_uri=https://foodcore.admin.app/login/callback
```

🔸 Authorization Code Flow

Fluxo mais seguro — retorna um código que o back-end troca por um JWT.
Esse método evita exposição do token diretamente na URL.

Exemplo:

```mermaid
sequenceDiagram
    participant Admin
    participant Cognito
    participant Backend
    participant APIM
    participant API

    Admin->>Cognito: Acessa Hosted UI (code URL)
    Cognito-->>Admin: Redireciona com Authorization Code
    Admin->>Backend: Envia code recebido
    Backend->>Cognito: Troca code por JWT
    Cognito-->>Backend: Retorna JWT
    Backend->>APIM: Chamada autenticada com JWT
    APIM->>API: Repassa token válido
    API-->>Backend: Retorna dados administrativos
```

Exemplo de URL (output Terraform):

```bash
https://foodcore-auth-domain.auth.us-east-1.amazoncognito.com/login?
client_id=xxxxxxx&
response_type=code&
scope=email+openid+profile&
redirect_uri=https://foodcore.admin.app/login/callback
```

✅ Resumo

| Tipo de Usuário   | Método de Login                           | Origem do JWT        | Meio de Validação                             |
| ----------------- | ----------------------------------------- | -------------------- | --------------------------------------------- |
| **Cliente**       | CPF/Email via Azure Function              | Cognito (via Lambda) | Azure Function valida assinatura e permissões |
| **Administrador** | Hosted UI Cognito (Implicit ou Code Flow) | Cognito Hosted UI    | APIM valida token via JWKS público da AWS     |
