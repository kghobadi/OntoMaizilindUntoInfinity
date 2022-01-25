using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class EventMgr : Singleton<EventMgr>
{
    private Dictionary<EventSinks.EventSinkName, UnityEventBase> eventSinks = new Dictionary<EventSinks.EventSinkName, UnityEventBase>();

    #region Create/Get Event Sink methods
    public UnityEvent CreateOrGetEventSink(EventSinks.EventSinkName EventSinkName)
    {
        UnityEventBase ret = null;

        if (!eventSinks.TryGetValue(EventSinkName, out ret))
        {
            ret = new UnityEvent();
            eventSinks.Add(EventSinkName, ret);
        }
        else if (!(ret is UnityEvent))
        {
            Debug.LogError("Event sink \"" + EventSinkName + "\" already created as (" + ret + ")");
        }

        return ret as UnityEvent;
    }

    public UnityEvent<T> CreateOrGetEventSink<T>(EventSinks.EventSinkName<T> EventSinkName)
    {
        UnityEventBase ret = null;

        if (!eventSinks.TryGetValue(EventSinkName, out ret))
        {
            ret = new UnityEvent();
            eventSinks.Add(EventSinkName, ret);
        }
        else if (!(ret is UnityEvent<T>))
        {
            Debug.LogError("Event sink \"" + EventSinkName + "\" already created as (" + ret + ")");
        }

        return ret as UnityEvent<T>;
    }

    public UnityEvent GetEventSink(EventSinks.EventSinkName EventSinkName)
    {
        UnityEventBase ret = null;

        if (!eventSinks.TryGetValue(EventSinkName, out ret))
        {
            Debug.LogError(EventSinkName + " does not exist as an event sink");
        }
        else if (!(ret is UnityEvent))
        {
            Debug.LogError("Event sink \"" + EventSinkName + "\" already created (" + ret + ")");
        }

        return ret as UnityEvent;
    }

    public UnityEvent<T> GetEventSink<T>(EventSinks.EventSinkName<T> EventSinkName)
    {
        UnityEventBase ret = null;

        if (!eventSinks.TryGetValue(EventSinkName, out ret))
        {
            Debug.LogError(EventSinkName + " does not exist as an event sink");
        }
        else if (!(ret is UnityEvent<T>))
        {
            Debug.LogError("Event sink \"" + EventSinkName + "\" already created (" + ret + ")");
        }

        return ret as UnityEvent<T>;
    }
    
    #endregion

    #region Subscription methods
    public UnityEvent Subscribe(EventSinks.EventSinkName EventSinkName, UnityAction ListenerCallback)
    {
        UnityEvent ret = null;

        UnityEventBase baseEvent = null;
        if (!eventSinks.TryGetValue(EventSinkName, out baseEvent))
        {
            // add new event sink
            ret = CreateOrGetEventSink(EventSinkName);
        }
        else if (!(baseEvent is UnityEvent))
        {
            Debug.LogError(EventSinkName + " is the wrong type of event sink (" + baseEvent + ")");
        }
        else
        {
            ret = baseEvent as UnityEvent;
        }

        ret?.AddListener(ListenerCallback);

        return ret;
    }

    public UnityEvent<T> Subscribe<T>(EventSinks.EventSinkName<T> EventSinkName, UnityAction<T> ListenerCallback)
    {
        UnityEvent<T> ret = null;

        UnityEventBase baseEvent = null;
        if (!eventSinks.TryGetValue(EventSinkName, out baseEvent))
        {
            // add new event sink
            ret = CreateOrGetEventSink<T>(EventSinkName);
        }
        else if (!(baseEvent is UnityEvent<T>))
        {
            Debug.LogError(EventSinkName + " is the wrong type of event sink (" + baseEvent + ")");
        }
        else
        {
            ret = baseEvent as UnityEvent<T>;
        }

        ret?.AddListener(ListenerCallback);

        return ret;
    }
    

    public UnityEvent Unsubscribe(EventSinks.EventSinkName EventSinkName, UnityAction ListenerCallback)
    {
        UnityEvent ret = null;

        UnityEventBase baseEvent = null;
        if (!eventSinks.TryGetValue(EventSinkName, out baseEvent))
        {
            Debug.LogError(EventSinkName + " does not yet exist as an event sink.");
        }
        else if (!(baseEvent is UnityEvent))
        {
            Debug.LogError(EventSinkName + " is the wrong type of event sink (" + baseEvent + ")");
        }
        else
        {
            ret = baseEvent as UnityEvent;
        }

        ret?.RemoveListener(ListenerCallback);

        return ret;
    }

    public UnityEvent<T> Unsubscribe<T>(EventSinks.EventSinkName<T> EventSinkName, UnityAction<T> ListenerCallback)
    {
        UnityEvent<T> ret = null;

        UnityEventBase baseEvent = null;
        if (!eventSinks.TryGetValue(EventSinkName, out baseEvent))
        {
            Debug.LogError(EventSinkName + " does not yet exist as an event sink.");
        }
        else if (!(baseEvent is UnityEvent<T>))
        {
            Debug.LogError(EventSinkName + " is the wrong type of event sink (" + baseEvent + ")");
        }
        else
        {
            ret = baseEvent as UnityEvent<T>;
        }

        ret?.RemoveListener(ListenerCallback);

        return ret;
    }
    
   
    #endregion

    #region Invokation Methods
    public void Invoke(EventSinks.EventSinkName EventSinkName)
    {
        var eventSink = GetEventSink(EventSinkName);
        
        if (eventSink != null)
        {
            eventSink.Invoke();
        }
        else
        {
            Debug.LogError(EventSinkName + " does not exist; cannot invoke.");
        }
    }
    public void Invoke<T>(EventSinks.EventSinkName<T> EventSinkName, T Arg)
    {
        var eventSink = GetEventSink<T>(EventSinkName);

        if (eventSink != null)
        {
            eventSink.Invoke(Arg);
        }
        else
        {
            Debug.LogError(EventSinkName + " does not exist; cannot invoke.");
        }
    }

    #endregion
}


