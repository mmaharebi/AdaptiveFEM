using AdaptiveFEM.Services;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class Design
    {
        public string Name { get; init; }

        public Model Model { get; private set; }

        public Solution Solution { get; private set; }

        private readonly MessageService _messageService;

        public event EventHandler? DesignChanged;

        public event EventHandler<Component>? ComponentAdded;

        public event EventHandler? DesignReset;

        public Design(string name, MessageService messageService)
        {
            Name = name;
            Model = new Model(messageService);
            Solution = new Solution(this);
            _messageService = messageService;
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

        private void OnComponentAdded(Component component)
        {
            DesignChanged?.Invoke(this, new EventArgs());
            ComponentAdded?.Invoke(this, component);
            _messageService.SendSuccessMessage($"{nameof(component)} added successfully.");
        }

        private void OnDesignReset()
        {
            DesignReset?.Invoke(this, new EventArgs());
            DesignChanged?.Invoke(this, new EventArgs());
            _messageService.SendInformationMessage("Design reset.");
        }
    }
}
