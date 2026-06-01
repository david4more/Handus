using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Handus.View
{
    public interface IUIElement
    {
        void Update(RenderWindow window);
        void Draw(RenderWindow window);
    }
}
