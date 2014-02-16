using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Common.Framework;
using DemoEDA;
using DemoEDA.Infrastructure;
using MvcNotes.Events;
using Domain;
using Domain.DataAccess;
using Domain.Entities;

namespace MvcNotes.Commands
{

    public class PresidentCommandHandlers :
        ICommandHandler<AddPresidentCommand>,
        ICommandHandler<RemovePresidentCommand>

    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventStore<Event> _eventStore;
        private readonly IPresidentUnitOfWork _presidentUnitOfWork;

        public PresidentCommandHandlers(IEventPublisher eventPublisher, IEventStore<Event> eventStore)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
            _presidentUnitOfWork =
                (DependencyResolver.Current as MefDependencyResolver).GetService<IPresidentUnitOfWork>();
        }

        public void Execute(AddPresidentCommand command)
        {

            Guard.NotNull(command, "command");
            Guard.NotNull(_presidentUnitOfWork, "Repository is not initialized.");

            Validator.ValidateObject(command.President, new ValidationContext(command.President), true);


            //Product product = command.President;
            //string shoppingCartId = command.Id;
            //int quantity = command.Quantity;

            //Cart cartItem = _presidentUnitOfWork.Carts.Find(
            //    c => c.CartId == shoppingCartId && c.ProductId == product.Id).FirstOrDefault();

            //if (cartItem == null)
            //{
            //    cartItem = new Cart
            //    {
            //        ProductId = product.Id,
            //        CartId = shoppingCartId,
            //        Count = quantity,
            //        DateCreated = DateTime.Now
            //    };
            //    _presidentUnitOfWork.Carts.Add(cartItem);
            //}
            //else
            //{
            //    cartItem.Count += quantity;
            //}

            //_eventStore.SaveEvent(((Command) command).Id, new ProductAddedEvent(product));
            //_presidentUnitOfWork.Commit();
        }

        public void Execute(RemovePresidentCommand command)
        {
            Guard.NotNull(command, "command");
            Guard.NotNull(_presidentUnitOfWork, "Repository is not initialized.");

            
            _presidentUnitOfWork.Commit();
        }
    }
}