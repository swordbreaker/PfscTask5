using OpenTK;
using System;
using OpenTK.Graphics.OpenGL4;
using Vector4 = OpenTK.Vector4;
using System.IO;
using System.Collections.Generic;
using OpenTK.Input;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace PfscTask5
{
    internal static class Program
    {
        private static Texture _texture;

        private static readonly CameraHelper CameraHelper = new CameraHelper(0, 0, 5);

        private static GlProgram _glProgram;
        private static GlProgram _shadelessProgramm;

        private static Matrix4 _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1, 0.1f, 100);

        private static GameWindow w;

        private static float m = 1;
        private static float a = 0.2f;
        private static float b = 0.3f;
        private static float c = 0.2f;

        private static readonly LinkedList<RotatingCube> Cubes = new LinkedList<RotatingCube>();

        private static Thread _uiThread;
        private static Application _uiApp;

        private static ExplodeType _explodeType = ExplodeType.Sphere;
        private static int _numExplode = 20;

        private static void Main()
        {
            w = new GameWindow(720, 480, null, "Task 5", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible);

            ShowInfoWindow();

            w.KeyDown += CameraHelper.OnKeyDown;
            w.KeyDown += OnKeyDown;
            w.MouseWheel += CameraHelper.OnMouseWheel;
            w.Closed += (object sender, EventArgs e) => _uiApp.Dispatcher.Invoke(() => _uiApp.Shutdown());

            w.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                var x = e.X * 2f / w.Width - 1;
                var y = e.Y * -2f / w.Height + 1;
                var vec = new Vector4(x, y, 0, 1);
                //var vec2 = projection * _cameraHelper.CameraMatrix * vec;

                var vm = CameraHelper.CameraMatrix;
                //var v = Vector4.Transform(vec, vm);
                //var v = Vector3.TransformPosition(vec, vm);
                var v =  vec * vm;
                v = _projection * v;
                v /= v.W;


                Explode(v.Xyz, _explodeType);
            };

            w.Load += OnLoad;
            w.RenderFrame += Render;

            w.Resize += Resize;

            w.Run();
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            //set up opengl
            GL.ClearColor(0.5f, 0.5f, 0.5f, 0);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.FramebufferSrgb);

            _glProgram = new GlProgram(@"Shaders\VertexShader.glsl", @"Shaders\FragmentShader.glsl");
            _shadelessProgramm = new GlProgram(@"Shaders\VertexShaderShadeless.glsl", @"Shaders\FragmentShaderShadeless.glsl");

            //Textures
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            _texture = new Texture(@"Textures\05.JPG");

            //check for errors during all previous calls
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception(error.ToString());

            _glProgram.Use();

            GL.Uniform4(GL.GetUniformLocation(_glProgram, "enviroment"), new Vector4(0f, 0f, 0f, 1));
            GL.Uniform1(GL.GetUniformLocation(_glProgram, "texture1"), _texture);
        }

        private static void Render(object sender, EventArgs e)
        {
            //clear screen and z-buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UniformMatrix4(GL.GetUniformLocation(_glProgram, "p"), false, ref _projection);

            var node = Cubes.First;

            while (node != null)
            {
                var cube = node.Value;
                cube.Render((float)w.RenderTime * 0.2f, CameraHelper.CameraMatrix);
                //_shadelessProgramm.Use();
                //GL.LineWidth(5);
                //var axis = cube.Rotation.ToAxisAngle();
                ////var line = Figures.Line(_shadelessProgramm, cube.Pos, cube.Pos + axis.Xyz, Colors.White);
                //var line = Figures.Line(_shadelessProgramm, new Vector3(1,1,0), new Vector3(0, 0, 0), Colors.White);
                //line.ViewModel = CameraHelper.CameraMatrix;
                //line.Render(PrimitiveType.Triangles);
                //_glProgram.Use();
                var newNode = node.Next;
                if (cube.TimeToLife < 0) Cubes.Remove(node);
                node = newNode;
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
            _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, r, 0.1f, 100);
            GL.Viewport(w.ClientRectangle);
        }

        public static void OnKeyDown(object sender, KeyboardKeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Key)
            {
                case Key.R:
                    CameraHelper.Azimut = 0;
                    CameraHelper.Elevation = 0;
                    CameraHelper.Radius = 5;
                    Cubes.Clear();
                    break;
                case Key.Space:
                    var p = new Vector3(-5, Mathf.GetRandomRange(-3, 3), 0);
                    Cubes.AddLast(new RotatingCube(_glProgram, a, b, c, m, p));
                    break;
                case Key.X:
                    var x = Mathf.GetRandomRange(-3, 3);
                    var y = Mathf.GetRandomRange(-2, 2);
                    Explode(new Vector3(x, y, 0), _explodeType);
                    break;
                case Key.C:
                    _explodeType = (ExplodeType)(((int)_explodeType + 1) % (Enum.GetValues(typeof(ExplodeType)).Length));
                    break;
                case Key.Y:
                    _numExplode--;
                    if (_numExplode < 1) _numExplode = 1;
                    break;
                case Key.V:
                    _numExplode++;
                    if (_numExplode > 100) _numExplode = 100;
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

        private enum ExplodeType { Sphere, Tan, Random, Cube }

        private static void Explode(Vector3 pos, ExplodeType explodeType)
        {
            if(explodeType == ExplodeType.Cube)
            {
                var verts = Figures.CubeVerts;
                var normals = Figures.CubeNormals;

                for (int i = 0; i < verts.Length; i += 3)
                {
                    var p = new Vector3(verts[i], verts[i + 1], verts[i + 2]);
                    var v = new Vector3(normals[i], normals[i + 1], normals[i + 2]);

                    Cubes.AddLast(new RotatingGravityCube(_glProgram, a, b, c, m, p, v * 2));
                }

                return;
            }

            var n = _numExplode;
            var phi = 2f * (float)Math.PI / n;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var v = Vector3.Zero;

                    switch (explodeType)
                    {
                        case ExplodeType.Sphere:
                            var vxy = new Vector3((float)Math.Cos(phi * i), (float)Math.Sin(phi * i), 0);
                            var vyz = new Vector3((float)Math.Cos(phi * j), 0, (float)Math.Sin(phi * j));
                            v = vxy + vyz;
                            break;
                        case ExplodeType.Tan:
                            v = new Vector3((float)Math.Cos(phi * i), (float)Math.Sin(phi * i), (float)Math.Tan(phi * j));
                            break;
                        case ExplodeType.Random:
                            v = new Vector3(Mathf.GetRandomRange(-1, 1), Mathf.GetRandomRange(-1, 1), Mathf.GetRandomRange(-1, 1));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(explodeType), explodeType, null);
                    }

                    Cubes.AddLast(new RotatingGravityCube(_glProgram, a, b, c, m, pos, v * 5));
                }

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
