# ms-auth

### Serviço de identificação de usuários

Esse microserviço é utilizado para fazer a autenticação dos usuários da aplicação [food-core-api](https://github.com/FIAP-SOAT-TECH-TEAM/food-core-api) 




### Estrutura do projeto

```
/ms-auth
/cmd
/server              → entrypoint
/internal
/domain              → entidades puras (User, Role)
/usecase             → casos de uso (Register, Login, etc.)
/repository          → interfaces (UserRepository)
/token               → serviço de JWT
/dto                 → request/response models
/infra
/db                  → conexão com banco (Postgres/GORM)
/repository          → implementações concretas (UserRepositoryImpl)
/http                → web framework (Gin Handlers, middlewares, etc.)
docker-compose.yml   → Arquivo docker para subir a aplicação
go.mod

├── Dockerfile
├── docker-compose.yml
├── cmd
│  └── server
│    └── main.go
├── config
│  └── config.go
├── go.mod
├── go.sum
├── infra
│   ├── db
│   │   └── postgres.go
│   ├── http
│   │   ├── error_map.go
│   │   └── handler.go
│   └── repository
│      └── user_repository.go
└── internal
    ├── domain
    │   ├── Role.go
    │   └── User.go
    ├── dto
    │   ├── register_user_input_dto.go
    │   └── user_presenter.go
    ├── exceptions
    │   └── user_exceptions.go
    ├── repository
    │   └── user_repository.go
    ├── shared
    │   └── audit_info.go
    ├── token
    │   └── token.go
    └── usecase
        └── auth_usecase.go
         
```