## Description

This is an implementation of a simple ledger. The following are the features implemented:
- Record transactions - Deposits and Withdrawals - for an account
- View current balance for an account
- View transaction history for an account

## Assumptions

For simplicity sake, the deposit and withdrawal transactions will only contain the following properties
- Account Id
- Amount
- TransactionDate
- Reference
- Correlation Id

It is also assumed that a single API will suffice without the need to separate the read and write operations in separate services.

Validations are done for each request. If the validation fails, the request is rejected (Bad Request).

#### Posting deposits
- An account is created, if not yet existing, on the first deposit.
- Each deposit request contains a correlation id. If the correlation id is found in the deposit history, the deposit is rejected (Conflict). This ensures that the transactions are idempotent.

#### Posting withdrawals
- If the account does not exist, the withdrawal request is rejected (Not Found).
- Same with deposits, each withdrawal request contains a correlation id. If the correlation id is found in the withdrawal history, the withdrawal is rejected. 
- If the withdrawal amount is greater than the current balance, the withdrawal request is rejected (Bad Request).

#### Getting current balance
- If the account does not exist, an error is returned (Not Found).

#### Getting transaction history
- If the account does not exist, an error is returned (Not Found).
- For simplicity sake, all transactions are returned ordered descending by TransactionDate. Pagination is not implemented.

## Design
The solution was implemented following Clean architecture. The project is divided in the following layers:

### Domain layer
The domain layer is implemented in the SimpleLedger.Domain project. This contains the domain entities, value objects, and business exceptions.
This also contains the interface for the account repository.

Account is the aggregate root. It contains the business logic for deposits and withdrawals.

### Application layer
The application layer is implemented in the SimpleLedger.Application project. This contains the models and the implmenetation of the services.

##### Models
Following the CQRS pattern, we have separate models for all requests and responses. Separate requests are also done for each operation.

Each response implements the IResponse<TData, TResponseCode> interface. The TData type refers to the data returned by the response if successful. 
The TResponseCode type refers to an enum (or any type) that's used by the operation to indicate the status of the operation. 
Each operation has its own response codes enum.

#### Validators
Validators are implemented for each request. This is done using FluentValidation.

#### AccountService
This serves as the orchestrator for account related operations.

### Infrastructure layer
#### SimpleLedger.Infrastructure.AccountRepository
This contains the implementation of the account repository.For simplicity sake, the repository only stores in-memory.

### Presentation Layer
The presentation layer is implemented in the SimpleLedger project. The layer is only responsible for calling the application layer and converting responses to Http responses.

## Testing
Unit tests are implemented in this solution. The tests are implemented using xUnit and is using Moq and AutoFixture for mocking and creating test data.

The unit tests are implemented in 3 different projects, one for each layer.

I've skipped integration tests for this exercise as I prefer to focus on the unit tests. You may run tests using the test explorer in Visual Studio.

### SimpleLedger.Domain.Tests
Contains the business logic unit tests for the Account entity.
### SimpleLedger.Application.Tests
Contains the tests for the application layer services including tests for the validators.
### SimpleLedger.Tests
Contains the tests for controller. The AccountService is just mocked.

## Running the application
- Open the solution using Visual Studio or similar IDE and run the SimpleLedger project via IIS Express. 
- You can access swagger using http://localhost:47152/swagger/index.html
- From there you can run the tests against the API.
- Use the same account id for testing the GET operations.

Sample Post Deposit Request
```
{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "deposit": {
    "amount": 10,
    "transactionDate": "2025-02-19T20:07:46.129Z",
    "reference": "Test Deposit"
  },
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

Sample Post Withdrawal Request
```
{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "withdrawal": {
    "amount": 5,
    "transactionDate": "2025-02-19T21:08:46.736Z",
    "reference": "Test Withdrawal"
  },
  "correlationId": "c2f6e62a-1f8b-4424-9b80-29fcfe962a73"
}
```

## Design considerations and Improvements
The design is based on the assumption that the load is not that heavy and a single API can handle both the read and write operations.

- In the real world, the load on both posting the transactions and reads can be heavy and may be different. It probably is a good idea to have separate services for both. This allows us the scale each service indepdently.
- The read operations can be cached to improve performance. The read operations can read from dedicated read models, possibly in a NoSQL database like Redis or similar.
- The current implementation of posting deposits and withdrawals requires that the transactions are loaded to be able to do the logic, this might not be feasible when the data grows.
    - We can possibly use a domain service and move all the logic there from the Account entity.
    - OR possibly use a snapshot of the data
- Good idea to have integration tests as well for the API.