/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Diagnostics;
using System.Numerics;
using NoZ.Graphics;

namespace NoZ.UI
{
    public static partial class UI
    {        
        public struct AutoEndElement : IDisposable { public void Dispose() => EndElement(); }
        public struct AutoEndCanvas : IDisposable { public void Dispose() => EndCanvas(); }
        public struct AutoEndHorizontal : IDisposable { public void Dispose() => EndHorizontal(); }
        public struct AutoEndVertical : IDisposable { public void Dispose() => EndVertical(); }
        public struct AutoEndFocus : IDisposable { public void Dispose() => EndFocus(); }

        private struct ImageElement
        {
            public Texture Texture;
            public Sprite Sprite;

            public Rect Source => Sprite != null ? Sprite.SourceRect : new Rect(0, 0, Texture.Width, Texture.Height);
        }
        
        private static readonly Element[] _elements = new Element[1024];
        private static readonly List<Canvas> _canvases = [];
        private static readonly Stack<int> _stack = new();
        private static readonly List<string> _text = new(1024);
        private static readonly List<Font> _fonts = new(1024);
        private static readonly List<ImageElement> _images = new(1024);
        private static Shader? _textShader;
        private static int _currentElement;
        private static Rect _screenRect;
        private static int _mouseOverControlId;
        private static bool _mouseOver;
        private static Vector2 _lastMousePosition;
        private static Vector2 _mousePosition;
        private static int _focusControlId;

        public static float Scale { get; set; }= 1.0f;
        
        /// <summary>
        /// Return the control identifier of the given control
        /// </summary>
        public static int ControlId { get; private set; }
        
        /// <summary>
        /// Currently active control 
        /// </summary>
        public static int HotControlId { get; private set; }

        public static bool IsMouseOver
        {
            get
            {
                if (_mouseOverControlId == ControlId)
                    return _mouseOver;
                
                ref var element = ref _elements[_currentElement];
                _mouseOverControlId = ControlId;
                _mouseOver = Raylib_cs.Raylib.CheckCollisionPointRec(_mousePosition, CanvasToScreen(element.Bounds));
                return _mouseOver;
            }
        }
        
        public static bool IsMouseEnter
        {
            get
            {
                ref var element = ref _elements[_currentElement];
                var wasOver = Raylib_cs.Raylib.CheckCollisionPointRec(_lastMousePosition, CanvasToScreen(element.Bounds));
                var isOver = Raylib_cs.Raylib.CheckCollisionPointRec(_mousePosition, CanvasToScreen(element.Bounds));
                return isOver && !wasOver;
            }
        }

        public static bool IsMouseClick => 
            Raylib_cs.Raylib.IsMouseButtonReleased(Raylib_cs.MouseButton.Left) && IsMouseOver && !IsMouseClickConsumed;
        
        public static bool IsMouseClickConsumed { get; set; }

        public static bool IsPressed { get; private set; }

        public static bool IsFocused => _focusControlId != 0 && _focusControlId == ControlId;

        public static void Initialize(string defaultFontName)
        {
            LoadFont(defaultFontName);
            _textShader = ResourceDatabase.LoadShader(null, "shaders/sdf");
        }
        
        public static void Shutdown()
        {
            _textShader?.Dispose();
            _textShader = null;

            _fonts.Clear();
            _images.Clear();
            _text.Clear();
            _canvases.Clear();
        }
        
        public static Rect CanvasToScreen(in Rect rect)  
        {
            return new Rect(
                _screenRect.X + rect.X * Scale,
                _screenRect.Y + rect.Y * Scale,
                rect.Width * Scale,
                rect.Height * Scale);
        }

        public static int LoadFont(string name)
        {
            for(var i=0; i<_fonts.Count; i++)
                if (_fonts[i].Path == name)
                    return i;

            _fonts.Add(ResourceDatabase.LoadFont(name)!);
            return _fonts.Count - 1;
        }

        public static Font GetFont(int fontIndex) =>
            _fonts[fontIndex];
        
        public static void BeginFrame(in Rect screenRect)
        {
            _screenRect = screenRect;
            _currentElement = -1;
            _mouseOverControlId = 0;
            IsMouseClickConsumed = false;
            _mouseOver = false;
            _stack.Clear();
            _text.Clear();
            _images.Clear();
            _canvases.Clear();
            _lastMousePosition = _mousePosition;
            _mousePosition = Raylib_cs.Raylib.GetMousePosition();

            ReadInput();
        }

        public static AutoEndCanvas BeginCanvas(int sortOrder)
        {
            var focus = 0;
            return BeginCanvas(sortOrder, ref focus);
        }

        public static AutoEndCanvas BeginCanvas(int sortOrder, ref int focus)
        {
            _stack.Clear();

            var screenWidth = _screenRect.Width * (1.0f / Scale);
            var screenHeight = _screenRect.Height * (1.0f / Scale);
            
            BeginElement(new Style
            {
                Width = screenWidth,
                Height = screenHeight,
            }, ElementType.Element);
         
            ref var canvasElement = ref _elements[_currentElement];
            canvasElement.Bounds = new Rect(0, 0, screenWidth, screenHeight);
            canvasElement.LayoutBounds = canvasElement.Bounds;
            canvasElement.BoundsWithMargins = canvasElement.Bounds;
            
            _canvases.Add(new Canvas
            {
                StartElement = _currentElement,
                ElementCount = 0,
                SortOrder = sortOrder
            });

            return new AutoEndCanvas();
        }

        public static void Element(in Style style)
        {
            BeginElement(style);
            EndElement();
        }

        public static AutoEndElement BeginElement(in Style style) =>
            BeginElement(style, ElementType.Element);
        
