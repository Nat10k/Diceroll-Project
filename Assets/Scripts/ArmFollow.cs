using Preliy.Flange;
using System.Collections;
using UnityEngine;

public class ArmFollow : MonoBehaviour
{
    [SerializeField] TargetFollower follower;
    [SerializeField] Controller robotController;
    [SerializeField] Transform target;
    Rigidbody dice;
    Vector3 dropPos;
    float _e1, _e2, _e3, _e4, _e5, _e6, moveSpeed = 3f;
    private Configuration _configuration;

    private void Awake()
    {
        dice = follower.gameObject.GetComponent<Rigidbody>();
        dropPos = transform.position;
        Debug.Log(dropPos);
    }

    public void GrabDice()
    {
        //follower.SetController(robotController);
        StartCoroutine(ReparentDice());
    }

    IEnumerator ReparentDice()
    {
        target.position = transform.position;
        while (Vector3.Distance(transform.position, dice.transform.position) > 1.5f)
        {
            target.position = Vector3.Lerp(target.position, dice.transform.position, moveSpeed * Time.deltaTime);
            var externalJoints = new ExtJoint(_e1, _e2, _e3, _e4, _e5, _e6);
            var solution = robotController.Solver.ComputeInverse(target.GetMatrix(), robotController.Tool.Value, _configuration, externalJoints, SolutionIgnoreMask.All);
            robotController.Solver.TryApplySolution(solution, false);
            yield return null;
        }
        dice.transform.parent = transform;
        dice.isKinematic = true;
        dice.transform.Rotate(new Vector3(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359)));
        //follower.SetController(null);

        StartCoroutine(DropDice());
    }

    IEnumerator DropDice()
    {
        target.position = transform.position;
        while (Vector3.Distance(transform.position, dropPos) > 1.5f)
        {
            target.position = Vector3.Lerp(target.position, dropPos, moveSpeed * Time.deltaTime);
            var externalJoints = new ExtJoint(_e1, _e2, _e3, _e4, _e5, _e6);
            var solution = robotController.Solver.ComputeInverse(target.GetMatrix(), robotController.Tool.Value, _configuration, externalJoints, SolutionIgnoreMask.All);
            robotController.Solver.TryApplySolution(solution, false);
            yield return null;
        }
        dice.transform.parent = null;
        dice.constraints = RigidbodyConstraints.None;
        dice.isKinematic = false;
        //follower.SetController(null);
    }
}
