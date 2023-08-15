using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is automatically added to an exploded object, in ExplodeOnImpact script.
/// Adds a fade out effect and destroys the container object.
/// </summary>
public class FadeDebris : MonoBehaviour
{
    private void Start() 
    {
        StartCoroutine(FadeCoroutine());
    }


    /// <summary>
    /// Gradually decreases the alpha of mesh renderers to gently fade out the pieces, 
    /// then destroys the game object.
    /// </summary>
    private IEnumerator FadeCoroutine()
    {
        yield return new WaitForSeconds(GameManager.Instance.DebrisPersistTime);

        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= 0.01f;
            Color color = new Color(1, 1, 1, alpha);

            foreach (Transform fragment in transform)
            {
                fragment.gameObject.GetComponent<MeshRenderer>().materials[0].color = color;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
