//using System;
//using Xunit;

//namespace SoftCube.Log.Formats.UnitTests
//{
//    public class BackupFilePathFormatTests
//    {
//        public class Constructor
//        {
//            [Fact]
//            public void null_ArgumentNullExceptionを投げる()
//            {
//                var ex = Record.Exception(() => new BackupFileFormat(null));

//                Assert.IsType<ArgumentNullException>(ex);
//            }

//            [Fact]
//            public void DateTimeを指定しない_許容する()
//            {
//                var ex = Record.Exception(() => new BackupFileFormat(@"{Index:\.000}.log"));

//                Assert.Null(ex);
//            }

//            [Fact]
//            public void Indexを指定しない_ArgumentExceptionを投げる()
//            {
//                var ex = Record.Exception(() => new BackupFileFormat(@"{DateTime:yyyy-MM-dd}.log"));

//                Assert.IsType<ArgumentException>(ex);
//            }
//        }

//        public class Convert
//        {
//            [Fact]
//            public void Index_無視する()
//            {
//                var format = new BackupFileFormat(@"{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1));

//                Assert.Equal("2020-01-01.log", actual);
//            }

//            [Fact]
//            public void Index_無視しない()
//            {
//                var format = new BackupFileFormat(@"{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal("2020-01-01.007.log", actual);
//            }

//            #region 特殊フォルダ

//            [Fact]
//            public void ApplicationData_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{ApplicationData}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void CommonApplicationData_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{CommonApplicationData}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void CommonDesktopDirectory_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{CommonDesktopDirectory}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void CommonDocuments_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{CommonDocuments}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void Desktop_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{Desktop}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void DesktopDirectory_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{DesktopDirectory}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void LocalApplicationData_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{LocalApplicationData}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void MyDocuments_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{MyDocuments}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void Personal_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{Personal}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}/2020-01-01.007.log", actual);
//            }

//            [Fact]
//            public void UserProfile_正しく変換する()
//            {
//                var format = new BackupFileFormat(@"{UserProfile}/{DateTime:yyyy-MM-dd}{Index:\.000}.log");

//                var actual = format.Convert("", new DateTime(2020, 1, 1), 7);

//                Assert.Equal($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/2020-01-01.007.log", actual);
//            }

//            #endregion
//        }
//    }
//}
