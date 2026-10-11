namespace FC.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class ModuleTypeAttribute : Attribute
    {
        public ModuleType ModuleType { get; private set; }

        public ModuleTypeAttribute(ModuleType moduleType)
        {
            ModuleType = moduleType;
        }
    }
}
