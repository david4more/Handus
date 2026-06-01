using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Handus.View
{
    internal class Button : IUIElement
    {
        public Sprite sprite { get; set; }
        private Texture texture;
        protected IntRect normalRect { get; private set; }
        protected IntRect pressedRect { get; private set; }

        public Button(string texturePath, float x, float y, IntRect normal, IntRect pressed)
        {
            texture = new Texture(texturePath);
            sprite = new Sprite(texture);
            sprite.Position = new Vector2f(x, y);
            normalRect = normal;
            pressedRect = pressed;

            sprite.TextureRect = normalRect;
            sprite.Scale = new Vector2f(5.0f, 5.0f);
        }

        public virtual void Update(RenderWindow window) {
            Vector2f mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
            bool pressed = Mouse.IsButtonPressed(Mouse.Button.Left) && sprite.GetGlobalBounds().Contains(mousePos);
            if (pressed)
            {
                sprite.TextureRect = pressedRect;
            }
            else
            {
                sprite.TextureRect = normalRect;
            }
        }
        public void Draw(RenderWindow window) {
            window.Draw(sprite);
        }

        public Action onClick;
        public void HandleEvent()
        {
            onClick?.Invoke();
        }
    }
    internal class CheckButton : Button
    {
        public bool IsChecked;

        public CheckButton(string texturePath, float x, float y, IntRect normal, IntRect pressed, bool isChecked = false) : base(texturePath, x, y, normal, pressed)
        {
            IsChecked = isChecked;
        }
        public bool HandleEventAndCheck(EventArgs e, RenderWindow window)
        {
                if (e is MouseButtonEventArgs)
                {
                    Vector2f mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
                if (sprite.GetGlobalBounds().Contains(mousePos))
                {
                    IsChecked = true;
                    return true;
                }
                }
            return false;
        }
        public override void Update(RenderWindow window)
        {
            Vector2f mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
            if (IsChecked)
            {
                sprite.TextureRect = pressedRect;
            }
            else
            {
                sprite.TextureRect = normalRect;
            }
        }
    }
    internal class TextBox : IUIElement
    {
        private static string filePrefix = "..\\..\\..\\Files\\";
        private static string texturePath = filePrefix + "Sprite-textBox.png";
        private static string fontFilePath = filePrefix + "ARLRDBD.TTF";

        private Sprite sprite;
        private Texture texture = new Texture(texturePath);
        private Text text = new(new Font(fontFilePath), "", 20);

        private bool isFocused = false;

        public TextBox(float x, float y, IntRect rect)
        {
            sprite = new Sprite(texture);
            sprite.Position = new Vector2f(x, y);

            sprite.TextureRect = rect;
            sprite.Scale = new Vector2f(5.0f, 5.0f);
            text.FillColor = Color.Black;
            text.Position = new Vector2f(x + 50f, y + 10f);
        }

        public string GetText()
        {
            return text.DisplayedString;
        }
        public void Update(RenderWindow window)
        {
            window.Draw(text);
        }
        public void Draw(RenderWindow window)
        {
            window.Draw(sprite);
            window.Draw(text);
        }

        public void HandleEvent(EventArgs e, RenderWindow window)
        {
            if(e is MouseButtonEventArgs)
            {
                Vector2f mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
                isFocused = sprite.GetGlobalBounds().Contains(mousePos);
            }
            if(e is TextEventArgs textEvent && isFocused)
            {
                string enteredText = textEvent.Unicode;
                if (enteredText == "\b"){
                if(text.DisplayedString.Length > 0)
                    {
                        text.DisplayedString = text.DisplayedString.Remove(text.DisplayedString.Length - 1);
                    }
                    return;
                }
                if (text.DisplayedString.Length >= 20)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(enteredText))
                {
                    text.DisplayedString += enteredText;
                }
            }
        }

    }
}
