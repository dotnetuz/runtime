// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Internal.Cryptography;
using System.ComponentModel;

namespace System.Security.Cryptography
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    // SHA256Managed has a copy of the same implementation as SHA256
    public sealed class SHA256Managed : SHA256
    {
        private readonly HashProvider _hashProvider;

        public SHA256Managed()
        {
            _hashProvider = HashProviderDispenser.CreateHashProvider(HashAlgorithmNames.SHA256);
            HashSizeValue = _hashProvider.HashSizeInBytes * 8;
        }

        protected sealed override void HashCore(byte[] array, int ibStart, int cbSize) =>
            _hashProvider.AppendHashData(array, ibStart, cbSize);

        protected sealed override void HashCore(ReadOnlySpan<byte> source) =>
            _hashProvider.AppendHashData(source);

        protected sealed override byte[] HashFinal() =>
            _hashProvider.FinalizeHashAndReset();

        protected sealed override bool TryHashFinal(Span<byte> destination, out int bytesWritten) =>
            _hashProvider.TryFinalizeHashAndReset(destination, out bytesWritten);

        public sealed override void Initialize()
        {
            // Nothing to do here. We expect HashAlgorithm to invoke HashFinal() and Initialize() as a pair. This reflects the
            // reality that our native crypto providers (e.g. CNG) expose hash finalization and object reinitialization as an atomic operation.
        }

        protected sealed override void Dispose(bool disposing)
        {
            _hashProvider.Dispose(disposing);
            base.Dispose(disposing);
        }
    }
}
