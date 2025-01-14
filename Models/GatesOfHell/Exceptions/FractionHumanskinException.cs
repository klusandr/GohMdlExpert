using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class FractionHumanskinException : GohResourcesException {
        private static readonly string s_message = "Fraction {0}humanskin load error.{1}";

        public GohFactionHumanskinResource? HumanskinResource { get; }

        public FractionHumanskinException(string? message = null, GohFactionHumanskinResource? humanskinResource = null, Exception? inner = null) : base(GetFullMessage(message, humanskinResource), inner) {
            HumanskinResource = humanskinResource;
        }

        private static string GetFullMessage(string? message, GohFactionHumanskinResource? humanskinResource) {
            return string.Format(s_message,
                humanskinResource != null ? $"\"{humanskinResource.Name}\" " : string.Empty,
                message ?? string.Empty
            );
        }
    }
}
