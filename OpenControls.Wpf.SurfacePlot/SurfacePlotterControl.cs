using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenControls.Wpf.SurfacePlot.Model;

namespace OpenControls.Wpf.SurfacePlot
{
    public class SurfacePlotControl : OpenTK.GLControl, Model.ILabelFormatter
    {
        //private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public SurfacePlotControl() : base(new OpenTK.Graphics.GraphicsMode(OpenTK.Graphics.ColorFormat.Empty, 24), 3, 1, OpenTK.Graphics.GraphicsContextFlags.Default)
        {
            _verticesLock = new object();
        }

        OpenControls.Wpf.SurfacePlot.TextRenderer.TextRenderer _textRenderer;

        public void Initialise(OpenControls.Wpf.SurfacePlot.Model.IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
            _iConfiguration.ConfigurationChanged += ConfigurationChangedEventHandler;

            MaxZValue = 0;
            MinZValue = 0;
            CreateTextRenderer();
        }

        private void CreateTextRenderer()
        {
            _textRenderer = new OpenControls.Wpf.SurfacePlot.TextRenderer.TextRenderer(
                "Arial",
                _iConfiguration.LabelFontSize > 4 ? _iConfiguration.LabelFontSize : 4,
                System.Drawing.Color.FromName(_iConfiguration.LabelColour),
                _iConfiguration.TransparentLabelBackground ? System.Drawing.Color.Transparent : System.Drawing.Color.FromName(_iConfiguration.BackgroundColour));
            _textRenderer.Load();
        }

        public bool IsRunning { get; set; }

        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string ZAxisTitle { get; set; }

        private string GetXAxisTitle()
        {
            return XAxisTitle != null ? XAxisTitle : "X Axis";
        }

        private string GetYAxisTitle()
        {
            return YAxisTitle != null ? YAxisTitle : "Y Axis";
        }

        private string GetZAxisTitle()
        {
            return ZAxisTitle != null ? ZAxisTitle : "Z Axis";
        }

        Model.ILabelFormatter _iLabelFormatter;
        public Model.ILabelFormatter ILabelFormatter
        {
            get
            {
                return _iLabelFormatter == null ? this : _iLabelFormatter;
            }
            set
            {
                _iLabelFormatter = value;
                _xAxisLabels = null;
                _yAxisLabels = null;
            }
        }

        #region Model.ILabelFormatter

        public string XLabel(float x) => x.ToString("G4");

        public string YLabel(float y) => y.ToString("G4");

        public string ZLabel(float z) => z.ToString("G4");

        #endregion Model.ILabelFormatter

        private AxisLabels _xAxisLabels;
        private AxisLabels _yAxisLabels;
        private AxisLabels _zAxisLabels;

        /*
         * The scaling was introduced to allow good display of text. With a scaling of 1
         * we have to use a small font size and the result looks awful. 
         */
        float _axisScaling = 100;
        private IConfiguration _iConfiguration;

        public void SetData(
            List<List<float>> lineData,
            float xMin,
            float xMax,
            int numberOfXLabels,
            float yMin,
            float yMax,
            int numberOfYLabels,
            float zMin,
            float zMax,
            int numberOfZLabels)
        {
            if (
                (_xAxisLabels == null) ||
                (_xAxisLabels.MaxValue != xMax) ||
                (_xAxisLabels.MinValue != xMin) ||
                (_xAxisLabels.Labels.Count != numberOfXLabels) ||
                (_yAxisLabels == null) ||
                (_yAxisLabels.MaxValue != yMax) ||
                (_yAxisLabels.MinValue != yMin) ||
                (_yAxisLabels.Labels.Count != numberOfYLabels) ||
                (_zAxisLabels == null) ||
                (_zAxisLabels.MaxValue != zMax) ||
                (_zAxisLabels.MinValue != zMin) ||
                (_zAxisLabels.Labels.Count != numberOfZLabels)
                )
            {
                _xAxisLabels = AxisLabels.Build(
                    xMin, 
                    xMax, 
                    numberOfXLabels, 
                    _iConfiguration.ViewProjection == ViewProjection.BirdsEye_270,
                    ILabelFormatter.XLabel,
                    _textRenderer.MeasureTextLength);

                _yAxisLabels = AxisLabels.Build( 
                    yMin, 
                    yMax, 
                    numberOfYLabels, 
                    (_iConfiguration.ViewProjection == ViewProjection.BirdsEye_180),
                    ILabelFormatter.YLabel,
                    _textRenderer.MeasureTextLength);

                _zAxisLabels = AxisLabels.Build(
                    zMin, 
                    zMax, 
                    numberOfZLabels,
                    false,
                    ILabelFormatter.ZLabel,
                    _textRenderer.MeasureTextLength);
            }

            _axisScaling = 1000f / (float)Math.Max(lineData.Count - 1, lineData[0].Count - 1);

            _lineData = lineData;
            LoadVertices();
        }

        // Scales the Z values according to user preferences
        public double ZScale
        {
            get
            {
                return _iConfiguration != null ? _iConfiguration.ZScale : 1f;
            }
        }

