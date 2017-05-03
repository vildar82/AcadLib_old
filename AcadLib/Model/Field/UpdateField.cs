using System;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Field
{
    /// <summary>
    /// PInvoke acdbEvaluateFields - обновление полей - с 2013 - 2017
    /// </summary>
    public static class UpdateField
    {
        #region PInvoke acdbEvaluateFields
        // Acad::ErrorStatus acdbEvaluateFields (
        //   const AcDbObjectId& objId, 
        //   int nContext, 
        //   const ACHAR* pszPropName = NULL, 
        //   AcDbDatabase* pDb        = NULL, 
        //   AcFd::EvalFields nEvalFlag = AcFd::kEvalRecursive,
        //   int* pNumFound           = NULL,
        //   int* pNumEvaluated       = NULL
        // );
        // AutoCAD 2013 и 2014 x64
        [DllImport("acdb19.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true,
          EntryPoint = "?acdbEvaluateFields@@YA?AW4ErrorStatus@Acad@@AEBVAcDbObjectId@@HPEB_WPEAVAcDbDatabase@@W4EvalFields@AcFd@@PEAH4@Z")]
        private static extern Int32 acdbEvaluateFields19x64(ref ObjectId id, Int32 context, IntPtr pszPropName, IntPtr db, Int32 eval, IntPtr i1, IntPtr i2);
        // AutoCAD 2013 и 2014 x86
        [DllImport("acdb19.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true,
        EntryPoint = "?acdbEvaluateFields@@YA?AW4ErrorStatus@Acad@@ABVAcDbObjectId@@HPB_WPAVAcDbDatabase@@W4EvalFields@AcFd@@PAH4@Z")]
        private static extern Int32 acdbEvaluateFields19x32(ref ObjectId id, Int32 context, IntPtr pszPropName, IntPtr db, Int32 eval, IntPtr i1, IntPtr i2);
        // AutoCAD 2015 и 2016 x64
        [DllImport("acdb20.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true,
          EntryPoint = "?acdbEvaluateFields@@YA?AW4ErrorStatus@Acad@@AEBVAcDbObjectId@@HPEB_WPEAVAcDbDatabase@@W4EvalFields@AcFd@@PEAH4@Z")]
        private static extern Int32 acdbEvaluateFields20x64(ref ObjectId id, Int32 context, IntPtr pszPropName, IntPtr db, Int32 eval, IntPtr i1, IntPtr i2);
        // AutoCAD 2015 и 2016 x86
        [DllImport("acdb20.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true,
        EntryPoint = "?acdbEvaluateFields@@YA?AW4ErrorStatus@Acad@@ABVAcDbObjectId@@HPB_WPAVAcDbDatabase@@W4EvalFields@AcFd@@PAH4@Z")]
        private static extern Int32 acdbEvaluateFields20x32(ref ObjectId id, Int32 context, IntPtr pszPropName, IntPtr db, Int32 eval, IntPtr i1, IntPtr i2);
        // AutoCAD 2017 x64
        [DllImport("acdb21.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true,
          EntryPoint = "?acdbEvaluateFields@@YA?AW4ErrorStatus@Acad@@AEBVAcDbObjectId@@HPEB_WPEAVAcDbDatabase@@W4EvalFields@AcFd@@PEAH4@Z")]
        private static extern Int32 acdbEvaluateFields21x64(ref ObjectId id, Int32 context, IntPtr pszPropName, IntPtr db, Int32 eval, IntPtr i1, IntPtr i2);
        // AutoCAD 2017x86
        [DllImport("acdb21.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true,
        EntryPoint = "?acdbEvaluateFields@@YA?AW4ErrorStatus@Acad@@ABVAcDbObjectId@@HPB_WPAVAcDbDatabase@@W4EvalFields@AcFd@@PAH4@Z")]
        private static extern Int32 acdbEvaluateFields21x32(ref ObjectId id, Int32 context, IntPtr pszPropName, IntPtr db, Int32 eval, IntPtr i1, IntPtr i2);
        #endregion
        private static Int32 acdbEvaluateFields(ref ObjectId id, Int32 context)
        {
            switch (Autodesk.AutoCAD.ApplicationServices.Core.Application.Version.Major)
            {
                case 21:
                    if (IntPtr.Size == 8)
                        return acdbEvaluateFields21x64(ref id, 16, IntPtr.Zero, IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero);
                    else
                        return acdbEvaluateFields21x32(ref id, 16, IntPtr.Zero, IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero);
                case 20:
                    if (IntPtr.Size == 8)
                        return acdbEvaluateFields20x64(ref id, 16, IntPtr.Zero, IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero);
                    else
                        return acdbEvaluateFields20x32(ref id, 16, IntPtr.Zero, IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero);
                case 19:
                    if (IntPtr.Size == 8)
                        return acdbEvaluateFields19x64(ref id, 16, IntPtr.Zero, IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero);
                    else
                        return acdbEvaluateFields19x32(ref id, 16, IntPtr.Zero, IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero);
                default:
                    break;
            }
            return 0;
        }

        /// <summary>
        /// Обновление полей в объекте.
        /// Можно передать id блока - все поля в блоке обновятся
        /// </summary>
        /// <param name="id"></param>
        public static int Update(ObjectId id)
        {            
            return acdbEvaluateFields(ref id, 16);
        }
       
        public static void UpdateInSelected ()
        {
            // Обновление полей в блоке
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var sel = ed.Select("Выбер объектов для обновления полей: ");
            foreach (var item in sel)
            {
                var id = item;
                acdbEvaluateFields(ref id, 16);
            }
        }
    }
}
