using Classes.Characters.Slime;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public VariableJoystick jc;
    public Transform tr;
    public GameObject mainCanvas;

    public float speed = 2.0f;
    protected Transform objectToFollow;

    private void Start()
    {
        //Generator.singletone.GenerateChuncks();
        objectToFollow = Character.Singleton.transform;
    }

    private void FixedUpdate()
    {
        if (!tr || !objectToFollow) return;
        if (Vector2.Distance(tr.position, objectToFollow.position) <= 10)
        {
            var interpolation = speed * Time.fixedDeltaTime;
            var position1 = tr.position;
            var position = position1;
            var position2 = objectToFollow.position;

            position.y = Mathf.Lerp(position1.y, position2.y, interpolation);
            position.x = Mathf.Lerp(position1.x, position2.x, interpolation);

            position1 = position;
            tr.position = position1;
        }
        else
        {
            var position = tr.position;
            var position1 = objectToFollow.position;

            position.x = position1.x;
            position.y = position1.y;

            tr.position = position;
        }
    }
}