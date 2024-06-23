//This script lets you load a Scene asynchronously.
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : NonInstantiatingSingleton<LoadSceneAsync>
{
    protected override LoadSceneAsync GetInstance () { return this; }
    [Tooltip("Check this if you are certain the scene to be loaded is next in the build order")]
    public bool loadsNextScene;
    [Tooltip("Write the name of the scene")]
    public string sceneToLoad;
    [Tooltip("Check this to start async load at start of the scene")]
    public bool loadPreparesOnStart;
    public bool preparing = false;
    public bool transition;
    public FadeUI fadeToBlack;

    MusicFader mFader;
    AsyncOperation asyncOperation = null;

    public UnityEvent onSceneLoad;

    void Start()
    {
        mFader = FindObjectOfType<MusicFader>();
        if (loadPreparesOnStart)
        {
            //Start loading the Scene asynchronously and output the progress bar
            StartCoroutine(LoadScene());
        }
    }

    public void Load()
    {
        //Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        asyncOperation = null;

        //load next scene in build order  
        if (loadsNextScene)
        {
            //Begin to load the next Scene
            asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
        //load specified scene
        else
        {
            //Begin to load the Scene you specify
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        }

        preparing = true;
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Debug.Log("ready to load " + sceneToLoad + "!");
                if (transition)
                {
                    //Debug.Log("Allowing Async load now!");
                    //Activate the Scene
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public void TransitionImmediate()
    {
        if(fadeToBlack)
            fadeToBlack.FadeIn();
        if(mFader)
            mFader.FadeOutBasic();

        transition = true;
        onSceneLoad.Invoke();
    }

    public void TransitionWait(float wait)
    {
        if (fadeToBlack)
            fadeToBlack.FadeIn();
        if (mFader)
            mFader.FadeOutBasic();

        StartCoroutine(WaitToTransition(wait));
    }

    IEnumerator WaitToTransition(float time)
    {
        yield return new WaitForSeconds(time);
        
        if(!transition)
            transition = true;
        //Activate the Scene
        else
            asyncOperation.allowSceneActivation = true;
    }

    #region Loading Screen Calls
    public void LoadNextScene()
    {
        LoadSceneMgr(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadSceneMgr(int sceneIndex)
    {
        LoadingScreenManager.LoadScene(sceneIndex);
    }

    #endregion
}