package main

import (
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/http"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/repository"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/internal/usecase"
	"github.com/FIAP-SOAT-TECH-TEAM/ms-auth/pkg/database"

	"github.com/gin-gonic/gin"
)

func main() {
	db := database.InitDB()

	repo := repository.NewUserRepository(db)
	authUC := usecase.NewAuthUsecase(repo)
	handler := http.NewAuthHandler(authUC)

	r := gin.Default()
	r.POST("/register", handler.Register)
	r.POST("/login", handler.Login)

	r.Run(":8080")
}
