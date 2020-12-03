using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class StepFactory
    {
        private IEnumerable<Type> FindClassesDeriving(Type type)
        {
            return (Assembly.GetAssembly(type)).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(type));
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


