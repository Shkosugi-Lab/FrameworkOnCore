#if !WebFormsForCore
using System.Data.Linq;
using System.Data.Objects;
#endif
using System.Globalization;
using System.Web.Resources;

namespace System.Web.DynamicData.ModelProviders {
    internal class SchemaCreator {
        private static SchemaCreator s_instance = new SchemaCreator();

        public static SchemaCreator Instance {
            get {
                return s_instance;
            }
        }

        public virtual DataModelProvider CreateDataModel(object contextInstance, Func<object> contextFactory) {
#if !WebFormsForCore
            if (IsDataContext(contextInstance.GetType())) {
                return new DLinqDataModelProvider(contextInstance, contextFactory);
            }
            if (IsObjectContext(contextInstance.GetType())) {
                return new EFDataModelProvider(contextInstance, contextFactory);
            }
#endif
            // .NET has neither LINQ to SQL's DataContext nor EF6's ObjectContext, so no context
            // model is recognized; a data model registered from one fails here as on .NET
            // Framework with an unknown context type. The reflection-based model that ItemType
            // uses (SimpleModelProvider) does not go through here.
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, DynamicDataResources.SchemaCreator_UnknownModel, contextInstance.GetType().FullName));
        }

        public virtual bool ValidDataContextType(Type contextType) {
            // 
            return IsDataContext(contextType) || IsObjectContext(contextType);
        }

#if !WebFormsForCore
        internal static bool IsDataContext(Type contextType) {
            return IsValidType<DataContext>(contextType);
        }

        internal static bool IsObjectContext(Type contextType) {
            return IsValidType<ObjectContext>(contextType);
        }

        private static bool IsValidType<T>(Type contextType) where T : class {
            return contextType != null && typeof(T).IsAssignableFrom(contextType);
        }
#else
        internal static bool IsDataContext(Type contextType) => false;

        internal static bool IsObjectContext(Type contextType) => false;
#endif
    }
}
