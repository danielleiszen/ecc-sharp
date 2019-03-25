using Crypto.Entities;
using Crypto.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Protocols
{
    public abstract class Process
    {
        public CurvePoint Generator { get; private set; }
        public Party Actor { get; private set; }

        public Process(CurvePoint generator, Party actor)
        {
            Generator = generator;
            Actor = actor;
        }
    }
}
