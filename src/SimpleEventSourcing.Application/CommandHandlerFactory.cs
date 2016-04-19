﻿using SimpleEventSourcing.Messages;
using SimpleEventSourcing.Messages.Store;
using SimpleEventSourcing.Domain.Store;
using SimpleEventSourcing.EventSourcing;
using SimpleEventSourcing.EventStore.InMemory;

using System;
using System.Collections.Generic;
using SimpleEventSourcing.EventSourcing.Exceptions;

namespace SimpleEventSourcing.Application
{
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly Dictionary<Type, Func<IHandler>> handlers = new Dictionary<Type, Func<IHandler>>();

        public CommandHandlerFactory(IEventStore eventStore)
        {
            Func<IRepository> newTransientRepo = () => new Repository(eventStore);

            handlers.Add(typeof(CreateNewCart), () => new ShoppingCartCommandHandler(newTransientRepo()));
        }

        public ICommandHandler<TCommand> Resolve<TCommand>() where TCommand : ICommand
        {
            if(handlers.ContainsKey(typeof(TCommand)))
            {
                var handler = handlers[typeof(TCommand)]() as ICommandHandler<TCommand>;
                if (handler != null)
                {
                    return handler;
                }
            }
            throw new NoCommandHandlerRegisteredException(typeof (TCommand));
        }
    }
}