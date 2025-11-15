# Docker - Auria API

Este documento explica como construir e executar a aplicação Auria API usando Docker.

## Pré-requisitos

- Docker instalado ([Download Docker](https://www.docker.com/products/docker-desktop))
- Docker Compose (incluído no Docker Desktop)

## Arquivos Docker

- **Dockerfile**: Define a imagem Docker da aplicação
- **docker-compose.yml**: Orquestra os serviços
- **.dockerignore**: Especifica arquivos a serem ignorados no build
- **docker-build.ps1**: Script PowerShell para facilitar operações Docker

## Build da Imagem

### Usando Docker diretamente

```bash
docker build -t auria-api:latest .
```

### Usando o script PowerShell

```powershell
.\docker-build.ps1 build
```

### Usando Docker Compose

```bash
docker-compose build
```

## Executar a Aplicação

### Opção 1: Docker Run

```bash
# Iniciar container
docker run -d -p 5000:80 --name auria-api auria-api:latest

# Ver logs
docker logs -f auria-api

# Parar container
docker stop auria-api
docker rm auria-api
```

### Opção 2: Script PowerShell

```powershell
# Construir e iniciar
.\docker-build.ps1 build
.\docker-build.ps1 run

# Ver logs
.\docker-build.ps1 logs

# Parar
.\docker-build.ps1 stop
```

### Opção 3: Docker Compose (Recomendado)

```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar todos os serviços
docker-compose down
```

Ou usando o script:

```powershell
# Iniciar
.\docker-build.ps1 compose-up

# Parar
.\docker-build.ps1 compose-down
```

## Acessar a Aplicação

Após iniciar o container, a API estará disponível em:

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## Configuração

### Variáveis de Ambiente

O container usa as seguintes variáveis de ambiente (configuradas no docker-compose.yml):

- `ASPNETCORE_ENVIRONMENT`: Define o ambiente (Production, Development, etc.)
- `ASPNETCORE_URLS`: URLs em que a aplicação escuta
- `ConnectionString`: String de conexão do banco de dados
- `Jwt__SecretKey`: Chave secreta para JWT
- `Cloudinary__*`: Configurações do Cloudinary

### Customizar Configurações

1. **Via docker-compose.yml**: Adicione variáveis de ambiente na seção `environment`

```yaml
environment:
  - ConnectionString=sua-connection-string
  - Jwt__SecretKey=sua-chave-secreta
  - Cloudinary__CloudName=seu-cloud-name
```

2. **Via arquivo .env**: Crie um arquivo `.env` no mesmo diretório do docker-compose.yml

```env
CONNECTION_STRING=sua-connection-string
JWT_SECRET_KEY=sua-chave-secreta
CLOUDINARY_CLOUD_NAME=seu-cloud-name
```

E referencie no docker-compose.yml:

```yaml
environment:
  - ConnectionString=${CONNECTION_STRING}
  - Jwt__SecretKey=${JWT_SECRET_KEY}
```

## Volumes

Os logs da aplicação são persistidos em um volume:

```yaml
volumes:
  - ./logs:/app/logs
```

Isso permite que os logs sejam acessíveis no host em `./logs/`

## Multi-stage Build

O Dockerfile usa multi-stage build para otimizar o tamanho da imagem:

1. **Build Stage**: Usa SDK .NET 7.0 para compilar a aplicação
2. **Runtime Stage**: Usa ASP.NET Runtime 7.0 (mais leve) para executar

Isso resulta em uma imagem final menor e mais segura.

## Comandos Úteis

```bash
# Ver imagens Docker
docker images

# Ver containers em execução
docker ps

# Ver todos os containers
docker ps -a

# Remover imagem
docker rmi auria-api:latest

# Limpar containers parados
docker container prune

# Limpar imagens não utilizadas
docker image prune

# Ver uso de recursos do container
docker stats auria-api

# Executar comando dentro do container
docker exec -it auria-api bash
```

## Publicar em Registry

### Docker Hub

```bash
# Login
docker login

# Tag da imagem
docker tag auria-api:latest seu-usuario/auria-api:latest

# Push
docker push seu-usuario/auria-api:latest
```

### Azure Container Registry

```bash
# Login
az acr login --name seu-registry

# Tag
docker tag auria-api:latest seu-registry.azurecr.io/auria-api:latest

# Push
docker push seu-registry.azurecr.io/auria-api:latest
```

### AWS ECR

```bash
# Login
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin seu-account-id.dkr.ecr.us-east-1.amazonaws.com

# Tag
docker tag auria-api:latest seu-account-id.dkr.ecr.us-east-1.amazonaws.com/auria-api:latest

# Push
docker push seu-account-id.dkr.ecr.us-east-1.amazonaws.com/auria-api:latest
```

## Troubleshooting

### Porta já em uso

Se a porta 5000 já estiver em uso, altere no docker-compose.yml:

```yaml
ports:
  - "5001:80"  # Mude 5000 para outra porta
```

### Container não inicia

Verifique os logs:

```bash
docker logs auria-api
```

### Problemas de conexão com banco de dados

Certifique-se de que a ConnectionString está corretamente configurada e que o banco está acessível do container.

### Reconstruir imagem após mudanças

```bash
docker-compose up -d --build --force-recreate
```

## Otimizações

### Cache de Layers

O Dockerfile está otimizado para usar cache de layers do Docker. Mudanças no código não forçam reinstalação de dependências.

### .dockerignore

Arquivos desnecessários são excluídos do contexto de build via `.dockerignore`, acelerando o build.

### Health Check (Opcional)

Adicione ao docker-compose.yml:

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:80/health"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 40s
```

## Segurança

1. **Não inclua senhas no Dockerfile ou docker-compose.yml**
2. **Use variáveis de ambiente ou secrets**
3. **Mantenha a imagem base atualizada**
4. **Scan de vulnerabilidades**:

```bash
docker scan auria-api:latest
```

## Monitoramento

### Ver logs em tempo real

```bash
docker-compose logs -f auria-api
```

### Acessar shell do container

```bash
docker exec -it auria-api bash
```

### Ver processos

```bash
docker top auria-api
```
