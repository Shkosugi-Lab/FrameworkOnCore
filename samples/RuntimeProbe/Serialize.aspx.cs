using System;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace RuntimeProbe
{
    public partial class Serialize : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // n2's ContentDetail, BlogEngine's extension settings: objects kept with BinaryFormatter. This one was written
            // by .NET Framework 4.8 (a Hashtable: a string, an int, a DateTime, a List<string>).
            var formatter = new BinaryFormatter();
            var table = (Hashtable)formatter.Deserialize(new MemoryStream(Convert.FromBase64String(Blob)));
            Response.Write("name=" + table["name"] + " count=" + table["count"] + " when=" + ((DateTime)table["when"]).ToString("s") + " list=" + string.Join(",", (List<string>)table["list"]) + "\n");
            var stream = new MemoryStream();
            formatter.Serialize(stream, table);
            stream.Position = 0;
            Response.Write("again=" + ((Hashtable)formatter.Deserialize(stream))["count"]);
        }

        const string Blob = "AAEAAAD/////AQAAAAAAAAAEAQAAABxTeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlBwAAAApMb2FkRmFjdG9yB1ZlcnNpb24IQ29tcGFyZXIQSGFzaENvZGVQcm92aWRlcghIYXNoU2l6ZQRLZXlzBlZhbHVlcwAAAwMABQULCBxTeXN0ZW0uQ29sbGVjdGlvbnMuSUNvbXBhcmVyJFN5c3RlbS5Db2xsZWN0aW9ucy5JSGFzaENvZGVQcm92aWRlcgjsUTg/BQAAAAoKBwAAAAkCAAAACQMAAAAQAgAAAAQAAAAGBAAAAAVjb3VudAYFAAAABGxpc3QGBgAAAARuYW1lBgcAAAAEd2hlbhADAAAABAAAAAgIKgAAAAkIAAAABgkAAAAJ5pel5pys6KqeCA0AYEf9qZ/MCAQIAAAAf1N5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLkxpc3RgMVtbU3lzdGVtLlN0cmluZywgbXNjb3JsaWIsIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5XV0DAAAABl9pdGVtcwVfc2l6ZQhfdmVyc2lvbgYAAAgICQoAAAACAAAAAgAAABEKAAAABAAAAAYLAAAAAWEGDAAAAAFiDQIL";
        void Unused()
        {
        }
    }
}
