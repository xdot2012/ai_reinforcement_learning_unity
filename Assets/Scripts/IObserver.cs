using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver {
    void Update(int symbol);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(int symbol);
    void Clear();
}

public class Subject : ISubject {
    public int State { get; set; } = -0;

    private List<IObserver> observers = new List<IObserver>();
    public void Attach(IObserver observer) {
        //Debug.Log("Subject: Attached an observer.");
        this.observers.Add(observer);
    }

    public void Detach(IObserver observer) {
        this.observers.Remove(observer);
        //Debug.Log("Subject: Detached an observer.");
    }
    public void Clear() {
        this.observers = new List<IObserver>();
    }

    public void Notify(int symbol) {
        foreach (var observer in observers) {
            observer.Update(symbol);
        }
    }
}
