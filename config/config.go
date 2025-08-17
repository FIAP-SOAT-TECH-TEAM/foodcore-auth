package config

import (
	"github.com/joho/godotenv"
	"github.com/kelseyhightower/envconfig"
	"log"
)

type Config struct {
	Host     string `envconfig:"DB_SERVER" envDefault:"localhost"`
	Port     string `envconfig:"DB_PORT" envDefault:"5432"`
	Username string `envconfig:"DB_USERNAME"`
	Password string `envconfig:"DB_PASSWORD"`
	Database string `envconfig:"DB_NAME"`
}

var Env Config

func LoadEnv() error {
	if err := godotenv.Load(); err != nil {
		log.Fatal("Não foi possível encontrar o arquivo .env. As variáveis de ambiente devem ser definidas externamente.")
	}

	if err := envconfig.Process("", &Env); err != nil {
		return err
	}
	return nil
}
