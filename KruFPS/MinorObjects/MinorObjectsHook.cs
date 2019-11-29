using System.Collections;
using System.Linq;
using UnityEngine;
using MSCLoader;

namespace KruFPS
{
    class MinorObjectsHook : MonoBehaviour
    {
        IEnumerator currentRoutine;

        public MinorObjectsHook()
        {
            FsmHook.FsmInject(this.gameObject, "Purchase", TriggerMinorObjectRefresh);
            InjectShoppingBags(Object.FindObjectsOfType<GameObject>());
        }

        public void TriggerMinorObjectRefresh()
        {
            currentRoutine = PurchaseCoroutine();

            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
            }

            StartCoroutine(currentRoutine);
        }

        IEnumerator PurchaseCoroutine()
        {
            // Wait for 5 second to let all objects spawn
            yield return new WaitForSeconds(2);

            MinorObjects.instance.minorObjects.Clear();

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                for (int j = 0; j < MinorObjects.instance.listOfMinorObjects.Length; j++)
                {
                    if (allObjects[i].name.Contains(MinorObjects.instance.listOfMinorObjects[j])
                        && allObjects[i].name.ContainsAny("(itemx)", "(Clone)")
                        && allObjects[i].activeSelf)
                    {
                        MinorObjects.instance.minorObjects.Add(allObjects[i]);
                    }
                }
            }

            InjectShoppingBags(allObjects);
            currentRoutine = null;
        }

        void InjectShoppingBags(GameObject[] allObjects)
        {
            GameObject[] shoppingBags = allObjects.Where(obj => obj.name.Contains("shopping bag") && obj.name.Contains("(itemx)")).ToArray();

            if (shoppingBags.Length > 0)
            {
                for (int i = 0; i < shoppingBags.Length; i++)
                {
                    FsmHook.FsmInject(shoppingBags[i], "Confirm", TriggerMinorObjectRefresh);
                    FsmHook.FsmInject(shoppingBags[i], "Spawn all", TriggerMinorObjectRefresh);
                }
            }
        }
    }
}
