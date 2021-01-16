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

        public List<IStep> GetStepsOfType<T>()
        {
            List<IStep> objects = new List<IStep>();
            foreach (Type type in FindClassesDeriving(typeof(T)))
            {
                objects.Add((IStep)Activator.CreateInstance(type));
            }
            return objects;
        }
    }
}


