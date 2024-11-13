using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
	public class GohException : ApplicationException {
		public GohException() { }
		public GohException(string message) : base(message) { }
		public GohException(string message, Exception inner) : base(message, inner) { }
	}
}
