using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Client.Controls;

public sealed class InputState
{
    private readonly List<char> _characters = [];
    private MouseState _previousMouse;
    private MouseState _currentMouse;
    private KeyboardState _previousKeyboard;
    private KeyboardState _currentKeyboard;

    public float ElapsedSeconds { get; private set; }

    public Point MousePosition => _currentMouse.Position;

    public bool HasClicked =>
        _previousMouse.LeftButton == ButtonState.Pressed && _currentMouse.LeftButton == ButtonState.Released;

    public bool IsButtonPressed => _currentMouse.LeftButton == ButtonState.Pressed;

    public IReadOnlyList<char> Characters => _characters;

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        ElapsedSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
        _previousMouse = _currentMouse;
        _currentMouse = Mouse.GetState();
        _previousKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
    }

    public void ClearText()
    {
        _characters.Clear();
    }

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
