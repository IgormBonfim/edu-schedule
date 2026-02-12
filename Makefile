.PHONY: help up down test logs clean restart api-bash

PRINT_COLOR = \033[36m
END_COLOR = \033[0m

help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(PRINT_COLOR)%-20s$(END_COLOR) %s\n", $$1, $$2}'

up:
	docker compose up --build -d
	@echo "------------------------------------------------"
	@echo "Ambiente rodando! 🚀"
	@echo "Frontend: http://localhost:5173"
	@echo "Swagger:  http://localhost:5077/swagger"
	@echo "Hangfire: http://localhost:5077/hangfire"
	@echo "------------------------------------------------"

down:
	docker compose down

test:
	docker compose run --rm tests

logs:
	docker compose logs -f

logs-api:
	docker compose logs -f api

clean:
	@echo "Limpando tudo (inclusive dados do banco)..."
	docker compose down -v --remove-orphans

restart: down up

api-bash:
	docker compose exec api /bin/sh