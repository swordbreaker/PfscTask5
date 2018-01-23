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
        public float TimeToLife { get; private set; }

        public virtual Vector3 Pos => _pos;

        public RotatingCube(int hProgram, float a, float b,  float c, float m, Vector3 pos, float lifeTime = 2)
        {
            _a = a;
            _b = b;
            _c = c;

            _pos = pos;
            TimeToLife = lifeTime;

            var w1 = Mathf.GetRandomRange(0.1f, 10f);
            var w2 = Mathf.GetRandomRange(0.1f, 10f);
            var w3 = Mathf.GetRandomRange(0.1f, 10f);

            var i1 = (1 / 12f) * m * (b * b + c * c);
            var i2 = (1 / 12f) * m * (a * a + c * c);
            var i3 = (1 / 12f) * m * (a * a + b * b);

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

        public virtual Matrix4 GetRotationM(float dt)
        {
            var q = _gyroDynamics.GetRotation();
            var m = Matrix4.CreateFromQuaternion(q);
            _pos.X = ((_pos.X + dt * 2 + xBorder) % (xBorder * 2)) - xBorder;
            _gyroDynamics.Move(dt);

            return Matrix4.CreateFromQuaternion(q);
        }

        public virtual Matrix4 GetTransaltionM(float dt)
        {
            var m = Matrix4.CreateTranslation(_pos);
            _pos.X = ((_pos.X + dt * 2 + xBorder) % (xBorder * 2)) - xBorder;
            return m;
        }

        public Quaternion Rotation => _gyroDynamics.GetRotation();

        public virtual void Render(float dt, Matrix4 m)
        {
            _cube.ViewModel = Matrix4.CreateScale(_a, _b, _c) * GetRotationM(dt) * GetTransaltionM(dt) * m;
            _cube.Render(PrimitiveType.Triangles);
            TimeToLife -= dt;
        }
    }
}
