using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Client.Controls;

public sealed class InputState
{
    // One wheel detent, the step the mouse reports between two notches.
    private const int WheelNotch = 120;

    private readonly List<char> _characters = [];
    private int _wheelCarry;
    private MouseState _previousMouse;
    private MouseState _currentMouse;
    private KeyboardState _previousKeyboard;
    private KeyboardState _currentKeyboard;

    public float ElapsedSeconds { get; private set; }

    public Point MousePosition => _currentMouse.Position;

    public bool HasClicked =>
        _previousMouse.LeftButton == ButtonState.Pressed && _currentMouse.LeftButton == ButtonState.Released;

    public bool IsButtonPressed => _currentMouse.LeftButton == ButtonState.Pressed;

    // Positive when the wheel is pushed away from the hand, which is the way the
    // board camera moves closer.
    public int ScrollNotches { get; private set; }

    public IReadOnlyList<char> Characters => _characters;

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        ElapsedSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
        _previousMouse = _currentMouse;
        _currentMouse = Mouse.GetState();
        _previousKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
        ReadWheel();
    }

    // A precision wheel or a trackpad reports steps smaller than one detent, so
    // what is left over is carried to the next frame instead of being dropped.
    private void ReadWheel()
    {
        int movement = _currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue + _wheelCarry;

        ScrollNotches = movement / WheelNotch;
        _wheelCarry = movement % WheelNotch;
    }

    public void ClearText()
    {
        _characters.Clear();
    }

    // Fed by the window TextInput event, which already resolves accents and the
    // user keyboard layout. Reading raw keys would need manual translation.
    public void AddCharacter(char character)
    {
        _characters.Add(character);
    }

    public bool IsKeyNewlyPressed(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);
    }

    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key);
    }
}
