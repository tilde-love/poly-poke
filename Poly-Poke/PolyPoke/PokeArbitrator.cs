using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PolyPoke
{
    public interface IPokeArbitrator : IDisposable
    {
        ILicenseToPoke GetLicense(GameObject subject);

        bool IsLicenceAvailable(GameObject subject);

        bool TryGetLicense(GameObject subject, out ILicenseToPoke license);
    }

    public interface ILicenseToPoke : IDisposable
    {
        bool Disposed { get; }
    }

    public class PokeArbitrator : IPokeArbitrator
    {
        private readonly Dictionary<GameObject, ILicenseToPoke> licenses = new();

        /// <inheritdoc />
        public void Dispose() => ClearLicenses();

        /// <inheritdoc />
        public ILicenseToPoke GetLicense(GameObject subject)
        {
            if (TryGetLicense(subject, out ILicenseToPoke license) == false) throw new InvalidOperationException("License is not available.");

            return license;
        }

        /// <inheritdoc />
        public bool IsLicenceAvailable(GameObject subject) => licenses.ContainsKey(subject) == false;

        /// <inheritdoc />
        public bool TryGetLicense(GameObject subject, out ILicenseToPoke license)
        {
            if (licenses.ContainsKey(subject))
            {
                license = default;

                return false;
            }

            license = new License(this, subject);

            licenses.Add(subject, license);

            return true;
        }

        private void ClearLicenses()
        {
            foreach (ILicenseToPoke license in licenses.Values.ToArray())
            {
                license.Dispose();
            }

            licenses.Clear();
        }

        private void RemoveLicense(License license) => licenses.Remove(license.Subject);

        public sealed class License : ILicenseToPoke
        {
            private readonly PokeArbitrator arbitrator;

            public GameObject Subject { get; private set; }

            internal License(PokeArbitrator arbitrator, GameObject subject)
            {
                this.arbitrator = arbitrator;

                this.Subject = subject;
            }

            public void Dispose()
            {
                if (Disposed) return;

                Disposed = true;

                arbitrator.RemoveLicense(this);

                Subject = null;
            }

            public bool Disposed { get; private set; }
        }
    }
}