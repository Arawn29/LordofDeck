using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfAnim : MonoBehaviour
{
    public void EndOfAnimation()
    {
        this.gameObject.SetActive(false);
    }
}
