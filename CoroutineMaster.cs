using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Coroutines
{
    public class CoroutineMaster : PlayerSystem
    {
        private static CoroutineMaster master;

        private void OnEnable()
        {
            master = this;
        }

        public static void WaitStatic(ICoroutineable coroutineable, float time)
        {
            master.StartCoroutine(master.wait(coroutineable, time));
        }

        public void Wait(ICoroutineable coroutineable, float time)
        {
            StartCoroutine(wait(coroutineable, time));
        }

        private IEnumerator wait(ICoroutineable _coroutineable, float _time)
        {
            if (_time >= 0)
                yield return new WaitForSecondsRealtime(_time);
            else
                throw new System.Exception("[CoroutineMaster] The 'time' parameter is less 0!");
            if (_coroutineable != null)
                _coroutineable.Continue();
        }
    }

    public interface public interface ICoroutineable
    {
        void Continue();
    }

}
