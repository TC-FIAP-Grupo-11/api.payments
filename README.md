# FCG.Api.Payments

**Tech Challenge - Fase 3**
API de processamento de pagamentos — event-driven via RabbitMQ, com invocação direta de Lambda.

> **⚠️ Este microsserviço faz parte de um sistema maior.**  
> Para executar toda a plataforma (Docker Compose ou Kubernetes), veja: [FCG.Infra.Orchestration](../FCG.Infra.Orchestration/README.md)

## Propósito

Esta API simula o processamento de pagamentos com cartão de crédito. Os pagamentos **NÃO são processados de verdade**. Apenas simulados e logados no console.

## Variáveis de Ambiente

```bash
# RabbitMQ
Messaging__RabbitMQ__Host="localhost"
Messaging__RabbitMQ__Username="guest"
Messaging__RabbitMQ__Password="guest"
```

## Como Executar

### Localmente
```bash
cd src/FCG.Api.Payments
dotnet run
```

Acesse: http://localhost:5003/health

### Docker
```bash
docker build -t fcg-payments .
docker run -p 5003:80 fcg-payments
```

---

## Fase 3 — Novidades

### Fluxo de Pagamento
1. `OrderPlacedEventConsumer` consome evento do RabbitMQ
2. `IPaymentService.ProcessPaymentAsync` simula o processamento
3. `ILambdaNotificationService` invoca `FCG.Lambda.Notification` diretamente via AWS SDK (fire-and-forget) com evento `PaymentProcessed`

### Endpoint REST
`POST /api/payments/process` — permite processamento direto de pagamento (sem fila).

### Variáveis de Ambiente (Fase 3)

| Variável | Descrição |
|----------|-----------|
| `AWS_REGION` | Região AWS (ex: `us-east-1`) |
| `AWS_ACCESS_KEY_ID` | Credencial AWS |
| `AWS_SECRET_ACCESS_KEY` | Credencial AWS |
| `AWS_NOTIFICATION_LAMBDA_NAME` | Nome da Lambda (ex: `fcg-notification-sender`) |

### Observabilidade
- **AWS X-Ray**: middleware `app.UseXRay("fcg-payments-api")` habilitado

### CI/CD (GitHub Actions)
- **CI** (`.github/workflows/ci.yml`): build em push/PR na `main`
- **CD** (`.github/workflows/cd.yml`): build Docker → push ECR → `kubectl set image` no EKS

**Secrets obrigatórios no repositório GitHub:**
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`

### Kubernetes
Manifests em `k8s/`:
- `deployment.yaml` — Deployment
- `service.yaml` — Service NLB interno (integrado ao AWS API Gateway via VPC Link)
- `configmap.yaml` — RabbitMQ host e credenciais