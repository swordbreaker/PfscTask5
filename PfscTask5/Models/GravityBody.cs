using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using OpenTK;

namespace PfscTask5
{
    public class GravityBody
    {
        public static float G = 9.81f;

        private readonly Vector3 V0;
        private Vector3 _v;
        private readonly float _mass;
        private readonly Vector3 _a = new Vector3(0f, -G, 0);

        public Vector3 Pos;

        public GravityBody(Vector3 v0, Vector3 pos0, float mass)
        {
            V0 = v0;
            _v = V0;
            _mass = mass;
            Pos = pos0;
        }

        public Vector3 Update(float dt)
        {
            Pos = Pos + _v * dt * _mass;
            _v = _v + _a * dt * _mass;
            return Pos;
        }
    }
}
