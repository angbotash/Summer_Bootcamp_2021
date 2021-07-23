using SummerBootCampTask.CoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Services
{
    public interface IEncryptionService
    {
        void Encrypt(Message message, int key);

        Message Decrypt(Message message, int key);
    }
}
