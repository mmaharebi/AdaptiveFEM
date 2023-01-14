using System.Collections.Generic;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class Solution
    {
        private List<LineGeometry> _uniformMesh;

        public List<LineGeometry> UniformMesh =>
            new List<LineGeometry>(_uniformMesh);

        const uint uniformMeshSamples = 1000;

        private readonly Design _design;

        private Mesh _mesh;

        public Solution(Design design)
        {
            _uniformMesh = new List<LineGeometry>();
            _design = design;
            _mesh = new Mesh(_design.Model);

            //
            _design.ComponentAdded += OnComponentAdded;
            _design.DesignReset += OnDesignReset;
        }


        public List<LineGeometry> GenerateUniformMesh()
        {
            if (_design.Model.Domain == null)
                throw new System.MissingMemberException(nameof(Model.Domain));

            _uniformMesh.InsertRange(0,
                _mesh.UniformMeshLines(_design.Model.Domain.Geometry,
                uniformMeshSamples));

            _design.Model.Regions.ForEach(r =>
            {
                _uniformMesh.InsertRange(_uniformMesh.Count,
                    _mesh.UniformMeshLines(r.Geometry, uniformMeshSamples));
            });

            return _uniformMesh;
        }

        private void OnComponentAdded(object? sender, Component e)
        {
            _uniformMesh.InsertRange(_uniformMesh.Count,
                _mesh.UniformMeshLines(e.Geometry, uniformMeshSamples));
        }
        private void OnDesignReset(object? sender, System.EventArgs e)
        {
            _uniformMesh.Clear();
        }
    }
}
