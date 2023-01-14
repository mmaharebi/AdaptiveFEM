using AdaptiveFEM.Services;
using System;
using System.Collections.Generic;

namespace AdaptiveFEM.Models
{
    public class Model
    {
        public Domain? Domain { get; private set; }


        private List<Region> _regions;

        public List<Region> Regions
        {
            get => new List<Region>(_regions);
            private set => _regions = value;
        }

        private readonly MessageService _messageService;

        public Model(MessageService messageService)
        {
            _messageService = messageService;

            Domain = null;
            _regions = new List<Region>();
        }

        public bool AddDomain(Domain domain, Action<Component> DomainAdded)
        {
            if (Domain != null)
            {
                return false;
            }
            Domain = domain;
            DomainAdded(Domain);
            return true;
        }

        public bool AddRegion(Region region, Action<Component> RegionAdded)
        {
            if (Domain == null)
            {
                _messageService.SendErrorMessage("Domain is null here.");
                return false;
            }
            else if (region.Conflicts(Domain))
            {
                _messageService.SendErrorMessage($"Region conflicts with domain.");
                return false;
            }
            else
            {
                foreach (Region existingRegion in Regions)
                {
                    if (existingRegion.Conflicts(region))
                    {
                        _messageService.SendErrorMessage($"Region intersects with existing regions.");
                        return false;
                    }
                }
                _regions.Add(region);
                RegionAdded(region);
                return true;
            }
        }
    }
}
