using InClassApp.Helpers;
using InClassApp.Helpers.Interfaces;
using Domain.Models.Entities;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Tests
{
    public class AttendanceCodeManagerTests
    {
        private IAttendanceCodeManager _attendanceCodeManager;

        [SetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(GetAppSettings())
                .Build();

            _attendanceCodeManager = Substitute.For<AttendanceCodeManager>(configuration);
        }

        [Test(Description = "[CreateAttendanceCode] => Should return attendance code")]
        public void CreateAttendanceCode_ShouldReturnCode()
        {
            //Arrange
            int codeLength = 6;

            //Act
            var code = _attendanceCodeManager.CreateAttendanceCode();

            //Assert
            Assert.IsNotNull(code);
            Assert.AreEqual(codeLength, code.Length);
        }

        [Test(Description = "[EncryptMeeting] => Encrypts given code and fills into meeting")]
        public void EncryptMeeting_ShouldEncryptCodeAndFillIntoMeeting()
        {
            //Arrange
            var meeting = new Meeting
            {
                Id = 1,
                GroupId = 1,
                MeetingStartDate = DateTime.Now.AddDays(-10),
                MeetingEndDate = DateTime.Now.AddDays(10)
            };
            var code = "489215";

            //Act
            _attendanceCodeManager.EncryptMeeting(meeting, code);

            //Assert
            Assert.IsNotNull(meeting.LastlyGeneratedCodeIV);
            Assert.IsNotNull(meeting.LastlyGeneratedCheckCode);
        }

        [Test(Description = "[GetDecryptedCode] => Decrypts code and returns original value")]
        public void GetDecryptedCode_ShouldReturnOriginalCode()
        {
            //Arrange
            var originalCode = "968532";
            var encryptedCode = "g4IQAR9iN7DQRymS3Z/VWw==";
            var IV = "bK2hmrJZumFnslafPcmTdg==";

            //Act
            var code = _attendanceCodeManager.GetDecryptedCode(encryptedCode, IV);

            //Assert
            Assert.AreEqual(originalCode, code);
        }

        private static Dictionary<string, string> GetAppSettings()
        {
            return new Dictionary<string, string>
            {
                {"AttendanceCodeSecrets:CodeGenerationSecret", "KBXWI43UMF34JBJAOBZGCY3Z" },
                {"AttendanceCodeSecrets:CodeEncryptionKey", "1e24ff554dc78aad47c7a5b644770921" }
            };
        }
    }
}