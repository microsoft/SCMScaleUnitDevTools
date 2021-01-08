using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class StepFactory
    {
        private List<Type> FindClassesDeriving(Type type)
        {
            List<Type> res = new List<Type>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(type)))
                {
                    res.Add(t);
                }
            }

            return res;
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


