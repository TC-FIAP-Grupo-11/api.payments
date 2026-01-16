# FCG.Api.Payments

API simples para simulação de processamento de pagamentos.

## Propósito

Esta API simula o processamento de pagamentos com cartão de crédito. Os pagamentos **NÃO são processados de verdade**. Apenas simulados e logados no console.

## Como executar

```bash
cd src/FCG.Api.Payments
dotnet run
```

Acesse: http://localhost:5001/swagger

## Endpoints

**POST /api/payments/process**

```json
{
  "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 99.90,
  "paymentMethod": "CreditCard",
  "cardNumber": "4111111111111111",
  "cardHolderName": "John Doe",
  "expiryDate": "12/25",
  "cvv": "123"
}
```

**Simulação de falha**: Use cartão começando com `0000` para simular pagamento recusado.

**GET /api/payments/{paymentId}**

Consulta status de um pagamento (sempre retorna APPROVED nesta versão simulada).
