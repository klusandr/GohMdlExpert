using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class GohOutputModProvider {
        private GohOutputMod? _mod;

        public GohOutputMod Mod { get => _mod ?? throw GohResourceSaveException.OutputModIsNotSet(); set => _mod = value; }

        public GohOutputModProvider() { }
    }
}
