using SFML.Graphics;
using Handus;
using SFML.System;
using SFML.Window;

RenderWindow window = new RenderWindow(new VideoMode(new Vector2u(1920, 1080)), "Handus", Styles.Default, State.Fullscreen);
Engine engine = new(window);
engine.Loop();
