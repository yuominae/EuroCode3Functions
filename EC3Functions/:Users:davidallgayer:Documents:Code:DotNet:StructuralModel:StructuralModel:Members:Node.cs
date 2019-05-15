using System.Collections.Generic;
using System.Numerics;

namespace StructuralModel.Members
{
    public class Node 
        : StructuralMemberBase
    {
        private Support support;

        public Node(int id, float x, float y, float z)
        {
            this.Id = id;

            this.Point = new Vector3(x, y, z);
        }

        public Vector3 Point { get; };

        public bool IsSupport
        {
            get { return this.Support != null; }
        }

        public Support Support
        {
            get { return this.support; }
            set
            {
                this.support = value;
                this.support.Nodes.Add(this);
            }
        }

        public HashSet<Beam> Beams { get; set; } = new HashSet<Beam>();

        public HashSet<NodeDisplacements> Displacements { get; set; } = new HashSet<NodeDisplacements>();
    }
}