        // A simple number to zoom in and out: initial value is sensible with TouchHub2
        public int Zoom
        {
            get
            {
                return _iConfiguration != null ? _iConfiguration.Zoom : 100;
            }
            set
            {
                if (_iConfiguration != null)
                {
                    _iConfiguration.Zoom = value;
                }
            }
        }

        private List<List<float>> _lineData;

        // Display range
        private VertexStore _rawDataValues;
        private Object _verticesLock;

        public float MaxZValue { get; set; }
        public float MinZValue { get; set; }

        /*
         * Map the z value to a colour in the spectrum with red as the minimum and blue as the maximum
         */
        Color4 _blue = new Color4(0f, 0f, 1f, 0f);
        Color4 _green = new Color4(0f, 1f, 0f, 0f);
        Color4 _yellow = new Color4(1f, 1f, 0f, 0f);
        Color4 _orange = new Color4(1f, 0.5f, 0f, 0f);
        Color4 _red = new Color4(1f, 0f, 0f, 0f);

        private Color4 GetVertexColour(float zValue, ShadingAlgorithm shadingAlgorithm, short blueLevel, short redLevel)
        {
            if (shadingAlgorithm == ShadingAlgorithm.FixedLevels)
            {
                if (zValue > redLevel)
                {
                    zValue = redLevel;
                }
                else if (zValue < blueLevel)
                {
                    zValue = blueLevel;
                }
            }
            else
            {
                redLevel = (short)MaxZValue;
                blueLevel = (short)MinZValue;
            }

            Int16 delta = (Int16)(zValue - blueLevel);
            float fraction = (redLevel == blueLevel) ? 1.0f : ((float)delta) / (float)(redLevel - blueLevel);
            Color4 color1;
            Color4 color2;
            if (fraction > 0.75f)
            {
                color1 = _orange;
                color2 = _red;
                fraction -= 0.75f;
            }
            else if (fraction > 0.5f)
            {
                color1 = _yellow;
                color2 = _orange;
                fraction -= 0.5f;
            }
            else if (fraction > 0.25f)
            {
                color1 = _green;
                color2 = _yellow;
                fraction -= 0.25f;
            }
            else
            {
                color1 = _blue;
                color2 = _green;
            }
            fraction *= 4.0f;
            float oneMinusFraction = 1.0f - fraction;
            Color4 color = new Color4(color1.R * oneMinusFraction + color2.R * fraction, color1.G * oneMinusFraction + color2.G * fraction, color1.B * oneMinusFraction + color2.B * fraction, 0.0f);
            return color;
        }

        private Color4 GetVertexColour(float zValue)
        {
            return GetVertexColour(zValue, _iConfiguration.ShadingAlgorithm, _iConfiguration.BlueLevel, _iConfiguration.RedLevel);
        }

        // Set up the initial values to display the view intuitively
        private float _xRotationInDegrees = 60;
        private float _yRotationInDegrees = 180;
        private float _zRotationInDegrees = 225;
        private const float constDegreesPerRadian = 57.295779513082f;
        const int RotationIncrementInDegrees = 2;

        private void StoreRawDataValue(int x, int y)
        {
            float z = _lineData[x][y];
            if (_iConfiguration.Hold)
            {
                if (_iConfiguration.HoldMaximum)
                {
                    z = Math.Max(_rawDataValues.CurrentZ(), z);
                }
                else
                {
                    z = Math.Min(_rawDataValues.CurrentZ(), z);
                }
                _lineData[x][y] = z;
            }
            // Do not set the colour here
            _rawDataValues.SetVertex(x, y, z, new Color4());
            Invalidate();
        }

        private void LoadVertices()
        {
            if ((_lineData != null) && (_lineData.Count > 0))
            {
                lock (_verticesLock)
                {
                    bool restart = (_rawDataValues == null) || (_rawDataValues.NumberOfNodes != _lineData.Count * _lineData[0].Count);
                    if (restart)
                    {
                        _rawDataValues = new VertexStore(_lineData.Count * _lineData[0].Count);
                    }
                    _rawDataValues.ResetNextFreeItemIndex();

                    MinZValue = float.MaxValue;
                    MaxZValue = float.MinValue;

                    for (int x = 0; x < _lineData.Count; ++x)
                    {
                        for (int y = 0; y < _lineData[x].Count; ++y)
                        {
                            // Store the raw data value (max/min if hold is active)

                            if ((x > 0) && (y > 0))
                            {
                                // Add two triangles per raw data value

                                StoreRawDataValue(x - 1, y - 1);
                                StoreRawDataValue(x, y - 1);
                                StoreRawDataValue(x, y);
                                StoreRawDataValue(x - 1, y);
                                StoreRawDataValue(x - 1, y - 1);
                            }
                        }
                    }

                    /*
                     * Evaluate the max/min values from the stored data. 
                     * Do not use live data as hold might be active. 
                     */

                    foreach (var vertex in _rawDataValues.Vertices)
                    {
                        if (vertex.Z < MinZValue)
                        {
                            MinZValue = vertex.Z;
                        }
                        if (vertex.Z > MaxZValue)
                        {
                            MaxZValue = vertex.Z;
                        }
                    }

                    // Now work out the colours => we need the latest max/min values for this to work correctly

                    foreach (var vertex in _rawDataValues.Vertices)
                    {
                        vertex.Colour = GetVertexColour(vertex.Z);
                    }
                }
            }
        }

