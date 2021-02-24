namespace OpenControls.Wpf.SurfacePlot.Model
{
    public enum ViewProjection
    {
        ThreeDimensional,
        Orthographic_Front,
        Orthographic_Side,
        BirdsEye_0,
        BirdsEye_90,
        BirdsEye_180,
        BirdsEye_270,
    }

    public static class ViewProjectionExtensionMethods
    {
        public static bool IsBirdsEye(this ViewProjection viewProjection)
        {
            switch (viewProjection)
            {
                case ViewProjection.BirdsEye_0:
                case ViewProjection.BirdsEye_90:
                case ViewProjection.BirdsEye_180:
                case ViewProjection.BirdsEye_270:
                    return true;
            }
            return false;
        }

        public static bool IsOrthographic(this ViewProjection viewProjection)
        {
            return (viewProjection == ViewProjection.Orthographic_Front) || (viewProjection == ViewProjection.Orthographic_Side);
        }
    }
}
