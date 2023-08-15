using System;
using Ossify;
using UnityEngine;

namespace PolyPoke
{
    public abstract class Whizz : Dispensable
    {
        public abstract void Begin(Vector3 worldPoint); 

        public abstract void During(Vector3 worldPoint);

        public abstract void End(Vector3 worldPoint);
    }
}