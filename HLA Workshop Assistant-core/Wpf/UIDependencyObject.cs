using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace HLA_Workshop_Assistant.Wpf
{
    /// <summary>
    /// This is a DependencyObject, except that all properties using GetValue and SetValue will always Get and Set on the UI Thread.
    /// </summary>
    public class UIDependencyObject : DependencyObject
    {
        /// <summary>
        /// Allows for the creation of a UIDependencyObject on the UI thread, regardless of what thread is actually running.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">None of the parameters can be null for this to work.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(params object[] parameters)
        {

            T retVal = default;
            List<ConstructorInfo> candidates = new List<ConstructorInfo>();

            foreach (var constructor in typeof(T).GetConstructors())
            {
                var parms = constructor.GetParameters();
                if (parms.Length == parameters.Length)
                {
                    bool isValid = true;

                    for (int i = 0; i < parms.Length; i++)
                    {
                        Type passParmType = parameters[i].GetType();
                        Type constructorParmType = parms[i].ParameterType;
                        if (passParmType == constructorParmType || passParmType.IsSubclassOf(constructorParmType))
                        {

                        }
                        else
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        candidates.Add(constructor);
                    }
                }
            }
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException("A constructor that matched the passed parameters was not found.");
            }
            else
            {
                ConstructorInfo constructorInfo = null;

                if (candidates.Count == 1)
                {
                    constructorInfo = candidates[0];
                }
                else
                {
                    foreach (var candidate in candidates)
                    {
                        bool isTrueMatch = true;
                        var parms = candidate.GetParameters();

                        for (int i = 0; i < parms.Length; i++)
                        {
                            Type passParmType = parameters[i].GetType();
                            Type constructorParmType = parms[i].ParameterType;
                            isTrueMatch = passParmType == constructorParmType;
                            if (!isTrueMatch)
                            {
                                break;
                            }
                        }
                        if (isTrueMatch)
                        {
                            constructorInfo = candidate;
                            break;
                        }
                    }

                    if (constructorInfo == null)
                    {
                        throw new InvalidOperationException("Too many possible constructors.  Use CreateInstance<T>(ConstructorInfo constructorInfo, params object[] parameters)");
                    }
                }

                retVal = CreateInstance<T>(constructorInfo, parameters);
            }
            return retVal;
        }
        /// <summary>
        /// Allows for the creation of a UIDependencyObject on the UI thread, regardless of what thread is actually running.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorSignature"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(ConstructorInfo constructorSignature, params object[] parameters)
        {

            if (IsUIThread)
            {
                T retVal = default;

                retVal = (T)constructorSignature.Invoke(parameters);

                return retVal;
            }
            else
            {
                return (T)Invoke(new Func<T>(() => { return CreateInstance<T>(constructorSignature, parameters); }));
            }
        }


        //
        // Summary:
        //     Initializes a new instance of the System.Windows.DependencyObject class.
        public UIDependencyObject()
        {
            if (!IsUIThread)
            {
                throw new NotUIThreadException();
            }
        }


        /// <summary>
        /// Synchronously Gets the property in the UI thread.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public new object GetValue(DependencyProperty property)
        {
            try
            {
                if (IsUIThread)
                {
                    return base.GetValue(property);

                }
                else
                {
                    return Invoke(new Func<DependencyProperty, object>(this.GetValue), property);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Raised whenever a property value changes.  Always runs on the UI thread.
        /// </summary>
        public event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;
        /// <summary>
        /// Synchronously sets the property in the UI thread.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public new void SetValue(DependencyProperty property, object value)
        {

            try
            {
                if (IsUIThread)
                {
                    object oldValue = base.GetValue(property);
                    base.SetValue(property, value);
                    if (oldValue != value)
                    {

                        PropertyChanged?.BeginInvoke(this, new DependencyPropertyChangedEventArgs(property, oldValue, value), null, null);
                    }

                }
                else
                {
                    Invoke(new Action<DependencyProperty, object>(this.SetValue), property, value);
                }
            }
            catch (Exception ex)
            {

            }

        }


        /// <summary>
        /// Asynchronously sets the property value in the UI thread.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void BeginSetValue(DependencyProperty property, object value)
        {

            if (IsUIThread)
            {
                base.SetValue(property, value);
            }
            else
            {
                BeginInvoke(new Action<DependencyProperty, object>(this.SetValue), property, value);
            }

        }
        /// <summary>
        /// Runs the Method on the background ThreadPool.
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="state"></param>
        public static void BeginPoolInvoke(System.Threading.WaitCallback Method, object state)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(Method, state);
        }
        public static void BeginPoolInvoke(System.Threading.WaitCallback Method)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(Method);
        }

        /// <summary>
        /// Runs the Method on the UI thread.
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object Invoke(Delegate Method, params object[] args)
        {
            return Application.Current.Dispatcher.Invoke(Method, args);
        }
        /// <summary>
        /// Begins invocation of the method on the UI dispatcher thread.
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="args"></param>
        public static void BeginInvoke(Delegate Method, params object[] args)
        {
            Application.Current.Dispatcher.BeginInvoke(Method, args);
        }
        /// <summary>
        /// Begins invocation of the method on the UI dispatcher thread.
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="priority"></param>
        /// <param name="args"></param>
        public static void BeginInvoke(Delegate Method, DispatcherPriority priority, params object[] args)
        {
            Application.Current.Dispatcher.BeginInvoke(Method, priority, args);
        }


        /// <summary>
        /// Checks the thread being run on.
        /// </summary>
        /// <returns>true if the current thread is the UI thread.</returns>
        public static bool IsUIThread
        {
            get
            {
                try
                {
                    return System.Windows.Threading.Dispatcher.CurrentDispatcher == Application.Current.Dispatcher;
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
        }
    }
}
