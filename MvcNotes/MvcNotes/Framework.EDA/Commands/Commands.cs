using System;
using Domain.Entities;
using Entity;
using President = Entity.President;

namespace MvcNotes.Commands
{
    public class AddPresidentCommand : Command
    {
        private readonly string _id;

        public AddPresidentCommand(string id, President president)
        {
            _id = id;
            President = president;
            base.Id = Guid.NewGuid();
        }

        public President President { get; private set; }

        public string Id
        {
            get { return _id; }
        }
    }

    public class RemovePresidentCommand : Command
    {
        private readonly string _id;

        public RemovePresidentCommand(string id)
        {
            _id = id;
            base.Id = Guid.NewGuid();
        }

        public string Id
        {
            get { return _id; }
        }
    }
}