namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public abstract class GohException : ApplicationException {
        protected abstract byte ExceptionTypeCode { get; }
        protected byte ExceptionCode { set => HResult = HResult | value; }

        public GohException(string? message = null, Exception? inner = null) : base(message ?? "Gate of hell exception", inner) {
            HResult = (1 << 31) | (ExceptionTypeCode << 8);
        }
    }
}
