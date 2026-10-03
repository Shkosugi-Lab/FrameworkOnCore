// WebFormsForCore: the member .NET Framework 4.8's SqlParameterCollection had and .NET's System.Data.SqlClient left out
// (referencesource, System.Data's SqlParameterCollection): obsolete since .NET Framework 2.0 (AddWithValue is the same),
// still in applications and libraries built for .NET Framework, which fail with MissingMethodException without it.

namespace System.Data.SqlClient
{
    public sealed partial class SqlParameterCollection
    {
        [Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).  http://go.microsoft.com/fwlink/?linkid=14202", false)]
        public SqlParameter Add(string parameterName, object value)
        {
            return Add(new SqlParameter(parameterName, value));
        }
    }
}
