using System;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;

namespace WebControlsProbe
{
    /// <summary>
    /// The web parts' personalization, in memory (web.config's personalization provider: WebPartsScenario.aspx): each page's
    /// shared and per-user data as the web part manager saves it. Reset() forgets all of it (each scenario starts from it).
    /// </summary>
    public sealed class ProbePersonalizationProvider : PersonalizationProvider
    {
        static readonly object gate = new object();
        static readonly Dictionary<string, byte[]> blobs = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public static void Reset()
        {
            lock (gate) blobs.Clear();
        }

        static string Key(string path, string userName) => path + "|" + (userName ?? "");

        public override string ApplicationName { get; set; } = "/";

        protected override void LoadPersonalizationBlobs(WebPartManager webPartManager, string path, string userName, ref byte[] sharedDataBlob, ref byte[] userDataBlob)
        {
            lock (gate)
            {
                blobs.TryGetValue(Key(path, null), out sharedDataBlob);
                userDataBlob = null;
                if (!string.IsNullOrEmpty(userName)) blobs.TryGetValue(Key(path, userName), out userDataBlob);
            }
        }

        protected override void ResetPersonalizationBlob(WebPartManager webPartManager, string path, string userName)
        {
            lock (gate) blobs.Remove(Key(path, userName));
        }

        protected override void SavePersonalizationBlob(WebPartManager webPartManager, string path, string userName, byte[] dataBlob)
        {
            lock (gate) blobs[Key(path, userName)] = dataBlob;
        }

        public override PersonalizationStateInfoCollection FindState(PersonalizationScope scope, PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return new PersonalizationStateInfoCollection();
        }

        public override int GetCountOfState(PersonalizationScope scope, PersonalizationStateQuery query) => 0;

        public override int ResetState(PersonalizationScope scope, string[] paths, string[] usernames) => 0;

        public override int ResetUserState(string path, DateTime userInactiveSinceDate) => 0;
    }
}