        private bool KeyPressHandled;

        private int _mouseX;
        private int _mouseY;
        private MouseButtons _mouseButton;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Capture = true;
            _mouseX = e.X;
            _mouseY = e.Y;
            _mouseButton = e.Button;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Capture = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!Capture)
            {
                return;
            }

            bool isControlPressed = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl);
            if (_mouseButton == MouseButtons.Left)
            {
                if (isControlPressed)
                {
                    _xCentre += 4 * (e.X - _mouseX);
                    _yCentre -= 4 * (e.Y - _mouseY);
                }
                else if (_iConfiguration.ViewProjection == ViewProjection.ThreeDimensional)
                {
                    _xRotationInDegrees -= (e.Y - _mouseY) / 2;
                    _zRotationInDegrees += (e.X - _mouseX) / 2;
                }
            }

            _mouseX = e.X;
            _mouseY = e.Y;
            Invalidate(ClientRectangle);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (Capture)
            {
                Zoom += e.Delta / 20;
                Invalidate(ClientRectangle);
            }
        }

        protected override void OnPreviewKeyDown(System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Oemplus:
                case System.Windows.Forms.Keys.Add:
                    --Zoom;
                    break;
                case System.Windows.Forms.Keys.OemMinus:
                case System.Windows.Forms.Keys.Subtract:
                    ++Zoom;
                    break;
                default:
                    if (_iConfiguration.ViewProjection != ViewProjection.ThreeDimensional)
                    {
                        return;
                    }
                    break;
            }

            // These only apply for 3D

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                case System.Windows.Forms.Keys.NumPad8:
                    _xRotationInDegrees += RotationIncrementInDegrees;
                    _xRotationInDegrees %= 360;
                    break;
                case System.Windows.Forms.Keys.Down:
                case System.Windows.Forms.Keys.NumPad2:
                    _xRotationInDegrees -= RotationIncrementInDegrees;
                    _xRotationInDegrees %= 360;
                    break;
                case System.Windows.Forms.Keys.Right:
                case System.Windows.Forms.Keys.NumPad6:
                    _yRotationInDegrees += RotationIncrementInDegrees;
                    _yRotationInDegrees %= 360;
                    break;
                case System.Windows.Forms.Keys.Left:
                case System.Windows.Forms.Keys.NumPad4:
                    _yRotationInDegrees -= RotationIncrementInDegrees;
                    _yRotationInDegrees %= 360;
                    break;
                case System.Windows.Forms.Keys.PageUp:
                    _zRotationInDegrees += RotationIncrementInDegrees;
                    _zRotationInDegrees %= 360;
                    break;
                case System.Windows.Forms.Keys.PageDown:
                    _zRotationInDegrees -= RotationIncrementInDegrees;
                    _zRotationInDegrees %= 360;
                    break;
                default:
                    KeyPressHandled = false;
                    return;
            }
            KeyPressHandled = true;
            Invalidate(ClientRectangle);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (KeyPressHandled)
            {
                // Some keypresses such as up arrow can cause an unwanted loss of focus
                Focus();
                KeyPressHandled = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MakeCurrent();
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            Invalidate();
        }

        /*
         * The projection matrix creates a cube centred on (0,0,0) with sides one unit long. 
         * Perspective is applied. Any shapes drawn outside of this cube are ignored. 
         */
        Matrix4 _projection;
        private void UpdateProjectionMatrices()
        {
            MakeCurrent();
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            if (!_iConfiguration.ViewProjection.IsOrthographic())
            {
                /*
                 * Be careful with the near and far distances as it can cause 'z-fighting' whereby hidden detail removal fails:
                 * 
                 *      https://learnopengl.com/Advanced-OpenGL/Depth-testing
                 */
                _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / (4f * _iConfiguration.Perspective), (float)ClientRectangle.Width / (float)ClientRectangle.Height, 25f, 6000.0f * _iConfiguration.Perspective);
            }
            else
            {

                _projection = Matrix4.CreateOrthographic((float)ClientRectangle.Width * Zoom / 200f, (float)ClientRectangle.Height * Zoom / 200f, -1000f, _axisScaling * 10000.0f);
            }

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _projection);
        }

        // The offset of the centre of the plot
        float _xCentre = 0;
        float _yCentre = 0;

        private void GetCurrentRotation(out Matrix4 rotation)
        {
            float xRotationInDegrees = _xRotationInDegrees;
            float yRotationInDegrees = _yRotationInDegrees;
            float zRotationInDegrees = _zRotationInDegrees;

            if (_iConfiguration.ViewProjection == ViewProjection.Orthographic_Front)
            {
                xRotationInDegrees = 90;
                yRotationInDegrees = 180;
                zRotationInDegrees = 180;
            }
            else if (_iConfiguration.ViewProjection == ViewProjection.Orthographic_Side)
            {
                xRotationInDegrees = 90;
                yRotationInDegrees = 180;
                zRotationInDegrees = 270;
            }
            else if (_iConfiguration.ViewProjection.IsBirdsEye())
            {
                xRotationInDegrees = -180;
                yRotationInDegrees = 0;
                switch (_iConfiguration.ViewProjection)
                {
                    case ViewProjection.BirdsEye_0:
                        zRotationInDegrees = 0;
                        break;
                    case ViewProjection.BirdsEye_90:
                        zRotationInDegrees = 90;
                        break;
                    case ViewProjection.BirdsEye_180:
                        zRotationInDegrees = 180;
                        break;
                    case ViewProjection.BirdsEye_270:
                        zRotationInDegrees = 270;
                        break;
                }
            }

            rotation =
                Matrix4.CreateRotationZ(zRotationInDegrees / constDegreesPerRadian) *
                Matrix4.CreateRotationY(yRotationInDegrees / constDegreesPerRadian) *
                Matrix4.CreateRotationX(xRotationInDegrees / constDegreesPerRadian);
        }

        private void GetCurrentTranslation(out float xRange, out Matrix4 translation)
        {
            xRange = (_lineData.Count - 1) * _axisScaling;
            float yRange = (_lineData[0].Count - 1) * _axisScaling;
            float xOffset = -0.5f * xRange;
            float yOffset = -0.5f * yRange;
            float zOffset = -0.5f * (_zAxisLabels.MaxValue - _zAxisLabels.MinValue);
            translation = Matrix4.CreateTranslation(xOffset, yOffset, zOffset);
        }

        /*
         * The Model View matrix transforms the model into the projection space, and applies any rotations. 
         */
        Matrix4 _modelview;
        private void UpdateModelViewMatrix()
        {
            GetCurrentTranslation(out float xRange, out Matrix4 translation);
            GetCurrentRotation(out Matrix4 rotation);

            Vector3 cameraLocation = new Vector3(0.0f, 0.0f, -xRange * (1.0f + (float)Zoom * 0.005f * (float)(_iConfiguration.Perspective * _iConfiguration.Perspective)));
            _modelview = translation * rotation * Matrix4.LookAt(cameraLocation, -Vector3.UnitZ, Vector3.UnitY);

            Matrix4 translationCentre = Matrix4.CreateTranslation(_xCentre, _yCentre, 0);

            _modelview = _modelview * translationCentre;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref _modelview);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        void DrawPoints(VertexStore vertices)
        {
            if ((vertices == null) || (vertices.NumberOfNodes != _lineData.Count * _lineData[0].Count))
            {
                return;
            }

            /*
             * Inefficient but it works!
             */

            GL.PointSize(4);
            for (int index = 0; index < vertices.Vertices.Length; index += 5)
            {
                GL.Color4(vertices.Vertices[index].Colour.R, vertices.Vertices[index].Colour.G, vertices.Vertices[index].Colour.B, 1);

                GL.VertexAttrib3(
                    0,
                    _axisScaling * vertices.Vertices[index].X,
                    _axisScaling * vertices.Vertices[index].Y,
                    _iConfiguration.ViewProjection.IsBirdsEye() ? 0 : _axisScaling * vertices.Vertices[index].Z * (float)ZScale);
                GL.DrawArrays(PrimitiveType.Points, 0, 1);
            }
            GL.PointSize(1);
        }

        void DrawShadedSurface(VertexStore vertices)
        {
            if ((vertices == null) || (vertices.NumberOfNodes != _lineData.Count * _lineData[0].Count))
            {
                return;
            }

            float axisScaling = 0.25f * _axisScaling;

            GL.Begin(PrimitiveType.Triangles);
            for (int index = 0; index < vertices.Vertices.Length; index += 5)
            {
                float midpointX = (axisScaling * (vertices.Vertices[index].X + vertices.Vertices[index + 1].X + vertices.Vertices[index + 2].X + vertices.Vertices[index + 3].X));
                float midpointY = (axisScaling * (vertices.Vertices[index].Y + vertices.Vertices[index + 1].Y + vertices.Vertices[index + 2].Y + vertices.Vertices[index + 3].Y));
                float midpointZ = (axisScaling * (vertices.Vertices[index].Z + vertices.Vertices[index + 1].Z + vertices.Vertices[index + 2].Z + vertices.Vertices[index + 3].Z));

                float midpointR = 0.25f * (vertices.Vertices[index].Colour.R + vertices.Vertices[index + 1].Colour.R + vertices.Vertices[index + 2].Colour.R + vertices.Vertices[index + 3].Colour.R);
                float midpointG = 0.25f * (vertices.Vertices[index].Colour.G + vertices.Vertices[index + 1].Colour.G + vertices.Vertices[index + 2].Colour.G + vertices.Vertices[index + 3].Colour.G);
                float midpointB = 0.25f * (vertices.Vertices[index].Colour.B + vertices.Vertices[index + 1].Colour.B + vertices.Vertices[index + 2].Colour.B + vertices.Vertices[index + 3].Colour.B);

                for (int offset = 0; offset < 4; ++offset)
                {
                    int indexStart = index + offset;

                    if (_iConfiguration.ShadingMethod == ShadingMethod.Coarse)
                    {
                        GL.Color4(midpointR, midpointG, midpointB, 1f);
                    }
                    else if (_iConfiguration.ShadingMethod == ShadingMethod.Fine)
                    {
                        GL.Color4(
                            (midpointR + vertices.Vertices[indexStart].Colour.R + vertices.Vertices[indexStart + 1].Colour.R) / 3f,
                            (midpointG + vertices.Vertices[indexStart].Colour.G + vertices.Vertices[indexStart + 1].Colour.G) / 3f,
                            (midpointB + vertices.Vertices[indexStart].Colour.B + vertices.Vertices[indexStart + 1].Colour.B) / 3f,
                            1f);
                    }

                    if (_iConfiguration.ShadingMethod == ShadingMethod.Interpolated)
                    {
                        GL.Color4(vertices.Vertices[indexStart].Colour.R, vertices.Vertices[indexStart].Colour.G, vertices.Vertices[indexStart].Colour.B, 1);
                    }
                    GL.Vertex3(_axisScaling * vertices.Vertices[indexStart].X, _axisScaling * vertices.Vertices[indexStart].Y, _iConfiguration.ViewProjection.IsBirdsEye() ? 0f : _axisScaling * vertices.Vertices[indexStart].Z * ZScale);

                    if (_iConfiguration.ShadingMethod == ShadingMethod.Interpolated)
                    {
                        GL.Color4(vertices.Vertices[indexStart + 1].Colour.R, vertices.Vertices[indexStart + 1].Colour.G, vertices.Vertices[indexStart + 1].Colour.B, 1);
                    }
                    GL.Vertex3(_axisScaling * vertices.Vertices[indexStart + 1].X, _axisScaling * vertices.Vertices[indexStart + 1].Y, _iConfiguration.ViewProjection.IsBirdsEye() ? 0f : _axisScaling * vertices.Vertices[indexStart + 1].Z * ZScale);

                    if (_iConfiguration.ShadingMethod == ShadingMethod.Interpolated)
                    {
                        GL.Color4(midpointR, midpointG, midpointB, 1);
                    }
                    GL.Vertex3(midpointX, midpointY, _iConfiguration.ViewProjection.IsBirdsEye() ? 0 : midpointZ * ZScale);
                }
            }

            GL.End();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            MakeCurrent();

            if ((_iConfiguration == null) || (_xAxisLabels == null) || (_yAxisLabels == null) || (_zAxisLabels == null))
            {
                GL.ClearColor(System.Drawing.Color.Black);
                return;
            }

            // Adjust the background colour here. 
            GL.ClearColor(System.Drawing.Color.FromName(_iConfiguration.BackgroundColour));
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateProjectionMatrices();

            if ((_lineData == null) || (_lineData.Count == 0))
            {
                SwapBuffers();
                return;
            }

            UpdateModelViewMatrix();

            /*
             * Draw the grid 
             */
            if (_iConfiguration.ShowGrid)
            {
                /*
                 * Add on a small offset to ensure that the grid is on top of the shading
                 * Too big a value and the grid appears to float above the shading
                 * Too small a value and the grid is seen from the underside
                 */
                float z0 = (_zAxisLabels.MaxValue - _zAxisLabels.MinValue) * 0.01f;

                System.Drawing.Color lineColour = System.Drawing.Color.FromName(_iConfiguration.GridColour);
                for (int x = 0; x < _lineData.Count; ++x)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color3(lineColour);

                    for (int y = 0; y < _lineData[x].Count; ++y)
                    {
                        GL.Vertex3(_axisScaling * x, _axisScaling * y, _iConfiguration.ViewProjection.IsBirdsEye() ? 0 : z0 + _axisScaling * (float)_lineData[x][y] * (float)ZScale);
                    }
                    GL.End();
                }

                // Assume a regular square grid!
                for (int y = 0; y < _lineData[0].Count; ++y)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color3(lineColour);
                    for (int x = 0; x < _lineData.Count; ++x)
                    {
                        GL.Vertex3(_axisScaling * x, _axisScaling * y, _iConfiguration.ViewProjection.IsBirdsEye() ? 0 : z0 + _axisScaling * (float)_lineData[x][y] * (float)ZScale);
                    }
                    GL.End();
                }
            }

            /*
             * Surface shading or scatter plot
             */

            if (_iConfiguration.ShowShading)
            {
                lock (_verticesLock)
                {
                    DrawShadedSurface(_rawDataValues);
                }
            }
            else if (_iConfiguration.ShowScatterPlot)
            {
                lock (_verticesLock)
                {
                    DrawPoints(_rawDataValues);
                }
            }

            /*
             * Draw the frame
             * 
             * Doing this after the surface reduces issues with hidden lines being visible through the surface. 
             */
            /*
                         * The orginal code
                         * 
                        float zGridRange = (float)(_axisScaling * (_zAxisLabels.MaxValue - _zAxisLabels.MinValue) * ZScale);
                        float zGridInc = zGridRange / (float)(_zAxisLabels.Labels.Count - 1);
                        float zMin = -zGridRange / 2;
                        float zMax = -zMin;
             */
            float zLen = _zAxisLabels.MaxValue - _zAxisLabels.MinValue;             //Compute zLen = zMax - zMin
            float zGridRange = (float)(_axisScaling * zLen * ZScale);               //Replace zMax - zMin with zLen
            float zGridInc = zGridRange / (float)(_zAxisLabels.Labels.Count - 1);
            float zMin = zGridRange * (_zAxisLabels.MinValue / zLen);               //Compute current zMin by its length ratio
            float zMax = zGridRange * (_zAxisLabels.MaxValue / zLen);               //Compute current zMax by its length ratio

            float xMax = _axisScaling * (_lineData.Count - 1);
            float yMax = _axisScaling * (_lineData[0].Count - 1);

            if (_iConfiguration.ShowAxes && (_iConfiguration.ViewProjection == ViewProjection.ThreeDimensional))
            {
                System.Drawing.Color lineColour = System.Drawing.Color.FromName(_iConfiguration.FrameColour);
                float z = (_iConfiguration.XYLabelPosition == XYLabelPosition.Bottom) ? zMin : 0f;

                GL.Begin(PrimitiveType.LineStrip);
                GL.Color3(lineColour);
                GL.Vertex3(0, 0, z);
                GL.Vertex3(xMax, 0, z);
                GL.End();

                GL.Begin(PrimitiveType.LineStrip);
                GL.Color3(lineColour);
                GL.Vertex3(0, 0, z);
                GL.Vertex3(0, yMax, z);
                GL.End();

                GL.Begin(PrimitiveType.LineStrip);
                GL.Color3(lineColour);
                GL.Vertex3(0, 0, zMin);
                GL.Vertex3(0, 0, zMax);
                GL.End();
            }

            if (!_iConfiguration.ViewProjection.IsBirdsEye() && _iConfiguration.ShowFrame)
            {
                System.Drawing.Color lineColour = System.Drawing.Color.FromName(_iConfiguration.FrameColour);

                /*
                 * Horizontal lines around sides and base
                 */

                float x = 0;
                float xIncrement = 0;
                float y = 0;
                float yIncrement = 0;

                float zValue = zMin;
                for (int i = 0; i < _zAxisLabels.Labels.Count; ++i, zValue += zGridInc)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color3(lineColour);
                    GL.Vertex3(0, 0, zValue);
                    GL.Vertex3(xMax, 0, zValue);
                    GL.End();

                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color3(lineColour);
                    GL.Vertex3(0, 0, zValue);
                    GL.Vertex3(0, yMax, zValue);
                    GL.End();

                    xIncrement = xMax / (float)(_xAxisLabels.Labels.Count - 1);
                    x = 0;
                    for (int j = 0; j < _xAxisLabels.Labels.Count; ++j, x += xIncrement)
                    {
                        GL.Begin(PrimitiveType.LineStrip);
                        GL.Color3(lineColour);
                        GL.Vertex3(x, 0, zMin);
                        GL.Vertex3(x, yMax, zMin);
                        GL.End();
                    }

                    yIncrement = yMax / (float)(_yAxisLabels.Labels.Count - 1);
                    y = 0;
                    for (int j = 0; j < _yAxisLabels.Labels.Count; ++j, y += yIncrement)
                    {
                        GL.Begin(PrimitiveType.LineStrip);
                        GL.Color3(lineColour);
                        GL.Vertex3(0, y, zMin);
                        GL.Vertex3(xMax, y, zMin);
                        GL.End();
                    }
                }

                /*
                 * Vertical lines around edges
                 */

                xIncrement = xMax / (float)(_xAxisLabels.Labels.Count - 1);
                x = 0;
                for (int j = 0; j < _xAxisLabels.Labels.Count; ++j, x += xIncrement)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color3(lineColour);
                    GL.Vertex3(x, 0, zMin);
                    GL.Vertex3(x, 0, zMax);
                    GL.End();
                }

                yIncrement = yMax / (float)(_yAxisLabels.Labels.Count - 1);
                y = 0;
                for (int j = 0; j < _yAxisLabels.Labels.Count; ++j, y += yIncrement)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color3(lineColour);
                    GL.Vertex3(0, y, zMin);
                    GL.Vertex3(0, y, zMax);
                    GL.End();
                }
            }

            float textHeight = _textRenderer.MeasureTextHeight();
            Matrix4 translation;
            Matrix4 rotation;
            Matrix4 textModelView;

            translation = Matrix4.CreateTranslation(xMax, 0, 0);
            rotation = Matrix4.CreateRotationX(0.5F * (float)Math.PI) * Matrix4.CreateRotationY(0.5F * (float)Math.PI) * Matrix4.CreateRotationZ(1F * (float)Math.PI);
            textModelView = rotation * translation * _modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref textModelView);
            _textRenderer.SetTiltInRadians(0);

            bool flipAxisTitle =
                (_iConfiguration.ViewProjection == ViewProjection.BirdsEye_180) ||
                (_iConfiguration.ViewProjection == ViewProjection.BirdsEye_270);

            if (_iConfiguration.ShowAxesTitles && (_iConfiguration.ViewProjection == ViewProjection.ThreeDimensional))
            {
                string text = GetZAxisTitle();
                float y = flipAxisTitle ? 3f * textHeight : - 3f * textHeight;
                if (_iConfiguration.ShowLabels)
                {
                    y += flipAxisTitle ? _zAxisLabels.MaxLabelLength : -_zAxisLabels.MaxLabelLength;
                }
                _textRenderer.DrawText(-0.5f * _textRenderer.MeasureTextLength(text), y, 0, text);
            }

            _textRenderer.SetTiltInRadians((Math.PI * (double)_iConfiguration.LabelAngleInDegrees) / 180f);

            if (_iConfiguration.ViewProjection == ViewProjection.BirdsEye_180)
            {
                translation = Matrix4.CreateTranslation(0, yMax, 0);
                rotation = Matrix4.CreateRotationX(0.5F * (float)Math.PI);
            }
            else
            {
                translation = Matrix4.CreateTranslation(xMax, 0, 0);
                rotation = Matrix4.CreateRotationX(0.5F * (float)Math.PI) * Matrix4.CreateRotationZ(1F * (float)Math.PI);
            }
            textModelView = rotation * translation * _modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref textModelView);

            float labelOffset = (
                                !_iConfiguration.ViewProjection.IsBirdsEye() &&
                                (_iConfiguration.XYLabelPosition == XYLabelPosition.Bottom)
                           ) ? zMin : 0f;

            if (_iConfiguration.ShowAxesTitles && !_iConfiguration.ViewProjection.IsOrthographic())
            {
                string text = GetXAxisTitle();
                float z = flipAxisTitle ? -3f * textHeight : yMax + 3f * textHeight;
                if (_iConfiguration.ShowLabels)
                {
                    z += flipAxisTitle ? -_xAxisLabels.MaxLabelLength : _xAxisLabels.MaxLabelLength;
                }
                _textRenderer.DrawText(
                    0.5f * (xMax - _textRenderer.MeasureTextLength(text)),
                    labelOffset,
                    z,
                    text);
            }

            if (_iConfiguration.ShowLabels)
            {
                if (!_iConfiguration.ViewProjection.IsOrthographic())
                {
                    // Y labels

                    float z = textHeight;
                    float zInc = yMax / (float)(_yAxisLabels.Labels.Count - 1);
                    for (int i = 0; i < _yAxisLabels.Labels.Count; ++i, z += zInc)
                    {
                        _textRenderer.DrawText(-_yAxisLabels.Labels[i].Length - textHeight, labelOffset, z, _yAxisLabels.Labels[i].Text);
                    }
                }

                if (!_iConfiguration.ViewProjection.IsBirdsEye())
                {
                    // Data labels 

                    _textRenderer.SetTiltInRadians(0);

                    float z = zMin;
                    for (int i = 0; i < _zAxisLabels.Labels.Count; ++i, z += zGridInc)
                    {
                        _textRenderer.DrawText(-_zAxisLabels.Labels[i].Length - textHeight, z, 0, _zAxisLabels.Labels[i].Text);
                    }
                }
            }

            // X labels

            _textRenderer.SetTiltInRadians((Math.PI * (double)_iConfiguration.LabelAngleInDegrees) / 180f);
            if (_iConfiguration.ViewProjection == ViewProjection.BirdsEye_270)
            {
                translation = Matrix4.CreateTranslation(xMax, 0, 0);
                rotation = Matrix4.CreateRotationX(1.5F * (float)Math.PI) * Matrix4.CreateRotationZ(1.5F * (float)Math.PI) * Matrix4.CreateRotationY((float)Math.PI);
            }
            else
            {
                translation = Matrix4.CreateTranslation(0, yMax, 0);
                rotation = Matrix4.CreateRotationX(0.5F * (float)Math.PI) * Matrix4.CreateRotationZ(0.5F * (float)Math.PI);
            }
            textModelView = rotation * translation * _modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref textModelView);

            if (_iConfiguration.ShowAxesTitles && !_iConfiguration.ViewProjection.IsOrthographic())
            {
                string text = GetYAxisTitle();
                float z = flipAxisTitle ? -3f * textHeight : xMax + 3f * textHeight;
                if (_iConfiguration.ShowLabels)
                {
                    z += flipAxisTitle ? -_yAxisLabels.MaxLabelLength : _yAxisLabels.MaxLabelLength;
                }
                _textRenderer.DrawText(
                    -0.5f * (yMax + _textRenderer.MeasureTextLength(text)),
                    labelOffset,
                    z,
                    text);
            }

            if (_iConfiguration.ShowLabels)
            {
                if (!_iConfiguration.ViewProjection.IsOrthographic())
                {
                    float z = textHeight;
                    float zInc = xMax / (float)(_xAxisLabels.Labels.Count - 1);
                    for (int i = 0; i < _xAxisLabels.Labels.Count; ++i, z += zInc)
                    {
                        _textRenderer.DrawText(_textRenderer.MeasureTextHeight(), labelOffset, z, _xAxisLabels.Labels[i].Text);
                    }
                }
            }

            if (_iConfiguration.ShowZBar)
            {
                // Reverse the current model view rotation and translation 

                GetCurrentTranslation(out float xRange, out translation);
                translation.Invert();

                GetCurrentRotation(out rotation);
                rotation.Transpose();

                textModelView = Matrix4.CreateRotationY((float)System.Math.PI) * rotation * translation * _modelview;
                textModelView = Matrix4.CreateTranslation(0, 0, -xRange / 2f) * textModelView;
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref textModelView);

                float width = _textRenderer.MeasureTextHeight();
                float x = 0.5f * (float)System.Math.Sqrt(xMax * xMax + yMax * yMax) + 1f * width;
                switch (_iConfiguration.ViewProjection)
                {
                    case ViewProjection.ThreeDimensional:
                        x = 0.5f * (float)System.Math.Sqrt(xMax * xMax + yMax * yMax) + 1f * width;
                        x += _zAxisLabels.MaxLabelLength;
                        break;
                    case ViewProjection.BirdsEye_90:
                    case ViewProjection.BirdsEye_270:
                        x = 0.5f * (float)System.Math.Max(xMax, yMax) + 3f * width;
                        x += _zAxisLabels.MaxLabelLength;
                        if (_iConfiguration.ShowAxesTitles)
                        {
                            x += 2f * textHeight;
                        }
                        break;
                    case ViewProjection.Orthographic_Front:
                    case ViewProjection.Orthographic_Side:
                    case ViewProjection.BirdsEye_0:
                    case ViewProjection.BirdsEye_180:
                        x = 0.5f * (float)System.Math.Max(xMax, yMax) + 3f * width;
                        break;
                }

                float y = zMin - 1.5f * zGridInc - 0.5f * textHeight;
                float yBar = zMin - 2f * zGridInc;
                switch (_iConfiguration.ViewProjection)
                {
                    case ViewProjection.BirdsEye_180:
                    case ViewProjection.BirdsEye_270:
                        yBar += 2f * zGridInc;
                        y += 2f * zGridInc;
                        break;
                    case ViewProjection.BirdsEye_0:
                    case ViewProjection.BirdsEye_90:
                        yBar += zGridInc;
                        y += zGridInc;
                        break;
                }
                _textRenderer.SetTiltInRadians(0);

                float zRaw = _zAxisLabels.MinValue;
                float zRawInc = (_zAxisLabels.MaxValue - _zAxisLabels.MinValue) / (float)(_zAxisLabels.Labels.Count - 1);
                for (int i = 0; i < _zAxisLabels.Labels.Count; ++i, y += zGridInc, zRaw += zRawInc, yBar += zGridInc)
                {
                    _textRenderer.DrawText(x + width + width, y, 0, _zAxisLabels.Labels[i].Text);

                    GL.Begin(PrimitiveType.Polygon);
                    GL.Color4(GetVertexColour(zRaw));
                    GL.Vertex3(x, yBar, 0);
                    GL.Vertex3(x, yBar + zGridInc, 0);
                    GL.Vertex3(x + width, yBar + zGridInc, 0);
                    GL.Vertex3(x + width, yBar, 0);
                    GL.Vertex3(x, yBar, 0);
                    GL.End();
                }
            }

            SwapBuffers();
        }

        #region Model.ConfigurationChangedEventHandler

        public void ConfigurationChangedEventHandler(ConfigurationItem configurationItem)
        {
            switch (configurationItem)
            {
                case ConfigurationItem.BackgroundColour:
                // Fall through
                case ConfigurationItem.LabelAngleInDegrees:
                // Fall through
                case ConfigurationItem.LabelColour:
                // Fall through
                case ConfigurationItem.LabelFontSize:
                // Fall through
                case ConfigurationItem.TransparentLabelBackground:
                    CreateTextRenderer();
                    break;
                case ConfigurationItem.ViewProjection:
                    _xAxisLabels.Refresh(
                        _iConfiguration.ViewProjection == ViewProjection.BirdsEye_270,
                        ILabelFormatter.XLabel,
                        _textRenderer.MeasureTextLength);

                    _yAxisLabels.Refresh(
                        (_iConfiguration.ViewProjection == ViewProjection.BirdsEye_180),
                        ILabelFormatter.YLabel,
                        _textRenderer.MeasureTextLength);

                    _zAxisLabels.Refresh(
                        false,
                        ILabelFormatter.ZLabel,
                        _textRenderer.MeasureTextLength);
                    break;
            }
            Invalidate(ClientRectangle);
        }

        #endregion Model.ConfigurationChangedEventHandler
    }
}
