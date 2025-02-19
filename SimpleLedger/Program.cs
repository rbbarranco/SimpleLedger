using FluentValidation;
using SimpleLedger.Application.Models.Requests.GetCurrentBalance;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Validators.GetCurrentBalance;
using SimpleLedger.Application.Models.Validators.GetTransactionHistory;
using SimpleLedger.Application.Models.Validators.PostDeposit;
using SimpleLedger.Application.Models.Validators.PostWithdrawal;
using SimpleLedger.Application.Services;
using SimpleLedger.Domain.Interfaces;
using SimpleLedger.Infrastructure.AccountRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IAccountService, AccountService>();

builder.Services.AddSingleton<IValidator<Deposit>, DepositValidator>();
builder.Services.AddSingleton<IValidator<PostDepositRequest>, PostDepositRequestValidator>();
builder.Services.AddSingleton<IValidator<Withdrawal>, WithdrawalValidator>();
builder.Services.AddSingleton<IValidator<PostWithdrawalRequest>, PostWithdrawalRequestValidator>();
builder.Services.AddSingleton<IValidator<GetCurrentBalanceRequest>, GetCurrentBalanceRequestValidator>();
builder.Services.AddSingleton<IValidator<GetTransactionHistoryRequest>, GetTransactionHistoryRequestValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
