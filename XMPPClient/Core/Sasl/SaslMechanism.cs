using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Sharp.Xmpp.Core.Sasl
{
    /// <summary>
    /// The abstract base class from which all classes implementing a Sasl
    /// authentication mechanism must derive.
    /// </summary>
    internal abstract class SaslMechanism
    {
        /// <summary>
        /// IANA name of the authentication mechanism.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// True if the authentication exchange between client and server
        /// has been completed.
        /// </summary>
        public abstract bool IsCompleted
        {
            get;
        }

        /// <summary>
        /// True if the mechanism requires initiation by the client.
        /// </summary>
        public abstract bool HasInitial
        {
            get;
        }

        /// <summary>
        /// A map of mechanism-specific properties which are needed by the
        /// authentication mechanism to compute it's challenge-responses.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get;
            private set;
        }

        /// <summary>
        /// Computes the client response to a challenge sent by the server.
        /// </summary>
        /// <param name="challenge"></param>
        /// <returns>The client response to the specified challenge.</returns>
        protected abstract byte[] ComputeResponse(byte[] challenge);

        /// <summary>
        /// </summary>
        internal SaslMechanism()
        {
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Retrieves the base64-encoded client response for the specified
        /// base64-encoded challenge sent by the server.
        /// </summary>
        /// <param name="challenge">A base64-encoded string representing a challenge
        /// sent by the server.</param>
        /// <returns>A base64-encoded string representing the client response to the
        /// server challenge.</returns>
        /// <remarks>The IMAP, POP3 and SMTP authentication commands expect challenges
        /// and responses to be base64-encoded. This method automatically decodes the
        /// server challenge before passing it to the Sasl implementation and
        /// encodes the client response to a base64-string before returning it to the
        /// caller.</remarks>
        /// <exception cref="SaslException">The client response could not be retrieved.
        /// Refer to the inner exception for error details.</exception>
        public string GetResponse(string challenge)
        {
            try
            {
                byte[] data = String.IsNullOrEmpty(challenge) ? new byte[0] :
                    Convert.FromBase64String(challenge);
                byte[] response = ComputeResponse(data);
                return Convert.ToBase64String(response);
            }
            catch (Exception e)
            {
                throw new SaslException("The challenge-response could not be " +
                    "retrieved.", e);
            }
        }

        /// <summary>
        /// Retrieves the client response for the specified server challenge.
        /// </summary>
        /// <param name="challenge">A byte array containing the challenge sent by
        /// the server.</param>
        /// <returns>An array of bytes representing the client response to the
        /// server challenge.</returns>
        public byte[] GetResponse(byte[] challenge)
        {
            return ComputeResponse(challenge);
        }

        /// <summary>
        /// Compute the PBKDF2 according to the dkLen, password, salt, iterationCount & hmac function.
        /// 
        /// For detail, please refer to URL below.
        /// https://en.wikipedia.org/wiki/PBKDF2
        /// </summary>
        /// <param name="dklen">
        /// The desired bit-length of the derived key in PBKDF2
        /// 
        /// When hmac is HMACSHA1, it should set to 20.
        /// When hmac is HMACSHA256, it should set to 32.
        /// When hmac is HMACSHA512, it should set to 64.
        /// </param>
        /// <param name="password">
        /// The master password from which a derived key is generated
        /// </param>
        /// <param name="salt">
        /// A sequence of bits, known as a cryptographic salt
        /// </param>
        /// <param name="iterationCount">
        /// The number of iterations desired
        /// </param>
        /// <param name="hmac">
        /// The pseudorandom function used in PBKDF2.
        /// It can be one of the following values: HMACSHA1, HMACSHA256, HMACSHA512
        /// </param>
        /// <returns>
        /// The full key derivation
        /// </returns>
        public byte[] PBKDF2(int dklen, byte[] password, byte[] salt, int iterationCount, HMAC hmac)
        {
            int hashLength = hmac.HashSize / 8;
            if ((hmac.HashSize & 7) != 0)
                hashLength++;
            int keyLength = dklen / hashLength;
            if ((long)dklen > (0xFFFFFFFFL * hashLength) || dklen < 0)
                throw new ArgumentOutOfRangeException("dklen");
            if (dklen % hashLength != 0)
                keyLength++;
            byte[] extendedkey = new byte[salt.Length + 4];
            Buffer.BlockCopy(salt, 0, extendedkey, 0, salt.Length);
            using (var ms = new System.IO.MemoryStream())
            {
                for (int i = 0; i < keyLength; i++)
                {
                    extendedkey[salt.Length] = (byte)(((i + 1) >> 24) & 0xFF);
                    extendedkey[salt.Length + 1] = (byte)(((i + 1) >> 16) & 0xFF);
                    extendedkey[salt.Length + 2] = (byte)(((i + 1) >> 8) & 0xFF);
                    extendedkey[salt.Length + 3] = (byte)(((i + 1)) & 0xFF);
                    byte[] u = hmac.ComputeHash(extendedkey);
                    Array.Clear(extendedkey, salt.Length, 4);
                    byte[] f = u;
                    for (int j = 1; j < iterationCount; j++)
                    {
                        u = hmac.ComputeHash(u);
                        for (int k = 0; k < f.Length; k++)
                        {
                            f[k] ^= u[k];
                        }
                    }
                    ms.Write(f, 0, f.Length);
                    Array.Clear(u, 0, u.Length);
                    Array.Clear(f, 0, f.Length);
                }
                byte[] dk = new byte[dklen];
                ms.Position = 0;
                ms.Read(dk, 0, dklen);
                ms.Position = 0;
                for (long i = 0; i < ms.Length; i++)
                {
                    ms.WriteByte(0);
                }
                Array.Clear(extendedkey, 0, extendedkey.Length);
                return dk;
            }
        }
    }
}