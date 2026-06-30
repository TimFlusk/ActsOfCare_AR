using UnityEngine;

public class ObjectPushing : MonoBehaviour
{
   // [Header("Tracking Setup")]
   [SerializeField] private Transform[] bonesToTrack; // Assign your key joints/bones here
   [SerializeField] private Transform[] objectsToPush; // Assign your 10 objects here

    private Vector2[] startPositions; // Store the starting positions of the objects
   private Vector2[] currentPos; // Store the current positions of the objects
    private Vector2[] bonePositions; // Store the current positions of the bones

    [Header("Push Settings")]
    [SerializeField] private float pushThreshold = 0.7f; // Distance at which pushing starts
    [SerializeField] private float speed = 1.0f; // Speed of pushing
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       startPositions = new Vector2[objectsToPush.Length];
       for (int i = 0; i < objectsToPush.Length; i++)
       {
           startPositions[i]= new Vector2(objectsToPush[i].transform.position.x, objectsToPush[i].transform.position.y);
       }
    }

    // Update is called once per frame
    void Update()
    {
       bonePositions = new Vector2[bonesToTrack.Length];
            for (int i = 0; i < bonesToTrack.Length; i++)
            {
                bonePositions[i]= new Vector2(bonesToTrack[i].position.x, bonesToTrack[i].position.y);
            }

        
        currentPos = new Vector2[objectsToPush.Length];
      
       for (int i = 0; i < objectsToPush.Length; i++)
       {
           currentPos[i]= new Vector2(objectsToPush[i].transform.position.x, objectsToPush[i].transform.position.y);
       }




      for (int i = 0; i < objectsToPush.Length; i++)
      {
        for (int j = 0; j < bonesToTrack.Length; j++)
       
        {
            float Distance = Vector2.Distance(bonePositions[j], currentPos[i]);
            Vector2 Direction = (currentPos[i] - bonePositions[j]).normalized;

            if(Distance < pushThreshold)
            {
                // Move the object away from the bone
                objectsToPush[i].transform.position += new Vector3(Direction.x, Direction.y, 0) * speed * Time.deltaTime;
            }else if (Distance > pushThreshold && currentPos[i] != startPositions[i])
                {
                     objectsToPush[i].transform.position = Vector3.Lerp(objectsToPush[i].transform.position, new Vector3(startPositions[i].x, startPositions[i].y, 0), speed * Time.deltaTime);
                }
           
         
        
        }

       }
    }
}

