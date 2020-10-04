using System;
// based on Boris Brock singleton baseclass, see link: https://www.codeproject.com/Articles/572263/A-Reusable-Base-Class-for-the-Singleton-Pattern-in


public abstract class SingletonBase<T> where T : class
{
    // constructor is private, so need to use lambda expression to construct an instance
    private static readonly Lazy<T> _instance = new Lazy<T>(() => CreateInstanceOfT());

    //gets instance of singleton
    public static T Instance { get { return _instance.Value; } }

    // creates an instance of T via reflection since T's constuctor is expected to be private
    private static T CreateInstanceOfT()
    {
        return Activator.CreateInstance(typeof(T), true) as T;
    }
}
