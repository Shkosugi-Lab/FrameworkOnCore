using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageProbe
{
    /// <summary>What every page of the probe writes: the events in order, as the server saw them.</summary>
    public static class Journal
    {
        public static void Add(string entry)
        {
            var items = HttpContext.Current.Items;
            var list = items["trace"] as List<string>;
            if (list == null) items["trace"] = list = new List<string>();
            list.Add(entry);
        }

        public static string Text()
        {
            var list = HttpContext.Current.Items["trace"] as List<string>;
            return list == null ? "" : string.Join("\n", list);
        }
    }

    /// <summary>A WebControl of its own tag (ul), attributes and contents; it logs its lifecycle.</summary>
    [ParseChildren(true, "Items")]
    public class TagList : WebControl
    {
        readonly List<ListItem> items = new List<ListItem>();

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<ListItem> Items { get { return items; } }

        public string Caption
        {
            get { return (string)ViewState["Caption"] ?? ""; }
            set { ViewState["Caption"] = value; }
        }

        protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Ul; } }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute("data-caption", Caption);
            writer.AddStyleAttribute(HtmlTextWriterStyle.ListStyleType, "square");
            base.AddAttributesToRender(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            foreach (var item in items)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.WriteEncodedText(item.Text);
                writer.RenderEndTag();
            }
        }

        protected override void OnInit(EventArgs e) { Journal.Add("TagList " + ID + " Init"); base.OnInit(e); }
        protected override void OnLoad(EventArgs e) { Journal.Add("TagList " + ID + " Load"); base.OnLoad(e); }
        protected override void OnPreRender(EventArgs e) { Journal.Add("TagList " + ID + " PreRender"); base.OnPreRender(e); }
    }

    /// <summary>A composite: a text box and a button made in CreateChildControls; the click bubbles a command.</summary>
    public class SearchBox : CompositeControl
    {
        TextBox text;

        public event CommandEventHandler Search;

        public string Text
        {
            get { EnsureChildControls(); return text.Text; }
            set { EnsureChildControls(); text.Text = value; }
        }

        protected override void CreateChildControls()
        {
            Journal.Add("SearchBox " + ID + " CreateChildControls");
            Controls.Clear();
            text = new TextBox { ID = "q" };
            var button = new Button { ID = "go", Text = "Search", CommandName = "search" };
            Controls.Add(new LiteralControl("<span class=\"search\">"));
            Controls.Add(text);
            Controls.Add(button);
            Controls.Add(new LiteralControl("</span>"));
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            var command = args as CommandEventArgs;
            if (command == null) return false;
            if (Search != null) Search(this, new CommandEventArgs(command.CommandName, text.Text));
            return true;
        }
    }

    /// <summary>Post data and a postback event of its own (IPostBackDataHandler, IPostBackEventHandler); control state
    /// that stays with EnableViewState off.</summary>
    public class Counter : Control, IPostBackDataHandler, IPostBackEventHandler
    {
        int count;
        string posted = "";

        public event EventHandler Changed;

        public int Count { get { return count; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
            Page.RegisterRequiresPostBack(this);
        }

        protected override object SaveControlState() { return new Pair(count, posted); }

        protected override void LoadControlState(object state)
        {
            var pair = (Pair)state;
            count = (int)pair.First;
            posted = (string)pair.Second;
        }

        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            var value = postCollection[UniqueID + "$note"] ?? "";
            Journal.Add("Counter LoadPostData '" + value + "'");
            if (value == posted) return false;
            posted = value;
            return true;
        }

        public void RaisePostDataChangedEvent()
        {
            Journal.Add("Counter RaisePostDataChangedEvent");
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            Journal.Add("Counter RaisePostBackEvent '" + eventArgument + "'");
            count += int.Parse(eventArgument);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<span id=\"" + ClientID + "\" class=\"counter\">count " + count + " ");
            writer.Write("<input type=\"text\" name=\"" + UniqueID + "$note\" value=\"" + HttpUtility.HtmlAttributeEncode(posted) + "\" /> ");
            writer.Write("<a id=\"" + ClientID + "_plus\" href=\"" + Page.ClientScript.GetPostBackClientHyperlink(this, "1", true) + "\">+1</a> ");
            writer.Write("<a id=\"" + ClientID + "_plus5\" href=\"javascript:void(0)\" onclick=\"" + Page.ClientScript.GetPostBackEventReference(this, "5") + "\">+5</a>");
            writer.Write("</span>");
        }
    }

    /// <summary>A templated, data-bound control: its template instantiated per item in a naming container.</summary>
    [ParseChildren(true), PersistChildren(false)]
    public class ItemList : Control, INamingContainer
    {
        [TemplateContainer(typeof(Item)), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false)]
        public ITemplate ItemTemplate { get; set; }

        public IEnumerable DataSource { get; set; }

        int Total
        {
            get { return (int?)ViewState["total"] ?? 0; }
            set { ViewState["total"] = value; }
        }

        public class Item : Control, INamingContainer, IDataItemContainer
        {
            public Item(object dataItem, int index) { DataItem = dataItem; DataItemIndex = index; }
            public object DataItem { get; private set; }
            public int DataItemIndex { get; private set; }
            public int DisplayIndex { get { return DataItemIndex; } }
        }

        public override void DataBind()
        {
            OnDataBinding(EventArgs.Empty);
            Controls.Clear();
            ClearChildViewState();
            var index = 0;
            foreach (var dataItem in DataSource) Add(dataItem, index++, true);
            Total = index;
            ChildControlsCreated = true;
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            for (var i = 0; i < Total; i++) Add(null, i, false);
        }

        void Add(object dataItem, int index, bool bind)
        {
            var item = new Item(dataItem, index) { ID = "i" + index };
            if (ItemTemplate != null) ItemTemplate.InstantiateIn(item);
            Controls.Add(item);
            if (bind) item.DataBind();
        }
    }
}
