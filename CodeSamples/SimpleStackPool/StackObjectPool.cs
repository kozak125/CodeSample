using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidwalker
{
    public class StackObjectPool<T> : IDisposable where T : UnityEngine.Object
    {
        private readonly Stack<T> objectStack;
        private Func<T> createFunc;
        private Action<T> destroyAction;
        private Action<T> getAction;
        private Action<T> releaseAction;
        private bool isDisposed;

        public StackObjectPool(Func<T> onCreateFunc, Action<T> onDestroyAction, int defaultAmount, Action<T> onGetAction = null, Action<T> onReleaseAction = null)
        {
            objectStack = new Stack<T>(defaultAmount);
            createFunc = onCreateFunc;
            destroyAction = onDestroyAction;
            getAction = onGetAction;
            releaseAction = onReleaseAction;
        }

        public T Get()
        {
            T objectToGet;

            if (objectStack.Count > 0)
            {
                objectToGet = objectStack.Pop();
            }
            else
            {
                objectToGet = CreateObject();
            }

            getAction?.Invoke(objectToGet);

            return objectToGet;
        }

        public void Release(T objectToStack)
        {
            objectStack.Push(objectToStack);
            releaseAction?.Invoke(objectToStack);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator ReleaseAfterSeconds(T objectToStack, float seconds)
        {
            yield return new WaitForSeconds(seconds);

            Release(objectToStack);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                while (objectStack.Count > 0)
                {
                    DestroyObject(objectStack.Pop());
                }

                objectStack.Clear();

                createFunc = null;
                destroyAction = null;
                getAction = null;
                releaseAction = null;
            }

            isDisposed = true;
        }

        private T CreateObject()
        {
            return createFunc.Invoke();
        }

        private void DestroyObject(T t)
        {
            if (t == null)
            {
                return;
            }

            destroyAction?.Invoke(t);
        }
    }
}
