using InClassApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Helpers.Interfaces
{
    public interface IAttendanceCodeManager
    {
        string CreateAttendanceCode();
        void EncryptMeeting(Meeting meetingToEncrypt, string generatedCode);
        string GetDecryptedCode(string codeEncrypted, string IV);
    }
}
