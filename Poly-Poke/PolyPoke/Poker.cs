using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace PolyPoke
{
    public interface IPoker
    {
        bool IsPressed { get; }
        Vector3 Position { get; }

        UniTask WaitUntilReleased(CancellationToken cancel);
    }

    public class TouchPoker : IPoker
    {
        private readonly TouchControl control;

        public TouchPoker(TouchControl control) => this.control = control;

        /// <inheritdoc />
        public bool IsPressed => control.press.isPressed;

        /// <inheritdoc />
        public Vector3 Position => control.position.ReadValue();

        /// <inheritdoc />
        public async UniTask WaitUntilReleased(CancellationToken cancel)
            => await UniTask.WaitUntil(() => IsPressed == false, cancellationToken: cancel);
    }

    public class MousePoker : IPoker
    {
        private readonly Mouse mouse;

        public MousePoker(Mouse mouse) => this.mouse = mouse;

        /// <inheritdoc />
        public bool IsPressed => mouse.leftButton.isPressed || mouse.rightButton.isPressed;

        /// <inheritdoc />
        public Vector3 Position => mouse.position.ReadValue();

        /// <inheritdoc />
        public async UniTask WaitUntilReleased(CancellationToken cancel)
            => await UniTask.WaitUntil(() => IsPressed == false, cancellationToken: cancel);
    }

    public class PenPoker : IPoker
    {
        private readonly Pen pen;

        public PenPoker(Pen pen) => this.pen = pen;

        /// <inheritdoc />
        public bool IsPressed => pen.press.isPressed;

        /// <inheritdoc />
        public Vector3 Position => pen.position.ReadValue();

        /// <inheritdoc />
        public async UniTask WaitUntilReleased(CancellationToken cancel)
            => await UniTask.WaitUntil(() => IsPressed == false, cancellationToken: cancel);
    }
}