using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;

namespace AcadLib.Jigs
{
    public class BlockInsertJig : EntityJig
    {
        private Point3d mCenterPt, mActualPoint;

        public BlockInsertJig([NotNull] BlockReference br)
          : base(br)
        {
            mCenterPt = br.Position;
        }

        public Entity GetEntity()
        {
            return Entity;
        }

        protected override SamplerStatus Sampler([NotNull] JigPrompts prompts)
        {
            var jigOpts =
              new JigPromptPointOptions
              {
                  UserInputControls =
              (UserInputControls.Accept3dCoordinates
              | UserInputControls.NoZeroResponseAccepted
              | UserInputControls.NoNegativeResponseAccepted),

                  Message =
              "\nУкажите точку вставки: "
              };

            var dres =
           prompts.AcquirePoint(jigOpts);

            if (mActualPoint == dres.Value)
            {
                return SamplerStatus.NoChange;
            }
            else
            {
                mActualPoint = dres.Value;
            }
            return SamplerStatus.OK;
        }

        protected override bool Update()
        {
            mCenterPt = mActualPoint;
            try
            {
                ((BlockReference)Entity).Position = mCenterPt;
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
    }
}