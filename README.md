# Payment Gateway

E-Commerce Payment Gateway, an API based application that will allow merchant to offer a way for their shoppers to pay for their product

## In Scope
- Payment Gateway API service
    - Validating Request
    - Store, Retreive Card Information (Encryption and Masking)
    - Forward Payment to Bank
    - Store Forwarding Response from Aquiring Bank
    - Accept Respone from Aquiring Bank

## Out of Scope
- Aquirer Bank API (can be mocked or stubbed)

## Tech Summary
- SDK used - .NET Core 8.0.204
- ASP .NET Core
- Entity Framework (in memory)
- MassTransit producer/consumer (in memory)
- FluentValidation
- xUnit for testing
- Stubbery for API stubbing
- Polly for Retry policy
- Idempotency feature
- Concurrency token on column

## Assumptions
1. Payment processing will be asynchronous
2. Payment request to Aquirer Bank will be integrated through asynchronous API
3. A WebHook will be exposed for Aquirer Bank to call back (Success, Failure, or any message communication)

### General Architecture
A simple architecture with API, Pub/Sub and relational database.

POST: Payment[SendToBank] : 
* API > Validation<br>
  + => Saves To DB(column encryption)<br>
    + => Event Producer<br>
      + => In Memory Bus<br>
        + => Event Consumer<br>
          + => Bank API Call<br>
            + => Update Payment details<br>
`

GET Payment:
* API => Validation
  + => Get from DB (masking and decryption)

POST: WebHook[ResponseFromBank] :
* WehHook API
  + => Event Producer
    + => In Memory Bus
      + => Event Consumber
        + => Update Payment details [SUCCESS or FAILED]

The API and event part can also be separately host if needed.

## Setup
- Startup Project `Payment.Api.csproj`
- Bank API Stub - `Payment.Bank.Stub.csproj`

The solution starts along with the Bank API Stub so it is self sufficient. It uses in-memory database(EF Core) amd in-memory bus (MassTransit). Ther is no container dependencies.

##### Success Scenario:
#### Step 1: Create a Payment
```
curl -X 'POST' \
  'https://localhost:7266/v1.0/Payments' \
  -H 'accept: */*' \
  -H 'IdempotencyKey: 12343' \
  -H 'Content-Type: application/json' \
  -d '{
  "source": {
    "type": "CreditCard",
    "number": "1111222233334444",
    "expiryMonth": 4,
    "expiryYear": 2024,
    "name": "John Doe"
  },
  "type": "OneTime",
  "amount": 1000,
  "currency": "GBP",
  "merchantId": "Amazon_123",
  "reference": "App_Clothes_123"
}'
```
#### Response: 202
```
{
    "id": "dc168f0f-2468-4e82-a03d-3f3668171526",
    "status": "EXECUTING"
}
`

Step 2: Get the Payment
`
curl -X 'GET' \
  'https://localhost:7266/v1.0/Payments/dc168f0f-2468-4e82-a03d-3f3668171526' \
  -H 'accept: */*'
`
#### Response: 200
`
{
    "id": "dc168f0f-2468-4e82-a03d-3f3668171526",
    "status": "EXECUTING",
    "source": {
        "maskedCardNumber": "xxxxxxxxxxxx4444"
    },
    "type": "OneTime",
    "amount": 1000,
    "currency": "GBP",
    "merchantId": "Amazon_123",
    "reference": "App_Clothes_123",
    "response": "Test Response",
    "createdDateTime": "2024-04-17T02:25:03.8728323+00:00",
    "lastChangedDateTime": "2024-04-17T02:25:03.8728889+00:00"
}
```

#### Step 3: Call WebHook to update Payment Status
```
curl --location --request POST 'https://localhost:7266/v1.0/WebHook' \
--header 'IdempotencyKey: 435466' \
--header 'Content-Type: application/json' \
--data-raw '{
  "id": "dc168f0f-2468-4e82-a03d-3f3668171526",
  "status": "SUCCESS",
  "message": "PAYMENT_SUCCESSFUL"
}'
```
#### Response: 202
#### Step 4: Get the updated Payment details
```
curl -X 'GET' \
  'https://localhost:7266/v1.0/Payments/dc168f0f-2468-4e82-a03d-3f3668171526' \
  -H 'accept: */*'
```
#### Response: 200
```
{
    "id": "dc168f0f-2468-4e82-a03d-3f3668171526",
    "status": "SUCCESS",
    "source": {
        "maskedCardNumber": "xxxxxxxxxxxx4444"
    },
    "type": "OneTime",
    "amount": 1000,
    "currency": "GBP",
    "merchantId": "Amazon_123",
    "reference": "App_Clothes_123",
    "response": "PAYMENT_SUCCESSFUL",
    "createdDateTime": "2024-04-17T02:25:03.8728323+00:00",
    "lastChangedDateTime": "2024-04-17T02:25:03.8728889+00:00"
}
```

### Sample Validation Error:
```
{
    "errors": {
        "Amount": [
            "Payment amount must be greater than 0."
        ],
        "Source": [
            "The Source field is required.",
            "'Source' must not be empty.",
            "Credit card has already expired."
        ],
        "Currency": [
            "The Currency field is required.",
            "Currency should be a valid ISO 4217 code."
        ],
        "Reference": [
            "The Reference field is required.",
            "'Reference' should not be null or empty."
        ],
        "MerchantId": [
            "The MerchantId field is required.",
            "'MerchantId' should not be null or empty."
        ],
        "Source.ExpiryYear": [
            "Year should be in 4 digit format e.g. 1999."
        ],
        "Source.ExpiryMonth": [
            "Expiry month should be between 1 and 12 (January to December)."
        ]
    },
    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-e3cdf4e7d804ea50024d7c6d2360def2-1c4e19b0dc48a6f7-00"
}
```

## Future Enhancement
1. Implement Outbox and Inbox pattern for processing outgoing and incoming events
2. Authorization e.g. Checking if Merchant Id is same as the merchant authorized for Payment
3. Replace in memory with actual service dependencies
4. Retry and Dead letter for events processing
5. Open Telemetry and tracing configuration
6. Refine Bank API payload
7. Validation of State change e.g. PaymentStatus
8. Database migration
9. Support for Recurring Payment