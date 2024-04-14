using Domain.Models.Entities;

namespace Application.Helpers.Interfaces
{
    public interface IAttendanceCodeManager
    {
        /// <summary>
        /// Creates and gets the attendance code
        /// </summary>
        /// <returns>Created attendance code</returns>
        string CreateAttendanceCode();

        /// <summary>
        /// Encrypts the code and save encryption data to meeting
        /// </summary>
        /// <param name="meetingToEncrypt">Meeting to save data in</param>
        /// <param name="generatedCode">Generated code</param>
        void EncryptMeeting(Meeting meetingToEncrypt, string generatedCode);

        /// <summary>
        /// Decrypts code and gets decrypted value
        /// </summary>
        /// <param name="codeEncrypted">Code to decrypt</param>
        /// <param name="IV">Initialization vector</param>
        /// <returns>Decrypted code</returns>
        string GetDecryptedCode(string codeEncrypted, string IV);
    }
}
