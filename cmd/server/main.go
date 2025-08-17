package main

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/config"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/infra/db"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/infra/http"
	repoInfra "github.com/FIAP-SOAT-TECH-TEAM/ms-auth/infra/repository"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/usecase"
	"github.com/gin-gonic/gin"
	"log"
)

func main() {
	if err := config.LoadEnv(); err != nil {
		log.Fatal(err)
	}

	database := db.NewPostgres()

	userRepo := repoInfra.NewUserRepositoryGorm(database)
	authUC := usecase.NewAuthUsecase(userRepo)
	authHandler := http.NewAuthHandler(authUC)

	r := gin.Default()
	r.POST("/users", authHandler.Register)
	r.POST("/users/login", authHandler.Login)

	r.Run(":8080")
}
