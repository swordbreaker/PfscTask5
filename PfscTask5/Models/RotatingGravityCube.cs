using OpenTK;

namespace PfscTask5
{
    public class RotatingGravityCube : RotatingCube
    {
        private const float Mass = 2;

        private readonly GravityBody _gBody;

        public override Vector3 Pos => _gBody.Pos;

        public RotatingGravityCube(int hProgram, float a, float b, float c, float m, Vector3 pos, Vector3 v) : base(hProgram, a, b, c, m, pos)
        {
            _gBody = new GravityBody(v, pos, Mass);
        }

        public override Matrix4 GetTransaltionM(float dt)
        {
            var pos = _gBody.Update(dt);
            return Matrix4.CreateTranslation(pos);
        }
    }
}