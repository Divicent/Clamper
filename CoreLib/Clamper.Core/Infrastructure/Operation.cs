using Clamper.Core.Infrastructure.Interfaces;
using Clamper.Core.Infrastructure.Models;

namespace Clamper.Core.Infrastructure
{
    internal class Operation : IOperation
    {
        internal Operation(OperationType type, BaseModel model)
        {
            Type = type;
            Object = model;
        }

        public OperationType Type { get; }
        public BaseModel Object { get; }
    }
}

