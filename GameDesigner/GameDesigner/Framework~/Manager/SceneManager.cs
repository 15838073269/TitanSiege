using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;

namespace Framework
{
    public class SceneManager : MonoBehaviour
    {
        public string sheetName = "Scene";

        public void Load(string sceneName)
        {
            StartCoroutine(AsyncLoadScene(sceneName));
        }

        private IEnumerator AsyncLoadScene(string sceneName)
        {
            var op = SM.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
            {
                Global.UI.Loading.ShowUI("加载场景中", op.progress);
                yield return null;
            }
            Global.UI.Loading.ShowUI("加载完成", 1f);
            yield return new WaitForSeconds(1f);
            op.allowSceneActivation = true;
            Global.UI.Loading.HideUI();
        }
    }
}