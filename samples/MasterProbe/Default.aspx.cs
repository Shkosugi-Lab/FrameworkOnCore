using System;
using System.Web.UI;

namespace MasterProbe
{
    public partial class Default : Page
    {
        private static readonly string[] Rows = { "あ", "い", "う" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            pRepeat.DataSource = Rows;
            pRepeat.DataBind();

            pRepeatHdr.DataSource = Rows;
            pRepeatHdr.DataBind();

            pList.DataSource = Rows;
            pList.DataBind();

            pGrid.DataSource = Rows;
            pGrid.DataBind();
        }

        protected void PageButton_Click(object sender, EventArgs e)
        {
            pClickResult.Text = "クリック済み";
        }
    }
}
