using System;
using System.Collections.Generic;
using System.Linq;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class StepFactory
    {
        private IEnumerable<Type> FindClassesDeriving(Type superType)
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where type.IsClass && !type.IsAbstract && type.IsSubclassOf(superType)
                   select type;
        }

        public List<Step> GetStepsOfType<T>()
        {
            List<Step> objects = new List<Step>();
            foreach (Type type in FindClassesDeriving(typeof(T)))
            {
                objects.Add((Step)Activator.CreateInstance(type));
            }
            return objects;
        }
    }
}


