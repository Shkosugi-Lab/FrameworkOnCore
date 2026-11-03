using AjaxControlToolkit;

namespace FrameworkOnCore.Runtime.Tests.Web
{
    /// <summary>
    /// The Ajax Control Toolkit's server side where FrameworkOnCore's Toolkit (20.1) keeps the behavior of the 4.1 the
    /// converted applications were written for (AjaxControlToolkit 4.1.40412, nopCommerce 1.90's: its decompiled code).
    /// </summary>
    public class ToolkitTests
    {
        static (Rating Rating, List<string> Changed, List<string> Clicked) Rating(int rendered)
        {
            var rating = new Rating { ID = "productRating", MaxRating = 5 };
            rating.CurrentRating = rendered;
            var changed = new List<string>();
            var clicked = new List<string>();
            rating.Changed += (_, e) => changed.Add(e.Value);
            rating.Click += (_, e) => clicked.Add(e.Value);
            return (rating, changed, clicked);
        }

        [Fact] // 4.1's RaisePostBackEvent raised Changed; on an AutoPostBack the posted client state is already the clicked rating (the script sets it before it posts): 20.1 compared with it and never raised Changed (nopCommerce's rating vote not saved)
        public void A_postback_raises_changed()
        {
            var (rating, changed, clicked) = Rating(rendered: 0);
            rating.CurrentRating = 3;   // the client state the postback carries
            rating.RaisePostBackEvent("1;3;");
            Assert.Equal(new[] { "3" }, changed);
            Assert.Equal(new[] { "3" }, clicked);
        }

        [Fact] // a callback posts the form's state as it was loaded: 20.1's comparison with it kept (Changed when the rating is another; Click for a click)
        public void A_callback_raises_changed_for_another_rating()
        {
            var (rating, changed, clicked) = Rating(rendered: 2);
            rating.RaiseCallbackEvent("1;4;");
            rating.RaiseCallbackEvent("1;2;");
            Assert.Equal(new[] { "4" }, changed);
            Assert.Equal(new[] { "4", "2" }, clicked);
        }
    }
}
