using System;
using System.Windows;

namespace _3dViewWPF {
    public class MouseMoveArgs : EventArgs {
        public Vector Vector { get; private set; }
        public MouseMoveArgs(Vector vector) {
            Vector = vector;
        }
    }

    public delegate void MouseDownMoveHandler(object sender, MouseMoveArgs e);
}