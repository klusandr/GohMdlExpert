using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class OutputModProvider {
        private OutputModResource? _mod;

        public OutputModResource Mod { get => _mod ?? throw GohResourceSaveException.OutputModIsNotSet(); set => _mod = value; }

        public OutputModProvider() { }
    }
}
