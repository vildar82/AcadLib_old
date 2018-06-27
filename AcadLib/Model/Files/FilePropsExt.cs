namespace AcadLib.Files
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Internal;
    using Autodesk.AutoCAD.Runtime;

    public static class FilePropsExt
    {
        public static void Method()
        {
            var fullName = @"C:\temp\ГП\Покрытия\Покрытия_Класс16_xdata.dwg";
            var br = new BinaryReader(new FileStream(fullName, FileMode.Open, FileAccess.Read));
            var versionId = new string(br.ReadChars(6));

            // six bytes of 0 (in R14, 5 0’s and the ACADMAINTVER variable) and a byte of 1
            br.ReadBytes(7);

            // Image seeker
            var imageSeeker = br.ReadInt32();

            // Unknown section
            br.ReadBytes(2);

            // DwgCodePage
            br.ReadBytes(2);
            var objectmapSeeker = 0;
            var count = br.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var recnum = br.ReadByte();
                var seeker = br.ReadInt32();
                var size = br.ReadInt32();
                if (recnum == 2)
                {
                    objectmapSeeker = seeker;
                    break;
                }
            }

            br.BaseStream.Seek(imageSeeker, SeekOrigin.Begin);
            var mask = new byte[] { 87, 71, 80, 82, 79, 80, 83, 32, 67, 79, 79, 75, 73, 69 };
            while (br.BaseStream.Position < objectmapSeeker)
            {
                // Search DWGPROPS COOKIE
                var d = br.ReadByte();
                if (d != 68)
                {
                    continue;
                }

                var test = br.ReadBytes(mask.Length);
                var temp = false;
                for (var i = 0; i < mask.Length; i++)
                {
                    if (test[i] != mask[i])
                    {
                        br.BaseStream.Seek(-mask.Length, SeekOrigin.Current);
                        temp = true;
                        break;
                    }
                }

                if (temp)
                {
                    continue;
                }

                //------------------------------------------------------------
                // Predefined properties, in this order:
                // title, subject, author, comments, keywords,  
                // lastSavedBy, revisionNumber
                // DXF codes : 2, 3, 4, 6, 7, 8, 9
                for (var i = 0; i < 7; i++)
                {
                    var code = br.ReadInt16(); // DXF code
                    var len = br.ReadInt16(); // String length
                    br.ReadByte(); // ? 
                    var bytes = br.ReadBytes(len);
                    var value = Encoding.ASCII.GetString(bytes);
                    System.Diagnostics.Debug.Print(value);
                }

                //------------------------------------------------------------
                // 10 custom properties in the format:
                // <name=value> 
                // DXF codes : 300, 301, 302, 303, 304, 305, 306, 307, 308, 309
                for (var i = 0; i < 10; i++)
                {
                    var code = br.ReadInt16(); // DXF code
                    var len = br.ReadInt16(); // String length
                    br.ReadByte(); // ?
                    var bytes = br.ReadBytes(len);
                    var value = Encoding.ASCII.GetString(bytes);

                    // Dim pairs() As String = value.Split(New Char() {"="c})
                    System.Diagnostics.Debug.Print(value);
                }

                // Total editing time, Create date time and Modified
                // date time in the format:
                // <Julian day number>.<Decimal fraction of a day> 
                // DXF codes: 40, 41 ,42
                for (var i = 0; i < 3; i++)
                {
                    var code = br.ReadInt16(); // DXF code
                    var value = br.ReadDouble(); // Value
                    System.Diagnostics.Debug.Print(value.ToString());
                }

                //------------------------------------------------------------
                // hyperlinkBase
                // DXF code: 1
                var c = br.ReadInt16(); // DXF code
                var l = br.ReadInt16(); // String length
                br.ReadByte(); // ? Unknown
                var b = br.ReadBytes(l);
                var v = Encoding.ASCII.GetString(b);
                System.Diagnostics.Debug.Print(v);
            }
        }
    }
}
