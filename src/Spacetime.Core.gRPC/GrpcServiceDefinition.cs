﻿namespace Spacetime.Core.gRPC
{
    public class GrpcServiceDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GrpcMethodDefinition> Methods { get; set; } = new ();
    }
}