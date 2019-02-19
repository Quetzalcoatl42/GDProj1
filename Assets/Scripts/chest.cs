using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chest : MonoBehaviour
{
    #region gameObject_variables
    [SerializeField]
    [Tooltip("HealthPack")]
    private GameObject healthPack;
    #endregion

    #region helper_functions
    public void Interact()
    {
        StartCoroutine(DeleteChest());
    }
    IEnumerator DeleteChest()
    {
        yield return new WaitForSeconds(0.3f);
        Instantiate(healthPack, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    #endregion
}
