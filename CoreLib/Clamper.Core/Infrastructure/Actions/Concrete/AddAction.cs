
using System;
using Clamper.Core.Infrastructure.Actions.Abstract;

namespace Clamper.Core.Infrastructure.Actions.Concrete
{
    public class AddAction: IAddAction
    {
        private readonly Action<object> _action;
        private readonly object _parameter;

        public AddAction(Action<object> action, object parameter )
        {
            _action = action;
            _parameter = parameter;
        }

        public void Run()
        {
            if(_action == null || _parameter ==null)
            { return; }
            _action(_parameter);
        }
    }
}
