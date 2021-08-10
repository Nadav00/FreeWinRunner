using System.Security.Cryptography;
using System.Text;


namespace Keys
{
    public class Keys
    {
        private string internalEntropy = "FreeWinRunnerIntern";

        public Keys()
        {

        }

        public byte[] Store(string filePath, string key)
        {
            byte[] plaintext;
            plaintext = Encoding.UTF8.GetBytes(key);

            byte[] entropy = new byte[20];
            entropy = Encoding.UTF8.GetBytes(internalEntropy);

            return (ProtectedData.Protect(plaintext, entropy,
                DataProtectionScope.LocalMachine));

        }

        public byte[] Retrieve(byte[] ciphertext)
        {
            byte[] entropy = new byte[20];
            entropy = Encoding.UTF8.GetBytes(internalEntropy);

            byte[] plaintext = ProtectedData.Unprotect(ciphertext, entropy,
                                DataProtectionScope.LocalMachine);

            return plaintext;
        }
    }
}
