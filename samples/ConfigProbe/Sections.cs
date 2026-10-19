using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml;

namespace ConfigProbe
{
    /// <summary>The old way (.NET 1.x): the section's XML node as the handler is given it, its comments and whitespace included.</summary>
    public class LegacyHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var lines = new List<string>();
            lines.Add("parent=" + (parent == null ? "(null)" : parent.GetType().Name));
            lines.Add("context=" + (configContext == null ? "(null)" : configContext.GetType().FullName));
            lines.Add("name=" + section.Name + " version=" + section.Attributes["version"]?.Value);
            foreach (XmlNode child in section.ChildNodes)
                lines.Add(child.NodeType + ":" + child.Name + "=" + (child.NodeType == XmlNodeType.Element ? child.InnerText : child.Value));
            lines.Add("outer=" + section.OuterXml);
            return lines;
        }
    }

    public enum ShopKind { Retail, Online }

    public class ShopSection : ConfigurationSection
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => (string)this["name"];

        [ConfigurationProperty("maxItems", DefaultValue = 10)]
        [IntegerValidator(MinValue = 1, MaxValue = 100)]
        public int MaxItems => (int)this["maxItems"];

        [ConfigurationProperty("opened")]
        public DateTime Opened => (DateTime)this["opened"];

        [ConfigurationProperty("ratio", DefaultValue = 0.25)]
        public double Ratio => (double)this["ratio"];

        [ConfigurationProperty("kind", DefaultValue = ShopKind.Online)]
        public ShopKind Kind => (ShopKind)this["kind"];

        [ConfigurationProperty("timeout", DefaultValue = "00:00:10")]
        [TypeConverter(typeof(TimeSpanConverter))]
        public TimeSpan Timeout => (TimeSpan)this["timeout"];

        [ConfigurationProperty("products")]
        [ConfigurationCollection(typeof(ProductElement))]
        public ProductCollection Products => (ProductCollection)this["products"];
    }

    public class ProductElement : ConfigurationElement
    {
        [ConfigurationProperty("sku", IsKey = true, IsRequired = true)]
        public string Sku => (string)this["sku"];

        [ConfigurationProperty("price")]
        public decimal Price => (decimal)this["price"];
    }

    public class ProductCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new ProductElement();
        protected override object GetElementKey(ConfigurationElement element) => ((ProductElement)element).Sku;
    }
}
