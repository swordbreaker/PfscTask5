using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace PfscTask5
{
    public class RotatingCube
    {
        const float xBorder = 4f;

        private float _a;
        private float _b;
        private float _c;
        private Vector3 _pos;

        private readonly GyroDynamics _gyroDynamics;
        private readonly Mesh _cube;

        public RotatingCube(int hProgram, float a, float b,  float c, float m, Vector3 pos)
        {
            _a = a;
            _b = b;
            _c = c;

            _pos = pos;

            var w1 = Mathf.GetRandomRange(0.1f, 10f);
            var w2 = Mathf.GetRandomRange(0.1f, 10f);
            var w3 = Mathf.GetRandomRange(0.1f, 10f);

            float i1 = (1 / 12f) * m * (b * b + c * c);
            float i2 = (1 / 12f) * m * (a * a + c * c);
            float i3 = (1 / 12f) * m * (a * a + b * b);

            _gyroDynamics = new GyroDynamics(i1, i2, i3);
            _gyroDynamics.SetState(w1, w2, w3, Quaternion.Identity);
            _cube = Figures.Cube(hProgram);
        }

        public void Update(float a, float b, float c, float m)
        {
            _a = a;
            _b = b;
            _c = c;

            _gyroDynamics.I1 = (1 / 12f) * m * (b * b + c * c);
            _gyroDynamics.I2 = (1 / 12f) * m * (a * a + c * c);
            _gyroDynamics.I3 = (1 / 12f) * m * (a * a + b * b);
        }

        public void Render(float dt, Matrix4 m)
        {
            var q = _gyroDynamics.GetRotation();

            _cube.ViewModel = Matrix4.CreateScale(_a, _b, _c) * Matrix4.CreateFromQuaternion(q) * Matrix4.CreateTranslation(_pos) * m;
            _cube.Render(PrimitiveType.Triangles);
            _gyroDynamics.Move(dt);
            _pos.X = ((_pos.X + dt * 2 + xBorder) % (xBorder * 2)) - xBorder;
        }
    }
}
