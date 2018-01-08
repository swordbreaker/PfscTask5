using OpenTK;
using System;
using OpenTK.Graphics.OpenGL4;
using Vector4 = OpenTK.Vector4;
using System.IO;
using System.Collections.Generic;
using OpenTK.Input;
using System.Threading;
using System.Windows;

namespace PfscTask5
{
    internal static class Program
    {
        private static Texture _texture;

        private static CameraHelper _cameraHelper = new CameraHelper(0, 0, 5);

        private static int hProgram = 0;

        private static Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1, 0.1f, 100);

        private static GameWindow w;

        private static float m = 1;
        private static float a = 0.2f;
        private static float b = 0.3f;
        private static float c = 0.2f;

        private static List<RotatingCube> cubes = new List<RotatingCube>();

        private static Thread _uiThread;
        private static Application _uiApp;

        private static void Main()
        {
            w = new GameWindow(720, 480, null, "Task 5", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible);

            ShowInfoWindow();

            w.KeyDown += _cameraHelper.OnKeyDown;
            w.KeyDown += OnKeyDown;
            w.MouseWheel += _cameraHelper.OnMouseWheel;
            w.Closed += (object sender, EventArgs e) => _uiApp.Dispatcher.Invoke(()=> _uiApp.Shutdown());

            //w.MouseDown += (object sender, MouseButtonEventArgs e) =>
            //{
            //    var x = e.X * 2f / w.Width - 1;
            //    var y = e.Y * -2f / w.Height + 1;
            //    var vec = new Vector4(x, y, 1, 0);
            //    var vec2 = projection * _cameraHelper.CameraMatrix * vec;
            //    cubes.Add(new RotatingCube(hProgram, a, b, c, m, vec2.Xyz));
            //};

            w.Load += OnLoad;
            w.RenderFrame += Render;

            w.Resize += Resize;

            w.Run();
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            //set up opengl
            GL.ClearColor(0.5f, 0.5f, 0.5f, 0);
            //GL.ClearDepth(1);
            GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Less);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.FramebufferSrgb);

            //load, compile and link shaders
            var VertexShaderSource = File.ReadAllText(@"Shaders\VertexShader.glsl");

            var hVertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(hVertexShader, VertexShaderSource);
            GL.CompileShader(hVertexShader);
            GL.GetShader(hVertexShader, ShaderParameter.CompileStatus, out int status);
            if (status != 1)
                throw new Exception(GL.GetShaderInfoLog(hVertexShader));

            var FragmentShaderSource = File.ReadAllText(@"Shaders\FragmentShader.glsl");

            var hFragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(hFragmentShader, FragmentShaderSource);
            GL.CompileShader(hFragmentShader);
            GL.GetShader(hFragmentShader, ShaderParameter.CompileStatus, out status);
            if (status != 1)
                throw new Exception(GL.GetShaderInfoLog(hFragmentShader));

            //link shaders to a program
            hProgram = GL.CreateProgram();
            GL.AttachShader(hProgram, hFragmentShader);
            GL.AttachShader(hProgram, hVertexShader);
            GL.LinkProgram(hProgram);
            GL.GetProgram(hProgram, GetProgramParameterName.LinkStatus, out status);
            if (status != 1)
                throw new Exception(GL.GetProgramInfoLog(hProgram));

            //Textures
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            _texture = new Texture(@"Textures\05.JPG");

            {
                //check for errors during all previous calls
                var error = GL.GetError();
                if (error != ErrorCode.NoError)
                    throw new Exception(error.ToString());
            }

            GL.UseProgram(hProgram);

            GL.Uniform4(GL.GetUniformLocation(hProgram, "enviroment"), new Vector4(0f, 0f, 0f, 1));
            GL.Uniform1(GL.GetUniformLocation(hProgram, "texture1"), _texture);
        }

        private static void Render(object sender, EventArgs e)
        {
            //clear screen and z-buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UniformMatrix4(GL.GetUniformLocation(hProgram, "p"), false, ref projection);

            foreach (var cube in cubes)
            {
                cube.Render((float)w.RenderTime * 0.2f, _cameraHelper.CameraMatrix);
            }

            //display
            w.SwapBuffers();

            var error = GL.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception(error.ToString());
        }

        private static void Resize(object sender, EventArgs e)
        {
            var r = w.Width / (float)w.Height;
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, r, 0.1f, 100);
            GL.Viewport(w.ClientRectangle);
        }

        public static void OnKeyDown(object sender, KeyboardKeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Key)
            {
                case Key.R:
                    _cameraHelper.Azimut = 0;
                    _cameraHelper.Elevation = 0;
                    _cameraHelper.Radius = 5;
                    cubes.Clear();
                    break;
                case Key.Space:
                    var p = new Vector3(-5, Mathf.GetRandomRange(-3, 3), 0);
                    cubes.Add(new RotatingCube(hProgram, a, b, c, m, p));
                    break;
                case Key.Number1:
                    a = 0.2f;
                    b = 0.2f;
                    c = 0.2f;
                    break;
                case Key.Number2:
                    a = 0.2f;
                    b = 0.3f;
                    c = 0.2f;
                    break;
                case Key.Number3:
                    a = 0.1f;
                    b = 0.3f;
                    c = 0.2f;
                    break;
                case Key.Number4:
                    a = 0.1f;
                    b = 0.2f;
                    c = 0.4f;
                    break;
                case Key.F1:
                    ShowInfoWindow();
                    break;
            }
        }

        private static void ShowInfoWindow()
        {
            if (_uiApp == null)
            {
                _uiThread = new Thread(() =>
                {
                    _uiApp = new UiApplication();
                    _uiApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    _uiApp.Run();
                });
                _uiThread.SetApartmentState(ApartmentState.STA);
                _uiThread.Start();
            }
            else
            {
                _uiApp.Dispatcher.Invoke(() =>
                {
                    var w = new InfoWindow();
                    w.Visibility = Visibility.Visible;
                    w.Show();
                });
            }
        }
    }
}
