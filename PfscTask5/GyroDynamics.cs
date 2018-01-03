﻿using OpenTK;
using System;

namespace PfscTask5
{
    public class GyroDynamics : Dynamics
    {
        //Trägheitsmomente
        private float _i1;
        private float _i2;
        private float _i3;

        public float I1 { set => _i1 = value; }
        public float I2 { set => _i1 = value; }
        public float I3 { set => _i1 = value; }

        private float[] _x;

        public GyroDynamics(float i1, float i2, float i3)
        {
            _i1 = i1;
            _i2 = i2;
            _i3 = i3;
        }

        public void Move(float dt)
        {
            _x = Runge(_x, dt);
        }


        protected override float[] F(float[] xs)
        {
            float w1 = _x[0], w2 = _x[1], w3 = _x[2];
            float q0 = _x[3], q1 = _x[4], q2 = _x[5], q3 = _x[6];

            float[] y = {
                (_i2 - _i3)/_i1 * w2 * w3,
                (_i3 - _i1)/_i2 * w3 * w1,
                (_i1 - _i2)/_i3 * w1 * w2,
                -0.5f *(q1 * w1 + q2 * w2 + q3 * w3),
                0.5f * (q0 * w1 + q2 * w3 - q3 * w2),
                0.5f * (q0 * w2 + q3 * w1 - q1 * w3),
                0.5f * (q0 * w3 + q1 * w2 - q2 * w1),
            };
            return y;
        }

        public void SetState(float[] state)
        {
            SetState(state[0], state[1], state[2], state[3], state[4], state[5], state[6]);
        }

        public void SetState(float w1, float w2, float w3, float phi, float n1, float n2, float n3)
        {
            SetState(w1, w2, w3, phi, new Vector3(n1, n2, n3));
        }

        public void SetState(float w1, float w2, float w3, float phi, Vector3 n)
        {
            //var q0 = (float)Math.Cos(Mathf.ToDeg(phi) / 2);
            //n = Vector3.Normalize(n);
            //var qn = n * (float) Math.Sin(0.5 * phi);

            //_x = new float[] { w1, w2, w3, q0, qn.X, qn.Y, qn.Z };

            _x = new float[] { w1, w2, w3, phi, n.X, n.Y, n.Z };
        }

        public void SetState(float w1, float w2, float w3, Quaternion q)
        {
            //q.Normalize();
            SetState(w1, w2, w3, q.W, q.Xyz);
        }

        public float[] GetState()
        {
            //float w1 = _x[0], w2 = _x[1], w3 = _x[2];
            //float q0 = _x[3], q1 = _x[4], q2 = _x[5], q3 = _x[6];

            ////var phi = (float)(2 * (float)Math.Acos(q0) * 180 / Math.PI);
            //var phi = (float)(2 * Math.Acos(q0));
            //return new[] { w1, w2, w3, phi, q1, q2, q3 };
            return _x;
        }

        public Quaternion GetRotation()
        {
            var q = new Quaternion(_x[4], _x[5], _x[6], _x[3]);
            q.Normalize();
            return q;
        }
    }
}
