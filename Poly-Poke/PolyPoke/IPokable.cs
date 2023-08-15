using System;
using Ossify;
using UnityEngine;

namespace PolyPoke
{
    public interface IPokable
    {
        IPokeSession GetPokeSession();
    }

    public interface IPokeSession : IDisposable
    {
        bool Disposed { get; }

        Dispenser<Whizz> WhizzDispenser { get; }

        void Update(Vector3 pokePosition);
    }

    public class PickupCollider2DPokeSession : IPokeSession
    {
        private readonly Collider2D collider2D;
        private readonly Vector3 offset;

        public PickupCollider2DPokeSession(Collider2D collider2D, Vector3 pokePosition)
        {
            this.collider2D = collider2D;

            offset = collider2D.transform.position - pokePosition;
        }

        /// <inheritdoc />
        public void Dispose() => Disposed = true;

        /// <inheritdoc />
        public bool Disposed { get; private set; }

        /// <inheritdoc />
        public void Update(Vector3 pokePosition) => collider2D.transform.position = pokePosition + offset;

        /// <inheritdoc />
        public Dispenser<Whizz> WhizzDispenser => null;
    }
}