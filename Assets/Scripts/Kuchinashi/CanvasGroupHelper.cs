using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kuchinashi
{
    public class CanvasGroupHelper
    {
        public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float alpha, float speed = 0.05f, float delay = 0f)
        {
            if (canvasGroup.alpha == alpha) yield break;

            yield return new WaitForSeconds(delay);

            if (alpha == 1f) canvasGroup.blocksRaycasts = true;
            else canvasGroup.interactable = false;

            while (!Mathf.Approximately(canvasGroup.alpha, alpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alpha, speed);
                yield return new WaitForFixedUpdate();
            }
            canvasGroup.alpha = alpha;

            if (alpha == 0f) canvasGroup.blocksRaycasts = false;
            else canvasGroup.interactable = true;
        }

        public static IEnumerator FadeCanvasGroupWithButton(CanvasGroup canvasGroup, Button button, float alpha, float speed = 0.05f)
        {
            if (canvasGroup.alpha == alpha) yield break;

            if (alpha == 1f)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = false;
            }
            else
            {
                canvasGroup.interactable = false;
                button.interactable = false;
            }

            while (!Mathf.Approximately(canvasGroup.alpha, alpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alpha, speed);
                yield return new WaitForFixedUpdate();
            }
            canvasGroup.alpha = alpha;

            if (alpha == 0f)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.interactable = true;
                button.interactable = true;
            }
        }
    }

}