        private static AutoEndElement BeginElement(in Style style, ElementType elementType, int controlId=0, int resourceId=0)
        {
            Debug.Assert(controlId == 0 || ControlId == 0, "Nested controls are not supported");

            _currentElement++;
            _elements[_currentElement] = new Element
            {
                ElementType = elementType,
                Style = style,
                ControlId = controlId,
                Resource = resourceId
            };
            
            if (controlId != 0)
                ControlId = controlId;

            UpdateElementLayout();
            
            _stack.Push(_currentElement);

            return new AutoEndElement();
        }

        public static void EndElement()
        {
            ref var element = ref _elements[_stack.Pop()];
            ref var parent = ref _elements[_stack.Peek()];

            if (element.ControlId != 0)
                ControlId = 0;

            switch (parent.ElementType)
            {
                case ElementType.Horizontal:
                {
                    var parentRight = parent.LayoutBounds.X + parent.LayoutBounds.Width;
                    parent.LayoutBounds.X = element.BoundsWithMargins.X + element.BoundsWithMargins.Width;
                    parent.LayoutBounds.Width = parentRight - parent.LayoutBounds.X;
                    break;
                }

                case ElementType.HorizontalReverse:
                {
                    parent.LayoutBounds.Width = MathF.Max(0, element.BoundsWithMargins.X - parent.LayoutBounds.X);
                    break;
                }

                case ElementType.Vertical:
                {
                    var parentBottom = parent.LayoutBounds.Y + parent.LayoutBounds.Height;
                    parent.LayoutBounds.Y = element.BoundsWithMargins.Y + element.BoundsWithMargins.Height;
                    parent.LayoutBounds.Height = parentBottom - parent.LayoutBounds.Y;
                    break;
                }
            }
        }
        
        public static void EndCanvas()
        {
            var canvas = _canvases[^1];
            canvas.ElementCount = _currentElement - canvas.StartElement + 1;
            _canvases[^1] = canvas;
        }

        public static void Label(string text, in Style style)
        {
            _text.Add(text);
            BeginElement(style, ElementType.Label, resourceId: _text.Count);
            EndElement();
        }
        
        public static void Image(Texture texture, in Style style)
        {
            _images.Add(new ImageElement { Texture = texture });
            BeginElement(style, ElementType.Image, resourceId: _images.Count);
            EndElement();
        }
        
        public static void Image(in Sprite sprite, in Style style)
        {
            _images.Add(new ImageElement { Sprite = sprite });
            BeginElement(style, ElementType.Image, resourceId: _images.Count);
            EndElement();
        }

        public static AutoEndHorizontal BeginHorizontal() =>
            BeginHorizontal(Style.StretchToFit);

        public static AutoEndHorizontal BeginHorizontal(Style style, bool reverse=false)
        {
            BeginElement(style, reverse ? ElementType.HorizontalReverse : ElementType.Horizontal);
            return new AutoEndHorizontal();
        }
        
        public static void EndHorizontal()
        {
            Debug.Assert(_elements[_stack.Peek()].ElementType is ElementType.Horizontal or ElementType.HorizontalReverse);
            EndElement();
        }

        public static AutoEndVertical BeginVertical() =>
            BeginVertical(Style.StretchToFit);

        public static AutoEndVertical BeginVertical(Style style)
        {
            BeginElement(style, ElementType.Vertical);
            return new AutoEndVertical();
        }
        
        public static void EndVertical()
        {
            Debug.Assert(_elements[_stack.Peek()].ElementType == ElementType.Vertical);
            EndElement();
        }

        public static AutoEndElement BeginButton(Style style, Action? onClick=null, int controlId=0)
        {
            Debug.Assert(controlId >= 0, "ControlId must be greater than or equal to zero");

            BeginElement(style, ElementType.Element, controlId: controlId);

            if (IsMouseClick || (Navigation == Navigation.Select && IsFocused))
            {
                // Mouse clicks count as keyboard input
                if (IsMouseClick)
                    UI.LastInputSource = Input.InputSource.Keyboard;
                IsMouseClickConsumed = true;
                onClick?.Invoke();
            }

            IsPressed = Raylib_cs.Raylib.IsMouseButtonDown(Raylib_cs.MouseButton.Left) && IsMouseOver;

            return new AutoEndElement();
        }

        public static void EndButton()
        {
            EndElement();
            IsPressed = false;
        }
        
        public static void EndFrame()
        {
            _canvases.Sort((a, b) => a.SortOrder - b.SortOrder);
        }

        public static void Render()
        {
            foreach (var canvas in _canvases)
                Render(_elements.AsSpan(canvas.StartElement, canvas.ElementCount));
        }

        public static AutoEndFocus BeginFocus(ref int focusControlId, int minFocusControlId, int maxFocusControlId, Action<int,int>? focusChanged=default, Func<int, Navigation, int>? customNavigation=null)
        {
            var oldFocusControlId = focusControlId;
            if (customNavigation != null)
                focusControlId = customNavigation(focusControlId, Navigation);
            else if (Navigation is Navigation.Right or Navigation.Down)
                focusControlId++;
            else if (Navigation is Navigation.Left or Navigation.Up)
                focusControlId--;

            var focusControlRange = maxFocusControlId - minFocusControlId + 1;
            focusControlId = minFocusControlId + ((focusControlRange + focusControlId - minFocusControlId) % focusControlRange);

            _focusControlId = focusControlId;

            if (oldFocusControlId != _focusControlId)
                focusChanged?.Invoke(oldFocusControlId, _focusControlId);

            return new AutoEndFocus();
        }

        public static void EndFocus()
        {
            _focusControlId = 0;
        }
    }
}