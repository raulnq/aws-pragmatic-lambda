﻿using Amazon.Lambda.Annotations;
using Amazon.Lambda.SQSEvents;
using FluentValidation;
using MyECommerceApp.ClientRequests;
using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.Clients
{
    public static class RegisterClient
    {
        public class Command
        {
            public Guid ClientId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class Result
        {
            public Guid ClientId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(command => command.Name).MaximumLength(100).NotEmpty();
                RuleFor(command => command.Address).MaximumLength(500).NotEmpty();
                RuleFor(command => command.PhoneNumber).MaximumLength(20).NotEmpty();
            }
        }

        public class Handler
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public Task<Result> Handle(Command command)
            {
                var clientRequest = new Client(command.ClientId, command.Name, command.Address, command.PhoneNumber);

                _context.Add(clientRequest);

                return Task.FromResult(new Result()
                {
                    ClientId = clientRequest.ClientId
                });
            }
        }
    }

    public class RegisterClientFunction : BaseFunction
    {
        [LambdaFunction]
        public Task<SQSBatchResponse> Handle(
            [FromServices] TransactionBehavior behavior,
            [FromServices] RegisterClient.Handler handler,
            [FromServices] GetClientRequest.Runner runner,
            SQSEvent sqsEvent)
        {
            return HandleFromSubscription<ClientRequestApproved>(async (clientRequestApproved) =>
            {
                var clientRequest = await runner.Run(new GetClientRequest.Query() { ClientRequestId = clientRequestApproved.ClientRequestId });
                var command = new RegisterClient.Command()
                {
                    ClientId = clientRequest.ClientRequestId,
                    Address = clientRequest.Address,
                    Name = clientRequest.Name,
                    PhoneNumber = clientRequest.PhoneNumber,
                };
                new RegisterClient.Validator().ValidateAndThrow(command);
                await behavior.Handle(() => handler.Handle(command));
            }, sqsEvent);
        }
    }
}
