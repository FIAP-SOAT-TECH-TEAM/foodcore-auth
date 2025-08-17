package db

import (
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
	"gorm.io/gorm/logger"
	"log"
	"os"
)

func NewPostgres() *gorm.DB {
	dsn := "host=" + os.Getenv("DB_SERVER") +
		" user=" + os.Getenv("DB_USER") +
		" password=" + os.Getenv("DB_PASSWORD") +
		" dbname=" + os.Getenv("DB_SCHEMA") +
		" port=5432 sslmode=disable TimeZone=America/Sao_Paulo"

	db, err := gorm.Open(postgres.Open(dsn), &gorm.Config{
		Logger: logger.Default.LogMode(logger.Error),
	})
	if err != nil {
		log.Fatalf("❌ erro ao conectar no banco: %v", err)
	}

	//if err := db.AutoMigrate(&domain.User{}, &domain.Role{}); err != nil {
	//	log.Fatalf("❌ erro ao migrar tabelas: %v", err)
	//}

	return db
}
