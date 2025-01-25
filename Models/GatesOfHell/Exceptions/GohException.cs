namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohException : ApplicationException {
        public GohException(string? message = null, Exception? inner = null) : base(message ?? "Gate of hell exception", inner) { }
    }
}
