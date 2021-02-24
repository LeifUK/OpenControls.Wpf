namespace OpenControls.Wpf.SurfacePlot.Model
{
    class Vertex
    {
        // Store each coordinate as an int even though the actual data is an int16 so we don't get an overflow when performing arithmetic on the data
        public float X;
        public float Y;
        public float Z;

        public OpenTK.Graphics.Color4 Colour { get; set; }

        public Vertex()
        {
        }
    }
}
