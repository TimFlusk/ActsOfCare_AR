using UnityEngine;
using System.Collections;

public class CharacterRandomize : MonoBehaviour
{
    public GameObject[] _characterList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
 IEnumerator Start()
    {
        
        yield return new WaitForEndOfFrame();

        
        Randomizer();
    }

    void Randomizer()
    {
        _characterList[Random.Range(0, _characterList.Length)].SetActive(false);
    }
   
}
