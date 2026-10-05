using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bastion.Client.Controls;
using Bastion.Client.Screens;

namespace Bastion.Client.Tests.Navigation;

public static class ScreenDriver
{
    private const string ClickedEvent = "Clicked";
    private const BindingFlags HiddenInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private const BindingFlags OwnPublicEvents = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    public static IReadOnlyList<Control> GetClickables(IScreen screen)
    {
        return GetControls<Control>(screen).Where(IsPressable).ToList();
    }

    public static IReadOnlyList<SidebarMenu> GetMenus(IScreen screen)
    {
        return GetControls<SidebarMenu>(screen);
    }

    public static IReadOnlyList<TextField> GetFields(IScreen screen)
    {
        return GetControls<TextField>(screen);
    }

    public static void ClickAt(IScreen screen, int index)
    {
        Control control = GetClickables(screen)[index];
        EventInfo press = GetPressEvent(control)
            ?? throw new InvalidOperationException($"The control cannot be pressed. Control={control.GetType().Name}");
        Raise(control, press.Name, EventArgs.Empty);
    }

    public static void ClickButton(IScreen screen, string title)
    {
        Button button = GetControls<Button>(screen).FirstOrDefault(candidate => candidate.Title == title)
            ?? throw new InvalidOperationException($"No button has that title. Title={title}");
        Raise(button, ClickedEvent, EventArgs.Empty);
    }

    public static void ChooseMenuItem(IScreen screen, int index)
    {
        SidebarMenu menu = GetMenus(screen)[0];
        if (!menu.Items[index].IsEnabled || index == menu.SelectedIndex)
        {
            return;
        }

        menu.SelectedIndex = index;
        Raise(menu, nameof(SidebarMenu.ItemChosen), new SelectionChangedEventArgs { SelectedIndex = index });
    }

    public static void Type(IScreen screen, IReadOnlyList<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        IReadOnlyList<TextField> fields = GetFields(screen);
        for (int fieldIndex = 0; fieldIndex < values.Count && fieldIndex < fields.Count; fieldIndex++)
        {
            fields[fieldIndex].SetText(values[fieldIndex]);
        }
    }

    public static void TickEveryBox(IScreen screen)
    {
        foreach (CheckBox box in GetControls<CheckBox>(screen))
        {
            box.IsChecked = true;
        }
    }

    public static void ConfirmDialog(MessageScreen dialog)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        Raise(dialog.Dialog, nameof(MessageDialog.PrimaryChosen), EventArgs.Empty);
    }

    private static bool IsPressable(Control control)
    {
        return GetPressEvent(control) is not null;
    }

    private static EventInfo? GetPressEvent(Control control)
    {
        return control.GetType()
            .GetEvents(OwnPublicEvents)
            .FirstOrDefault(candidate => candidate.EventHandlerType == typeof(EventHandler));
    }

    private static bool IsRespondingByItself(Control control)
    {
        return control.GetType().GetEvents(OwnPublicEvents).Length > 0;
    }

    private static IReadOnlyList<T> GetControls<T>(object screen)
        where T : Control
    {
        ArgumentNullException.ThrowIfNull(screen);

        var found = new List<T>();
        Collect(screen, found, new HashSet<object>(ReferenceEqualityComparer.Instance));
        return found;
    }

    private static void Collect<T>(object owner, List<T> found, HashSet<object> seen)
        where T : Control
    {
        if (!seen.Add(owner))
        {
            return;
        }

        for (Type? type = owner.GetType(); type is not null; type = type.BaseType)
        {
            foreach (FieldInfo field in type.GetFields(HiddenInstance | BindingFlags.DeclaredOnly))
            {
                Consider(field.GetValue(owner), found, seen);
            }
        }
    }

    private static void Consider<T>(object? value, List<T> found, HashSet<object> seen)
        where T : Control
    {
        if (value is T control && !found.Contains(control))
        {
            found.Add(control);
        }

        if (value is Control nested)
        {
            if (!IsRespondingByItself(nested))
            {
                Collect(nested, found, seen);
            }

            return;
        }

        if (value is IEnumerable items and not string)
        {
            foreach (object? item in items)
            {
                Consider(item, found, seen);
            }
        }
    }

    private static void Raise(object source, string eventName, EventArgs arguments)
    {
        FieldInfo backing = source.GetType().GetField(eventName, HiddenInstance)
            ?? throw new InvalidOperationException($"The event has no backing field. Event={eventName}");
        var handler = backing.GetValue(source) as Delegate;
        handler?.DynamicInvoke(source, arguments);
    }
}
