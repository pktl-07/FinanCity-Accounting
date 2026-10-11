namespace FC.Core.Data
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class RecordIndexerAttribute : Attribute
    {
        bool _autoGenerateIdReferencesOnSave = false;
        bool _autoGenerateIdReferencesOnPost = false;

        public RecordIndexerAttribute() { }
        
        public bool AutoGenerateIdReferencesOnSave
        {
            get { return _autoGenerateIdReferencesOnSave; }
            set { _autoGenerateIdReferencesOnSave = value; }
        }

        public bool AutoGenerateIdReferencesOnPost
        {
            get { return _autoGenerateIdReferencesOnPost; }
            set { _autoGenerateIdReferencesOnPost = value; }
        }
    }   
}
