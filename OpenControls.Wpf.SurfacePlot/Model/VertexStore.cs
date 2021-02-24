using System;

namespace OpenControls.Wpf.SurfacePlot.Model
{
    internal class VertexStore
    {
        public Model.Vertex[] Vertices { get; private set; }
        /*
         * The index of the next item. 
         * Used when filling the array.
         */
        public int NextFreeItemIndex { get; private set; }

        /*
         * The total number of nodes in the grid
         * There are more vertices than nodes
         */
        public int NumberOfNodes { get; private set; }

        private const int constInitialNumberOfVertices = 5000;

        public VertexStore(int numberOfNodes)
        {
            NumberOfNodes = numberOfNodes;
            Vertices = new Model.Vertex[constInitialNumberOfVertices];
            for (int i = 0; i < constInitialNumberOfVertices; ++i)
            {
                Vertices[i] = new Vertex();
            }
            NextFreeItemIndex = 0;
        }

        public void ResetNextFreeItemIndex()
        {
            NextFreeItemIndex = 0;
        }

        public float CurrentZ()
        {
            if ((Vertices == null) || (NextFreeItemIndex >= Vertices.Length))
            {
                return 0;
            }

            return Vertices[NextFreeItemIndex].Z;
        }

        public void SetVertex(int x, int y, float z, OpenTK.Graphics.Color4 colour)
        {
            if (NextFreeItemIndex == Vertices.Length)
            {
                Model.Vertex[] vertices = Vertices;
                Array.Resize(ref vertices, NextFreeItemIndex * 2);
                Vertices = vertices;
                for (int i = NextFreeItemIndex; i < Vertices.Length; ++i)
                {
                    Vertices[i] = new Vertex();
                }
            }
            Vertices[NextFreeItemIndex].X = x;
            Vertices[NextFreeItemIndex].Y = y;
            Vertices[NextFreeItemIndex].Z = z;
            Vertices[NextFreeItemIndex].Colour = colour;
            ++NextFreeItemIndex;
        }
    }
}
