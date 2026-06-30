using UnityEngine;

public class EmotionTracking : MonoBehaviour
{
    
   
    public GameObject _trackBone;

    public GameObject _compareHappyBone;

    public GameObject _compareSadBone;
    private float BonePosY;
    private float CompareBoneHappyPosY;
    private float CompareBoneSadPosY;
    private int _emotion = 0;

    [Range(0, 3)]
    public int RightHand;

     [Range(0, 3)]
    public int LeftHand;

    public GameObject _character;
    private Animator m_Animator;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Animator = _character.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       BonePosY =   _trackBone.transform.position.y;
       CompareBoneHappyPosY = _compareHappyBone.transform.position.y;
       CompareBoneSadPosY = _compareSadBone.transform.position.y;
       

        if (BonePosY > CompareBoneHappyPosY)
        {
            _emotion = 1; // Happy
        }
        else if (CompareBoneSadPosY < BonePosY && BonePosY < CompareBoneHappyPosY)
        {
            _emotion = 0; 
        }
        else if (BonePosY < CompareBoneSadPosY)
        {
            _emotion = 2; // Sad
        }

        this.GetComponent<Renderer>().material.SetFloat("_Emotion", _emotion);

        m_Animator.SetInteger("RightHand", RightHand);
        m_Animator.SetInteger("LeftHand", LeftHand);
    }
}
