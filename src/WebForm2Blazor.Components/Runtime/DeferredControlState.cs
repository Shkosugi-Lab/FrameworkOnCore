using System.Reflection;

namespace WebForm2Blazor.Components;

/// <summary>
/// Carries the properties a page set on a control BEFORE that control existed.
///
/// WebForms built the control tree and then called OnInit, so touching a declared control
/// from OnInit is ordinary code:
///
///     protected override void OnInit(EventArgs e) { ucCommentList.Visible = ShowComments; }
///
/// In Blazor those fields are @ref, and @ref is assigned after the first render. The field
/// is null during OnInit and the page dies with a NullReferenceException - BlogEngine's
/// /post did exactly that.
///
/// The two constraints cannot both be met by moving code: OnInit has to run BEFORE the
/// first render (it produces the data the markup binds to - deferring it was tried and
/// merely moved the NullReferenceException onto Post), and @ref cannot exist before that
/// render. So the assignment is not moved; it is RECORDED against a stand-in and replayed
/// onto the real control the moment @ref delivers it.
///
/// Only what the page actually assigned is replayed. A property nobody touched is left at
/// whatever the markup gave it, which is what WebForms would have done too.
/// </summary>
public interface IDeferredControlState
{
    /// <summary>Property name -> value, for assignments made while no render handle existed.</summary>
    IDictionary<string, object> PendingState { get; }
}

/// <summary>
/// Replays recorded assignments onto a real control.
/// </summary>
public static class DeferredControlStateExtensions
{
    /// <summary>
    /// Copies everything the stand-in recorded onto the control @ref just produced, then
    /// clears the record so a later re-render does not replay stale values.
    ///
    /// A property that no longer exists, or that cannot be written, is skipped rather than
    /// throwing: the stand-in is a compatibility device, and failing here would replace a
    /// null-reference crash with a different crash.
    /// </summary>
    public static void ReplayPendingStateOnto(this IDeferredControlState source, object target)
    {
        if (source is null || target is null)
        {
            return;
        }

        // Children added while the control did not exist yet. PendingState only carries
        // property assignments, and a control tree built in OnInit lands here instead -
        // BlogEngine's PostList does "posts.Controls.Add(postView)" for every article, so
        // without this the posts are built, handed to the stand-in, and dropped.
        if (source is IWebFormsControl { Controls.Count: > 0 } pendingControl
            && target is IWebFormsControl realControl)
        {
            foreach (var child in pendingControl.Controls)
            {
                realControl.Controls.Add(child);
            }
            pendingControl.Controls.Clear();
        }

        var type = target.GetType();
        foreach (var (name, value) in source.PendingState.ToList())
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (property?.CanWrite != true)
            {
                continue;
            }

            try
            {
                property.SetValue(target, value);
            }
            catch (Exception exception) when (exception is ArgumentException or TargetInvocationException)
            {
                // Type mismatch or a setter that rejects the value: the assignment is
                // dropped, as it would have been had the control never existed.
            }
        }

        source.PendingState.Clear();
    }
}
