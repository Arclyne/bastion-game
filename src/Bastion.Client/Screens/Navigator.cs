using System;
using System.Collections.Generic;
using log4net;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Screens.Messages;

namespace Bastion.Client.Screens;

public sealed class Navigator : INavigator
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(Navigator));

    private readonly ClientServices _services;
    private readonly List<(ScreenId Id, string? Argument)> _history = [];

    private IScreen? _current;
    private IScreen? _dialog;
    private string? _currentArgument;
    private Action? _pendingConfirm;

    public Navigator(ClientServices services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
    }

    public IScreen? Current => _dialog ?? _current;

    public ScreenId CurrentId { get; private set; }

    public void Start(ScreenId screen)
    {
        Restart(screen);
    }

    public void GoTo(ScreenId screen, string? argument = null)
    {
        if (!IsAvailable(screen))
        {
            return;
        }

        if (_current is not null)
        {
            _history.Add((CurrentId, _currentArgument));
        }

        Show(screen, argument);
    }

    public void GoBack()
    {
        if (_history.Count == 0)
        {
            Show(CurrentId, _currentArgument);
            return;
        }

        (ScreenId id, string? argument) = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        Show(id, argument);
    }

    public void ReturnTo(ScreenId screen)
    {
        int index = _history.FindLastIndex(entry => entry.Id == screen);
        if (index >= 0)
        {
            (ScreenId id, string? argument) = _history[index];
            _history.RemoveRange(index, _history.Count - index);
            Show(id, argument);
            return;
        }

        if (CurrentId != screen)
        {
            GoTo(screen);
            return;
        }

        Show(screen, _currentArgument);
    }

    public void Restart(ScreenId screen)
    {
        if (!ScreenRegistry.Contains(screen))
        {
            throw new InvalidOperationException($"The screen is not registered. Screen={screen}");
        }

        _history.Clear();
        Show(screen, null);
    }

    public void ShowConfirm(ConfirmRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var confirm = new MessageConfirmScreen();
        confirm.ShowWithLabels(request.Body, GetPrimaryLabel(request), GetSecondaryLabel(request));
        if (request.Title.Length > 0)
        {
            confirm.Dialog.Title = request.Title;
        }

        confirm.Dialog.Detail = request.Detail;
        confirm.Dialog.PrimaryChosen += OnConfirmAccepted;
        confirm.Dialog.SecondaryChosen += OnDialogDismissed;
        _pendingConfirm = request.OnConfirm;
        _dialog = confirm;
    }

    public void ShowMessage(DialogTone tone, string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        MessageScreen screen = BuildMessage(tone);
        screen.Show(body);
        screen.Dialog.PrimaryChosen += OnDialogDismissed;
        _dialog = screen;
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (_dialog is not null)
        {
            _dialog.Update(input);
            return;
        }

        _current?.Update(input);
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        _current?.Draw(canvas);
        _dialog?.Draw(canvas);
    }

    private bool IsAvailable(ScreenId screen)
    {
        if (ScreenRegistry.Contains(screen))
        {
            return true;
        }

        _logger.Warn($"A screen that is not registered was requested. Screen={screen}");
        ShowMessage(DialogTone.Warning, TextCatalog.CommonScreenUnavailable);
        return false;
    }

    private void Show(ScreenId screen, string? argument)
    {
        CurrentId = screen;
        _currentArgument = argument;
        _current = ScreenRegistry.Create(screen, new ScreenContext(this, argument, _services));
        _dialog = null;
    }

    private void CloseDialog()
    {
        _dialog = null;
        _pendingConfirm = null;
    }

    private void OnDialogDismissed(object? sender, EventArgs e)
    {
        CloseDialog();
    }

    private void OnConfirmAccepted(object? sender, EventArgs e)
    {
        Action? confirmed = _pendingConfirm;
        CloseDialog();
        confirmed?.Invoke();
    }

    private static MessageScreen BuildMessage(DialogTone tone)
    {
        switch (tone)
        {
            case DialogTone.Error:
                return new MessageErrorScreen();
            case DialogTone.Success:
                return new MessageSuccessScreen();
            default:
                return new MessageWarningScreen();
        }
    }

    private static string GetPrimaryLabel(ConfirmRequest request)
    {
        return string.IsNullOrEmpty(request.PrimaryLabel) ? TextCatalog.DialogConfirmButton : request.PrimaryLabel;
    }

    private static string GetSecondaryLabel(ConfirmRequest request)
    {
        return string.IsNullOrEmpty(request.SecondaryLabel) ? TextCatalog.DialogCancelButton : request.SecondaryLabel;
    }
}