namespace EventSinks
{
    /// <summary>
    /// Represents the name of an event sink that can be used with <see
    /// cref="EventMgr"/>.
    /// </summary>
    /// <remarks>
    /// <para>Use this class to refer to event sinks that do not take
    /// parameters.</para>
    /// <para>This class can be implicitly created with a string:
    /// <example>
    /// EventSinkName doThing = "MyEvents/DoThing";
    /// </example>
    /// </para>
    /// </remarks>
    public class EventSinkName {
        public string Name { get; protected set; }

        public static implicit operator EventSinkName(string name) {
            return new EventSinkName { Name = name };
        }
    }

    /// <summary>
    /// Represents the name of an event sink that can be used with <see
    /// cref="EventMgr"/>.
    /// </summary>
    /// <remarks>
    /// <para>Use this class to refer to event sinks that take a parameter of type <typeparamref name="T"/>.</para>
    /// <para>This class can be implicitly created with a string:
    /// <example>
    /// EventSinkName doThing&lt;string&gt; = "MyEvents/DoThing";
    /// </example>
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the parameter associated with this
    /// event.</typeparam>
    public class EventSinkName<T> : EventSinkName {
        public static implicit operator EventSinkName<T>(string name) {
            return new EventSinkName<T> { Name = name };
        }
    }
    
    /// <summary>
    /// Represents the name of an event sink that can be used with
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class EventSinkName<T1, T2> : EventSinkName 
    {
        public static implicit operator EventSinkName<T1, T2>(string name) {
            return new EventSinkName<T1, T2> { Name = name };
        }
    }

    public static class Animation
    {
        public static readonly EventSinkName <string, bool> OnFlagUpdated = "Animation/OnFlagUpdated";
    }

    public static class Dialogue
    {
        public static readonly EventSinkName OnStart = "Dialogue/OnDialogueStart";
        public static readonly EventSinkName OnEnd = "Dialogue/OnDialogueEnd";
        public static readonly EventSinkName OnAdvance = "Dialogue/DialogueAdvanceEvent";
        public static readonly EventSinkName OnBack = "Dialogue/DialogueBackEvent";
        public static readonly EventSinkName NextChoice = "Dialogue/DialogueNextChoiceEvent";
        public static readonly EventSinkName PrevChoice = "Dialogue/DialoguePrevChoiceEvent";
        public static readonly EventSinkName<string> PlayDialogue = "Dialogue/PlayDialogueEventString";
        public static readonly EventSinkName OnCloseBubble = "Dialogue/OnCloseBubble";
        public static readonly EventSinkName OnOpenComplete = "Dialogue/OnOpenComplete";
    }

    public static class Menus
    {
        public static readonly EventSinkName<bool> OnPauseGame = "Menus/OnPauseGame";
        public static readonly EventSinkName OnOK = "Menus/MenuOKEvent";
        public static readonly EventSinkName OnBack = "Menus/MenuBackEvent";
        public static readonly EventSinkName <float> OnMoveMenu = "Menus/MoveMenusEvent";
        public static readonly EventSinkName  OnMoveMenuDown = "Menus/MoveMenuDownEvent";
        public static readonly EventSinkName  OnMoveMenuUp = "Menus/MoveMenuUpEvent";
        public static readonly EventSinkName OnMenuClose = "Menus/MenuCloseEvent";
    }
    
    public static class Scene
    {
        public static readonly EventSinkName<string> LoadScene = "Scene/LoadScene"; //called when loading a scene begins
        public static readonly EventSinkName<string> OnSceneLoaded = "Scene/OnSceneLoaded"; //called when loading a scene finishes
    }

    public static class System
    {
        //called within SaveLoadMgr after save/load
        public static readonly EventSinkName OnSavedGame = "System/OnSavedGame";
        public static readonly EventSinkName<bool> OnLoadedGame = "System/OnLoadedGame";
        //called to trigger save/load outside SaveLoadMgr
        public static readonly EventSinkName SaveGame = "System/SaveGame";
        public static readonly EventSinkName<bool> LoadGame = "System/LoadGame";
        public static readonly EventSinkName DeleteSaveGame = "System/DeleteSaveGame";
    }
    
}