# FCG.Api.Payments

**Tech Challenge - Fase 2**  
API simples para simulação de processamento de pagamentos.

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