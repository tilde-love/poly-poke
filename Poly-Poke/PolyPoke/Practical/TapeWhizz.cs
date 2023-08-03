using Ossify;
using UnityEngine;

namespace PolyPoke
{
    public class TapeWhizz : Whizz 
    {
        [SerializeReference] private Honker honker;
        [SerializeReference] private ObjectDispenser start;
        [SerializeReference] private Dispenser<Whizz> tape;
        [SerializeReference] private ObjectDispenser end;

        Whizz tapeInstance; 

        /// <inheritdoc />
        public override void Begin(Vector3 worldPoint)
        {
            if (start != null) Plop(worldPoint,start.Dispense());

            if (tape != null)
            { 
                tapeInstance = tape.Dispense();
                tapeInstance.During(worldPoint);
            }
        }

        /// <inheritdoc />
        public override void During(Vector3 worldPoint)
        {
            if (tapeInstance != null) tapeInstance.During(worldPoint);
        }

        /// <inheritdoc />
        public override void End(Vector3 worldPoint)
        {
            if (tapeInstance != null) tapeInstance.End(worldPoint);
            if (end != null) Plop(worldPoint,end.Dispense());
        }

        void Plop(Vector3 pos, GameObject item) 
        {
            item.transform.position = pos;            
        }
    }
}