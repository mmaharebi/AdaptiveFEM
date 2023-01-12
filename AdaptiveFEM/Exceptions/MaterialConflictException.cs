using AdaptiveFEM.Models.Materials;
using System;
using System.Runtime.Serialization;

namespace AdaptiveFEM.Exceptions
{
    public class MaterialConflictException : Exception
    {
        private Material _exisingMaterial;
        private Material _incomingMaterial;

        public MaterialConflictException(Material exisingMaterial,
            Material incomingMaterial)
        {
            _exisingMaterial = exisingMaterial;
            _incomingMaterial = incomingMaterial;
        }

        public MaterialConflictException(string? message,
            Material exisingMaterial,
            Material incomingMaterial) : base(message)
        {
            _exisingMaterial = exisingMaterial;
            _incomingMaterial = incomingMaterial;
        }

        public MaterialConflictException(string? message,
            Exception? innerException,
            Material exisingMaterial,
            Material incomingMaterial) : base(message, innerException)
        {
            _exisingMaterial = exisingMaterial;
            _incomingMaterial = incomingMaterial;
        }

        protected MaterialConflictException(SerializationInfo info,
            StreamingContext context,
            Material exisingMaterial,
            Material incomingMaterial) : base(info, context)
        {
            _exisingMaterial = exisingMaterial;
            _incomingMaterial = incomingMaterial;
        }
    }
}
