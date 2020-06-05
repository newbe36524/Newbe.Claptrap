// ReSharper disable MemberCanBePrivate.Global

namespace Newbe.Claptrap.Extensions
{
    public static class ClaptrapDesignExtensions
    {
        public static bool ContainsMaster(
            this IClaptrapDesign design)
        {
            return design.ClaptrapMasterDesign != null;
        }

        public static bool IsMinion(
            this IClaptrapDesign design)
        {
            return ContainsMaster(design);
        }

        public static string MasterOfSelfTypeCode(this IClaptrapDesign design)
        {
            return design.ContainsMaster() ? design.ClaptrapMasterDesign.ClaptrapTypeCode : design.ClaptrapTypeCode;
        }
    }
}