#define ARCH_SINGLETON_AUTOINIT

#pragma warning disable S2696 //static method warning
#pragma warning disable S2743 //static field in generic class warning

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Backend
{
    //Enforces the rule that T MUST be a child class of AbstractSingleton<T> (recursive) - this allows the class to return a child type (how cool is that???)
    public abstract class AbstractSingleton<T> : MonoBehaviour where T : AbstractSingleton<T> 
    {
        //this is to make the singleton thread-safe
        private static readonly object accessLock = new object();

        private static T instance = default(T);

        /// <summary>
        /// This method must be called in either Start() or Awake() to correctly set up the singleton behaviour
        /// </summary>
#if ARCH_SINGLETON_AUTOINIT
        private
#else
        protected
#endif
            void MakeSingleton()
        {
            DontDestroyOnLoad(gameObject);

            if (instance == null)
            {
                instance = (T)this;
                SingletonAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// This method must be called in either Start() or Awake() to correctly set up the singleton behaviour
        /// </summary>
        /// <param name="Init">An action to execute if the singleton must be (re)initialized - this would contain Start() function kind of stuff.</param>
#if ARCH_SINGLETON_AUTOINIT
        private
#else
        protected
#endif
        void MakeSingleton(Action Init)
        {
            DontDestroyOnLoad(gameObject);

            if (instance == null)
            {
                instance = (T)this;

                SingletonAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Thread-safe access to the singleton instance
        /// </summary>
        public static T Instance
        {
            private set { instance = value; }
            get
            {
                //this is to make the singleton thread-safe
                lock (accessLock)
                {
                    return instance;
                }
            }
        }

#if ARCH_SINGLETON_AUTOINIT
        private void Awake()
        {
            MakeSingleton();
            
        }

        /// <summary>
        /// Treat this exactly like the original Unity Awake() function
        /// </summary>
        protected abstract void SingletonAwake();
#endif
    }
}