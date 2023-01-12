using AdaptiveFEM.Services;
using System;

namespace AdaptiveFEM.Models
{
    public class Design
    {
        public string Name { get; init; }

        public Model Model { get; private set; }

        public Mesh Mesh { get; private set; }

        private readonly MessageService _messageService;

        public event EventHandler? DesignChanged;

        public Design(string name, MessageService messageService)
        {
            Name = name;
            Model = new Model(messageService);
            _messageService = messageService;
            Mesh = new Mesh(Model);
        }

        public void Reset()
        {
            Model = new Model(_messageService);
            OnDesignReset();
        }

        public bool AddDomain(Domain domain)
        {
            return Model.AddDomain(domain, OnComponentAdded);
        }

        public bool AddRegion(Region region)
        {
            return Model.AddRegion(region, OnComponentAdded);
        }

        private void OnComponentAdded(ComponentType componentType)
        {
            DesignChanged?.Invoke(this, new EventArgs());
            _messageService.SendSuccessMessage($"{componentType} added.");
        }

        private void OnDesignReset()
        {
            DesignChanged?.Invoke(this, new EventArgs());
            _messageService.SendInformationMessage("Design reset.");
        }
    }
}
