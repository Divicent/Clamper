using System.Collections.Generic;
using Clamper.Core.Infrastructure.Actions.Abstract;
using Clamper.Core.Infrastructure.Interfaces;

namespace Clamper.Core.Infrastructure.Models
{
    public enum ModelStatus
    {
        JustInMemory = 1,
        Retrieved = 2,
        Deleted = 3,
        ToAdd = 4
    }

    public abstract class BaseModel
    {
        public HashSet<string> __UpdatedProperties { get; set; }
        public ModelStatus __DatabaseModelStatus { get; set; }
        public IUnitOfWork __DatabaseUnitOfWork { get; set; }
        public List<IAddAction> __ActionsToRunWhenAdding { get; set; } 
        public abstract void SetId(object id);

		/// <summary>
        /// Checks the status of the object , and registers as updated property
        /// </summary>
        /// <param name="propertyName">The updated property name</param>
		protected void __Updated(string propertyName) 
		{
		    if(__UpdatedProperties == null)
                __UpdatedProperties = new HashSet<string>();

			if( __DatabaseModelStatus == ModelStatus.Retrieved ) 
				__UpdatedProperties.Add(propertyName);
		}
    }
}

