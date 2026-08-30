using System.Collections.Generic;

namespace DefaultsProbe
{
    /// <summary>ObjectDataSource 検証用のデータクラス(宣言的バインドの TypeName ターゲット)。</summary>
    public class ProbeData
    {
        public List<ProbeItem> GetItems()
        {
            return new List<ProbeItem>
            {
                new ProbeItem { Name = "表", Price = 1m },
                new ProbeItem { Name = "裏", Price = 2m },
            };
        }
    }

    public class ProbeItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
