using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zadanie_2
{
    public class DependencyConstructor : System.Attribute
    {
        
    }

    public class DependencyProperty : System.Attribute
    {

    }

    public class DependencyMethod : System.Attribute
    {
        
    }

    public class SimpleContainer
    {
        private Dictionary<Type, bool> IsSingleton;
        private Dictionary<Type, object> Singleton;
        private Dictionary<Type, Type> Implementation;
        private Dictionary<Type, object> Instances;

        public SimpleContainer()
        {
            this.IsSingleton = new Dictionary<Type, bool>();
            this.Singleton = new Dictionary<Type, object>();
            this.Implementation = new Dictionary<Type, Type>();
            this.Instances = new Dictionary<Type, object>();
        }

        public void RegisterType<T>(bool Singleton) where T : class
        {
            if (this.IsSingleton.ContainsKey(typeof(T))) {
                this.IsSingleton[typeof(T)] = Singleton;
            } else {
                this.IsSingleton.Add(typeof(T), Singleton);
            }
            // unregister instance
            if (this.Instances.ContainsKey(typeof(T))) {
                this.Instances.Remove(typeof(T));
            }
        }

        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            if (this.Implementation.ContainsKey(typeof(From))) {
                this.Implementation[typeof(From)] = typeof(To); 
            } else {
                this.Implementation.Add(typeof(From), typeof(To));
            }
            if (this.IsSingleton.ContainsKey(typeof(To))) {
                this.IsSingleton[typeof(To)] = Singleton;
            } else {
                this.IsSingleton.Add(typeof(To), Singleton);
            }
            // unregister instance
            if (this.Instances.ContainsKey(typeof(From))) {
                this.Instances.Remove(typeof(From));
            }
        }

        public void RegisterInstance<T>(T Instance)
        {
            if (this.Instances.ContainsKey(typeof(T))) {
                this.Instances[typeof(T)] = Instance;
            } else {
                this.Instances.Add(typeof(T), Instance);
            }
        }

        public T Resolve<T>()
        {
            return (T)ResolveRecursive(typeof(T), new HashSet<Type>(new[] {typeof(T)}));
        }

        public void BuildUp<T>(T Instance)
        {
            // recursively try to resolove properties
            this.ResolveProperties(Instance, new HashSet<Type>());

            // recursively try to resolove methods
            this.ResolveMethods(Instance, new HashSet<Type>());
        }

        private object ResolveRecursive(Type needed_type, HashSet<Type> UsedTypes)
        {
            // check if needed_type have registered instance
            if (this.Instances.ContainsKey(needed_type)) {
                return this.Instances[needed_type];
            }

            // check if needed_type was registered to change implementation
            if (this.Implementation.ContainsKey(needed_type)) {
                needed_type = this.Implementation[needed_type];
            } else {
                if (needed_type.IsInterface) {
                    throw new ArgumentException("Cannot create instance of interface!");
                } else if (needed_type.IsAbstract) {
                    throw new ArgumentException("Cannot create instance of abstract class!");
                }
            }

            // if needed_type was registered as Singleton
            if (this.IsSingleton.ContainsKey(needed_type)) {
                if (this.IsSingleton[needed_type]) {
                    // if object of type needed_type wasn't already created
                    // create one
                    if (!this.Singleton.ContainsKey(needed_type))
                        this.Singleton.Add(needed_type, Activator.CreateInstance(needed_type));
                    
                    return this.Singleton[needed_type];
                }
            }

            // search for constructor with many parameters
            int max_params = -1;
            ConstructorInfo used_constr = null;
            int marked_constr_cnt = 0;
            ConstructorInfo marked_constr = null;
            foreach (var constr in needed_type.GetConstructors()) {
                int constr_args_len = constr.GetParameters().Length;
                if (max_params < constr_args_len) {
                    used_constr = constr;
                    max_params = constr_args_len;
                }
                if (constr.GetCustomAttribute(typeof(DependencyConstructor), true) != null) {
                    marked_constr_cnt++;
                    marked_constr = constr;
                }
            }
            if (used_constr == null) {
                throw new ArgumentException("Cannot resolve this type, no constructors found!");
            }
            if (marked_constr_cnt == 1) {
                used_constr = marked_constr;
            }

            // recursively try to resolve parameters for constructor
            UsedTypes.Add(needed_type);
            object[] constr_params = new object[used_constr.GetParameters().Length];
            
            for (int i = 0; i < used_constr.GetParameters().Length; i++) {
                Type constr_param_type = used_constr.GetParameters()[i].ParameterType;
                if (UsedTypes.Contains(constr_param_type)) {
                    throw new ArgumentException("Cannot resolve, cycle apperead with type: " + constr_param_type.ToString());
                }
                constr_params[i] = ResolveRecursive(constr_param_type, new HashSet<Type>(UsedTypes));
            }

            object res_obj = used_constr.Invoke(constr_params);

            // recursively try to resolove properties
            this.ResolveProperties(res_obj, UsedTypes);

            // recursively try to resolove methods
            this.ResolveMethods(res_obj, UsedTypes);

            return res_obj;
        }

        private void ResolveProperties(object res_obj, HashSet<Type> UsedTypes)
        {
            foreach (var property in res_obj.GetType().GetProperties()) {
                if (property.CanWrite 
                    && 
                    property.GetCustomAttribute(typeof(DependencyProperty), true) != null
                    &&
                    property.GetSetMethod() != null) {
                    if (UsedTypes.Contains(property.PropertyType)) {
                        throw new ArgumentException("Cannot resolve, cycle apperead with type: " + property.PropertyType.ToString());
                    }
                    property.SetValue(res_obj, ResolveRecursive(property.PropertyType, new HashSet<Type>(UsedTypes)));
                }
            }
        }

        private void ResolveMethods(object res_obj, HashSet<Type> UsedTypes)
        {
            foreach (var method in res_obj.GetType().GetMethods()) {
                if (method.ReturnType == typeof(void)
                    &&
                    method.GetCustomAttribute(typeof(DependencyMethod), true) != null
                    &&
                    method.GetParameters().Length > 0) {
                    // recursively try to resolve parameters for method
                    object[] method_params = new object[method.GetParameters().Length];
            
                    for (int i = 0; i < method.GetParameters().Length; i++) {
                        Type method_param_type = method.GetParameters()[i].ParameterType;
                        if (UsedTypes.Contains(method_param_type)) {
                            throw new ArgumentException("Cannot resolve, cycle apperead with type: " + method_param_type.ToString());
                        }
                        method_params[i] = ResolveRecursive(method_param_type, new HashSet<Type>(UsedTypes));
                    }

                    method.Invoke(res_obj, method_params);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
