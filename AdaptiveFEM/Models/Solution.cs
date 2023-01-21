using System;
using System.Collections.Generic;

namespace AdaptiveFEM.Models
{
    public class Solution
    {
        private List<MeshLineComponent> _uniformMesh;

        public List<MeshLineComponent> MeshLines { get; private set; }

        const uint uniformMeshSamples = 100;

        private readonly Design _design;

        private Mesh _mesh;

        public Solution(Design design)
        {
            _uniformMesh = new List<MeshLineComponent>();
            MeshLines = new List<MeshLineComponent>();

            _design = design;
            _mesh = new Mesh(_design.Model);

            //
            _design.DesignReset += OnDesignReset;
        }

        public void GenerateMesh()
        {
            // First of all, uniform mesh should be generated.
            GenerateUniformMesh();

            // Warning: this line is written to test uniform mesh
            MeshLines = _uniformMesh;
        }

        private void GenerateUniformMesh()
        {
            ResetMesh();

            if (_design.Model.Domain == null)
                throw new MissingMemberException(nameof(Model.Domain));

            for (int i = 0; i < uniformMeshSamples; i++)
            {
                _uniformMesh.Add(
                    new MeshLineComponent(_mesh
                    .UniformMeshLines(_design
                    .Model.Domain.Geometry.Clone(), uniformMeshSamples)[i]));
            }

            for (int i = 0; i < _design.Model.Regions.Count; i++)
                for (int j = 0; j < uniformMeshSamples; j++)
                    _uniformMesh.Add(
                        new MeshLineComponent(_mesh
                        .UniformMeshLines(_design
                        .Model.Regions[i].Geometry.Clone(), uniformMeshSamples)[j]));
        }

        private void OnDesignReset(object? sender, System.EventArgs e)
        {
            ResetMesh();
        }

        private void ResetMesh()
        {
            MeshLines = new List<MeshLineComponent>();
        }
    }
}
