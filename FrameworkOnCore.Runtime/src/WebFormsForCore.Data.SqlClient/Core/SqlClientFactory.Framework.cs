// WebFormsForCore: the data source enumerator .NET Framework 4.8's SqlClientFactory gave (SqlDataSourceEnumerator), which
// .NET's System.Data.SqlClient left out.

using System.Data.Common;
using System.Data.Sql;

namespace System.Data.SqlClient
{
    public sealed partial class SqlClientFactory
    {
        public override bool CanCreateDataSourceEnumerator => true;

        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => SqlDataSourceEnumerator.Instance;
    }
}
