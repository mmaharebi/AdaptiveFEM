using AdaptiveFEM.Models;
using System;
using System.Runtime.Serialization;

namespace AdaptiveFEM.Exceptions
{
    public class RegionConflictException : Exception
    {
        private Region _existingRegion;
        private Region _incomingRegion;

        public RegionConflictException(Region existingRegion,
            Region incomingRegion)
        {
            _existingRegion = existingRegion;
            _incomingRegion = incomingRegion;
        }

        public RegionConflictException(string? message,
            Region existingRegion,
            Region incomingRegion) : base(message)
        {
            _existingRegion = existingRegion;
            _incomingRegion = incomingRegion;
        }

        public RegionConflictException(string? message,
            Exception? innerException,
            Region existingRegion,
            Region incomingRegion) : base(message, innerException)
        {
            _existingRegion = existingRegion;
            _incomingRegion = incomingRegion;
        }

        protected RegionConflictException(SerializationInfo info,
            StreamingContext context,
            Region existingRegion,
            Region incomingRegion) : base(info, context)
        {
            _existingRegion = existingRegion;
            _incomingRegion = incomingRegion;
        }
    }
}
